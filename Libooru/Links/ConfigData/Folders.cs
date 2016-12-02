using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Links.ConfigData
{
    [DataContract]
    public class Folders
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Path { get; set; }

        public Folders(string Name, string Path)
        {
            this.Name = Name;
            this.Path = Path;
        }
    }
}
