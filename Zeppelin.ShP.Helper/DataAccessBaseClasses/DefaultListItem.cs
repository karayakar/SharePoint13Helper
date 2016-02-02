using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.AdditionalStructures;
using Zeppelin.ShP.Helper.Constants;
using Zeppelin.ShP.Helper.DataAccessBaseClasses;
using Zeppelin.ShP.Helper.Enums;
using Zeppelin.ShP.Helper.Extenders;
using Zeppelin.ShP.Helper.Helpers;


namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    public class DefaultListItem : OriginInfo //: ItemRecordWithAttachments
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public UserData CreatedBy { get; set; }
        public DateTime Modified { get; set; }
        public UserData ModifiedBy { get; set; }
        public string ContentTypeId { get; set; }
        public Dictionary<string, string> ContentTypes { get; set; }
        public string ContentType { get; set; }
        public Guid GUID { get; set; }
        public SPModerationStatusType ModerationStatus { get; set; }
        public string ModerationComments { get; set; }

        //public Guid ListID { get; set; }
        //public string ListTitle { get; set; }
        //public string ListLocalTitle { get; set; }

        //public Guid WebID { get; set; }
        //public string WebUrl { get; set; }

        public object UIVersionString { get; set; }

        public void GetSPListItemData(SPListItem spItem)
        {
            //base.Init(spItem, AttachmentsLoadMode.Short);
            ID = spItem.GetFieldIntValue(ListItemDefaultFields.ID);
            Title = spItem.GetFieldStringValue(ListItemDefaultFields.Title);
            GUID = spItem.GetFieldGuidValue(ListItemDefaultFields.GUID);
            Created = spItem.GetFieldDateTimeValue(ListItemDefaultFields.Created);
            CreatedBy = spItem.GetFieldUserValueOrEmpty(ListItemDefaultFields.CreatedBy);
            Modified = spItem.GetFieldDateTimeValue(ListItemDefaultFields.Modified);
            ModifiedBy = spItem.GetFieldUserValueOrEmpty(ListItemDefaultFields.ModifiedBy);
            ContentTypeId = Convert.ToString(spItem.ContentTypeId);
            ContentType = spItem.ContentType.Name;
            SPContentTypeCollection ctsParentList = spItem.ParentList.ContentTypes;
            foreach (SPContentType ct in ctsParentList)
            {
                UlsLogging.LogInformation("SPContentType Name: {0} Id: {1}", ct.Name, Convert.ToString(ct.Id));
                //ContentTypes.Add(ct.Name, Convert.ToString(ct.Id));
            }
            if (spItem.ModerationInformation != null)
            {
                ModerationStatus = spItem.ModerationInformation.Status;
                ModerationComments = Convert.ToString(spItem.ModerationInformation.Comment);
            }
            //ListID = spItem.ParentList.ID;
            //ListTitle = spItem.ParentList.Title;
            //WebID = spItem.Web.ID;
            //WebUrl = spItem.Web.Url;
            UIVersionString = spItem[ListItemDefaultFields.UIVersionString];

            OriginInfo OriginInfo = new OriginInfo();
            OriginInfo.GetData(spItem.ParentList);
        }

    }
}
