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

        public Core(MainWindow switcher)
        {
            this.switcher = switcher;
            var documentsPaths = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = documentsPaths + @"/Libooru";
            this.config = new Config();
            config.AppFolderPath = path;
            config.GetConfig();
            foldersWorker = new FoldersWorker(this);
        }
    }
}
