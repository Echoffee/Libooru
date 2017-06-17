using Libooru.Links;
using Libooru.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for PicturePage.xaml
    /// </summary>
    public partial class PicturePage : Page, ISwitchable
    {

        public Core core { get; set; }

        public int DisplayedId { get; set; }

        public ObservableCollection<PictureTag> TagList { get; set; }

        public PicturePage()
        {
            InitializeComponent();
            this.TagList = new ObservableCollection<PictureTag>();
        }

		/// <summary>
		/// Link the view to the Core.
		/// </summary>
		/// <param name="core">Core object to link to.</param>
        public void UtilizeState(Core core)
        {
            this.core = core;
        }

		/// <summary>
		/// Update view.
		/// </summary>
        public void UpdateView()
        {
            LoadPicture(DisplayedId);
        }

		/// <summary>
		/// Called when the Back button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void goToMain(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMain();
        }

		/// <summary>
		/// Called when Check tags is clicked. Starts tagging process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private async void checkTags(object sender, RoutedEventArgs e)
        {
            core.SetStatus("Tag in progress...");
			this.progressBar.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
				if (!core.taggerWorker.TagPicture(DisplayedId))
					core.SetStatus("SEARCH FAILED! Please try again.");
				else
					core.SetStatus("Done.");
			});
            
            
			this.progressBar.Visibility = Visibility.Hidden;
			UpdateView();
        }

		/// <summary>
		/// Change progress bar value.
		/// </summary>
		/// <param name="value">New value of the progress bar (0-100).</param>
		public void SetProgress(int value)
		{
			this.Dispatcher.Invoke(() =>
		   {
			   this.progressBar.Value = value;
		   });
		}

		/// <summary>
		/// Load and display a picture from its Id.
		/// </summary>
		/// <param name="id">Id of the Picture object in the database to display.</param>
        public void LoadPicture(int id)
        {
            this.DisplayedId = id;
            var p = core.picturesWroker.GetPicture(id);
            var b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(p.Path);
            b.EndInit();

            this.image.Source = b;

            TagList.Clear();
            var t = core.tagsWorker.GetTagsForPicture(id);
            foreach (var item in t)
                TagList.Add(item);


            this.mainlb.DataContext = this;
			var n = TagList.Where(x => x.Type == TagType.Artist).FirstOrDefault();
			if (n != null)
				this.label_artist.Content = n.Name;
			else
				this.label_artist.Content = "<Unknown>";

            mainlb.Items.Refresh();
        }

    }
}
