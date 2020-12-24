using System;
using System.ComponentModel;
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
        readonly DispatcherTimer timer;

        public MainWindowViewModel()
        {
            Screen = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            emulatorModel = new EmulatorModel(height, width);

            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 16)
            };
            timer.Tick += (object sender, EventArgs e) => Screen.WritePixels(new Int32Rect(0, 0, width, height), emulatorModel.Buffer, width * 4, 0, 0);
            timer.Start();
        }
    }
}
