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
                if (memory.LY < displayHeight)
                {
                    UpdateBG();
                    if (memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableWindow)) UpdateWindow();
                    UpdateGraphicMode(Memory.STATReg.ScanOAMMode);
                }
                else if (memory.LY >= displayHeight + 10)
                {
                    memory.LY = 0;
                    UpdateBG();
                    if (memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableWindow)) UpdateWindow();
                    UpdateGraphicMode(Memory.STATReg.ScanOAMMode);
                }
                else
                {
                    if (memory.LY == displayHeight) {
                        UpdateSprite();
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
                memory.LY++;
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
                displayData[target] = displayData[target + 1] = displayData[target + 2] = GetColor(tileID, memory.SCX % 8 + x, (memory.SCY + memory.LY) % 8 * 2);
                displayData[target + 3] = 255;
            }
        }

        private void UpdateWindow()
        {
            if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;
            if (memory.WX < 0 && displayWidth + 7 <= memory.WX && memory.WY < 0 && displayHeight <= memory.WY) return;
            if (memory.LY < memory.WY) return;
            for (byte x = 0; x < displayWidth; x++)
            {
                ushort tilemap = memory.LCDC.HasFlag(Memory.LCDCReg.SelectWindowTileMap) ? 0x9C00 : 0x9800;
                byte tileID = memory[tilemap + ((x - memory.WX - 7) / 8) + ((memory.LY - memory.WY) / 8 * 0x20)];
                int target = (memory.LY * displayWidth + x) * 4;
                displayData[target] = displayData[target + 1] = displayData[target + 2] = GetColor(tileID, x - memory.WX - 7, (memory.LY - memory.WY) % 8 * 2);
                displayData[target + 3] = 255;
            }
        }

        private void UpdateSprite()
        {
            if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;
            for (int i = 0; i < 40; i++)
            {
                int y = memory[0xFE00 + i * 4] - 16;
                int x = memory[0xFE00 + i * 4 + 1] - 8;
                byte tileID = memory[0xFE00 + i * 4 + 2];
                byte property = memory[0xFE00 + i * 4 + 3];
                int height = 8;
                if (memory.LCDC.HasFlag(Memory.LCDCReg.SelectSpriteSize))
                {
                    height = 16;
                    tileID &= 0xFE;
                }
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        if (x + j < 0 || x + j >= displayWidth) continue;
                        if (y + k < 0 || y + k >= displayHeight) continue;
                        byte paid = GetPaletteID(tileID, j, k);
                        int ax = (property & 0x20) != 0 ? 7 - j : j;
                        int ay = (property & 0x40) != 0 ? 7 - k : k;
                        int ho;
                        if ((property & 0x10) != 0) ho = (memory.OBP0 >> (paid * 2)) & 3;
                        else ho = (memory.OBP1 >> (paid * 2)) & 3;
                        if (paid != 0)
                        {
                            byte color = ho switch
                            {
                                0 => 255,
                                1 => 130,
                                2 => 60,
                                3 => 1,
                                _ => 0,
                            };
                            int target = ((y + ay) * displayWidth + x + ax) * 4;
                            displayData[target] = displayData[target + 1] = displayData[target + 2] = color;
                            displayData[target + 3] = 255;
                        }
                    }
                }
            }
        }

        private byte GetColor(byte tileID, int x, int y)
        {
            x %= 8;
            int address = memory.LCDC.HasFlag(Memory.LCDCReg.SelectTileData) ? 0x8000 + tileID * 0x10 : 0x9000 + ((sbyte)tileID * 0x10);
            address += y;
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

        private byte GetPaletteID(byte tileID, int x, int y)
        {
            x %= 8;
            int address = 0x8000 + tileID * 0x10 + y * 2;
            int palette = (memory[address] & (1 << (7 - x))) != 0 ? 1 : 0;
            if ((memory[address + 1] & (1 << (7 - x))) != 0) palette += 2;
            return (byte)palette;
        }
    }
}
