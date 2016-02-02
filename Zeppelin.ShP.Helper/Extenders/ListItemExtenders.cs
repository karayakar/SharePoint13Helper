using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Constants;
using Zeppelin.ShP.Helper.DataAccessBaseClasses;
using Zeppelin.ShP.Helper.Enums;
using Zeppelin.ShP.Helper.Helpers;
using Zeppelin.ShP.Helper.Managers;

namespace Zeppelin.ShP.Helper.Extenders
{
    /// <summary>
    /// Class to extend SPListItem to easly work with data 
    /// </summary>
    public static class ListItemExtenders
    {
        #region General

        /// <summary>
        /// Uses to check if listItem is null
        /// Uses for system purpose
        /// </summary>
        /// <param name="listItem"></param>
        public static void CheckListItemIsNull(SPListItem listItem)
        {
            if (TryCheckListItemIsNull(listItem))
            {
                UlsLogging.LogError("ListItemExtenders.CheckListItemIsNull: listItem is null");
                throw new ArgumentException("listItem cannot be null");
            }
        }

        private static bool TryCheckListItemIsNull(SPListItem listItem)
        {
            bool result = (listItem == null);
            if (result)
            {
                UlsLogging.LogError("ListItemExtenders.TryCheckListItemIsNull: listItem is null");
            }
            return result;
        }

        /// <summary>
        /// Gets splists field value by spItem and fieldName
        /// </summary>
        /// <param name="listItem">spItem - item (row) of source splist</param>
        /// <param name="internalFieldName">fieldName - source list field internal name</param>
        /// <returns></returns>
        public static object GetFieldValueInternal(SPListItem listItem, string internalFieldName)
        {
            SPField field = listItem.Fields.GetFieldByInternalName(internalFieldName);
            return GetFieldValueInternal(listItem, field);
        }

        public static object GetFieldValueInternal(SPListItem listItem, Guid fieldGuid)
        {
            SPField field = listItem.Fields[fieldGuid];
            return GetFieldValueInternal(listItem, field);
        }

        public static object GetFieldValueInternal(SPListItem listItem, SPField field)
        {
            try
            {
                object value = null;
                try
                {
                    value = listItem[field.Id];
                }
                catch
                {
                    //fix for queries with joins
                    try
                    {
                        value = listItem[field.InternalName];
                    }
                    catch (Exception ex)
                    {
                        UlsLogging.LogError("ListItemExtenders.GetFieldValueInternal: result is null. ListTitle: {0}, ItemID: {1}, FieldTitle: {2} ", listItem.ParentList.Title, listItem.ID, field.InternalName);
                        string errorMessage = "ListItemExtenders.GetFieldValueInternal: Error while reading. ListTitle: " + listItem.ParentList.Title +
                            ", ItemID: " + listItem.ID + ", FieldInternalName: " + field.InternalName;
                    }
                }

                if (value == null)
                {
                    UlsLogging.LogInformation("ListItemExtenders.GetFieldValueInternal: result is null. ListTitle: {0}, ItemID: {1}, FieldTitle: {2} ", listItem.ParentList.Title, listItem.ID, field.InternalName);
                }
                return value;
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("ListItemExtenders.GetFieldValueInternal: result is null. ListTitle: {0}, ItemID: {1}, FieldTitle: {2} ", listItem.ParentList.Title, listItem.ID, field.InternalName);
                string errorMessage = "ListItemExtenders.GetFieldValueInternal: Error while reading. ListTitle: " + listItem.ParentList.Title +
                            ", ItemID: " + listItem.ID + ", FieldInternalName: " + field.InternalName;
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// get spfield (converted to TFieldType) by internal or display name
        /// </summary>
        /// <typeparam name="TFieldType"></typeparam>
        /// <param name="listItem"></param>
        /// <param name="internalFieldName"></param>
        /// <returns></returns>
        public static TFieldType GetFieldInternal<TFieldType>(SPListItem listItem, string internalFieldName)
        {
            SPField field = listItem.Fields.GetFieldByInternalName(internalFieldName);
            if (field == null)
            {
                field = listItem.Fields[internalFieldName];
            }

            Type conversionType = typeof(TFieldType);
            if (field is TFieldType)
            {
                return (TFieldType)Convert.ChangeType(field, conversionType);
            }
            else
            {
                UlsLogging.LogError("ListItemExtenders.GetFieldInternal: result is null. ListTitle: {0}, internalFieldName: {1}, fieldType: {2} ", listItem.ParentList.Title, internalFieldName, conversionType);
                //warning may be throw exception
                return default(TFieldType);
            }
        }

        /// <summary>
        /// Проверяет есть ли поле
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName">Отображаемое или внутреннее поле</param>
        /// <returns></returns>
        public static bool IsFieldExists(this SPListItem listItem, string fieldName)
        {
            try
            {
                return listItem.Fields.ContainsField(fieldName);
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("ListItemExtenders.IsFieldExists. ListTitle: {0}, fieldName: {1}", listItem.ParentList.Title, fieldName);
                return false;
            }
        }

        private static T ChangeType<T>(object value, Type conversionType)
        {
            if (conversionType.IsGenericType
                && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }
                conversionType = Nullable.GetUnderlyingType(conversionType);
            }

            if (conversionType.IsEnum && value.GetType().FullName == "System.String")
            {
                return (T)Enum.Parse(conversionType, value.ToString());
            }
            else
            {
                if (conversionType.FullName == "System.Guid")
                {
                    return (T)Convert.ChangeType(new Guid(value.ToString()), conversionType);
                }
            }

            //Dont catch exception here because we need to know where error was.
            return (T)Convert.ChangeType(value, conversionType);
        }

