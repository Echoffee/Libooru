using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Libooru.Links;

namespace Libooru.Workers
{
    public class FoldersWorker
    {
        public Core core { get; set; }

        public long pictureNumber { get; set; }

        public long newPictureNumber { get; set; }

        private string[] pictureFileExtensions = { ".jpg", ".jpeg", ".png" };

        public FoldersWorker(Core core)
        {
            this.core = core;
        }

        public void scanForPictures()
        {
            core.SetStatus("Scanning folders...");
            if (core.config.Data.newPictureFolderPath.Equals(core.config.Data.pictureFolderPath))
                newPictureNumber = getPictureFilesNumWithoutDiving(core.config.Data.newPictureFolderPath);
            else
                newPictureNumber = getPictureFilesNum(core.config.Data.newPictureFolderPath);
            pictureNumber = getPictureFilesNum(core.config.Data.pictureFolderPath);
            if (core.config.Data.newPictureFolderPath.Equals(core.config.Data.pictureFolderPath))
                pictureNumber -= newPictureNumber;
            core.SetStatus("");
        }

        public long getPictureFilesNum(string path)
        {
            var result = 0L;
            var dInfo = new DirectoryInfo(path);
            foreach (var d in dInfo.GetDirectories())
            {
                result += getPictureFilesNum(d.FullName);
            }
            foreach (var f in dInfo.GetFiles())
            {
                if (pictureFileExtensions.Contains(f.Extension))
                    result++;
            }
            return result;
        }

        public long getPictureFilesNumWithoutDiving(string path)
        {
            var result = 0L;
            var dInfo = new DirectoryInfo(path);
            foreach (var f in dInfo.GetFiles())
            {
                if (pictureFileExtensions.Contains(f.Extension))
                    result++;
            }
            return result;
        }
    }
}
