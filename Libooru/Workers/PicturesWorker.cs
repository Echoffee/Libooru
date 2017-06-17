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

        public LiteCollection<Source> sourceCollection { get; internal set; }

        public PicturesWorker(Core core)
        {
            this.core = core;
        }

		/// <summary>
		/// Retrieve a collection of pictures from the database.
		/// </summary>
		/// <param name="offset">From which index to retrieve pictures. Default is 0.</param>
		/// <param name="count">Number of pictures to retrieve. Default is 5.</param>
		/// <param name="tags">Tags to search for. Currently not implemented.</param>
		/// <returns>PictureQueryResult with search results.</returns>
        public PictureQueryResult RetrievePictures(int offset = 0, int count = 5, IList<PictureTag> tags = null)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Date);
            var r = new List<Picture>();
            if (tags == null)
                r = pictureCollection.Find(Query.All(Query.Descending), offset, count).ToList();

            result.Pictures = r;
            return result;
        }

		/// <summary>
		/// Generate a thumbnail for a given picture.
		/// </summary>
		/// <param name="path">Path of the picture.</param>
		/// <param name="resolution">Resolution of the thumbnail (width/height max).</param>
		/// <returns>The thumbnail as a byte array.</returns>
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

		/// <summary>
		/// Get all pictures from the database in a given folder.
		/// </summary>
		/// <param name="id">Id of the folder in the database.</param>
		/// <returns>PictureQueryResult with the pictures.</returns>
        public PictureQueryResult GetPicturesInFolder(int id)
        {
            var result = new PictureQueryResult();
            pictureCollection.EnsureIndex(x => x.Path);
            var r = pictureCollection.Find(x => x.FolderId.Equals(id));
            result.Pictures = r.ToList();
            return result;
        }

		/// <summary>
		/// Get a picture from the database.
		/// </summary>
		/// <param name="id">Id of the picture requested.</param>
		/// <returns>Picture object</returns>
        public Picture GetPicture(int id)
        {
            pictureCollection.EnsureIndex(x => x.Path);
            var r = pictureCollection.Find(x => x.Id.Equals(id));
            return r.ToList().First();
        }

		/// <summary>
		/// UNSTABLE - Generate Pictures objects for discovered pictures since previous scan.
		/// </summary>
        public void HandleNewPictures()
        {
            pictureCollection.EnsureIndex("IsNew");
            var p = pictureCollection.Find(x => x.IsNew).ToList();
			foreach (var picture in p)
            {
                picture.Thumbnail = GenerateThumbnail(picture.Path);
                using (d.Bitmap img = new d.Bitmap(picture.Path))
                {
                    picture.Width = img.Width;
                    picture.Height = img.Height;
                }
                picture.Size = (new FileInfo(picture.Path)).Length;
                picture.IsNew = false;
                pictureCollection.Update(picture);
            }
        }

		/// <summary>
		/// Get the thumbnail from a given picture id.
		/// </summary>
		/// <param name="fileId">Id of the picture in the database.</param>
		/// <returns>The thumbnail as a byte array.</returns>
        public byte[] GetThumbnail(int fileId)
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

		/// <summary>
		/// Insert a new Picture object in the database.
		/// </summary>
		/// <param name="p">Picture object to add.</param>
        public void InsertNewPicture(Picture p)
        {
            pictureCollection.Insert(p);
        }

		/// <summary>
		/// Update an existing Picture object in the database.
		/// </summary>
		/// <param name="picture">Picture object to update.</param>
		internal void UpdatePicture(Picture picture)
		{
			pictureCollection.Update(picture);
		}

		/// <summary>
		/// Delete an existing Picture object from the database.
		/// </summary>
		/// <param name="oldPicture">Picture object to remove.</param>
		internal void DeletePicture(Picture oldPicture)
		{
			pictureCollection.Delete(x => x.Id == oldPicture.Id);
		}

		/// <summary>
		/// Insert a new collection of Picture objects in the database.
		/// </summary>
		/// <param name="p">Collection of Picture objects to add.</param>
		public void InsertNewPictures(IList<Picture> p)
        {
            pictureCollection.Insert(p);
        }

		/// <summary>
		/// No idea what is this for.
		/// </summary>
		/// <returns>Boolean value.</returns>
        public bool ThumbnailCallback()
        {
            return false;
        }

		/// <summary>
		/// Remove all Picture objects on the database from a given folder.
		/// </summary>
		/// <param name="id">Id of the folder in the database.</param>
        internal void RemovePicturesFromFolder(int id)
        {
            pictureCollection.Delete(x => x.FolderId == id);
        }
    }
}
