using Libooru.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Links
{
    public class Core
    {
        public FoldersWorker foldersWorker { get; set; }

        public Core()
        {
            this.foldersWorker = new FoldersWorker();
        }
    }
}
