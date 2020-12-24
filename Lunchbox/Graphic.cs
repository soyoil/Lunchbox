using System;
using System.Collections.Generic;

namespace Lunchbox
{
    public class Graphic
    {
        private const byte displayHeight = 144;

        public struct DisplayData
        {
            public byte x;
            public byte y;
            public byte color;
        }

        public DisplayData displayData;

        private readonly Memory memory;
        private readonly Fetcher fetcher;
        private readonly PixelFIFO pixelFIFO;
        private int tick;
        public int fulltick;
        private bool isProcessing;

        private Action action;
        private readonly Action ScanOAM;
        private readonly Action ScanVRAM;
        private readonly Action HBlank;
        private readonly Action VBlank;

        public Graphic(Memory memory)
        {
            this.memory = memory;
            pixelFIFO = new PixelFIFO(this, memory);
            fetcher = new Fetcher(this, memory, pixelFIFO);
            displayData = new DisplayData();

            ScanOAM = () =>
            {
                isProcessing = ++tick < 80;
            };

            ScanVRAM = () =>
            {
                tick++;
                fetcher.Run();
                if (pixelFIFO.Count != 0)
                {
                    isProcessing = pixelFIFO.DequeueData();
                    if (!isProcessing)
                    {
                        pixelFIFO.Reset();
                        fetcher.Reset();
                    }
                    return;
                }
                isProcessing = true;
            };

            HBlank = () =>
            {
                isProcessing = ++tick < 456;
            };

            VBlank = () =>
            {
                isProcessing = ++tick < 456;
            };

            action = ScanOAM;
        }

        internal void Update()
        {
            // if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;

            action();
            if (isProcessing) return;

            switch (memory.STAT & (Memory.STATReg)0b11)
            {
                case Memory.STATReg.ScanOAMMode:
                    memory.STAT |= Memory.STATReg.ScanVRAMMode;
                    action = ScanVRAM;
                    break;

                case Memory.STATReg.ScanVRAMMode:
                    memory.STAT &= (Memory.STATReg)0xFC; // Memory.STATReg.HBlankMode;
                    action = HBlank;
                    break;

                case Memory.STATReg.HBlankMode:
                    fulltick += tick;
                    tick = 0;
                    if (memory.LY++ == displayHeight - 1)
                    {
                        memory.STAT |= Memory.STATReg.VBlankMode;
                        action = VBlank;
                    }
                    else
                    {
                        memory.STAT |= Memory.STATReg.ScanOAMMode;
                        action = ScanOAM;
                    }
                    break;

                case Memory.STATReg.VBlankMode:
                    fulltick += tick;
                    tick = 0;
                    if (memory.LY++ >= displayHeight + 9)
                    {
                        memory.LY = 0;
                        memory.STAT++; // Memory.STATReg.ScanOAMMode;
                        action = ScanOAM;
                    }
                    else
                    {
                        action = VBlank;
                    }
                    break;
            }
        }

        internal class PixelFIFO
        {
            private readonly Graphic graphic;
            private readonly Memory memory;
            private readonly Queue<byte> FIFO;
            private byte x;

            internal int Count { get => FIFO.Count; }

            internal PixelFIFO(Graphic graphic, Memory memory)
            {
                this.graphic = graphic;
                this.memory = memory;
                FIFO = new Queue<byte>(16);
            }

            internal void EnqueueData(byte data0, byte data1)
            {
                for (int i = 7; i >= 0; i--)
                {
                    byte data = (byte)((((data1 >> i) * 0b10) + ((data0 >> i) & 1)) & 0b11);
                    FIFO.Enqueue(data);
                }
            }

            internal bool DequeueData()
            {
                graphic.displayData.x = x++;
                graphic.displayData.y = memory.LY;
                graphic.displayData.color = FIFO.Dequeue();
                return !(x == 160);
            }

            internal void Reset()
            {
                x = 0;
                FIFO.Clear();
            }
        }

        internal class Fetcher
        {
            private enum FetcherState
            {
                ReadTileID,
                ReadData0,
                ReadData1,
                Sleep,
            };

            private readonly Memory memory;
            private readonly PixelFIFO pixelFIFO;
            private FetcherState state;
            private sbyte tileID;
            private byte data0;
            private byte data1;
            private bool workFlag;
            private byte currentX;

            internal Fetcher(Graphic graphic, Memory memory, PixelFIFO pixelFIFO)
            {
                this.memory = memory;
                this.pixelFIFO = pixelFIFO;
                state = FetcherState.ReadTileID;
                workFlag = true;
            }

            internal void Run()
            {
                workFlag = !workFlag;
                if (!workFlag) return;

                switch (state)
                {
                    case FetcherState.ReadTileID:
                        ushort tilemap = memory.LCDC.HasFlag(Memory.LCDCReg.SelectBGTileMap) ? 0x9C00 : 0x9800;
                        byte xAddress = (byte)(((memory.SCX / 8) + currentX) & 0x1F);
                        ushort yAddress = (ushort)(((memory.SCY / 8 + memory.LY / 8) & 0x1F) * 0x20);
                        tileID = (sbyte)memory.Ram[tilemap + xAddress + yAddress];
                        state = FetcherState.ReadData0;
                        break;

                    case FetcherState.ReadData0:
                        data0 = GetData(0);
                        state = FetcherState.ReadData1;
                        break;

                    case FetcherState.ReadData1:
                        data1 = GetData(1);
                        state = FetcherState.Sleep;
                        break;
                }

                if (state == FetcherState.Sleep && pixelFIFO.Count <= 8)
                {
                    pixelFIFO.EnqueueData(data0, data1);
                    currentX++;
                    state = FetcherState.ReadTileID;
                }
            }

            private byte GetData(int num)
            {
                ushort address = memory.LCDC.HasFlag(Memory.LCDCReg.SelectTileData) ? 0x8000 : 0x9000;
                return memory.Ram[address + tileID * 0x10 + (memory.SCY + memory.LY) % 8 * 2 + num];
            }

            internal void Reset()
            {
                state = FetcherState.ReadTileID;
                workFlag = true;
                tileID = (sbyte)(data0 = data1 = 0);
                currentX = 0;
            }
        }
    }
}
