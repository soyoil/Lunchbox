namespace Lunchbox_Frontend
{
    class EmulatorModel : BasePropertyChanged
    {
        const int clocksPerFrame = 70224;

        readonly int _height;
        readonly int _width;
        readonly Lunchbox.Lunchbox lunchbox;
        readonly byte[] pixels;

        public byte[] Buffer
        {
            get
            {
                for (int i = 0; i < clocksPerFrame; i++)
                {
                    lunchbox.Run();
                    int target = (lunchbox.graphic.displayData.y * _width + lunchbox.graphic.displayData.x) * 4;
                    byte color;
                    if (lunchbox.graphic.displayData.color == 0) {
                        color = 255;
                    }
                    else
                    {
                        color = 30;
                    }
                    pixels[target] = pixels[target + 1] = pixels[target + 2] = color;
                    pixels[target + 3] = 255;
                }
                return pixels;
            }
        }

        public EmulatorModel(int height, int width)
        {
            _height = height;
            _width = width;
            lunchbox = new Lunchbox.Lunchbox();
            pixels = new byte[_width * _height * 4];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = 255;
        }
    }
}
