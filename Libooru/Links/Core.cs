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
            tagsWorker.tagCollection = Database.GetCollection<PictureTag>("tags");
            picturesWroker.pictureCollection = Database.GetCollection<Picture>("pictures");
            picturesWroker.sourceCollection = Database.GetCollection<Source>("sources");
            foldersWorker.FolderCollection = Database.GetCollection<Folder>("folders");


            this.config = new Config(this);
            config.AppFolderPath = path;
            config.GetConfig();
        }

		/// <summary>
		/// Initialize Libooru environment by creating config files, etc...
		/// </summary>
        public void FirstLaunch()
        {
            var f = new Folder();
            f.Name = "Default";
            f.Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            f.FileCount = 0;
            foldersWorker.FolderCollection.Insert(f);
        }

		/// <summary>
		/// Do a scan for new detected pictures.
		/// </summary>
        public void SoftUpdate()
        {
			/*if (foldersWorker.ScanForNewPictures())
                picturesWroker.HandleNewPictures();*/
			//var modifiedFolders = foldersWorker.DoFullScan();
			var fastFolders = foldersWorker.FolderCollection.Find(x => x.ScanAtStart).ToList();
			if (fastFolders.Count > 0)
				foldersWorker.Scan(fastFolders);

            switcher.UpdateAllViews();
        }

		/// <summary>
		/// Do a full scan on all pictures.
		/// </summary>
		public void HardUpdate()
		{
			var modifiedFolders = foldersWorker.DoFullScan();
			if (modifiedFolders.Count > 0)
				foldersWorker.Scan(modifiedFolders);

			switcher.UpdateAllViews();
		}

		/// <summary>
		/// Set status bar message on all views.
		/// </summary>
		/// <param name="status">Message to display.</param>
        public void SetStatus(string status)
        {
            switcher.SetAllViewsStatus(status);
        }

		/// <summary>
		/// Set progress bar progression on all compatible views.
		/// </summary>
		/// <param name="value">The new value of the progress bar (0-100).</param>
		public void SetProgress(int value)
		{
			switcher.SetProgress(value);
		}

		/// <summary>
		/// Set the current picture to display on all compatible views.
		/// </summary>
		/// <param name="id">Id of the Picture object in the database to display.</param>
		public void SetPicture(int id)
        {
            switcher.SetPicture(id);
        }

		/// <summary>
		/// Initialize app (startup).
		/// </summary>
        internal void Initialize()
        {
            config.SetLibooruEnv();
            SoftUpdate();
            //taggerWorker.SearchForMd5(@"link");
        }
    }
}
