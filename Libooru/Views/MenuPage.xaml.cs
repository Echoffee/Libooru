using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Libooru.Links;

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page, ISwitchable
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        public Core core { get; set; }

        public void UtilizeState(Core core)
        {
            this.core = core;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMain();
        }

        private void goToDirectories(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu_Directories();
        }

        public void UpdateView()
        {
            CountFiles();
            CountTags();
        }

        public void CountFiles()
        {
            this.textInfos.Text = core.foldersWorker.pictureNumber + " picture"
                            + (core.foldersWorker.pictureNumber > 1 ? "s" : "")
                            + (core.foldersWorker.newPictureNumber > 0 ? " + "
                            + core.foldersWorker.newPictureNumber + " new" : "");

            this.textPictures.Text = core.foldersWorker.pictureNumber + " picture"
                            + (core.foldersWorker.pictureNumber > 1 ? "s" : "");

        }

        public void CountTags()
        {
            this.textTags.Text = core.tagsWorker.tagNumber + " tag"
                            + (core.tagsWorker.tagNumber > 1 ? "s" : "");
        }
    }
}
