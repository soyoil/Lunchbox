using System;
namespace Lunchbox
{
    public partial class Memory
    {
        public byte[] Ram;

        internal LCDCReg LCDC { get => (LCDCReg)Ram[0xFF40]; set => Ram[0xFF40] = (byte)value; }
        internal STATReg STAT { get => (STATReg)Ram[0xFF41]; set => Ram[0xFF41] = (byte)value; }
        internal byte SCY { get => Ram[0xFF42]; set => Ram[0xFF42] = value; }
        internal byte SCX { get => Ram[0xFF43]; set => Ram[0xFF43] = value; }
        internal byte LY { get => Ram[0xFF44]; set => Ram[0xFF44] = value; }
        internal byte LYC { get => Ram[0xFF45]; set => Ram[0xFF45] = value; }
        internal byte WY { get => Ram[0xFF4A]; set => Ram[0xFF4A] = value; }
        internal byte WX { get => Ram[0xFF4B]; set => Ram[0xFF4B] = value; }

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

        public Memory()
        {
            Ram = new byte[0x10000];
            Array.Copy(bootRom, Ram, bootRom.Length);
        }
    }
}
