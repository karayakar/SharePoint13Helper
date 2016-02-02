using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using Microsoft.SharePoint.Workflow;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls.WebParts;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.Managers
{


    public class ListManager
    {
        public static SPList GetListInfoByGuid(SPWeb web, string guid)
        {
            SPList list = null;
            try
            {
                list = web.Lists[new Guid(guid)];
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("List. GetListInfoByName(SPWeb web, string ListName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return list;
        }

        public static SPList GetListInfoByListPath(SPWeb web, string listPath)
        {
            SPList list = null;
            try
            {
                list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, listPath));
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("List. GetListInfoByListPath(SPWeb web, string ListName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return list;
        }
        public static SPList GetListInfoByListRelativeUrl(SPWeb web, string listPath)
        {
            SPList list = null;
            try
            {
                list = web.GetList(listPath);
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("List. GetListInfoByListRelativeUrl(SPWeb web, string ListName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return list;
        }

        public static SPList GetListInfoByName(SPWeb web, string listName)
        {
            SPList list = null;
            try
            {
                list = web.Lists[listName];
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("List. GetListInfoByName(SPWeb web, string ListName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return list;
        }

        public class ListItem
        {

            public static bool CheckedAttachmentInItem(SPListItem item)
            {
                bool res = false;
                try
                {
                    if (item.Attachments.Count > 0)
                        res = true;
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListItem. CheckedAttachmentInItem(SPListItem item). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return res;
            }

            public static SPFileCollection GetAttachmentInItem(SPListItem item)
            {
                SPFileCollection res = null;
                try
                {
                    foreach (SPFile file in item.Attachments)
                    {
                        res.Add(file.Url, file.OpenBinary());
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListItem. CheckedAttachmentInItem(SPListItem item). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return res;
            }

            public string GetDocumentUrl(SPListItem item)
            {
                return (string)item[SPBuiltInFieldId.EncodedAbsUrl];
            }

            /// <summary>
            ///  Returns the value of a Lookup-Field with multiple values.
            /// </summary>
            /// <param name="item"></param>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public static IEnumerable<string> GetFieldValueLookupCollection(SPListItem item, string fieldName)
            {
                List<string> result = new List<string>();
                if (item != null)
                {
                    SPFieldLookupValueCollection values = item[fieldName] as SPFieldLookupValueCollection;
                    foreach (SPFieldLookupValue value in values)
                    {
                        result.Add(value.LookupValue);
                    }
                }
                return result;
            }

            /// <summary>
            ///  Returns SPFieldLookupValue from Multy Lookup field.
            /// </summary>
            /// <param name="item"></param>
            /// <param name="fieldName"></param>
            /// <returns></returns>
            public static IEnumerable<SPFieldLookupValue> GetFieldLookupValueCollection(SPListItem item, string fieldName)
            {
                List<SPFieldLookupValue> result = new List<SPFieldLookupValue>();
                if (item != null)
                {
                    SPFieldLookupValueCollection values = item[fieldName] as SPFieldLookupValueCollection;
                    foreach (SPFieldLookupValue value in values)
                    {
                        result.Add(value);
                    }
                }
                return result;
            }

            public static string GetWorkflowItemHistory(SPListItem item)
            {
                string wfListData = "";
                try
                {
                    Dictionary<string, string> wfDic = new Dictionary<string, string>();
                    List<Dictionary<string, string>> wfList = new List<Dictionary<string, string>>();
                    SPWorkflowCollection workflows = item.Workflows;
                    foreach (SPWorkflow workflow in workflows)
                    {
                        SPListItemCollection wfHistoryItems = workflow.HistoryList.Items;
                        foreach (SPListItem worflowHistoryItem in wfHistoryItems)
                        {
                            if (("{" + workflow.InstanceId.ToString() + "}" == worflowHistoryItem["Workflow History Parent Instance"].ToString())
                                && (item.ID.ToString() == worflowHistoryItem["Primary Item ID"].ToString()))
                            {
                                wfDic = ListField.GetFieldsItemValue(worflowHistoryItem);
                                wfList.Add(wfDic);
                            }
                        }
                    }
                    wfListData = Constants.Common.Jss.Serialize(wfList);
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListItem. GetWorkflowItemHistory(SPListItem item). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return wfListData;
            }


            public class Permission
            {
                /// <summary>
                /// /// <summary>
                /// format param "userID#RoleName,UserID#RoleName"
                /// format param "groupName#RoleName,groupName#RoleName"
                /// </summary>
                /// <param name="item"></param>
                /// <param name="parameters">
                ///     <param name="UsersIDAndRole"></param>
                ///     <param name="GroupsAndRole"></param>
                /// </param>
                /// <returns></returns>
                public static bool SetPermissionToListItem(SPListItem item, IDictionary<string, string> parameters)
                {
                    bool res = false;
                    try
                    {
                        RunSite.ByWeb(item.ParentList.ParentWeb, (site, web) =>
                        {
                            SPListItem _item = web.GetList(item.ParentList.DefaultViewUrl).Items.GetItemById(item.ID);

                            _item.BreakRoleInheritance(false);
                            while (_item.RoleAssignments.Count > 0)
                            {
                                _item.RoleAssignments.Remove(0);
                            }

                            if (parameters.ContainsKey("UsersIDAndRole"))
                            {
                                string users = parameters["UsersIDAndRole"];
                                string[] fields = users.Split(',');
                                foreach (string field in fields)
                                {
                                    int userID = Int32.Parse(field.Split('#')[0]);
                                    SPUser user = web.AllUsers.GetByID(userID);
                                    string roleName = Convert.ToString(field.Split('#')[1]);
                                    SPRoleAssignment roleassignment_user = new SPRoleAssignment(user);
                                    SPRoleDefinition roleDefinition = web.RoleDefinitions.Cast<SPRoleDefinition>().FirstOrDefault(r => r.Name == roleName);
                                    roleassignment_user.RoleDefinitionBindings.Add(roleDefinition);
                                    _item.RoleAssignments.Add(roleassignment_user);
                                }
                            }
                            if (parameters.ContainsKey("GroupsAndRole"))
                            {
                                string groups = parameters["GroupsAndRole"];
                                string[] fields = groups.Split(',');
                                foreach (string field in fields)
                                {
                                    string[] pField = field.Split('#');
                                    string groupName = pField[0];
                                    SPGroup group = web.SiteGroups[groupName];

                                    string roleName = Convert.ToString(pField[1]);
                                    if (group == null)
                                    {
                                        UlsLogging.LogInformation("On site {0} not found group {1}", web.Url, field);
                                    }
                                    else
                                    {
                                        SPRoleAssignment roleassignment_user = new SPRoleAssignment(group);
                                        SPRoleDefinition roleDefinition = web.RoleDefinitions.Cast<SPRoleDefinition>().FirstOrDefault(r => r.Name == roleName);
                                        roleassignment_user.RoleDefinitionBindings.Add(roleDefinition);
                                        _item.RoleAssignments.Add(roleassignment_user);
                                    }
                                }
                            }
                            _item.Update();
                            res = true;

                        });

                    }
                    catch (Exception ex)
                    {
                        UlsLogging.LogError("ListItem. GetWorkflowItemHistory(SPListItem item). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                    }
                    return res;
                }
            }
        }

        public class ListField
        {
            public static void AddFieldToList(SPList list, string fieldName)
            {
                try
                {
                    list.ParentWeb.AllowUnsafeUpdates = true;
                    SPField lField = list.Fields.GetFieldByInternalName(fieldName);
                    if (lField != null)
                    {
                        bool del = RemoveField(lField);
                        SPField field = list.ParentWeb.Fields.GetFieldByInternalName(fieldName);
                        list.Fields.Add(field);
                        list.Update();
                    }

                    list.ParentWeb.AllowUnsafeUpdates = false;
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. AddFieldToList(SPList list, string fieldName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }

            /// <summary>
            /// fieldsName example: "Name1,Name2,Name3"
            /// </summary>
            /// <param name="list"></param>
            /// <param name="fieldsName"></param>
            public static void AddFieldsToList(SPList list, string fieldsName)
            {
                try
                {
                    var fields = fieldsName.Split(',');
                    foreach (string fieldName in fields)
                    {
                        AddFieldToList(list, fieldName);
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. AddFieldsToList(SPList list, string fieldsName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }

            public static void GetFieldsInListToLog(SPList list)
            {
                try
                {
                    SPFieldCollection lFields = list.Fields;
                    foreach (SPField field in lFields)
                    {
                        UlsLogging.LogInformation("    ListField GetFieldsInList(SPList list) field: {0}", field.InternalName);
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. GetFieldsInListToLog(SPList list) Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }

            public static string GetFieldsInternalNameInList(SPList list)
            {
                string res = "";
                try
                {
                    SPFieldCollection lFields = list.Fields;
                    foreach (SPField field in lFields)
                    {
                        res += field.InternalName + ",";
                    }
                    res = res.Substring(0, res.Length - 1);
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. GetFieldsInListToLog(SPList list) Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return res;
            }

            public static List<SPField> GetFieldsInList(SPList list)
            {
                List<SPField> fList = new List<SPField>();
                SPFieldCollection lFields = list.Fields;
                foreach (SPField field in lFields)
                {
                    fList.Add(field);
                }
                return fList;
            }

            public static Dictionary<string, string> GetFieldsItemValue(SPListItem item)
            {
                Dictionary<string, string> fCol = new Dictionary<string, string>();
                try
                {
                    foreach (SPField f in item.Fields)
                    {
                        fCol.Add(f.StaticName, Convert.ToString(item[f.StaticName]));
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. GetFieldsItemValue(SPListItem item). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return fCol;
            }

            public static bool UpdateFields(SPWeb cWeb, IDictionary<string, string> parameters)
            {
                bool res = false;
                try
                {
                    RunSite.RootSiteByUrl(cWeb.Url, (site, web) =>
                    {
                        web.AllowUnsafeUpdates = true;
                        string fieldNames = parameters["FieldsName"];
                        var fields = fieldNames.Split(',');
                        foreach (var fieldName in fields)
                        {
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                SPField webField = null;
                                #region Get field sitecolunm
                                foreach (SPField f in web.Fields)
                                {
                                    if (f.InternalName == fieldName)
                                    {
                                        webField = f;
                                        break;
                                    }
                                }
                                #endregion
                                if (parameters.ContainsKey("ListPath") && parameters.ContainsKey("ContentTypeName"))
                                {
                                    SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                                    UpdateListContentType(webField, list, parameters["ContentTypeName"]);
                                }
                            }
                        }

                        web.AllowUnsafeUpdates = false;
                    });
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. UpdateFieldInContentType(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);

                }
                return res;
            }

            public static bool UpdateWebContentType(SPField field, string contentType)
            {
                bool res = false;

                return res;
            }

            public static bool UpdateListContentType(SPField field, SPList list, string contentType)
            {
                bool res = false;
                SPContentType ct = list.ContentTypes[contentType];
                foreach (SPField f in ct.Fields)
                {
                    if (f.InternalName == field.InternalName)
                    {
                        f.SchemaXml = field.SchemaXml;
                        f.Update();
                        break;
                    }
                }
                ct.Update();
                return res;
            }

            public static bool UpdateListField(SPField field, SPList list)
            {
                bool res = false;
                foreach (SPField f in list.Fields)
                {
                    if (f.InternalName == field.InternalName)
                    {
                        f.SchemaXml = field.SchemaXml;
                        f.Update();
                        break;
                    }
                }
                list.Update();
                return res;
            }

            public static void RebuildFieldsChoiceInList(SPWeb web, IDictionary<string, string> parameters)
            {
                try
                {
                    string fieldNames = parameters["FieldsName"];
                    var fields = fieldNames.Split(',');
                    foreach (var fieldName in fields)
                    {
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            web.AllowUnsafeUpdates = true;
                            #region in list
                            if (parameters.ContainsKey("ListPath"))
                            {
                                SPFieldChoice field = (SPFieldChoice)web.Fields.GetFieldByInternalName(fieldName);
                                string wChoiceFieldSchema = field.SchemaXml;

                                SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                                SPFieldChoice lField = (SPFieldChoice)list.Fields.GetFieldByInternalName(fieldName);
                                string lChoiceFieldSchema = lField.SchemaXml;

                                lField.Choices.Clear();
                                foreach (string c in field.Choices)
                                {
                                    lField.Choices.Add(c);
                                }

                                lField.Update();
                            }
                            #endregion
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. RebuildFieldsChoiceInList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }

            public static bool RemoveField(SPField spField)
            {
                bool res = false;
                try
                {
                    if (spField == null)
                    {
                        return res;
                    }
                    // check if it's a ReadOnly field.
                    // if so, reset it
                    if (spField.ReadOnlyField)
                    {
                        spField.ReadOnlyField = false;
                        spField.Update();
                    }

                    // check if it's a Hidden field.
                    // if so, reset it
                    if (spField.Hidden)
                    {
                        spField.Hidden = false;
                        spField.Update();
                    }

                    // check if the AllowDeletion property is set to false.
                    // if so, reset it to true
                    if (spField.AllowDeletion == null || !spField.AllowDeletion.Value)
                    {
                        spField.AllowDeletion = true;
                        spField.Update();
                    }

                    // finally, remove the field
                    spField.Delete();
                    spField.ParentList.Update();

                    res = true;
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. RemoveField(SPField spField). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }

                return res;
            }

            public static bool RemoveFieldsFromList(SPWeb web, IDictionary<string, string> parameters)
            {
                bool res = false;
                try
                {
                    string fieldNames = parameters["FieldName"];
                    var fields = fieldNames.Split(',');
                    foreach (var fieldName in fields)
                    {
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            web.AllowUnsafeUpdates = true;
                            #region in list
                            if (parameters.ContainsKey("ListPath"))
                            {
                                SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                                SPField lField = list.Fields.GetFieldByInternalName(fieldName);
                                res = RemoveField(lField);
                            }
                            #endregion
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListField. RemoveFieldsFromList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return res;
            }

            public static void UpdateRequiredFieldsInList(SPWeb web, IDictionary<string, string> parameters)
            {
                try
                {
                    web.AllowUnsafeUpdates = true;
                    if (parameters.ContainsKey("ListPath"))
                    {
                        SPFieldCollection webFields = web.Fields;

                        SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                        SPFieldCollection listFields = web.Fields;

                        string fieldNames = parameters["FieldsName"];
                        var fields = fieldNames.Split(',');
                        foreach (var fieldName in fields)
                        {
                            foreach (SPField f in webFields)
                            {
                                if (fieldName == f.InternalName)
                                {
                                    foreach (SPField fList in webFields)
                                    {
                                        if (fieldName == fList.InternalName)
                                        {
                                            fList.Required = f.Required;
                                            fList.Update();
                                            list.Update();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    web.AllowUnsafeUpdates = false;
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("Zeppelin.ZIA.Applications.Classes.eCommon. UpdateRequiredFieldsInList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }
        }

        public class ListViews
        {
            public static List<SPView> GetAllViewsList(SPWeb web, IDictionary<string, string> parameters)
            {
                List<SPView> views = null;
                try
                {
                    SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                    foreach (SPView view in list.Views)
                    {
                        views.Add(view);
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListViews. RemoveFieldFromAllViewsList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return views;
            }

            public static bool AddFieldToAllViewsList(SPWeb web, IDictionary<string, string> parameters)
            {
                bool res = false;
                web.AllowUnsafeUpdates = true;
                try
                {
                    if (parameters.ContainsKey("ListPath"))
                    {
                        if (parameters.ContainsKey("FieldsName"))
                        {
                            SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                            SPViewCollection views = list.Views;

                            string fieldNames = parameters["FieldsName"];
                            var fields = fieldNames.Split(',');
                            foreach (var fieldName in fields)
                            {
                                if (!string.IsNullOrEmpty(fieldName))
                                {
                                    foreach (SPView view in views)
                                    {
                                        SPViewFieldCollection viewFields = view.ViewFields;
                                        StringCollection fieldColumnNames = viewFields.ToStringCollection();
                                        bool usedFieldInView = false;
                                        foreach (String columnName in fieldColumnNames)
                                        {
                                            if (columnName == fieldName) usedFieldInView = true;
                                        }

                                        if (usedFieldInView) break;

                                        view.ViewFields.Add(fieldName);
                                        view.Update();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListViews. AddFieldToAllViewsList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                web.AllowUnsafeUpdates = false;
                return res;
            }

            public static bool RemoveFieldFromAllViewsList(SPWeb web, IDictionary<string, string> parameters)
            {
                bool res = false;
                web.AllowUnsafeUpdates = true;
                try
                {
                    SPList list = web.GetList(SPUrlUtility.CombineUrl(web.ServerRelativeUrl, parameters["ListPath"]));
                    SPViewCollection views = list.Views;
                    foreach (SPView view in views)
                    {
                        string fieldNames = parameters["FieldName"];
                        var fields = fieldNames.Split(',');
                        foreach (var fieldName in fields)
                        {
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                SPViewFieldCollection viewFields = view.ViewFields;
                                StringCollection fieldColumnNames = viewFields.ToStringCollection();
                                bool usedFieldInView = false;
                                foreach (String columnName in fieldColumnNames)
                                {
                                    if (columnName == fieldName) usedFieldInView = true;
                                }

                                if (!usedFieldInView) break;

                                view.ViewFields.Delete(fieldName);
                                view.Update();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("ListViews. RemoveFieldFromAllViewsList(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                web.AllowUnsafeUpdates = false;
                return res;
            }
        }

        public class SiteUserInfoList
        {
            public static void GetSPUserProfileInfoToLog(SPWeb web, int userID)
            {
                try
                {
                    SPUser spUser = web.AllUsers.GetByID(userID);
                    UlsLogging.LogInformation("SPUser LoginName: {0}", spUser.LoginName);
                    SPList userList = web.SiteUserInfoList;
                    SPListItem userItem = userList.GetItemById(userID);
                    foreach (SPField f in userList.Fields)
                    {
                        UlsLogging.LogInformation("SiteUserInfoList InternalName: {0}, Title: {1}, Value: {}", f.InternalName, f.Title, Convert.ToString(userItem[f.InternalName]));
                    }

                    SPServiceContext serviceContext = SPServiceContext.GetContext(web.Site);
                    UserProfileManager userProfileMgr = new UserProfileManager(serviceContext);
                    UserProfile userProfile = userProfileMgr.GetUserProfile(spUser.LoginName);
                    ProfileSubtypePropertyManager pspm = userProfileMgr.DefaultProfileSubtypeProperties;
                    foreach (ProfileSubtypeProperty prop in pspm.PropertiesWithSection)
                    {
                        if (prop.IsSection)
                            UlsLogging.LogInformation("SiteUserInfoList DisplayName: {0}, Title: {1}, Value: {}", prop.DisplayName);
                        else
                        {
                            UlsLogging.LogInformation("SiteUserInfoList Name: {0}, DisplayName: {1}, Value: {}", prop.Name, prop.DisplayName, userProfile[prop.Name].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("SiteUserInfoList. GetSPUserProfileInfoToLog(SPWeb web, int userID). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
            }
        }

        public class WorkflowHistory
        {
            public string DateOccurred { get; set; }
            public string EventType { get; set; }
            public string Initianor { get; set; }
            public string Description { get; set; }
            public string Outcome { get; set; }
        }

        public class Permission
        {
            public static bool AddGroupRole(SPList list, string groupName, SPRoleType role)
            {
                bool res = false;
                try
                {
                    SPGroupCollection groups = list.ParentWeb.SiteGroups;
                    foreach (SPGroup gr in groups)
                    {
                        if (gr.Name == groupName) return true;
                    }
                    SPGroup group = GroupManager.CreateGroup(list.ParentWeb, groupName);
                    if (group == null) return res;

                    SPRoleAssignment roleAssignmentAdmin = new SPRoleAssignment((SPPrincipal)group);
                    SPRoleDefinition roleAdmin = list.ParentWeb.RoleDefinitions.GetByType(role);
                    roleAssignmentAdmin.RoleDefinitionBindings.Add(roleAdmin);
                    list.RoleAssignments.Add(roleAssignmentAdmin);
                    list.Update();
                    res = true;
                }
                catch (Exception ex)
                {
                    UlsLogging.LogError("AddGroupRole(SPList list, string groupName, SPRoleType role).  Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                }
                return res;
            }
        }

        public class Form
        {
            public static void SetFormJSLink(SPList list, PAGETYPE formBaseView, string jsLinkUrl)
            {
                var file = list.ParentWeb.GetFile(list.Forms[formBaseView].Url);
                file.CheckOut();

                using (var manager = file.GetLimitedWebPartManager(PersonalizationScope.Shared))
                {
                    var webPart = manager.WebParts.OfType<ListFormWebPart>().FirstOrDefault();
                    if (webPart != null)
                    {
                        webPart.JSLink = jsLinkUrl ?? string.Empty;
                        manager.SaveChanges(webPart);
                    }
                }

                file.CheckIn("Added JSLink to the Form");
            }
        }

        //public static string SPQueryBuilder() {
        //    string query = "";


        //    return query;
        //}
        //public static string SPQueryRow(string) {
        //    string row = "";


        //    return row;
        //}


    }
}