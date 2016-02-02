using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Constants;
using Zeppelin.ShP.Helper.Extenders;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    public class DefaultLibItem : DefaultListItem
    {
        public string FileRef { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        //public string FileRelUrl { get; set; }
        public string FileAbsUrl { get; set; }
        public string DocumentIcon { get; set; }
        public UserData CheckoutUser { get; set; }
        public DateTime CheckoutDate { get; set; }
        public object CheckOutStatus { get; set; }
        public string CheckinComment { get; set; }

        public void GetSPLibItemData(SPListItem spItem)
        {
            base.GetSPListItemData(spItem);
            FileName = spItem.File.Name;
            FileSize = spItem.File.Length;
            FileUrl = spItem.File.ServerRelativeUrl;
            //FileRelUrl = spItem.File.Url;
            FileAbsUrl = Convert.ToString(spItem[LibraryItemDefaultFields.EncodedAbsUrl]);
            if (string.IsNullOrEmpty(Title))
            {
                Title = this.FileName;
            }
            CheckOutStatus = spItem.File.CheckOutStatus;
            if ((int)CheckOutStatus != 0)
            {
                CheckoutUser = new UserData(spItem.File.CheckedOutByUser);
                CheckoutDate = spItem.File.CheckedOutDate;
            }
            
            CheckinComment = spItem.File.CheckInComment;

            //DocumentIcon = SPUtility.MapToIcon(spItem.Web, this.FileName, string.Empty);



            //FileRef = spItem.File.Url;// spItem.GetFieldStringValue(LibraryItemDefaultFields.FileRef);
            //FileSize = spItem.File.Length;//.GetFieldStringValue(LibraryItemDefaultFields.FileSize);
            //FileName = spItem.File.Name;// spItem.GetFieldStringValue(LibraryItemDefaultFields.LinkFilename);
            //EncodedAbsUrl = Convert.ToString(spItem[LibraryItemDefaultFields.EncodedAbsUrl]);
            //ServerUrl = spItem.GetFieldStringValue(LibraryItemDefaultFields.ServerUrl);
            //DocIcon = spItem.GetFieldStringValue(LibraryItemDefaultFields.DocIcon);
            //FileInfo = spItem.File;//.GetFieldUserValueOrEmpty(LibraryItemDefaultFields.CheckoutUser);
            //CheckinComment = spItem.GetFieldStringValue(LibraryItemDefaultFields.CheckinComment);
        }
    }
}
