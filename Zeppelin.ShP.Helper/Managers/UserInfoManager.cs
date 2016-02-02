using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.Managers
{
    public class UserInfoManager
    {
        public static string GetUserProfile(SPWeb web, string userName)
        {
            string res = "";
            try
            {
                SPServiceContext serviceContext = SPServiceContext.GetContext(web.Site);
                UserProfileManager userProfileMgr = new UserProfileManager(serviceContext);
                UserProfile userProfile = userProfileMgr.GetUserProfile(userName);
                ProfileSubtypePropertyManager pspm = userProfileMgr.DefaultProfileSubtypeProperties;
                res += "<br/><table><tr><td>DisplayName</td><td>Name</td><td>Value</td></tr>";
                foreach (ProfileSubtypeProperty prop in pspm.PropertiesWithSection)
                {
                    if (prop.IsSection)
                        res += "<tr><td colspan='3'><b>" + prop.DisplayName + "</b></td></tr>";
                    else
                    {
                        res += "<tr><td>" + prop.DisplayName + "</td><td>" + prop.Name + "</td><td>" + userProfile[prop.Name].Value + "</td></tr>";
                    }
                }
                res += "</table>";
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("UserInfo. GetUserProfile. Message {0}. StackTrace {1}", ex.Message, ex.StackTrace);
                res += "<br/><strong>UserInfo. GetUserProfile.</strong> <br/>ex.Message: " + ex.Message + "<br/>ex.StackTrace" + ex.StackTrace;

            }
            return res;
        }

        public static string GetUserInfoList(SPWeb web, int userID)
        {
            string res = "";
            try
            {
                SPList userList = web.SiteUserInfoList;
                SPListItem userItem = userList.GetItemById(userID);
                res += "<br/><table><tr><td>InternalName</td><td>Title</td><td>Value</td></tr>";
                foreach (SPField f in userList.Fields)
                {
                    res += "<tr><td>" + f.InternalName + "</td><td>" + f.Title + "</td><td>" + Convert.ToString(userItem[f.InternalName]) + "</td></tr>";
                }
                res += "</table>";
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("UserInfo. GetUserInfoList. Message {0}. StackTrace {1}", ex.Message, ex.StackTrace);
                res += "<br/><strong>UserInfo. GetUserInfoList.</strong> <br/>ex.Message: " + ex.Message + "<br/>ex.StackTrace" + ex.StackTrace;

            }
            return res;
        }

        public static string GetUsersInWeb(SPWeb web)
        {
            string res = "";
            try
            {
                foreach (SPUser u in web.SiteUsers)
                {
                    res += "<br/>ID: " + u.ID + " LoginName: " + u.LoginName;
                }
            }
            catch (Exception ex)
            {
                UlsLogging.LogError("UserInfo. GetUsersInWeb. Message {0}. StackTrace {1}", ex.Message, ex.StackTrace);
                res += "<br/><strong>UserInfo. GetUsersInWeb.</strong> <br/>ex.Message: " + ex.Message + "<br/>ex.StackTrace" + ex.StackTrace;
            }
            return res;
        }
    }
}
