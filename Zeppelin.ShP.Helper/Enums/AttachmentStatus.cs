using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Статусы вложений
    /// </summary>
    public enum AttachmentStatus
    {
        /// <summary>
        /// Добавлено
        /// </summary>
        Added,
        /// <summary>
        /// Удалено
        /// </summary>
        Deleted,
        /// <summary>
        /// Загружено краткая информация
        /// </summary>
        Loaded,
        /// <summary>
        /// Загружено вместе с содержимым
        /// </summary>
        LoadedWithoutContent
        //Changed,
    }
}
