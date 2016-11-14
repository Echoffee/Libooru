using Libooru.Workers;
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

        public string status { get; set; }

        public Core(MainWindow switcher)
        {
            this.switcher = switcher;
            var documentsPaths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = documentsPaths + @"/Libooru";

            this.config = new Config(this);
            config.AppFolderPath = path;
            config.GetConfig();

            foldersWorker = new FoldersWorker(this);

            tagsWorker = new TagsWorker(this);
            tagsWorker.TagsFolderPath = path + @"/tags";

            taggerWorker = new TaggerWorker(this);
        }

        public void Update()
        {
            foldersWorker.scanForPictures();
            switcher.UpdateAllViews();
        }

        public void SetStatus(string status)
        {
            switcher.SetAllViewsStatus(status);
        }

        internal void Initialize()
        {
            config.SetLibooruEnv();
            Update();
            //taggerWorker.SearchForMd5(@"link");
        }
    }
}
