using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeppelin.ShP.Helper.Enums;
using Zeppelin.ShP.Helper.Helpers;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    /// <summary>
    /// Класс для работы с элементом списка с вложениями
    /// </summary>
    public class ItemRecordWithAttachments : DefaultListItem//: ItemRecord    ???
    {

        protected List<ItemRecordAttachment> _attachments = new List<ItemRecordAttachment>();

        public List<ItemRecordAttachment> Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="listItem">Элемент списка</param>
        /// <param name="mode">Режим загрузки</param>
        public virtual void Init(SPListItem listItem, AttachmentsLoadMode mode)
        {
            //LoadDataFromSPListItem(listItem);             ???
            base.GetSPListItemData(listItem);
            if (mode != AttachmentsLoadMode.None)
            {
                if (listItem.Attachments == null || listItem.Attachments.Count <= 0)
                {
                    return;
                }
                for (int i = 0; i < listItem.Attachments.Count; i++)
                {
                    //SPAttachmentCollection
                    var curItem = listItem.Attachments[i];
                    Attachments.Add(new ItemRecordAttachment(listItem, curItem, mode));
                }
            }
        }

        ///// <summary>
        ///// Сохранение элемента и его вложений
        ///// </summary>
        ///// <param name="listItem"></param>
        ///// <returns></returns>
        //protected override bool SaveData(SPListItem listItem)
        //{
        //    bool result = base.SaveData(listItem);

        //    if (!result)
        //        return result;

        //    foreach (ItemRecordAttachment attachment in Attachments)
        //    {
        //        if ((attachment.Status == AttachmentStatus.Added))// || (attachment.Status == AttachmentStatus.Changed))
        //        {
        //            if ((attachment.Content == null) || (attachment.Content.Length == 0))
        //            {
        //                throw new ArgumentException("All attachment files should have content");
        //            }
        //        }
        //    }

        //    if (Attachments.Count > 0)
        //    {
        //        SPWeb web = listItem.Web;
        //        web.ValidateFormDigest();
        //        using (new AllowUnsafeUpdatesWebScope(web))
        //        {
        //            for (int i = Attachments.Count - 1; i >= 0; i--)
        //            {
        //                ItemRecordAttachment attachment = Attachments[i];
        //                if (attachment.Status == AttachmentStatus.Deleted)
        //                {
        //                    listItem.Attachments.Delete(attachment.Name);
        //                    Attachments.RemoveAt(i);
        //                }

        //                if (attachment.Status == AttachmentStatus.Added)
        //                {
        //                    listItem.Attachments.Add(attachment.Name, attachment.Content);
        //                    attachment.Status = AttachmentStatus.Loaded;
        //                }
        //            }
        //        }
        //        result = true;
        //    }
        //    else
        //        result = true;
        //    return result;
        //}

        //public override BaseFieldsProvider GetFieldsProvider()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
