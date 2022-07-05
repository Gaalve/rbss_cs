using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL1.RBSS_CS
{
    public interface IBifunctor
    {
        byte[] Hash { get; set; }
        public void Apply(byte[] bytes);
        public void Apply(IBifunctor other);

        public IBifunctor GetNewEmpty();
    }
}
