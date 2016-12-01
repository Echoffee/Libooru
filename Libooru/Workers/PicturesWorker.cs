using Libooru.Links;
using d = System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Libooru.Models;
using LiteDB;
using Libooru.Queries;

namespace Libooru.Workers
{
    public class PicturesWorker
    {
        public Core core { get; set; }

        public string thumbnailsFolderPath { get; set; }

        public LiteCollection<Picture> pictureCollection { get; internal set; }

        public PicturesWorker(Core core)
        {
            this.core = core;
        }

        public PictureQueryResult RetrievePictures(int offset = 0, int count = 5)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Date);
            var r = pictureCollection.Find(Query.All(Query.Descending), offset, count);
            result.Pictures = r.ToList();
            return result;
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

        public PictureQueryResult GetPicturesInFolder(string path)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Path);
            var r = pictureCollection.Find(x => x.Directory.Equals(path));
            result.Pictures = r.ToList();
            return result;
        }

        public byte[] GetThumbnail(string file, string path = "")
        {
            var dInfo = new DirectoryInfo(thumbnailsFolderPath);
            if (File.Exists(thumbnailsFolderPath + "/" + file))
            {
                return File.ReadAllBytes(thumbnailsFolderPath + "/" + file);
            }else
            {
                
                GenerateThumbnail(file, core.config.Data.Folders.PictureFolderPath + "/" + file);
                return File.ReadAllBytes(thumbnailsFolderPath + "/" + file);
            }
        }

        public void InsertNewPicture(Picture p)
        {
            pictureCollection.Insert(p);
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
