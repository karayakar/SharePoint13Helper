using Microsoft.SharePoint;
using ShP.Helper.Testing.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Zeppelin.ShP.Helper.Constants;
using Zeppelin.ShP.Helper.DataAccessBaseClasses;
using Zeppelin.ShP.Helper.Helpers;
using Zeppelin.ShP.Helper.Managers;

namespace ShP.Helper.Testing.WebParts.LibraryItemFields
{
    [ToolboxItemAttribute(false)]
    public partial class LibraryItemFields : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public LibraryItemFields()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ListFieldsDataLabel.Text += "<br/>Custom empty list---------------------------------------------------------------------------------------------------<br/>";
                SPWeb cWeb = SPContext.Current.Web;
                ListFieldsDataLabel.Text += "<br/>list: " + Lib.DefaultLibName;
                SPList lib2 = ListManager.GetListInfoByListPath(cWeb, Lib.DefaultLibName);
                ListFieldsDataLabel.Text += "<br/>list: " + lib2.Title;
                SPFieldCollection fColl = lib2.Fields;
                ListFieldsDataLabel.Text += "<br/>fColl: " + fColl.Count;
                int i = 1;
                foreach (SPField f in fColl)
                {
                    ListFieldsDataLabel.Text += "<br/>" + i + ". InternalName: <strong>" + f.InternalName + "</strong> Type: <strong>" + f.Type + "</strong> Value: <strong>" + lib2.Items[0][f.InternalName] + "</strong>";
                    i++;
                }
                ListFieldsDataLabel.Text += "<br/>--------------------------------------------------------------------------------------------------------------------<br/>";

                //string siteUrl = cWeb.Site.WebApplication.Sites[0].Url + "/sites/zia2";
                //string webRelativeUrl = siteUrl;// +"/sub";
                RunSite.ByWeb(cWeb, (site, web) =>
                {
                    SPList lib = ListManager.GetListInfoByListPath(web, Lib.DefaultLibName);
                    SPListItemCollection items = lib.Items;
                    ListItemDataLabelInfo.Text = "<br/>items.Count: " + items.Count;
                    List<DefaultLibItem> iList = new List<DefaultLibItem>();
                    foreach (SPListItem item in items)
                    {
                        DefaultLibItem dItem = new DefaultLibItem();
                        dItem.GetSPLibItemData(item);
                        //dItem.LoadItemData(item);
                        iList.Add(dItem);
                    }
                    string data = Common.Jss.Serialize(iList);
                    ListItemDataLabel.Text = data;
                });

            }
            catch (Exception ex)
            {
                ErrorLabel.Text = "Message: " + ex.Message + "<br/>StackTrace: " + ex.StackTrace;
                UlsLogging.LogError("ListItemFields. Page_Load. Message: {0}; StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
