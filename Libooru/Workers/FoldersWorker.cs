using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Libooru.Workers
{
    public class FoldersWorker
    {
        public string pictureFolderPath { get; set; }

        public string newPictureFolderPath { get; set; }

        public long pictureNumber { get; set; }

        public long newPictureNumber { get; set; }

        public FoldersWorker()
        {
            
        }
    }
}
