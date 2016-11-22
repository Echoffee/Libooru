using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Workers.Iqdb
{
    public class IqdbMatch
    {
        public string url { get; set; }

        public string md5 { get; set; }

        public int accuracy { get; set; }

        public string provider { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public float size { get; set; }
    }
}
