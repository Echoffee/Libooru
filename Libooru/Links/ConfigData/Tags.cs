using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Links.ConfigData
{
    [DataContract]
    public class Tags
    {
        [DataMember]
        public int SafetyLevel { get; set; }
    }
}
