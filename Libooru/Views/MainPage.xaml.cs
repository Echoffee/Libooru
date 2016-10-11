﻿using MetroRadiance.UI;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, ISwitchable
    {
        public Core core { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ThemeService.Current.ChangeTheme(Theme.Dark);
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void searchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            this.searchBar.Text = "";
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new MenuPage());
        }

        public void UtilizeState(Core core)
        {
            this.core = core;
        }
    }
}