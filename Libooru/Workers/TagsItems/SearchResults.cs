using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Workers.TagsItems
{
    public class SearchResults
    {
        public string tag { get; set; }

        public List<string> results { get; set; }

        public SearchResults(string tag)
        {
            this.tag = tag;
            results = new List<string>();
        }
    }
}
