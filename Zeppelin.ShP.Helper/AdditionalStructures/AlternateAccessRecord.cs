using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.AdditionalStructures
{
    public class AlternateAccessRecord
    {
        public string Application { get; set; }
        public SPUrlZone Zone { get; set; }
        public string Url { get; set; }
    }
}
