using Libooru.Links;
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

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for MenuPage_Externals.xaml
    /// </summary>
    public partial class MenuPage_Externals : Page
    {
        public Core core { get; set; }

        public MenuPage_Externals()
        {
            InitializeComponent();
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
		/// Called when the General button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void goToMenu(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu();
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
		/// Called when the Directories button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void goToFolders(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu_Directories();
        }

		/// <summary>
		/// Called when the Apply button is clicked. Edit the config file according to new settings and update the view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ApplyChanges(object sender, RoutedEventArgs e)
        {
            core.SetStatus("Applying changes...");
            core.config.Data.Externals.Danbooru.Login = textBoxLogin.Text;
            core.config.Data.Externals.Danbooru.ApiKey= textBoxApiKey.Text;
            core.config.ApplyChanges();
            core.SetStatus("Done.");
            UpdateView();
            core.SetStatus("");
        }

		/// <summary>
		/// update view.
		/// </summary>
        public void UpdateView()
        {
            CountFiles();
            SetFields();
        }

		/// <summary>
		/// Apply text fields according to the config file.
		/// </summary>
        public void SetFields()
        {
            //TODO: Fix for more tools
            textBoxLogin.Text = core.config.Data.Externals.Danbooru.Login;
            textBoxApiKey.Text = core.config.Data.Externals.Danbooru.ApiKey;
        }

		/// <summary>
		/// Count and display the number of Picture objects in the database.
		/// </summary>
        public void CountFiles()
        {
            this.textInfos.Text = core.foldersWorker.pictureNumber + " picture"
                            + (core.foldersWorker.pictureNumber > 1 ? "s" : "")
                            + (core.foldersWorker.newPictureNumber > 0 ? " + "
                            + core.foldersWorker.newPictureNumber + " new" : "");

        }
    }
}
