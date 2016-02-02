using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Helpers;
using Zeppelin.ShP.Helper.Managers;

namespace Zeppelin.ShP.Helper.AdditionalStructures
{
    /// <summary>
    /// Class to store information about original location of item
    /// </summary>
    [Serializable]
    public class OriginInfo
    {
        public OriginInfo()
        {
        }

        public OriginInfo(SPList list)
        {
            UlsLogging.LogInformation("OriginInfo(SPList list)");
            this.SiteId = list.ParentWeb.Site.ID;
            this.WebId = list.ParentWeb.ID;
            this.WebName = list.ParentWeb.Title;
            this.WebUrl = list.ParentWeb.ServerRelativeUrl;
            this.ListId = list.ID;
            this.ListName = list.Title;
            UlsLogging.LogInformation("OriginInfo(SPList list) 1");
        }

        public void GetData(SPList list)
        {
            UlsLogging.LogInformation("OriginInfo(SPList list)");
            this.SiteId = list.ParentWeb.Site.ID;
            this.WebId = list.ParentWeb.ID;
            this.WebName = list.ParentWeb.Title;
            this.WebUrl = list.ParentWeb.ServerRelativeUrl;
            this.ListId = list.ID;
            this.ListName = list.Title;
            UlsLogging.LogInformation("OriginInfo(SPList list) 1");
        }

        public string NewFormURL
        {
            get
            {
                return "";// UrlHelper.CombineUrl(this.WebUrl, string.Format("/_layouts/listform.aspx?PageType=8&ListId={0}", this.ListId));
            }
        }

        public Guid SiteId { get; protected set; }
        public Guid WebId { get; protected set; }
        public string ListName { get; protected set; }
        public Guid ListId { get; protected set; }
        public string WebUrl { get; protected set; }
        public string WebName { get; protected set; }

        //public ItemRecordOriginInfo(IDataLoadManager DataLoadManager)
        //{
        //    readOnly = true;
        //    _webUrl = DataLoadManager.WebUrl;
        //    _webName = DataLoadManager.WebName;
        //    _listId = DataLoadManager.ListId;
        //}

        //private string _webUrl;
        //public string WebUrl 
        //{ 
        //    get
        //    {
        //            return _webUrl;
        //    }
        //    set
        //    {
        //        if (!readOnly)
        //        {
        //            _webUrl = value;
        //        }
        //        else
        //        {
        //            throw new ApplicationException("This opertation is not supported for items returned using Data Manager");
        //        }
        //    }
        //}

        //private string _webName;
        //public string WebName
        //{
        //    get
        //    {
        //        return _webName;
        //    }
        //    set
        //    {
        //        if (!readOnly)
        //        {
        //            _webName = value;
        //        }
        //        else
        //        {
        //            throw new ApplicationException("This opertation is not supported for items returned using Data Manager");
        //        }
        //    }
        //}

        //private Guid _listId;
        //public Guid ListId 
        //{
        //    get
        //    {
        //        return _listId;
        //    }
        //    set
        //    {
        //        if (!readOnly)
        //        {
        //            _listId = value;
        //        }
        //        else
        //        {
        //            throw new ApplicationException("This opertation is not supported for items returned using Data Manager");
        //        }
        //    }
        //}
    }
}