        private static TFieldType GetFieldValue<TFieldType>(this SPListItem listItem, string fieldName)
        {
            return ChangeType<TFieldType>(
                GetFieldValueInternal(listItem, fieldName)
                , typeof(TFieldType)
                );
        }

        private static TFieldType GetFieldValue<TFieldType>(this SPListItem listItem, Guid id)
        {
            return ChangeType<TFieldType>(
                GetFieldValueInternal(listItem, id)
                , typeof(TFieldType)
                );
        }

        public static TFieldType GetFieldValueOrDefault<TFieldType>(this SPListItem listItem, string fieldName, TFieldType defaultVal)
        {
            try
            {
                return listItem[fieldName] == null ? defaultVal : ChangeType<TFieldType>(listItem[fieldName], typeof(TFieldType));
            }
            catch (ArgumentException)
            {
                return defaultVal;
            }
        }

        public static Nullable<TFieldType> GetFieldNullableValue<TFieldType>(this SPListItem listItem,
            string fieldName) where TFieldType : struct
        {
            try
            {
                if (listItem[fieldName] == null)
                    return null;
            }
            catch (ArgumentException)
            {
                //This method can ignore invalid field name
                return null;
            }

            return ChangeType<TFieldType>(listItem[fieldName], typeof(TFieldType));
        }
        #endregion

        #region Вспомогательные приватные методы для работы с полями на более низком уровне

        public static string GetFieldCalculated(this SPListItem listItem, string fieldName)
        {
            SPField field = listItem.Fields.GetFieldByInternalName(fieldName);
            return field.GetFieldValueForEdit(GetFieldValueInternal(listItem, fieldName));
        }

        private static SPFieldLookupValue GetSPFieldLookupValue(SPFieldLookup field, string value)
        {
            var lookupValue = field.GetFieldValue(value) as SPFieldLookupValue;
            if (lookupValue == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                            "Value {0} is not lookup field value", field));
            return lookupValue;
        }

        private static SPFieldLookup GetSPFieldLookup(this SPListItem spItem, string fieldName)
        {
            var lookupField = spItem.Fields.GetFieldByInternalName(fieldName) as SPFieldLookup;
            if (lookupField == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                            "Field {0} is not lookup in list", fieldName));
            return lookupField;
        }

        private static SPFieldUserValue GetSPFieldUserValue(this SPListItem listItem, string fieldName)
        {
            SPFieldUser userField = ListItemExtenders.GetFieldInternal<SPFieldUser>(listItem, fieldName);
            //listItem.Fields.Cast<SPField>().Single(field => field.InternalName == fieldName) as SPFieldUser;
            if (userField != null)
            {
                object fieldValue = ListItemExtenders.GetFieldValueInternal(listItem, fieldName);
                if (userField != null && fieldValue != null)
                {
                    SPFieldUserValue result = userField.GetFieldValue(fieldValue.ToString()) as SPFieldUserValue;
                    return result;
                }
            }
            return null;
        }

