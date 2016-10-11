using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Libooru.Views;
using Libooru.Links;

namespace Libooru
{
    public static class Switcher
    {
        public static MainWindow pageSwitcher;

        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(UserControl newPage, Core core)
        {
            pageSwitcher.Navigate(newPage, core);
        }

        public static void Switch(Page newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch(Page newPage, Core core)
        {
            pageSwitcher.Navigate(newPage, core);
        }
    }
}
