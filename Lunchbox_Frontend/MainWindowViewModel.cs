using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Lunchbox_Frontend
{
    class MainWindowViewModel : BasePropertyChanged
    {
        const int height = 144;
        const int width = 160;

        public WriteableBitmap Screen { get; set; }

        readonly EmulatorModel emulatorModel;
        readonly OpenFileDialog dialog;
        readonly DispatcherTimer timer;

        public MainWindowViewModel(Window window)
        {
            Screen = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            dialog = new OpenFileDialog()
            {
                Filter = "GameBoy ROM (*.gb)|*.gb"
            };

            if (dialog.ShowDialog() == false) window.Close();
            emulatorModel = new EmulatorModel(height, width, dialog.FileName);

            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 16)
            };
            timer.Tick += (object sender, EventArgs e) => Screen.WritePixels(new Int32Rect(0, 0, width, height), emulatorModel.Buffer, width * 4, 0, 0);
            timer.Start();
        }
    }
}
