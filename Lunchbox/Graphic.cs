namespace Lunchbox
{
    public class Graphic
    {
        private const byte displayWidth = 160;
        private const byte displayHeight = 144;

        public readonly byte[] displayData;

        private readonly Memory memory;
        private int tick;

        internal Graphic(Memory memory)
        {
            this.memory = memory;
            displayData = new byte[displayWidth * displayHeight * 4];
            tick = 0;
        }


        internal void Update()
        {
            if (tick == 80)
            {
                UpdateGraphicMode(Memory.STATReg.ScanVRAMMode);
            }
            else if (tick == 252)
            {
                UpdateGraphicMode(Memory.STATReg.HBlankMode);
                if (memory.STAT.HasFlag(Memory.STATReg.IsEnableHBlankInterrupt)) SetInterrupt(Memory.IFReg.IsRequestedSTATInterrupt);
            }
            else if (tick == 456)
            {
                tick = 0;
                memory.LY++;
                if (memory.LY < displayHeight)
                {
                    UpdateBG();
                    // if (memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableWindow)) UpdateWindow();
                    UpdateGraphicMode(Memory.STATReg.ScanOAMMode);
                }
                else if (memory.LY >= displayHeight + 10)
                {
                    memory.LY = 0;
                    UpdateBG();
                    // if (memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableWindow)) UpdateWindow();
                    UpdateGraphicMode(Memory.STATReg.ScanOAMMode);
                }
                else
                {
                    if (memory.LY == displayHeight) {
                        // UpdateSprite();
                        SetInterrupt(Memory.IFReg.IsRequestedVBlankInterrupt);
                        if (memory.STAT.HasFlag(Memory.STATReg.IsEnableVBlankInterrupt)) SetInterrupt(Memory.IFReg.IsRequestedSTATInterrupt);
                    }
                    UpdateGraphicMode(Memory.STATReg.VBlankMode);
                }

                if (memory.LY == memory.LYC)
                {
                    memory.STAT |= Memory.STATReg.SelectCoincidence;
                    if (memory.STAT.HasFlag(Memory.STATReg.IsEnableCoincidenceInterrupt)) SetInterrupt(Memory.IFReg.IsRequestedSTATInterrupt);
                }
                else
                {
                    memory.STAT &= ~Memory.STATReg.SelectCoincidence;
                }
            }
            tick++;
        }

        private void UpdateGraphicMode(Memory.STATReg mode)
        {
            memory.STAT &= (Memory.STATReg)0b11111100;
            memory.STAT |= mode;
        }

        private void SetInterrupt(Memory.IFReg reg)
        {
            if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;
            memory.IF |= reg;
        }

        private void UpdateBG()
        {
            if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;
            for (byte x = 0; x < displayWidth; x++)
            {
                ushort tilemap = memory.LCDC.HasFlag(Memory.LCDCReg.SelectBGTileMap) ? 0x9C00 : 0x9800;
                byte tileID = memory[tilemap + (((memory.SCX + x) / 8) & 0x1F) + (((memory.SCY + memory.LY) & 0xFF) / 8 * 0x20)];
                int target = (memory.LY * displayWidth + x) * 4;
                displayData[target] = displayData[target + 1] = displayData[target + 2] = GetColor(tileID, (byte)(memory.SCX % 8 + x));
                displayData[target + 3] = 255;
            }
        }

        private byte GetColor(byte tileID, byte x)
        {
            x %= 8;
            int address = memory.LCDC.HasFlag(Memory.LCDCReg.SelectTileData) ? 0x8000 + tileID * 0x10 : 0x9000 + ((sbyte)(tileID) * 0x10);
            address += (memory.SCY + memory.LY) % 8 * 2;
            int palette = (memory[address] & (1 << (7 - x))) != 0 ? 1 : 0;
            if ((memory[address + 1] & (1 << (7 - x))) != 0) palette += 2;
            return ((memory.BGP >> (palette * 2)) & 0b11) switch
            {
                0 => 255,
                1 => 130,
                2 => 60,
                3 => 1,
                _ => 0,
            };
        }
    }
}
