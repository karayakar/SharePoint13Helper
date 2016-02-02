using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Режим сохранения
    /// </summary>
    public enum ItemRecordSaveMode
    {
        /// <summary>
        /// Обьчное
        /// </summary>
        Simple,
        /// <summary>
        /// Системное 
        /// </summary>
        SystemUpdate,
        /// <summary>
        /// Установка только систмной информации
        /// </summary>
        SetSystemInformation
    }
}
