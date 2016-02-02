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
    /// Класс для работы с вложениями элементов списков
    /// </summary>
    public class ItemRecordAttachment
    {
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="fileName">Название файла</param>
        /// <param name="content">Содержимое</param>
        public ItemRecordAttachment(string fileName, byte[] content)
        {
            this.Name = fileName;
            this._content = content;
            this.Size = content.Length;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="listItem">Элемент списка</param>
        /// <param name="fileName">Название файла</param>
        /// <param name="mode">Режим загрузки</param>
        public ItemRecordAttachment(SPListItem listItem, string fileName, AttachmentsLoadMode mode)
        {
            this.Name = fileName;//Name;
            this.Url = listItem.Attachments.UrlPrefix + fileName;//Name;
            if ((mode == AttachmentsLoadMode.None))// || (mode == AttachmentsLoadMode.Present))
            {
                throw new ArgumentException("Incorrect value of 'mode' parameter");
            }
            SPFile file = listItem.Web.GetFile(listItem.Attachments.UrlPrefix + fileName);
            this.Size = file.Length;
            if (mode == AttachmentsLoadMode.Full)
            {
                this._content = file.OpenBinary();
            }
        }

        /// <summary>
        /// Название файла
        /// </summary>
        public string Name { get; private set; }

        byte[] _content;
        /// <summary>
        /// Содержимое файла
        /// </summary>
        public byte[] Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Путь
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Размер
        /// </summary>
        public long? Size { get; private set; }

        /// <summary>
        /// Размер в такстовом виде (суффикс на английском)
        /// </summary>
        public string SizeStr
        {
            get
            {
                if (Size.HasValue)
                    return Convert.ToString(Size.Value); //FormatHelper.FormatDocumentsFileSize(Size.Value);
                else
                    return "N/A";
            }
        }

        /// <summary>
        /// Статус загрузки
        /// </summary>
        public AttachmentStatus Status;
    }
}
