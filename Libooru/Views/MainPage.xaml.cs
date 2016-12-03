using MetroRadiance.UI;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Libooru.Links;
using System.Collections.ObjectModel;

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, ISwitchable
    {
        public Core core { get; set; }

        public ObservableCollection<Pic> listPic { get; set; }

        private bool searchBarClearOnFocus = true;

        public MainPage()
        {
            InitializeComponent();
            ThemeService.Current.ChangeTheme(Theme.Dark);
            this.listPic = new ObservableCollection<Pic>();
            
        }

        public void UpdateView()
        {
            RefreshList();
            //CountFiles();
        }

        private void RefreshList(int index = 0, int limit = 5)
        {
            if (index >= listPic.Count)
            {
                var result = core.picturesWroker.RetrievePictures(index, limit);
                foreach (var item in result.Pictures)
                {
                    var p = new Pic();
                    p.Picture = item.Thumbnail;
                    p.Title = "text";
                    listPic.Add(p);
                }
                this.picGrid.DataContext = this;
            }
        }

        public void CountFiles()
        {
            this.textInfos.Text = core.foldersWorker.pictureNumber + " picture"
                            + (core.foldersWorker.pictureNumber > 1 ? "s" : "")
                            + (core.foldersWorker.newPictureNumber > 0 ? " + "
                            + core.foldersWorker.newPictureNumber + " new" : "");
        }

        private void searchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBarClearOnFocus)
            {
                searchBarClearOnFocus = false;
                this.searchBar.Text = "";
            }
        }

        private void searchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.searchBar.Text.Equals(""))
            {
                searchBarClearOnFocus = true;
                this.searchBar.Text = "";
            }
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu();
            //core.taggerWorker.QueryDanbooruIQDB(@"C:\Users\echo\Documents\Libooru\thumbnails\Saved Pictures\572f9631fd502dbe39364b16a953d2f6.jpg");
        }

        public void UtilizeState(Core core)
        {
            this.core = core;
        }

        private void mainlb_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight && e.VerticalOffset + e.ViewportHeight != 0)
            {
                RefreshList(listPic.Count);
            }
        }
    }
}
