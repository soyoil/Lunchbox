using System;
using System.IO;

namespace Lunchbox
{
    internal partial class Memory
    {
        internal byte this[int index]
        {
            get
            {
                if (Ram[0xFF50] == 0 && index < 0x100)
                {
                    return bootRom[index];
                }
                if (index == 0xFF00) return GetJOYPValue();
                return Ram[index];
            }

            set
            {
                if (index < 0x8000) return;
                if (index == 0xFF44) return;
                if (index == 0xFF46)
                {
                    for (int i = 0; i < 0x9F; i++)
                    {
                        Ram[0xFE00 + i] = Ram[value * 0x100 + i];
                    }
                }
                Ram[index] = value;
            }
        }

        private readonly byte[] Ram;

        internal byte JOYP { get => GetJOYPValue(); set => Ram[0xFF00] = value; }
        internal IFReg IF { get => (IFReg)Ram[0xFF0F]; set => Ram[0xFF0F] = (byte)value; }
        internal LCDCReg LCDC { get => (LCDCReg)Ram[0xFF40]; set => Ram[0xFF40] = (byte)value; }
        internal STATReg STAT { get => (STATReg)Ram[0xFF41]; set => Ram[0xFF41] = (byte)value; }
        internal byte SCY { get => Ram[0xFF42]; set => Ram[0xFF42] = value; }
        internal byte SCX { get => Ram[0xFF43]; set => Ram[0xFF43] = value; }
        internal byte LY { get => Ram[0xFF44]; set => Ram[0xFF44] = value; }
        internal byte LYC { get => Ram[0xFF45]; set => Ram[0xFF45] = value; }
        internal byte DMA 
        { 
            get => Ram[0xFF46]; 
            set 
            {
                for (int i = 0; i < 0x9F; i++)
                {
                    Ram[0xFE00 + i] = Ram[value * 0x100 + i];
                }
            } 
        }
        internal byte BGP { get => Ram[0xFF47]; set => Ram[0xFF47] = value; }
        internal byte OBP0 { get => Ram[0xFF48]; set => Ram[0xFF48] = value; }
        internal byte OBP1 { get => Ram[0xFF49]; set => Ram[0xFF49] = value; }
        internal byte WY { get => Ram[0xFF4A]; set => Ram[0xFF4A] = value; }
        internal byte WX { get => Ram[0xFF4B]; set => Ram[0xFF4B] = value; }
        internal IEReg IE { get => (IEReg)Ram[0xFFFF]; set => Ram[0xFFFF] = (byte)value; }

        [Flags]
        internal enum LCDCReg : byte
        {
            IsEnableDisplay = 1 << 7,
            SelectWindowTileMap = 1 << 6,
            IsEnableWindow = 1 << 5,
            SelectTileData = 1 << 4,
            SelectBGTileMap = 1 << 3,
            SelectSpriteSize = 1 << 2,
            IsEnableSprite = 1 << 1,
            SelectDisplayPriority = 1
        }

        [Flags]
        internal enum STATReg : byte
        {
            IsEnableCoincidenceInterrupt = 1 << 6,
            IsEnableOAMInterrupt = 1 << 5,
            IsEnableVBlankInterrupt = 1 << 4,
            IsEnableHBlankInterrupt = 1 << 3,
            SelectCoincidence = 1 << 2,
            HBlankMode = 0,
            VBlankMode = 1,
            ScanOAMMode = 2,
            ScanVRAMMode = 3
        }

        [Flags]
        internal enum IEReg : byte
        {
            IsEnableVBlankInterrupt = 1,
            IsEnableSTATInterrupt = 1 << 1,
            IsEnableTimerInterrupt = 1 << 2,
            IsEnableSerialInterrupt = 1 << 3,
            IsEnableJoypadInterrupt = 1 << 4
        }

        [Flags]
        internal enum IFReg : byte
        {
            IsRequestedVBlankInterrupt = 1,
            IsRequestedSTATInterrupt = 1 << 1,
            IsRequestedTimerInterrupt = 1 << 2,
            IsRequestedSerialInterrupt = 1 << 3,
            IsRequestedJoypadInterrupt = 1 << 4
        }

        private Joypad joypad = null;

        internal Memory(string filepath)
        {
            Ram = new byte[0x10000];
            // Array.Copy(bootRom, Ram, bootRom.Length);
            if (filepath != "")
            using (var fs = new FileStream(@filepath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(Ram, 0, (int)fs.Length);
            }
        }

        internal void SetJoypadPtr(Joypad joypad) => this.joypad = joypad;

        private byte GetJOYPValue()
        {
            if ((Ram[0xFF00] & 0b00100000) == 0)
            {
                //return (byte)(Ram[0xFF00] | ~(joypad.PushedKeyDictionary >> 4));
                return 0;
            }
            if ((Ram[0xFF00] & 0b00010000) == 0)
            {
                return (byte)(Ram[0xFF00] | ~(joypad.PushedKeyDictionary & 0xF));
            }
            //return (byte)(result);
            // if (!joypad.PushedKeyDictionary[(Joypad.Keys)2]) result = 0b00001001;
            return (byte)(Ram[0xFF00] & 0xF0);
        }
    }
}