        private static SPFieldUserValueCollection GetSPFieldUserValueCollection(this SPListItem listItem, string internalFieldName)
        {
            SPFieldUser userField = ListItemExtenders.GetFieldInternal<SPFieldUser>(listItem, internalFieldName);
            //listItem.Fields.Cast<SPField>().Single(field => field.InternalName == fieldName) as SPFieldUser;
            if (userField != null)
            {
                object fieldValue = ListItemExtenders.GetFieldValueInternal(listItem, internalFieldName);
                if (userField != null && fieldValue != null)
                {
                    SPFieldUserValueCollection result = userField.GetFieldValue(fieldValue.ToString()) as SPFieldUserValueCollection;
                    return result;
                }
            }
            return null;
        }

        //private static SPFieldUserValue GetSPFieldUserValue(SPWeb web, UserData userData)
        //{
        //    SPFieldUserValue userValue = null;

        //    if (userData.Type == Enums.UserDataType.Login || userData.Type == Enums.UserDataType.WindowsGroup || userData.Type == Enums.UserDataType.SPGroup)
        //    {
        //        if (!String.IsNullOrEmpty(userData.LoginName) && userData.Id != 0)
        //        {
        //            userValue = new SPFieldUserValue(web, userData.Id, userData.LoginName);
        //        }
        //        else if ((userData.Type == Enums.UserDataType.Login || userData.Type == Enums.UserDataType.WindowsGroup) && !String.IsNullOrEmpty(userData.Email))
        //        {
        //            try
        //            {
        //                SPUser spUser = web.SiteUsers.GetByEmail(userData.LoginName);
        //                userValue = new SPFieldUserValue(web, spUser.ID, spUser.LoginName);
        //            }
        //            catch
        //            {
        //                //TODO Why hide exception? May be write warning
        //            }
        //        }
        //        else
        //        {
        //            // // TODO: log unexpected UserData
        //        }
        //    }
        //    else
        //    {
        //        // TODO: log unexpected UserDataType
        //    }
        //    return userValue;
        //}
        #endregion

        #region Получение данных простых типов Простые поля - Дата, Целое, Да/Нет, Decimal, Double, строка

        public static DateTime GetFieldDateTimeValue(this SPListItem listItem, string fieldName)
        {
            return GetFieldValue<DateTime>(listItem, fieldName);
        }

        public static DateTime? GetFieldDateTimeValueOrNull(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return null;
            }

