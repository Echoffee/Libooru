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

        public LiteCollection<Picture> pictureCollection { get; internal set; }

        public PicturesWorker(Core core)
        {
            this.core = core;
        }

        public PictureQueryResult RetrievePictures(int offset = 0, int count = 5, IList<Tag> tags = null)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Date);
            var r = new List<Picture>();
            if (tags == null)
                r = pictureCollection.Find(Query.All(Query.Descending), offset, count).ToList();
            result.Pictures = r;
            return result;
        }

        public byte[] GenerateThumbnail(string path, int resolution = 150)
        {
            d.Bitmap img = new d.Bitmap(path);
            d.Image.GetThumbnailImageAbort abort = new d.Image.GetThumbnailImageAbort(ThumbnailCallback);
            var ratio = Math.Max(img.Height, img.Width) / 150;
            if (ratio <= 0)
            {
                ratio = 150 / Math.Max(img.Height, img.Width);
            }

            var t = img.GetThumbnailImage(img.Width / ratio, img.Height / ratio, abort, IntPtr.Zero);
            using (var s = new MemoryStream())
            {
                t.Save(s, d.Imaging.ImageFormat.Jpeg);
                return s.ToArray();
            }
        }

        public PictureQueryResult GetPicturesInFolder(string path)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Path);
            var r = pictureCollection.Find(x => x.Directory.Equals(path));
            result.Pictures = r.ToList();
            return result;
        }

        public void HandleNewPictures()
        {
            var p = pictureCollection.Find(x => x.IsNew).ToList();
            foreach (var picture in p)
            {
                picture.Thumbnail = GenerateThumbnail(picture.Path);
                d.Bitmap img = new d.Bitmap(picture.Path);
                picture.Width = img.Width;
                picture.Height = img.Height;
                picture.Size = (new FileInfo(picture.Path)).Length;
                picture.IsNew = false;
                pictureCollection.Update(picture);
            }
        }

        public byte[] GetThumbnail(int fileId, string path = "")
        {
            var c = pictureCollection.Find(x => x.Id.Equals(fileId));
            var result = c.First().Thumbnail;
            return result;
        }

        /*public byte[] GetThumbnailOld(string file, string path = "")
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
        }*/

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
