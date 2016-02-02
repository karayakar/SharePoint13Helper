using Microsoft.SharePoint;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Zeppelin.ShP.Helper.Helpers;
using ShP.Helper.Testing.Constants;
using Microsoft.SharePoint.Utilities;
using Zeppelin.ShP.Helper.Managers;
using Zeppelin.ShP.Helper.Constants;
using System.Collections.Generic;
using Zeppelin.ShP.Helper.DataAccessBaseClasses;

namespace ShP.Helper.Testing.WebParts.ListItemFields
{
    [ToolboxItemAttribute(false)]
    public partial class ListItemFields : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ListItemFields()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            SPWeb cWeb = SPContext.Current.Web;
            ListFieldsDataLabel.Text += "<br/>Custom empty list---------------------------------------------------------------------------------------------------<br/>";
            try
            {
                ListFieldsDataLabel.Text += "<br/>list: " + List.Default2ListPath;
                SPList list2 = ListManager.GetListInfoByListPath(cWeb, List.Default2ListPath);
                ListFieldsDataLabel.Text += "<br/>list: " + list2.Title;
                SPFieldCollection fColl = list2.Fields;
                ListFieldsDataLabel.Text += "<br/>fColl: " + fColl.Count;
                int i = 1;
                foreach (SPField f in fColl)
                {
                    ListFieldsDataLabel.Text += "<br/>" + i + ". InternalName: <strong>" + f.InternalName + "</strong> Type: <strong>" + f.Type + "</strong> Value: <strong>" + list2.Items[0][f.InternalName] + "</strong>";
                    i++;
                }
            }
            catch (Exception ex)
            {
                ListFieldsDataLabelError.Text = "Message: " + ex.Message + "<br/>StackTrace: " + ex.StackTrace;
                UlsLogging.LogError("ListItemFields. Page_Load. Message: {0}; StackTrace: {1}", ex.Message, ex.StackTrace);
            }
            ListFieldsDataLabel.Text += "<br/>--------------------------------------------------------------------------------------------------------------------<br/>";
            try
            {
                //string siteUrl = cWeb.Site.WebApplication.Sites[0].Url + "/sites/zia2";
                //string webRelativeUrl = siteUrl;// +"/sub";
                RunSite.ByWeb(cWeb, (site, web) =>
                {
                    SPList list = ListManager.GetListInfoByListPath(web, List.DefaultListPath);
                    SPListItemCollection items = list.Items;
                    ListItemDataLabelInfo.Text = "<br/>items.Count: " + items.Count;
                    List<Default> iList = new List<Default>();
                    foreach (SPListItem item in items)
                    {
                        Default dItem = new Default(item);
                        //dItem.LoadItemData(item);
                        iList.Add(dItem);
                    }
                    string data = Common.Jss.Serialize(iList);
                    ListItemDataLabel.Text = data;
                });

            }
            catch (Exception ex)
            {
                ListItemDataLabelError.Text = "Message: " + ex.Message + "<br/>StackTrace: " + ex.StackTrace;
                UlsLogging.LogError("ListItemFields. Page_Load. Message: {0}; StackTrace: {1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