            return GetFieldValue<DateTime>(listItem, fieldName);
        }

        public static DateTime GetFieldDateTimeValue(this SPListItem listItem, string fieldName, DateTime defaultValue)
        {
            if (listItem[fieldName] == null)
            {
                return defaultValue;
            }

            return GetFieldValue<DateTime>(listItem, fieldName);
        }

        public static int? GetFieldIntValueOrNull(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return null;
            }

            return GetFieldValue<int>(listItem, fieldName);
        }

        public static int GetFieldIntValue(this SPListItem listItem, string fieldName, int defaultValue)
        {
            if (listItem[fieldName] == null)
                return defaultValue;
            return GetFieldValue<int>(listItem, fieldName);
        }

        public static int GetFieldIntValue(this SPListItem listItem, string fieldName)
        {
            return GetFieldValue<int>(listItem, fieldName);
        }

        public static double GetFieldDoubleValue(this SPListItem listItem, string fieldName)
        {
            return GetFieldValue<double>(listItem, fieldName);
        }

        public static double? GetFieldDoubleValueOrNull(this SPListItem listItem, string fieldName)
        {
            if (GetFieldValueInternal(listItem, fieldName) == null)
            {
                return null;
            }

            return GetFieldValue<double>(listItem, fieldName);
        }

        public static double GetFieldDoubleValue(this SPListItem listItem, string fieldName, double defaultValue)
        {
            if (listItem[fieldName] == null)
                return defaultValue;
            return GetFieldValue<double>(listItem, fieldName);
        }

        public static bool GetFieldBooleanValue(this SPListItem listItem, string fieldName)
        {
            return GetFieldValue<bool>(listItem, fieldName);
        }

        //private static ImageFieldValue GetSPImageFieldValue(object fieldValue)
        //{
        //    if (fieldValue == null)
        //        return null;

        //    var value = fieldValue as ImageFieldValue;
        //    if (value != null)
        //    {
        //        return value;
        //    }

        //    var stringValue = fieldValue.ToString();

        //    if (string.IsNullOrEmpty(stringValue))
        //        return null;

        //    try
        //    {
        //        return new ImageFieldValue(stringValue);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public static ImageFieldValue GetSPImageFieldValue(this SPListItem listItem, string fieldName)
        //{
        //    ListItemExtenders.CheckListItemIsNull(listItem);
        //    return GetSPImageFieldValue(listItem[fieldName]);
        //}

        //public static string GetFieldImageUrlValue(this SPListItem listItem, string fieldName)
        //{
        //    ImageFieldValue value = GetSPImageFieldValue(listItem, fieldName);
        //    if (value == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return value.ImageUrl;
        //    }
        //}

        public static bool? GetFieldBooleanValueOrNull(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null) return null;
            return GetFieldValue<bool>(listItem, fieldName);
        }

        public static bool GetFieldBooleanValue(this SPListItem listItem, string fieldName, bool defaultValue)
        {
            if (listItem[fieldName] == null)
                return defaultValue;
            return GetFieldValue<bool>(listItem, fieldName);
        }

        public static decimal GetFieldDecimalValue(this SPListItem listItem, string fieldName)
        {
            return GetFieldValue<decimal>(listItem, fieldName);
        }

        public static decimal? GetFieldDecimalValueOrNull(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null) return null;
            return GetFieldValue<decimal>(listItem, fieldName);
        }

        public static decimal GetFieldDecimalValue(this SPListItem listItem, string fieldName, decimal defaultValue)
        {
            if (listItem[fieldName] == null) return defaultValue;
            return GetFieldValue<decimal>(listItem, fieldName);
        }

        /// <summary>
        /// Return string field value. If field contains null - return string.Empty
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetFieldStringValue(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return string.Empty;
            }
            return GetFieldValue<string>(listItem, fieldName);
        }
        #endregion

        #region Получение данных из сложных типов, многосточный тескт, пользователи, урлы, подстановки

        public static List<string> GetFieldMultiChoiceValues(this SPListItem listItem, string fieldName)
        {
            List<string> choices = new List<string>();
            object rawValueObject = listItem[fieldName];
            if (rawValueObject != null)
            {
                SPFieldMultiChoiceValue typedValue = new SPFieldMultiChoiceValue(rawValueObject.ToString());
                for (int i = 0; i < typedValue.Count; i++)
                {
                    choices.Add(typedValue[i]);
                }
            }
            return choices;
        }

        /// <summary>
        /// Возвращает многосточный текст с HTML разметкой
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetFieldHTMLValueFromMultiLine(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return string.Empty;
            }
            SPFieldMultiLineText field = GetFieldInternal<SPFieldMultiLineText>(listItem, fieldName);
            object fieldValue = GetFieldValueInternal(listItem, fieldName);
            return field.GetFieldValueAsHtml(fieldValue);
        }

        /// <summary>
        /// Возвращает тект без тегов
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetFieldTextValueFromMultiLine(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return string.Empty;
            }
            SPFieldMultiLineText field = GetFieldInternal<SPFieldMultiLineText>(listItem, fieldName);
            object fieldValue = GetFieldValueInternal(listItem, fieldName);
            return field.GetFieldValueAsText(fieldValue);
        }

        /// <summary>
        /// Возвращает тект без тегов
        /// Включена дополнительная проверка 
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string GetFieldTextValueFromMultiLine2(this SPListItem listItem, string fieldName)
        {
            var result = listItem.GetFieldTextValueFromMultiLine(fieldName);
            return SPHttpUtility.ConvertSimpleHtmlToText(result, -1);
        }

        ///// <summary>
        ///// Возвращает UrlValue (Url, Description) на основе внутреннего имени
        ///// </summary>
        ///// <param name="listItem"></param>
        ///// <param name="internalFieldName"></param>
        ///// <returns></returns>
        //public static UrlValue GetFieldUrlValue(this SPListItem listItem, string internalFieldName)
        //{
        //    UrlValue result = new UrlValue();
        //    SPField field = listItem.Fields.GetFieldByInternalName(internalFieldName);
        //    object value = GetFieldValueInternal(listItem, internalFieldName);
        //    if (value != null)
        //    {
        //        SPFieldUrlValue fieldValue = new SPFieldUrlValue(value.ToString());
        //        result.Url = fieldValue.Url;
        //        result.Description = fieldValue.Description;
        //    }
        //    return result;
        //}


        /// <summary>
        /// Возвращает UserData для поля пользователь или группв (for spuser and SPGroup)
        /// Если поле пустое возвращает Null
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static UserData GetFieldUserValue(this SPListItem listItem, string fieldName)
        {
            UserData result = null;
            SPFieldUserValue field = GetSPFieldUserValue(listItem, fieldName);
            if (field != null)
                result = new UserData(listItem.Web, field);
            return result;
        }

        private static UserData GetFieldUserValueQuick(this SPListItem listItem, string fieldName)
        {
            UserData result = null;
            LookupValue data = GetFieldLookupValue(listItem, fieldName);
            if (data != null)
                result = new UserData(data.Id, data.Value);
            return result;
        }

        public static UserData GetFieldUserValueOrEmpty(this SPListItem listItem, string fieldName)
        {
            UserData user = GetFieldUserValue(listItem, fieldName);
            if (user != null)
                return user;
            else
                return UserData.GetEmpty();
        }

        public static UserData GetFieldUserValueQuickOrEmpty(this SPListItem listItem, string fieldName)
        {
            UserData user = GetFieldUserValueQuick(listItem, fieldName);
            if (user != null)
                return user;
            else
                return UserData.GetEmpty();
        }

        public static List<UserData> GetFieldMultiUserValues(this SPListItem listItem, string internalFieldName)
        {
            List<UserData> result = new List<UserData>();
            object fieldValue = GetFieldValueInternal(listItem, internalFieldName);

            if (fieldValue != null)
            {
                SPWeb parentWeb = listItem.Web;
                SPList parentList = listItem.ParentList;
                SPField baseField = listItem.Fields.GetFieldByInternalName(internalFieldName);
                SPFieldUser userField = baseField != null && baseField.Type == SPFieldType.User ? baseField as SPFieldUser : null;

                if (userField != null)
                {
                    if (userField.AllowMultipleValues)
                    {
                        SPFieldUserValueCollection fieldUserValues = GetSPFieldUserValueCollection(listItem, internalFieldName);
                        if (fieldUserValues != null && fieldUserValues.Count != 0)
                        {
                            foreach (SPFieldUserValue fieldUserValue in fieldUserValues)
                            {
                                if (fieldUserValue != null)
                                {
                                    result.Add(new UserData(listItem.Web, fieldUserValue));
                                }
                            }
                        }
                    }
                    else
                    {
                        SPFieldUserValue fieldUserValue = new SPFieldUserValue(parentWeb, fieldValue.ToString());
                        if (fieldUserValue != null)
                        {
                            result.Add(new UserData(listItem.Web, fieldUserValue));
                        }
                    }
                }
            }

            return result;
        }

        public static string GetFieldLookupValue(object listItem)
        {
            if (listItem != null)
                return new SPFieldLookupValue(Convert.ToString(listItem)).LookupValue;
            else return "";
        }

        public static int GetFieldLookupIDValue(object listItem)
        {
            if (listItem != null)
                return new SPFieldLookupValue(Convert.ToString(listItem)).LookupId;
            else return 0;
        }

        public static LookupValue GetFieldLookupValue(this SPListItem listItem, string fieldName)
        {
            LookupValue result = null;
            UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 1");
            var value = GetFieldValueInternal(listItem, fieldName);
            UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 2");
            if (value != null)
            {
                UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 3");
                var lookupField = GetSPFieldLookup(listItem, fieldName);
                UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 4");
                SPFieldLookupValue spFieldLookupValue = GetSPFieldLookupValue(lookupField, value.ToString());
                UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 5");
                result = new LookupValue();
                UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 6");
                result.SetValue(spFieldLookupValue.LookupId, spFieldLookupValue.LookupValue);
                UlsLogging.LogInformation("GetFieldLookupValue(this SPListItem listItem, string fieldName) 7");
            }
            else
            {
                //result = new LookupValue(); //Error if field is empty
                result = null;
            }
            return result;
        }

        public static List<LookupValue> GetFieldMultiLookupValues(this SPListItem listItem, string fieldName)
        {
            List<LookupValue> result = new List<LookupValue>();
            var itemsLookup = listItem[fieldName] as IEnumerable<SPFieldLookupValue>;
            if (itemsLookup != null)
            {
                foreach (var lookupItem in itemsLookup)
                {
                    LookupValue newLookupValue = new LookupValue();
                    newLookupValue.SetValue(lookupItem.LookupId, lookupItem.LookupValue);
                    result.Add(newLookupValue);
                }
            }
            return result;
        }
        #endregion

        public static Guid GetFieldGuidValue(this SPListItem listItem, string fieldName)
        {
            if (listItem[fieldName] == null)
            {
                return Guid.Empty;
            }
            return GetFieldValue<Guid>(listItem, fieldName);
        }

        #region Установка значений
        //public static void SetFieldBoolValue(this SPListItem spItem, string fieldName, bool? value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldMultiChoiceValue(this SPListItem spItem, string fieldName, List<string> choices)
        //{
        //    SPFieldMultiChoiceValue fieldValue = new SPFieldMultiChoiceValue();
        //    if (choices != null)
        //    {
        //        foreach (string choice in choices)
        //        {
        //            fieldValue.Add(choice);
        //        }
        //    }
        //    spItem[fieldName] = fieldValue;
        //}

        //public static void SetFieldDateTimeValue(this SPListItem spItem, string fieldName, DateTime value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldDateTimeValue(this SPListItem spItem, string fieldName, DateTime? value)
        //{
        //    if (value.HasValue)
        //    {
        //        SetFieldDateTimeValue(spItem, fieldName, value.Value);
        //    }
        //    else
        //    {
        //        SetFieldNullValue(spItem, fieldName);
        //    }
        //}

        //public static void SetFieldDoubleValue(this SPListItem spItem, string fieldName, double value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldDoubleValue(this SPListItem spItem, string fieldName, double? value)
        //{
        //    if (value.HasValue)
        //    {
        //        SetFieldDoubleValue(spItem, fieldName, value.Value);
        //    }
        //    else
        //    {
        //        SetFieldNullValue(spItem, fieldName);
        //    }
        //}

        //public static void SetFieldDecimalValue(this SPListItem spItem, string fieldName, decimal value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldDecimalValue(this SPListItem spItem, string fieldName, decimal? value)
        //{
        //    if (value.HasValue)
        //    {
        //        SetFieldDecimalValue(spItem, fieldName, value.Value);
        //    }
        //    else
        //    {
        //        SetFieldNullValue(spItem, fieldName);
        //    }
        //}

        //public static void SetFieldIntValue(this SPListItem spItem, string fieldName, int value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldIntValue(this SPListItem spItem, string fieldName, int? value)
        //{
        //    if (value.HasValue)
        //    {
        //        SetFieldDoubleValue(spItem, fieldName, value.Value);
        //    }
        //    else
        //    {
        //        SetFieldNullValue(spItem, fieldName);
        //    }
        //}

        //public static void SetFieldLookupValue(this SPListItem spItem, string fieldName, int lookupId, string value)
        //{
        //    SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue(lookupId, value);
        //    spItem[fieldName] = fieldLookupValue;
        //}

        //public static void SetFieldLookupValue(this SPListItem spItem, string fieldName, int? lookupId)
        //{
        //    if ((lookupId == null) || (!lookupId.HasValue))
        //    {
        //        spItem[fieldName] = null;
        //    }
        //    else
        //    {
        //        SPField lookupFld = spItem.Fields.GetFieldByInternalName(fieldName);
        //        SPFieldLookupValue fieldLookupValue = (SPFieldLookupValue)lookupFld.GetFieldValue(lookupId.Value.ToString());

        //        spItem[fieldName] = fieldLookupValue;
        //    }
        //}

        //public static void SetFieldLookupValue(this SPListItem spItem, string fieldName, LookupValue lookupValue)
        //{
        //    if (lookupValue == null || lookupValue.IsEmpty)
        //    {
        //        spItem[fieldName] = null;
        //    }
        //    else
        //    {
        //        SPField lookupFld = spItem.Fields.GetFieldByInternalName(fieldName);
        //        SPFieldLookupValue fieldLookupValue = (SPFieldLookupValue)lookupFld.GetFieldValue(lookupValue.Id.ToString());

        //        spItem[fieldName] = fieldLookupValue;
        //    }
        //}

        //public static void SetFieldMultiLookupValues(this SPListItem spItem, string fieldName, List<LookupValue> lookupValues)
        //{
        //    SPFieldLookupValueCollection fieldLookupValue = new SPFieldLookupValueCollection();
        //    if (lookupValues != null)
        //    {
        //        foreach (var lookupValue in lookupValues)
        //        {
        //            if (!lookupValue.IsEmpty)
        //            {
        //                fieldLookupValue.Add(new SPFieldLookupValue(lookupValue.Id, lookupValue.Value));
        //            }
        //        }
        //        spItem[fieldName] = fieldLookupValue;
        //    }
        //}

        //public static void SetFieldNullValue(this SPListItem spItem, string fieldName)
        //{
        //    spItem[fieldName] = null;
        //}

        ///// <summary>
        ///// Set string value
        ///// </summary>
        ///// <param name="spItem">current item</param>
        ///// <param name="fieldName">item field name</param>
        ///// <param name="value">field new value</param>
        //public static void SetFieldStringValue(this SPListItem spItem, string fieldName, string value)
        //{
        //    spItem[fieldName] = value;
        //}

        //public static void SetFieldUrValue(this SPListItem listItem, string fieldName, string urlDescription, string urlValue)
        //{

        //    SPFieldUrlValue urlField = new SPFieldUrlValue();
        //    urlField.Url = urlValue;
        //    urlField.Description = urlDescription;
        //    listItem[fieldName] = urlField;
        //}

        //public static void SetFieldUrlValue(this SPListItem listItem, string fieldName, UrlValue value)
        //{

        //    SPFieldUrlValue urlField = new SPFieldUrlValue() { Url = value.Url, Description = value.Description };
        //    listItem[fieldName] = urlField;
        //}

        //public static void SetFieldUserValue(this SPListItem listItem, string fieldName, UserData userData)
        //{
        //    listItem[fieldName] = GetSPFieldUserValue(listItem.ParentList.ParentWeb, userData);
        //}

        //public static void SetFieldMultiUsersValues(this SPListItem listItem, string fieldName, List<UserData> userDataCollection)
        //{
        //    SPFieldUserValueCollection userValues = new SPFieldUserValueCollection();
        //    foreach (UserData userData in userDataCollection)
        //    {
        //        SPFieldUserValue userValue = GetSPFieldUserValue(listItem.ParentList.ParentWeb, userData);
        //        if (userValue != null)
        //        {
        //            userValues.Add(userValue);
        //        }
        //    }
        //    listItem[fieldName] = userValues;
        //}

        //public static void SetFieldMultiUsersValues(this SPListItem spItem, string fieldName, List<string> userLogins)
        //{
        //    SPFieldUserValueCollection usercollection = new SPFieldUserValueCollection();

        //    foreach (var login in userLogins)
        //    {
        //        SPUser requireduser = spItem.Web.GetSPUserByLoginName(login);
        //        SPFieldUserValue usertoadd = new SPFieldUserValue(spItem.Web, requireduser.ID, requireduser.LoginName);
        //        usercollection.Add(usertoadd);
        //    }

        //    spItem[fieldName] = usercollection;
        //}

        //public static void SetFieldUser(this SPListItem spItem, string fieldName, SPUser user)
        //{
        //    SPFieldUserValue userValue = new SPFieldUserValue(spItem.Web, user.ID, user.LoginName);
        //    spItem[fieldName] = userValue;
        //}
        #endregion



    }
}
