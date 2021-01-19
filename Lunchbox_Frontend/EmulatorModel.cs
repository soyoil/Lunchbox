using System.Windows.Input;
using System.Collections.Generic;
using System;

namespace Lunchbox_Frontend
{
    class EmulatorModel : BasePropertyChanged
    {
        const int clocksPerFrame = 70224;
        readonly Lunchbox.Lunchbox lunchbox;
        readonly Dictionary<Key, Lunchbox.Joypad.Keys> pairs;

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
            pairs = new Dictionary<Key, Lunchbox.Joypad.Keys>
            {
                { Key.Up, Lunchbox.Joypad.Keys.Up },
                { Key.Down, Lunchbox.Joypad.Keys.Down },
                { Key.Left, Lunchbox.Joypad.Keys.Left },
                { Key.Right, Lunchbox.Joypad.Keys.Right },
                { Key.A, Lunchbox.Joypad.Keys.A },
                { Key.S, Lunchbox.Joypad.Keys.B },
                { Key.Z, Lunchbox.Joypad.Keys.Start },
                { Key.X, Lunchbox.Joypad.Keys.Select },
            };
        }

        public void HandleKeyDown(Key k)
        {
            if (pairs.ContainsKey(k)) lunchbox.joypad.PushedKeyDictionary |= (byte)pairs[k];
            Console.WriteLine("up");
        }

        public void HandleKeyUp(Key k)
        {
            if (pairs.ContainsKey(k)) lunchbox.joypad.PushedKeyDictionary &= (byte)~(byte)pairs[k];
            Console.WriteLine("down");
        }
    }
}
