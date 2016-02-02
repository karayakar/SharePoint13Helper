using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.DataAccessBaseClasses;
using Zeppelin.ShP.Helper.Extenders;


namespace ShP.Helper.Testing.Constants
{
    public class Default : ItemRecordWithAttachments
    {
        public string SingleLineOfText { get; set; }
        public double Number { get; set; }
        public double NumberDecimals { get; set; }
        public string MultipleLinesOfText { get; set; }
        public string MultiRichTextHTML { get; set; }
        public string PublishingHTML { get; set; }

        public LookupValue Lookup { get; set; }
        public List<LookupValue> MultiLookupColumns { get; set; }

        public bool? Boolean { get; set; }

        public Default(SPListItem spItem)
        {
            //base.GetSPListItemData(spItem);
            base.Init(spItem, Zeppelin.ShP.Helper.Enums.AttachmentsLoadMode.Short);
            SingleLineOfText = spItem.GetFieldStringValue(DefaultList.Fields.SingleLineOfText);
            Number = spItem.GetFieldDoubleValue(DefaultList.Fields.Number, 0);
            NumberDecimals = spItem.GetFieldDoubleValue(DefaultList.Fields.NumberDecimals, 0);
            MultipleLinesOfText = spItem.GetFieldStringValue(DefaultList.Fields.MultipleLinesOfText);
            MultiRichTextHTML = spItem.GetFieldStringValue(DefaultList.Fields.MultiRichTextHTML);
            PublishingHTML = spItem.GetFieldStringValue(DefaultList.Fields.PublishingHTML);

            Lookup = spItem.GetFieldLookupValue(DefaultList.Fields.Lookup);
            MultiLookupColumns = spItem.GetFieldMultiLookupValues(DefaultList.Fields.MultiLookupColumns);
            Boolean = spItem.GetFieldBooleanValueOrNull(DefaultList.Fields.Boolean);
            
        }
    }
}
