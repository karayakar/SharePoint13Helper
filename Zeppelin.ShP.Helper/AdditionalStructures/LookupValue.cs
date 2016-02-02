using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.DataAccessBaseClasses
{
    public class LookupValue
    {
        int? _id = null;
        string _value = string.Empty;

        public LookupValue() { }

        public LookupValue(int id, string value)
        {
            _id = id;
            _value = value;
        }

        public bool IsEmpty
        {
            get
            {
                return ((_id == null) || (!_id.HasValue) || (_id.Value == 0));
            }
        }

        public int Id
        {
            get
            {
                if (IsEmpty)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return _id.Value;
                }
            }
        }

        public string Value
        {
            get
            {
                if (IsEmpty)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return _value;
                }
            }
        }

        public void SetValue(int id, string value)
        {
            _id = id;
            _value = value;
        }

        public void SetEmpty()
        {
            _id = null;
            _value = string.Empty;
        }
    }
}
