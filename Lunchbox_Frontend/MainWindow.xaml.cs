using System.Windows;
using System.Windows.Input;

namespace Lunchbox_Frontend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this);
        }

        public void OnKeyDownEventHandler(object sender, KeyEventArgs e) => ((MainWindowViewModel)DataContext).OnKeyDownHandler(sender, e);
        public void OnKeyUpEventHandler(object sender, KeyEventArgs e) => ((MainWindowViewModel)DataContext).OnKeyUpHandler(sender, e);
    }
}
