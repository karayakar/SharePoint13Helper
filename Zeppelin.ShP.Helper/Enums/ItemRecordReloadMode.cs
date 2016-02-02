using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    public enum ItemRecordReloadMode
    {
        /// <summary>
        /// Без перечитывания
        /// </summary>
        None,
        /// <summary>
        /// Простое чтение
        /// </summary>
        Reload,
        /// <summary>
        /// Чтение с повышенными привелегиями
        /// </summary>
        ReloadWithEvevantedPriveleges
    }
}
