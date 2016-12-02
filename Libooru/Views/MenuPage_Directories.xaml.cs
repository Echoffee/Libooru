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

        public MenuPage_Directories()
        {
            InitializeComponent();
            this.listFolders = new ObservableCollection<Folder>();
            
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
            core.SetStatus("Done.");
            UpdateView();
            core.SetStatus("");
        }

        private void GetFolders()
        {
            listFolders = new ObservableCollection<Folder>();
            var result = core.foldersWorker.GetFolders();
            foreach (var item in result)
            {
                listFolders.Add(item);    
            }
            this.mainlb.DataContext = this;
        }
        /*public void ChooseDirectoryOpt1(object sender, RoutedEventArgs e)
        {
            ChooseDirectory(1);
        }

        public void ChooseDirectoryOpt2(object sender, RoutedEventArgs e)
        {
            ChooseDirectory(2);
        }*/

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            core.foldersWorker.AddNewFolder(textboxFolderName.Text, textboxFolderPath.Text);
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
    }
}
