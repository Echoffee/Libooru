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

        public void UtilizeState(Core core)
        {
            this.core = core;
        }

        public void UpdateView()
        {
            LoadPicture(DisplayedId);
        }

        private void goToMain(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMain();
        }

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

		public void SetProgress(int value)
		{
			this.Dispatcher.Invoke(() =>
		   {
			   this.progressBar.Value = value;
		   });
		}

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
