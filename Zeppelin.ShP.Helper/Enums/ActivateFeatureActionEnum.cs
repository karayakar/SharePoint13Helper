using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zeppelin.ShP.Helper.Enums
{
    /// <summary>
    /// Режим активации фичи
    /// </summary>
    public enum ActivateFeatureActionEnum
    {
        /// <summary>
        /// Обычный, в случае если фича уже активирована - не переактивировать
        /// </summary>
        None, 
        /// <summary>
        /// В режиме force
        /// </summary>
        Force, 
        /// <summary>
        /// Если фича уже активирована - деактивировать и активировать снова
        /// </summary>
        ReActivate
    }
}
