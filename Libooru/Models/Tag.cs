using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Models
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TagType Type { get; set; }

        public IList<string> Synonyms { get; set; }

        public IList<int> PictureIDs { get; set; }
    }

    public enum TagType
    {
        Meta,
        Common,
        Character,
        Artist,
        Copyright
    }
}
