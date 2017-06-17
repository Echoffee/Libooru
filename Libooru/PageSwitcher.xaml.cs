using Libooru.Links;
using Libooru.Views;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Libooru
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Views.MainPage mainPage { get; set; }

        private Views.MenuPage menuPage { get; set; }

        private Views.MenuPage_Directories menuPage_Directories { get; set; }

        private Views.MenuPage_Externals menuPage_Externals { get; set; }

        private Views.PicturePage picturePage { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var core = new Core(this);
            ThemeService.Current.ChangeTheme(Theme.Dark);
            Switcher.pageSwitcher = this;
            mainPage = new Views.MainPage();
            menuPage = new Views.MenuPage();
            menuPage_Directories = new MenuPage_Directories();
            menuPage_Externals = new MenuPage_Externals();
            picturePage = new PicturePage();

            mainPage.core = core;
            menuPage.core = core;
            menuPage_Directories.core = core;
            menuPage_Externals.core = core;
            picturePage.core = core;

            core.Initialize();
            Switcher.Switch(mainPage);
            mainPage.UpdateView();
        }


        internal void SetAllViewsStatus(string status)
        {
            mainPage.textStatus.Text = status;
            menuPage.textStatus.Text = status;
            menuPage_Directories.textStatus.Text = status;
            menuPage_Externals.textStatus.Text = status;
            picturePage.textStatus.Text = status;
        }

        internal void UpdateAllViews()
        {
            mainPage.listPic.Clear();
            mainPage.UpdateView();
            menuPage.UpdateView();
            menuPage_Directories.UpdateView();
            menuPage_Externals.UpdateView();
        }

        internal void GoToMenu_Externals()
        {
            Switcher.Switch(menuPage_Externals);
            menuPage_Externals.UpdateView();
        }

        internal void SetPicture(int id)
        {
            picturePage.LoadPicture(id);
        }

        public void GoToMain()
        {
            Switcher.Switch(mainPage);
            mainPage.UpdateView();
        }

        public void GoToMenu()
        {
            Switcher.Switch(menuPage);
            menuPage.UpdateView();
        }

		public void SetProgress(int value)
		{
			picturePage.SetProgress(value);
		}

		public void GoToMenu_Directories()
        {
            Switcher.Switch(menuPage_Directories);
            menuPage_Directories.UpdateView();
        }

        public void GoToPicture()
        {
            Switcher.Switch(picturePage);
            picturePage.UpdateView();
        }

        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, Core core)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(core);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }

        public void Navigate(Page nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(Page nextPage, Core core)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(core);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }

    }
}