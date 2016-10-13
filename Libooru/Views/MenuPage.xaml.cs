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
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page, ISwitchable
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        public Core core { get; set; }

        public void UtilizeState(Core core)
        {
            this.core = core;
        }

        private void menuButton_Click(object sender, RoutedEventArgs e)
        {
            core.switcher.GoToMain();
        }

        internal void UpdateView()
        {
        }
    }
}
