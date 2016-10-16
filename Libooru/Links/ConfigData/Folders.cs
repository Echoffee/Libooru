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
        public string PictureFolderPath { get; set; }
        [DataMember]
        public string NewPictureFolderPath { get; set; }
    }
}
