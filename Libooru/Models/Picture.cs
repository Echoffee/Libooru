using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Models
{
    public class Picture
    {
        public int Id { get; set; }

        public string Md5 { get; set; }

        public string Path { get; set; }

        public string Directory { get; set; }

        //public List<Tag> Tags { get; set; }
        //Maybe not a good idea tbh

        public byte[] Thumbnail { get; set; }

        public DateTime Date { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Source { get; set; }

        public bool? HighDefinition { get; set; }

        public bool IsNew { get; set; }
    }
}
