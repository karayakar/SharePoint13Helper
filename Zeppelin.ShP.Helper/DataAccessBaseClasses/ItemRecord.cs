using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.AdditionalStructures;
using Zeppelin.ShP.Helper.Enums;
using Zeppelin.ShP.Helper.Managers;
using Zeppelin.ShP.Helper.Extenders;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    /// <summary>
    /// Базовай класс для работы с элементами списка
    /// </summary> 
    [Serializable]
    public class ItemRecord
    {
        private OriginInfo _OriginInfo;
        public OriginInfo OriginInfo
        {
            get
            {
                if (_OriginInfo == null) _OriginInfo = new OriginInfo();
                return _OriginInfo;
            }
        }

        /// <summary>
        ////Создание нового элемента
        /// </summary>
        public ItemRecord()
        {
            IsNew = true;
        }

        /// <summary>
        /// Создание нового элемента на основе элемента списка и информации о списке
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        /// <param name="originInfo">Информация о списке</param>
        public ItemRecord(SPListItem spItem, OriginInfo originInfo)
        {
            LoadData(spItem, originInfo);
            IsNew = false;
        }

        /// <summary>
        /// Создание нового элемента на основе DataRow 
        /// </summary>
        /// <param name="row">Строка таблицы данных</param>
        /// <param name="fieldProvider">Провайдер для получения перечня полей</param>
        /// <param name="originInfo">Информация о списке</param>
        /// <param name="spSite">Сайт, на котором находятся данные</param>
        public ItemRecord(DataRow row, BaseFieldsProvider fieldProvider, OriginInfo originInfo, SPSite spSite)
        {
            LoadDataFromDataRow(row, fieldProvider, originInfo, spSite);
            IsNew = false;
        }

        /// <summary>
        /// Загрузка данных на основе DataRow
        /// </summary>
        /// <param name="row">Строка таблицы данных</param>
        /// <param name="fieldProvider">Провайдер для получения перечня полей</param>
        /// <param name="originInfo">Информация о списке</param>
        /// <param name="spSite">Сайт, на котором находятся данные</param>
        public virtual void LoadDataFromDataRow(DataRow row, BaseFieldsProvider fieldProvider, OriginInfo originInfo, SPSite spSite)
        {
            IsNew = false;
            this._OriginInfo = originInfo;

            if ((fieldProvider != null) && (fieldProvider.FieldExists(Constants.FieldNamesBaseTypes.Id)))
                _id = int.Parse((string)row[Constants.FieldNamesBaseTypes.Id]);

            if ((fieldProvider != null) && (fieldProvider.FieldExists(Constants.FieldNamesBaseTypes.Title)))
                Title = (string)row[Constants.FieldNamesBaseTypes.Title];
            ItemIcon = UrlHelper.GetFileIconUrl(null);
        }

        /// <summary>
        /// Инизиализация базовых полей данных полей из SPListItem
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        protected void InitBaseFields(SPListItem spItem)
        {
            _id = spItem.ID;
            Title = spItem.Title;

            if (spItem.Fields.ContainsField(Constants.FieldNamesBaseTypes.DocIcon))
                this.ItemIcon = UrlHelper.GetFileIconUrl(spItem.GetFieldStringValue(Constants.FieldNamesBaseTypes.DocIcon));
            else
                this.ItemIcon = UrlHelper.GetFileIconUrl(null);
        }

        /// <summary>
        /// Загрузка данных полей из SPListItem и информации о списке
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        /// <param name="locationInfo">Информация о списке</param>
        public void LoadData(SPListItem spItem, OriginInfo locationInfo)
        {
            LoadData(spItem, locationInfo, null);
        }

        /// <summary>
        /// Загрузка данных полей из SPListItem и информации о списке
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        /// <param name="locationInfo">Информация о списке</param>
        /// <param name="fieldProvider">Провайдер для получения перечня полей</param>
        public void LoadData(SPListItem spItem, OriginInfo locationInfo, BaseFieldsProvider columnProvider)
        {
            this._OriginInfo = locationInfo;
            IsNew = false;
            if (spItem == null)
            {
                //LogServiceNS.LoggingManager.GetLogWriter().WriteError("Trying to convert empty sharepoint item to type:" + this.GetType());
                UlsLogging.LogInformation("Trying to convert empty sharepoint item to type:" + this.GetType());
                throw new NullReferenceException("spItem");
            }
            InitBaseFields(spItem);
            if (columnProvider == null)
            {
                LoadDataFromSPListItem(spItem);
            }
            else
            {
                LoadDataFromSPListItem(spItem, columnProvider);
            }
        }

        /// <summary>
        /// Загрузка данных полей из SPListItem
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        public void LoadData(SPListItem spItem)
        {
            OriginInfo locationInfo = OriginInfoCacheManager.GetGlobalOriginInfo(spItem.ParentList);
            LoadData(spItem, locationInfo);
        }

        /// <summary>
        /// Виртуальный метод для выполнения загрузки других полей в наследниках
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        protected virtual void LoadDataFromSPListItem(SPListItem spItem)
        {
        }

        /// <summary>
        /// Виртуальный метод для выполнения загрузки других полей в наследниках
        /// </summary>
        /// <param name="spItem">Элемент списка</param>
        /// <param name="columnProvider">Провайдкр для получения перечня полей</param>
        protected virtual void LoadDataFromSPListItem(SPListItem spItem, BaseFieldsProvider columnProvider)
        {
        }

        /// <summary>
        /// Получение связанного с данным классом провайдера полей
        /// </summary>
        /// <returns></returns>
        public virtual BaseFieldsProvider GetFieldsProvider()
        {
            return new BaseFieldsProvider();
        }

        /// <summary>
        /// Возвращает новый ли элемент
        /// </summary>
        public bool IsNew { get; set; }

        protected int _id;

        /// <summary>
        /// ИД элемента
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; IsNew = false; }
        }

        /// <summary>
        /// Название элемента
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Иконка 
        /// </summary>
        public string ItemIcon { get; set; }

        /// <summary>
        /// Метод по сохранению 
        /// </summary>
        /// <param name="spItem"></param>
        /// <returns></returns>
        protected virtual bool SaveData(SPListItem spItem)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                spItem[Constants.FieldNamesBaseTypes.Title] = Title;
            }
            else
            {
                spItem[Constants.FieldNamesBaseTypes.Title] = string.Empty;
            }
            return true;
        }

        /// <summary>
        /// Сохранить данные в элемент списка используя стандартное сохранение (с обновлением информации о авторе и редакторе)
        /// После сохранения перечитать информацию
        /// </summary>
        /// <param name="listItem">Элемент списка</param>
        /// <returns>Успешно ли было сохранение</returns>
        public bool Save(SPListItem listItem)
        {
            return Save(listItem, ItemRecordSaveMode.Simple, ItemRecordReloadMode.Reload);
        }

        /// <summary>
        /// Сохранить данные в элемент списка без изменения информации о авторе и редакторе
        /// После сохранения перечитать информацию
        /// </summary>
        /// <param name="listItem">Элемент списка</param>
        /// <returns>Успешно ли было сохранение</returns>
        public bool SystemSave(SPListItem listItem)
        {
            return Save(listItem, ItemRecordSaveMode.SystemUpdate, ItemRecordReloadMode.Reload);
        }

        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="listItem">Элемент списка</param>
        /// <param name="saveMode">Режим сохранения</param>
        /// <param name="reloadMode">Нужно ли перечитать данные списка после сохранения</param>
        /// <returns></returns>
        public bool Save(SPListItem listItem, ItemRecordSaveMode saveMode, ItemRecordReloadMode reloadMode)
        {
            bool result = false;
            try
            {
                result = SaveData(listItem);
                if (!result)
                {
                    return false;
                }

                if (saveMode == ItemRecordSaveMode.Simple)
                {
                    listItem.Update();
                }
                else
                {
                    listItem.SystemUpdate();
                }

                IsNew = false;
                result = true;
                Reload(listItem, reloadMode);
            }
            catch
            {
                if (IsNew)
                {
                    listItem.Delete();
                }
                throw;
            }
            return result;
        }

        /// <summary>
        /// Повторное чтение элемента списка
        /// </summary>
        /// <param name="listItem">Элемент спискак</param>
        /// <param name="mode">Режим</param>
        protected void Reload(SPListItem listItem, ItemRecordReloadMode mode)
        {
            if (mode == ItemRecordReloadMode.None)
            { }
            else if (mode == ItemRecordReloadMode.Reload)
            {
                Reload(listItem.Web, listItem.ParentList.ID, listItem.ID);
            }
            else
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = new SPSite(listItem.Web.Site.ID))
                    {
                        Reload(spSite.RootWeb, listItem.ParentList.ID, listItem.ID);
                    }
                });
            }
        }

        /// <summary>
        /// Повторное чтение элемента списка
        /// </summary>
        /// <param name="web">сайт</param>
        /// <param name="listID">GUID списка</param>
        /// <param name="listItemId">Ид элемента</param>
        protected void Reload(SPWeb web, Guid listID, int listItemId)
        {
            SPList list = web.Lists[listID];
            SPListItem item = list.GetItemById(listItemId);
            LoadData(item, OriginInfoCacheManager.GetGlobalOriginInfo(list));
        }

        /// <summary>
        /// Выполняет сравнение элементов
        /// Элементы считаются одинаковыми, еси у них одинакотоые типы и совпадают Ид
        /// </summary>
        /// <param name="obj">Объект с которым выполняется сравнение</param>
        /// <returns>True - совпадают, False - нет</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() == obj.GetType())
            {
                if (Id == ((ItemRecord)obj).Id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Функция получения хеша. Использыется для хранения объектов в хеш-списках
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!IsNew)
                return this.Id;
            else
            {
                return (new Random().Next());
            }
        }

        /// <summary>
        /// Путь к форме просмотра элемента
        /// </summary>
        public string DispFormUrl
        {
            get
            {
                return UrlHelper.CombineUrl(
                    this.OriginInfo.WebUrl,
                    string.Format("/_layouts/listform.aspx?PageType=4&ListId={0}&ID={1}", this.OriginInfo.ListId, this._id)
                    );
            }
        }

        /// <summary>
        /// Путь к форме редактирования элемента
        /// </summary>
        public string EditFormURL
        {
            get
            {
                return UrlHelper.CombineUrl(
                    this.OriginInfo.WebUrl,
                    string.Format("/_layouts/listform.aspx?PageType=6&ListId={0}&ID={1}", this.OriginInfo.ListId, this._id)
                    );
            }
        }
    }
}
