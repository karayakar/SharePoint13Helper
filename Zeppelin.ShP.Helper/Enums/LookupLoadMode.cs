using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Режим загрузки элеметов типа подстановка
    /// </summary>
    public enum LookupLoadMode
    {
        /// <summary>
        /// Быстрая - данные беруться из элемента поля
        /// </summary>
        Quick,
        /// <summary>
        /// Полная - значение проверяется из связанного списка
        /// </summary>
        FullLoad
    }
}
