using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingSkill
{
    public class GenericCompare : IEqualityComparer<Subject> //where T : BaseEntity
    {
        public bool Equals(Subject x, Subject y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(Subject obj)
        {
            if (object.ReferenceEquals(obj, null)) return 0;

            return obj.Id;
        }
    }
}
