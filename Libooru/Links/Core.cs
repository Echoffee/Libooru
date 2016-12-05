using Libooru.Links.ConfigData;
using Libooru.Models;
using Libooru.Workers;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libooru.Links
{
    public class Core
    {
        public MainWindow switcher { get; set; }

        public Config config { get; set; }

        public FoldersWorker foldersWorker { get; set; }

        public TagsWorker tagsWorker { get; set; }

        public TaggerWorker taggerWorker { get; set; }

        public PicturesWorker picturesWroker { get; set; }

        public LiteDatabase Database { get; set; }

        public string status { get; set; }

        public Core(MainWindow switcher)
        {
            this.switcher = switcher;
            var documentsPaths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = documentsPaths + @"/Libooru";

            foldersWorker = new FoldersWorker(this);
            tagsWorker = new TagsWorker(this);
            picturesWroker = new PicturesWorker(this);
            taggerWorker = new TaggerWorker(this);
            Database = new LiteDatabase(path + @"/data.db");
            tagsWorker.tagCollection = Database.GetCollection<Tag>("tags");
            picturesWroker.pictureCollection = Database.GetCollection<Picture>("pictures");
            picturesWroker.sourceCollection = Database.GetCollection<Source>("sources");
            foldersWorker.FolderCollection = Database.GetCollection<Folder>("folders");


            this.config = new Config(this);
            config.AppFolderPath = path;
            config.GetConfig();
        }

        public void FirstLaunch()
        {
            var f = new Folder();
            f.Name = "Default";
            f.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            f.FileCount = 0;
            foldersWorker.FolderCollection.Insert(f);
        }

        public void Update()
        {
            if (foldersWorker.ScanForNewPictures())
                picturesWroker.HandleNewPictures();
            switcher.UpdateAllViews();
        }

        public void SetStatus(string status)
        {
            switcher.SetAllViewsStatus(status);
        }

        public void SetPicture(int id)
        {
            switcher.SetPicture(id);
        }

        internal void Initialize()
        {
            config.SetLibooruEnv();
            Update();
            //taggerWorker.SearchForMd5(@"link");
        }
    }
}
