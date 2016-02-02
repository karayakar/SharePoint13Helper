using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Zeppelin.ShP.Helper.Managers;

namespace ShP.Helper.Testing.WebParts.UserInfo
{
    [ToolboxItemAttribute(false)]
    public partial class UserInfo : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public UserInfo()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }


        SPWeb cWeb = SPContext.Current.Web;
        protected void Page_Load(object sender, EventArgs e)
        {

            //Dictionary<string, string> managers = new Dictionary<string, string>();
            //managers.Add("key1", "value1");
            //managers.Add("key2", "value2");
            //managers.Add("key3", "value3");

            //for(int i = 1; i <= 5; i++){
            //    string email = "key" + i;
            //    string localName = "value" + i;
            //    UlsLogging.LogInformation("key: {0}", email);
            //    if (!managers.ContainsKey(email))
            //    {
            //        UlsLogging.LogInformation("GetManagerLeaners !ContainsKey");
            //        managers.Add(email, localName);
            //        UlsLogging.LogInformation("GetManagerLeaners added: email: {0}, iUserLearner: {1}", email, localName);
            //    }
            //    else
            //    {
            //        UlsLogging.LogInformation("GetManagerLeaners ContainsKey");
            //        string names = managers[email] + localName + ",";
            //        UlsLogging.LogInformation("GetManagerLeaners names: {0}", names);
            //        managers[email] = names;
            //        UlsLogging.LogInformation("GetManagerLeaners update: email: {0}, names: {1}", email, names);
            //    }
            //}
            //UlsLogging.LogInformation("GetManagerLeaners managers: {0}", managers.Count);
            //foreach (KeyValuePair<string, string> item in managers)
            //{
            //    UlsLogging.LogInformation("item email(key): {0}; DisplayName: {1}", item.Key, item.Value);
            //}





            SPUser cUser = cWeb.CurrentUser;
            #region Get current user info
            LabelCurrentUser.Text += "<br/>CurrentUser LoginName: " + cUser.LoginName;
            LabelCurrentUser.Text += "<br/>CurrentUser UserID: " + cUser.ID;

            LabelCurrentUser.Text += "<br/><br/><strong>UserInfoList</strong>";

            LabelCurrentUser.Text += "<br/>" + UserInfoManager.GetUserInfoList(cWeb, cUser.ID);

            LabelCurrentUser.Text += "<br/><br/><strong>UserProfile</strong>";

            LabelCurrentUser.Text += "<br/>" + UserInfoManager.GetUserProfile(cWeb, cUser.LoginName);

            LabelCurrentUser.Text += "<br/><br/><strong>UsersInWeb</strong>";
            LabelCurrentUser.Text += "<br/>" + UserInfoManager.GetUsersInWeb(cWeb);

            #endregion


        }

        protected void GetUserInfoButton_Click(object sender, EventArgs e)
        {
            string sUserID = UserID.Value;
            string userLogin = UserLogin.Value;

            LabelCurrentUser.Text += "<br/>User LoginName: " + userLogin;
            LabelCurrentUser.Text += "<br/>User UserID: " + sUserID;

            int userID = 0;
            if (Int32.TryParse(sUserID, out userID))
                if (userID != 0)
                {
                    LabelUser.Text += "<br/><br/><strong>UserInfoList</strong>";
                    LabelUser.Text += "<br/>" + UserInfoManager.GetUserInfoList(cWeb, userID);
                }

            LabelUser.Text += "<br/><br/><strong>UserProfile</strong>";
            LabelUser.Text += "<br/>" + UserInfoManager.GetUserProfile(cWeb, userLogin);
        }
    }
}
