using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Тип пользователя
    /// </summary>
    public enum UserDataType
    {
        /// <summary>
        ///Windows группа
        /// </summary>
        WindowsGroup,
        /// <summary>
        /// Windows (or Claims login) login
        /// </summary>
        Login,
        /// <summary>
        /// Группа SharePoint
        /// </summary>
        SPGroup,
        /*FormsUser,*/
        /// <summary>
        /// Не задано
        /// </summary>
        Empty,
        /// <summary>
        /// Не определено
        /// </summary>
        UnKnown //in case we read name only
    }
}
