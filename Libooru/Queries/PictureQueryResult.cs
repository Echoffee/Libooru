using Libooru.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Queries
{
    public class PictureQueryResult
    {
        public QueryStatus Status { get; set; }

        public IList<Picture> Pictures { get; set; }
    }
}
