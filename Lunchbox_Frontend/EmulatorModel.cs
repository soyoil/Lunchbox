namespace Lunchbox_Frontend
{
    class EmulatorModel : BasePropertyChanged
    {
        const int clocksPerFrame = 70224;
        readonly Lunchbox.Lunchbox lunchbox;

        public byte[] Buffer
        {
            get
            {
                for (int i = 0; i < clocksPerFrame; i++) lunchbox.Run();
                return lunchbox.graphic.displayData;
            }
        }

        public EmulatorModel(int height, int width, string filepath)
        {
            lunchbox = new Lunchbox.Lunchbox(filepath);
        }
    }
}
