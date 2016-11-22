using Libooru.Links;
using d = System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Libooru.Workers
{
    public class ThumbnailsWorker
    {
        public Core core { get; set; }

        public string thumbnailsFolderPath { get; set; }

        public ThumbnailsWorker(Core core)
        {
            this.core = core;
        }

        public string GenerateThumbnail(string file, string path, int resolution = 150)
        {
            d.Bitmap img = new d.Bitmap(path);
            d.Image.GetThumbnailImageAbort abort = new d.Image.GetThumbnailImageAbort(ThumbnailCallback);
            var ratio = Math.Max(img.Height, img.Width) / 150;
            if (ratio <= 0)
            {
                ratio = 150 / Math.Max(img.Height, img.Width);
            }

            var t = img.GetThumbnailImage(img.Width / ratio, img.Height / ratio, abort, IntPtr.Zero);
            var fullPath = thumbnailsFolderPath + "/" + file;
            string c = "";
            string[] a = file.Split('\\', '/');
            for (int i = 0; i < a.Length - 1; i++)
            {
                c += a[i];
                if (!Directory.Exists(thumbnailsFolderPath + "/" + c))
                    Directory.CreateDirectory(thumbnailsFolderPath + "/" + c);
                c += "/";
            }
            t.Save(fullPath);
            return fullPath;
        }

        public byte[] GetThumbnail(string file, string path = "")
        {
            var dInfo = new DirectoryInfo(thumbnailsFolderPath);
            //if (dInfo.GetFiles().Where(x => x.Name.Equals(file)).ToArray().Length >= 1)
            if (File.Exists(thumbnailsFolderPath + "/" + file))
            {
                return File.ReadAllBytes(thumbnailsFolderPath + "/" + file);
            }else
            {
                GenerateThumbnail(file, core.config.Data.Folders.PictureFolderPath + "/" + file);
                return File.ReadAllBytes(thumbnailsFolderPath + "/" + file);
            }
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
