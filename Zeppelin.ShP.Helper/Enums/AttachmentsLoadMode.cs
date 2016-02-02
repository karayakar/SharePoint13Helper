using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Режим загружки вложений при работе с элементов
    /// </summary>
    public enum AttachmentsLoadMode
    {
        /// <summary>
        /// Без загружки
        /// </summary>
        None,
        //Present,
        /// <summary>
        /// Краткая информация: название файла, размер, путь
        /// </summary>
        Short,
        /// <summary>
        /// Полная - включая содержимое вложения
        /// </summary>
        Full
    }
}
