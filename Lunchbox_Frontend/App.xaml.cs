using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Lunchbox;

namespace Lunchbox_Frontend
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Lunchbox.Lunchbox lunchbox;
        public App()
        {
            lunchbox = new Lunchbox.Lunchbox();
        }
    }
}
