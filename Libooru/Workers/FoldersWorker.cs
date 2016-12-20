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
        public List<Pic> list { get; set; }
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

        public bool ScanForNewPictures()
        {
            var result = false;
            var folderList = FolderCollection.Find(Query.All(Query.Descending)).ToList();
            foreach (var folderItem in folderList)
            {
                var folder = new DirectoryInfo(folderItem.Path);
                if (folder.GetFiles().Where(x => pictureFileExtensions.Contains(x.Extension)).ToArray().Length != folderItem.FileCount)
                {
                    result = true;
                    var fileItems = core.picturesWroker.GetPicturesInFolder(folderItem.Id).Pictures;
                    var files = folder.GetFiles().Where(x => pictureFileExtensions.Contains(x.Extension)).ToList();
                    var newPictures = new List<Picture>();
                    var md5s = new Dictionary<string, FileInfo>();
                    Parallel.ForEach(files, file =>
                    {
                        var md5 = core.taggerWorker.GetMd5FromFile(file.FullName);
                        if (!md5s.ContainsKey(md5))
                            md5s.Add(md5, file);
                    });

                    foreach (var file in fileItems)
                    {
                        if (md5s.ContainsKey(file.Md5))
                            md5s.Remove(file.Md5);
                    }

                    Parallel.ForEach(md5s, file => 
                    {
                        var p = new Picture();
                        p.Date = DateTime.UtcNow;
                        p.FolderId = folderItem.Id;
                        p.Md5 = file.Key;
                        p.Path = file.Value.FullName;
                        p.IsNew = true;
                        newPictures.Add(p);
                    });
                    core.picturesWroker.InsertNewPictures(newPictures);
                    folderItem.FileCount = folder.GetFiles().Length;
                    FolderCollection.Update(folderItem);
                }
            }

            return result;
        }

        public void AddNewFolder(string name, string path)
        {
            var o = new Folder();
            o.Name = name;
            o.Path = path;
            o.FileCount = 0;
            FolderCollection.Insert(o);
        }

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

        

        /*public void scanForPicturesOld()
        {
            core.SetStatus("Scanning folders...");
            if (core.config.Data.Folders.NewPictureFolderPath.Equals(core.config.Data.Folders.PictureFolderPath))
                newPictureNumber = getPictureFilesNumWithoutDiving(core.config.Data.Folders.NewPictureFolderPath);
            else
                newPictureNumber = getPictureFilesNum(core.config.Data.Folders.NewPictureFolderPath);
            pictureNumber = getPictureFilesNum(core.config.Data.Folders.PictureFolderPath);
            if (core.config.Data.Folders.NewPictureFolderPath.Equals(core.config.Data.Folders.PictureFolderPath))
                pictureNumber -= newPictureNumber;
            core.config.SavePictureList(list);
            core.SetStatus("");
        }*/

        //internal ResultGetPictureFiles getPictureFiles(int num, int limit = 5)
        //{
        //    var result = new ResultGetPictureFiles();
        //    var resultList = new List<Pic>();
        //    var dInfo = new DirectoryInfo(core.config.Data.Folders.PictureFolderPath);
        //    if (dInfo.GetFiles().Length > num + limit)
        //    {
        //        result.status = ResultGetPictureFiles.Status.Success_More;
        //        result.statusReply = dInfo.GetFiles().Length - num + limit;
        //    }
        //    else
        //    {
        //        result.status = ResultGetPictureFiles.Status.Success_Empty;
        //    }
        //    //foreach (var f in dInfo.GetFiles())
        //    var fs = dInfo.GetFiles().Where(x => pictureFileExtensions.Contains(x.Extension)).ToList();
        //    for (int i = num; i < limit + num && i < fs.Count; i++)
        //    {

        //        var f = fs[i];
        //        if (pictureFileExtensions.Contains(f.Extension))
        //        {
        //            ////d.Bitmap img = new d.Bitmap(f.FullName);
        //            ////d.Image.GetThumbnailImageAbort abort = new d.Image.GetThumbnailImageAbort(ThumbnailCallback);
        //            ////var ratio = Math.Max(img.Height, img.Width) / 150;
        //            ////var t = img.GetThumbnailImage(img.Width / ratio, img.Height / ratio, abort, IntPtr.Zero);

        //            ////using (MemoryStream ms = new MemoryStream())
        //            ////{
        //            ////    t.Save(ms, d.Imaging.ImageFormat.Jpeg);

        //            ////    var b = ms.ToArray();
        //            var b = core.thumbnailsWroker.GetThumbnail(f.Name, f.FullName);
        //            var p = new Pic();
        //            p.Picture = b;
        //            p.Title = f.Name;
        //            resultList.Add(p);
        //            ////}
        //        }
        //    }
        //    result.list = resultList;
        //    return result;
        //}

        /*internal ResultGetPictureFiles getPictureFiles(int num, int limit = 5)
        {
            var result = new ResultGetPictureFiles();
            var resultList = new List<Pic>();
            var fileList = core.config.GetPictures(num, limit);
            foreach (var f in fileList)
            {
                if (f != null)
                { 
                    var b = core.picturesWroker.GetThumbnail(f);
                    var p = new Pic();
                    p.Picture = b;
                    resultList.Add(p);
                }
            }
            result.list = resultList;
            return result;
        }*/

        /*public long getPictureFilesNum(string path)
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
                {
                    result++;
                    list.Add(f.FullName.Replace(core.config.Data.Folders.PictureFolderPath + @"\", ""));
                }
            }
            return result;
        }*/
        /*
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
        */
        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
