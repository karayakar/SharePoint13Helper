using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.Managers
{
    public class GroupManager
    {
        public static SPGroup CheckGroupInWebByName(SPWeb web, string groupName)
        {
            SPGroup res = null;
            try
            {
                SPGroupCollection collGroups = web.SiteGroups;
                foreach (SPGroup oGroup in collGroups)
                {
                    if (oGroup.Name == groupName)
                    {
                        res = oGroup;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. CheckGroupInWebByName(SPWeb web, string groupName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static SPGroup GetGroupInWebByName(SPWeb web, string groupName)
        {
            SPGroup res = null;
            try
            {
                SPGroupCollection collGroups = web.SiteGroups;
                foreach (SPGroup oGroup in collGroups)
                {
                    if (oGroup.Name == groupName)
                    {
                        res = oGroup;
                    }
                    else
                    {
                        UlsLogging.LogError("GetGroupInWebByName(SPWeb web, string grName). SPGroup " + groupName + " not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. GetGroupInWebByName(SPWeb web, string grName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static SPGroup GetGroupInWebByID(SPWeb web, int groupID)
        {
            SPGroup res = null;
            try
            {
                SPGroupCollection collGroups = web.SiteGroups;
                foreach (SPGroup oGroup in collGroups)
                {
                    if (oGroup.ID == groupID)
                    {
                        res = oGroup;
                    }
                    else
                    {
                        UlsLogging.LogError("GetGroupInWebByName(SPWeb web, int groupID). SPGroup " + groupID + " not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. GetGroupInWebByName(SPWeb web, string grName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static SPGroup CreateGroup(SPWeb web, string groupName)
        {
            SPGroup res = null;
            try
            {
                res = CheckGroupInWebByName(web, groupName);
                if (res == null)
                {
                    web.SiteGroups.Add(groupName, web.CurrentUser, web.CurrentUser, groupName);
                    web.Update();
                    res = CheckGroupInWebByName(web, groupName);
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. CreateGroup(SPWeb web, string groupName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static SPGroup CreateGroup(SPWeb web, IDictionary<string, string> parameters)
        {
            SPGroup res = null;
            try
            {
                if (parameters.ContainsKey("GroupName"))
                {
                    string groupName = parameters["GroupName"];
                    res = CheckGroupInWebByName(web, groupName);
                    if (res == null)
                    {
                        SPUser user = web.CurrentUser;
                        if (parameters.ContainsKey("UserLogin"))
                            user = web.SiteUsers[parameters["UserLogin"]];

                        string description = groupName;
                        if (parameters.ContainsKey("GroupDescription"))
                            description = parameters["GroupDescription"];

                        web.SiteGroups.Add(groupName, user, user, description);
                        web.Update();
                        res = CheckGroupInWebByName(web, groupName);
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. CreateGroup(SPWeb web, IDictionary<string, string> parameters). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static bool AddUserToGroup(SPWeb cWeb, string groupName, int userID)
        {
            bool res = false;
            try
            {
                RunSite.ByUrl(cWeb.Url, (site, web) =>
                {
                    SPUser spUser = web.AllUsers.GetByID(userID);
                    SPGroup spGroup = web.SiteGroups[groupName];
                    if (spGroup != null)
                    {
                        spGroup.AddUser(spUser);
                        spGroup.Update();
                        res = true;
                    }
                });
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. AddUserToGroup(SPWeb cWeb, string grName, int userID). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static bool AddUserToGroup(SPWeb cWeb, string groupName, string userLogin)
        {
            bool res = false;
            try
            {
                RunSite.ByUrl(cWeb.Url, (site, web) =>
                {
                    SPUser spUser = web.AllUsers[userLogin];
                    SPGroup spGroup = web.SiteGroups[groupName];
                    if (spGroup != null)
                    {
                        spGroup.AddUser(spUser);
                        spGroup.Update();
                        res = true;
                    }
                });
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. AddUserToGroup(SPWeb cWeb, string groupName, string userLogin). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static bool RemoveUserFromGroup(SPWeb cWeb, string groupName, int userID)
        {
            bool res = false;
            try
            {
                RunSite.ByUrl(cWeb.Url, (site, web) =>
                {
                    SPUser spUser = web.AllUsers.GetByID(userID);
                    SPGroup spGroup = web.SiteGroups[groupName];
                    if (spGroup != null)
                    {
                        spGroup.RemoveUser(spUser);
                        spGroup.Update();
                        res = true;
                    }
                });
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. RemoveUserFromGroup(SPWeb cWeb, string groupName, int userID). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static bool RemoveUserFromGroup(SPWeb cWeb, string groupName, string userLogin)
        {
            bool res = false;
            try
            {
                RunSite.ByUrl(cWeb.Url, (site, web) =>
                {
                    SPUser spUser = web.AllUsers[userLogin];
                    SPGroup spGroup = web.SiteGroups[groupName];
                    if (spGroup != null)
                    {
                        spGroup.RemoveUser(spUser);
                        spGroup.Update();
                        res = true;
                    }
                });
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. RemoveUserFromGroup(SPWeb cWeb, string groupName, string userLogin). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }

            return res;
        }

        public static bool GetUserInGroupByUserID(SPWeb web, string groupName, int userID)
        {
            bool res = false;
            try
            {
                SPUser spUser = web.AllUsers.GetByID(userID);
                SPGroupCollection userGroups = spUser.Groups;
                foreach (SPGroup group in userGroups)
                {
                    if (group.Name == groupName)
                        res = true;
                }

            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. GetUserInGroup(SPWeb web, string groupName, int userID). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                res = false;
            }
            return res;
        }

        public static bool GetUserInGroupByUserLogin(SPWeb web, string groupName, int userLogin)
        {
            bool res = false;
            try
            {
                SPUser spUser = web.AllUsers[userLogin];
                SPGroupCollection userGroups = spUser.Groups;
                foreach (SPGroup group in userGroups)
                {
                    if (group.Name == groupName)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("GetUserInGroup. Помилка. . Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
                res = false;
            }
            return res;
        }

        public static List<SPUser> GetUsersInGroup(SPWeb web, string groupName)
        {
            List<SPUser> uList = new List<SPUser>();
            try
            {
                SPGroup group = web.SiteGroups[groupName];
                foreach (SPUser user in group.Users)
                {
                    uList.Add(user);
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. GetUsersInGroup(SPWeb web, string grName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return uList;
        }

        public static bool RemoveGroupByName(SPWeb web, string groupName)
        {
            bool res = false;
            try
            {
                SPGroupCollection collGroups = web.SiteGroups;
                foreach (SPGroup oGroup in collGroups)
                {
                    if (oGroup.Name == groupName)
                    {
                        collGroups.Remove(groupName);
                        res = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. RemoveGroupByName(SPWeb web, string groupName). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

        public static bool RemoveGroupByName(SPWeb web, int groupID)
        {
            bool res = false;
            try
            {
                SPGroupCollection collGroups = web.SiteGroups;
                foreach (SPGroup oGroup in collGroups)
                {
                    if (oGroup.ID == groupID)
                    {
                        collGroups.Remove(groupID);
                        res = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("Group. RemoveGroupByName(SPWeb web, int groupID). Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }
    }

    public class ListGroup 
    {
        public static bool CreateGroupForListWithRole(SPList list, string groupName, SPRoleType role)
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
                UlsLogging.LogError("ListGroup. CreateGroupForListWithRole(SPList list, string groupName, SPRoleType role) Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            return res;
        }

    }
}
