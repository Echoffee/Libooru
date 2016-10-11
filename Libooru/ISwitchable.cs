using Libooru.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru
{
    public interface ISwitchable
    {
        void UtilizeState(Core core);
    }
}
