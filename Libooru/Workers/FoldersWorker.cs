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

        private string[] pictureFileExtensions = { ".jpg", ".jpeg", ".png" };

        public FoldersWorker(Core core)
        {
            this.core = core;
        }

        public void scanForPictures()
        {
            core.SetStatus("Scanning folders...");
            if (core.config.Data.Folders.NewPictureFolderPath.Equals(core.config.Data.Folders.PictureFolderPath))
                newPictureNumber = getPictureFilesNumWithoutDiving(core.config.Data.Folders.NewPictureFolderPath);
            else
                newPictureNumber = getPictureFilesNum(core.config.Data.Folders.NewPictureFolderPath);
            pictureNumber = getPictureFilesNum(core.config.Data.Folders.PictureFolderPath);
            if (core.config.Data.Folders.NewPictureFolderPath.Equals(core.config.Data.Folders.PictureFolderPath))
                pictureNumber -= newPictureNumber;
            core.SetStatus("");
        }

        internal ResultGetPictureFiles getPictureFiles(int num, int limit = 10)
        {
            var result = new ResultGetPictureFiles();
            var resultList = new List<Pic>();
            var dInfo = new DirectoryInfo(core.config.Data.Folders.PictureFolderPath);
            if (dInfo.GetFiles().Length > num + limit)
            {
                result.status = ResultGetPictureFiles.Status.Success_More;
                result.statusReply = dInfo.GetFiles().Length - num + limit;
            }else
            {
                result.status = ResultGetPictureFiles.Status.Success_Empty;
            }
            //foreach (var f in dInfo.GetFiles())

            for (int i = num; i < limit; i++)
            {
                
                var f = dInfo.GetFiles()[i];
                if (pictureFileExtensions.Contains(f.Extension))
                {
                    d.Bitmap img = new d.Bitmap(f.FullName);
                    d.Image.GetThumbnailImageAbort abort = new d.Image.GetThumbnailImageAbort(ThumbnailCallback);
                    var ratio = Math.Max(img.Height, img.Width) / 150;
                    var t = img.GetThumbnailImage(img.Width / ratio, img.Height / ratio, abort, IntPtr.Zero);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        t.Save(ms, d.Imaging.ImageFormat.Jpeg);

                        var b = ms.ToArray();
                        var p = new Pic();
                        p.Picture = b;
                        p.Title = f.Name;
                        resultList.Add(p);
                    }
                }
            }
            result.list = resultList;
            return result;
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

        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}
