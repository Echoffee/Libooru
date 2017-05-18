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

		private ListBoxItem prevItem = null;

        public MenuPage_Directories()
        {
            InitializeComponent();
            this.listFolders = new ObservableCollection<Folder>();
			this.menuButton_addedit.IsEnabled = false;
			this.menuButton_remove.IsEnabled = false;
            
        }

        public void UtilizeState(Core core)
        {
            this.core = core;
        }

        private void goToMenu(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu();
        }

        private void goToMain(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMain();
        }

        private void goToExternals(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMenu_Externals();
        }

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

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            core.foldersWorker.AddNewFolder(textboxFolderName.Text, textboxFolderPath.Text);
            GetFolders();
        }

        private void RemoveFolder(object sender, RoutedEventArgs e)
        {
            var item = (Folder) mainlb.SelectedItem;
            core.foldersWorker.RemoveFolder(item.Id);
            GetFolders();
        }

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

        private void Bar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (folderNameBarClearOnFocus)
            {
                this.textboxFolderName.Text = "";
                folderNameBarClearOnFocus = false;
            }
        }

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

        public void UpdateView()
        {
            //CountFiles();
            SetPaths();
        }

        public void SetPaths()
        {
            GetFolders();
            //textboxPictureFolder.Text = core.config.Data.Folders.PictureFolderPath;
            //textboxNewPictureFolder.Text = core.config.Data.Folders.NewPictureFolderPath;
        }

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

		private void mainlb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (prevItem != null)
			{
				var dt = prevItem.DataContext as Folder;
			}

			prevItem = ItemsControl.ContainerFromElement(this.mainlb, e.OriginalSource as DependencyObject) as ListBoxItem;
			if (prevItem != null)
			{
				
			}
		}
	}
}
