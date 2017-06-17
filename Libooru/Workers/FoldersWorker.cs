using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Libooru.Links;
using System.Windows.Controls;
using Libooru.Views;
using d = System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Libooru.Queries;
using LiteDB;
using Libooru.Models;

namespace Libooru.Workers
{
    public class ResultGetPictureFiles
    {
        public List<Picture> list { get; set; }
        public enum Status
        {
            Success_Empty,
            Success_More,
            Failure
        };

        public Status status { get; set; }
        public int statusReply { get; set; }
    }

    public class FoldersWorker
    {
        public Core core { get; set; }

        public long pictureNumber { get; set; }

        public long newPictureNumber { get; set; }

        public LiteCollection<Folder> FolderCollection { get; set; }

        private string[] pictureFileExtensions = { ".jpg", ".jpeg", ".png" };

        public List<string> list { get; set; }

        public FoldersWorker(Core core)
        {
            this.core = core;
            this.list = new List<string>();
        }

        public List<Folder> GetFolders()
        {
            FolderCollection.EnsureIndex("Id");
            return FolderCollection.Find(Query.All(Query.Ascending)).ToList();
        }

		/// <summary>
		/// Do a full scan of  folders. Check if sizes changed since last scan.
		/// </summary>
		/// <returns>A list of modified folders.</returns>
		public IList<Folder> DoFullScan()
		{
			var result = new List<Folder>();
			var folderCollection = FolderCollection.Find(Query.All(Query.Descending)).ToList();
			foreach (var folderItem in folderCollection)
			{
				var oldSize = folderItem.Size;
				long newSize = 0;
				var files = Directory.GetFiles(folderItem.Path);
				foreach (var file in files)
					newSize += new FileInfo(file).Length;

				if (oldSize != newSize)
					result.Add(folderItem);
			}

			return result;
		}

		/// <summary>
		/// Scan files in given folders and edit database
		/// </summary>
		/// <param name="folders">The folders to scan</param>
		public void Scan(IList<Folder> folders)
		{
			foreach (var item in folders)
				Scan(item);
		}

		/// <summary>
		/// Scan files in a given folder and edit database
		/// </summary>
		/// <param name="folder">The folder to scan</param>
		public void Scan(Folder folder)
		{
			var oldPictures = core.picturesWroker.GetPicturesInFolder(folder.Id);
			var folderFiles = Directory.GetFiles(folder.Path);
			foreach (var file in folderFiles)
			{
				var fileInfo = new FileInfo(file);
				if (pictureFileExtensions.Contains(fileInfo.Extension))
				{
					var oldPictureName = oldPictures.Pictures.Where(x => x.Path.Equals(fileInfo.FullName)).FirstOrDefault();
					var md5 = Tools.GetMd5FromFile(fileInfo.FullName);
					var oldPictureMd5 = oldPictures.Pictures.Where(x => x.Md5.Equals(md5));

					if (oldPictureMd5.Count() == 1 && oldPictureName == null) // name change
					{
						oldPictureMd5.First().Path = fileInfo.FullName;
						core.picturesWroker.UpdatePicture(oldPictureMd5.First());
					}

					if (oldPictureMd5.Count() == 0 && oldPictureName != null) // new pic with old name
					{
						core.picturesWroker.DeletePicture(oldPictureName);
						oldPictureName = null;
					}

					if (oldPictureMd5.Count() == 0 && oldPictureName == null) // new picture
					{
						using (d.Bitmap img = new d.Bitmap(fileInfo.FullName))
						{
							var p = new Picture()
							{
								Date = DateTime.UtcNow,
								FolderId = folder.Id,
								IsNew = true,
								Path = fileInfo.FullName,
								Md5 = md5,
								Size = fileInfo.Length,
								Width = img.Width,
								Height = img.Height,
								Thumbnail = core.picturesWroker.GenerateThumbnail(fileInfo.FullName),
							};

							core.picturesWroker.InsertNewPicture(p);
						}
					}
				}
			}
		}

		/// <summary>
		/// Add new folder to scan in the database.
		/// </summary>
		/// <param name="name">Name of the folder as it appears on the list.</param>
		/// <param name="path">Path of the folder.</param>
        public void AddNewFolder(string name, string path)
        {
            var o = new Folder();
            o.Name = name;
            o.Path = path;
            o.FileCount = 0;
            FolderCollection.Insert(o);
        }

		/// <summary>
		/// Remove a folder from the database.
		/// </summary>
		/// <param name="Id">Id of the folder in the database.</param>
		/// <returns></returns>
        public bool RemoveFolder(int Id)
        {
            var result = false;

            if (FolderCollection.Find(x => x.Id == Id).ToList().Count == 1)
            {
                FolderCollection.Delete(x => x.Id == Id);
                result = true;
                core.picturesWroker.RemovePicturesFromFolder(Id);
            }

            return result;
        }

		/// <summary>
		/// Callback I don't know what for.
		/// </summary>
		/// <returns>Boolean value</returns>
        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
