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
using Libooru.Models;

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, ISwitchable
    {
        public Core core { get; set; }

        public ObservableCollection<Picture> listPic { get; set; }

        private bool searchBarClearOnFocus = true;

        public MainPage()
        {
            InitializeComponent();
            ThemeService.Current.ChangeTheme(Theme.Dark);
			this.listPic = new ObservableCollection<Picture>();


		}

		/// <summary>
		/// Update view.
		/// </summary>
        public void UpdateView()
        {
            RefreshList();
            //CountFiles();
        }

		/// <summary>
		/// Refresh displayed list of Picture objects from a given interval.
		/// </summary>
		/// <param name="index">Id of the Picture object to look from.</param>
		/// <param name="limit">Number of Picture objects to display.</param>
        private void RefreshList(int index = 0, int limit = 5)
        {
            if (index >= listPic.Count)
            {
                var result = core.picturesWroker.RetrievePictures(index, limit);
				foreach (var item in result.Pictures)
					this.listPic.Add(item);

                this.picGrid.DataContext = this;
            }
        }

		/// <summary>
		/// Count and display the number of Picture objects saved in the database.
		/// </summary>
        public void CountFiles()
        {
            this.textInfos.Text = core.foldersWorker.pictureNumber + " picture"
                            + (core.foldersWorker.pictureNumber > 1 ? "s" : "")
                            + (core.foldersWorker.newPictureNumber > 0 ? " + "
                            + core.foldersWorker.newPictureNumber + " new" : "");
        }

		/// <summary>
		/// Called when the search bar is focused.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void searchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBarClearOnFocus)
            {
                searchBarClearOnFocus = false;
                this.searchBar.Text = "";
            }
        }

		/// <summary>
		/// Called when the search bar lost the focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void searchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.searchBar.Text.Equals(""))
            {
                searchBarClearOnFocus = true;
                this.searchBar.Text = "";
            }
        }

		/// <summary>
		/// Called when the Menu button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu();
            //core.taggerWorker.QueryDanbooruIQDB(@"C:\Users\echo\Documents\Libooru\thumbnails\Saved Pictures\572f9631fd502dbe39364b16a953d2f6.jpg");
        }

		/// <summary>
		/// Link the view to the Core.
		/// </summary>
		/// <param name="core">Core to link to.</param>
        public void UtilizeState(Core core)
        {
            this.core = core;
        }

		/// <summary>
		/// Called when the list view scroll position has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void mainlb_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight && e.VerticalOffset + e.ViewportHeight != 0)
            {
                RefreshList(listPic.Count);
            }
        }

		/// <summary>
		/// Called when an item in the list view is double clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void mainlb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (Picture)mainlb.SelectedItem;
            if (item != null)
            {
                core.SetPicture(item.Id);
                core.switcher.GoToPicture();
            }
        }

		/// <summary>
		/// Called when the Rescan button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void scanButton_Click(object sender, RoutedEventArgs e)
		{
			core.HardUpdate();
		}
	}
}
