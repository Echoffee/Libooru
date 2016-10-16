using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Links.ConfigData.ExternalsData
{
    [DataContract]
    public class Danbooru
    {
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string ApiKey { get; set; }
    }
}
