using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Workers.TagsItems
{
    [DataContract]
    public class TagFile
    {
        public string[] Files { get; set; }
    }
}
