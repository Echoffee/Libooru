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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using Libooru.Models;

namespace Libooru.Views
{
    /// <summary>
    /// Interaction logic for MenuPage_Directories.xaml
    /// </summary>
    public partial class MenuPage_Directories : Page, ISwitchable
    {
        public Core core { get; set; }

        public ObservableCollection<Folder> listFolders { get; set; }

        private bool folderNameBarClearOnFocus = true;

        public MenuPage_Directories()
        {
            InitializeComponent();
            this.listFolders = new ObservableCollection<Folder>();
			this.menuButton_addedit.IsEnabled = false;
            
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
		/// Called when the Externals button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void goToExternals(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu_Externals();
        }

		/// <summary>
		/// Called when the Apply button is clicked. Edit the config file according to new settings and update the view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void ApplyChanges(object sender, RoutedEventArgs e)
        {
            core.SetStatus("Applying changes...");
            //core.config.Data.Folders.PictureFolderPath = textboxPictureFolder.Text;
            //core.config.Data.Folders.NewPictureFolderPath = textboxNewPictureFolder.Text;
            core.config.ApplyChanges();
            core.SoftUpdate();
            core.SetStatus("Done.");
            UpdateView();
            core.SetStatus("");
        }

		/// <summary>
		/// Retrieve Folder objects from the database and display them.
		/// </summary>
        private void GetFolders()
        {
            listFolders.Clear();
            var result = core.foldersWorker.GetFolders();
            foreach (var item in result)
            {
                    listFolders.Add(item);
            }

            this.mainlb.DataContext = this;
            mainlb.Items.Refresh();
        }

		/// <summary>
		/// Called when the Add button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void AddFolder(object sender, RoutedEventArgs e)
        {
            core.foldersWorker.AddNewFolder(textboxFolderName.Text, textboxFolderPath.Text);
            GetFolders();
        }

		/// <summary>
		/// Called when the Remove button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[System.Obsolete]
        private void RemoveFolder(object sender, RoutedEventArgs e)
        {
            var item = (Folder) mainlb.SelectedItem;
            core.foldersWorker.RemoveFolder(item.Id);
            GetFolders();
        }

		/// <summary>
		/// Called when the "..." button is clicked. Display a pop-up to select a folder.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void ChooseDirectory(object sender, RoutedEventArgs e)
        { 
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = false;
            var title = "New pictures folder";
            dialog.Title = title;
            CommonFileDialogResult result = dialog.ShowDialog();
            textboxFolderPath.Text = dialog.FileName;
			if (textboxFolderPath.Equals("..."))
				this.menuButton_addedit.IsEnabled = false;
			else
				this.menuButton_addedit.IsEnabled = true;

		}

		/// <summary>
		/// Called when the folder name textbar is focused.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Bar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (folderNameBarClearOnFocus)
            {
                this.textboxFolderName.Text = "";
                folderNameBarClearOnFocus = false;
            }
        }

		/// <summary>
		/// Called when the folder name textbar lost its focus.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Bar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.textboxFolderName.Text.Equals(""))
            {
                this.textboxFolderName.Text = "Enter folder name";
				this.menuButton_addedit.IsEnabled = false;
				folderNameBarClearOnFocus = true;
            }else
				this.menuButton_addedit.IsEnabled = true;
        }

		/// <summary>
		/// Update view.
		/// </summary>
        public void UpdateView()
        {
            //CountFiles();
            SetPaths();
        }

		/// <summary>
		/// Set folder paths on the list.
		/// </summary>
        public void SetPaths()
        {
            GetFolders();
            //textboxPictureFolder.Text = core.config.Data.Folders.PictureFolderPath;
            //textboxNewPictureFolder.Text = core.config.Data.Folders.NewPictureFolderPath;
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

		private void scanOnStartCheckBox_Checked(object sender, RoutedEventArgs e)
		{

		}

		/// <summary>
		/// Called when a Remove button is clicked in the list view.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void remButton_Click(object sender, RoutedEventArgs e)
		{
			var o = ((sender as Button).Tag as Folder);
			core.foldersWorker.RemoveFolder(o.Id);
			GetFolders();
		}

	}
}
