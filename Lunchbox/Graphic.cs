using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunchbox
{
    public class Graphic
    {
        private const byte displayHeight = 144;

        public Dictionary<string, byte> DisplayData { get; private set; }

        private readonly Memory memory;
        private readonly Fetcher fetcher;
        private readonly PixelFIFO pixelFIFO;
        private byte x;
        private int tick;
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
            DisplayData = new Dictionary<string, byte>
            {
                {"x", 0 },
                {"y", 0 },
                {"color", 0 },
            };

            ScanOAM = () =>
            {
                isProcessing = ++tick < 80;
            };

            ScanVRAM = () =>
            {
                fetcher.Run();
                if (pixelFIFO.Count > 8)
                {
                    isProcessing = pixelFIFO.DequeueData();
                    if (!isProcessing)
                    {
                        pixelFIFO.Reset();
                        fetcher.Reset();
                    }
                }
            };

            HBlank = () =>
            {
                isProcessing = ++tick < 456;
            };

            VBlank = () =>
            {
                isProcessing = ++tick < 456;
            };
        }

        internal void Update()
        {
            if (!memory.LCDC.HasFlag(Memory.LCDCReg.IsEnableDisplay)) return;

            action();
            if (isProcessing) return;

            switch (memory.STAT & (Memory.STATReg)0b11)
            {
                case Memory.STATReg.ScanOAMMode:
                    memory.STAT |= Memory.STATReg.ScanVRAMMode;
                    action = ScanVRAM;
                    break;

                case Memory.STATReg.ScanVRAMMode:
                    memory.STAT |= Memory.STATReg.HBlankMode;
                    action = HBlank;
                    break;

                case Memory.STATReg.HBlankMode:
                    tick = 0;
                    if (++memory.LY == displayHeight)
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
                    tick = 0;
                    if (++memory.LY > displayHeight + 9)
                    {
                        memory.LY = 0;
                        memory.STAT |= Memory.STATReg.ScanOAMMode;
                        action = ScanOAM;
                    } else
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
                    byte data = (byte)(((data1 >> i) * 0b10) + (data0 >> i));
                    FIFO.Enqueue(data);
                }
            }

            internal bool DequeueData()
            {
                graphic.DisplayData["x"] = x++;
                graphic.DisplayData["y"] = memory.LY;
                graphic.DisplayData["color"] = FIFO.Dequeue();
                return !(x == 160);
            }

            internal void Reset()
            {
                x = 0;
            }
        }

        internal class Fetcher
        {
            private enum FetcherState
            {
                ReadTileID,
                ReadData0,
                ReadData1,
                Sleep
            };

            private readonly Memory memory;
            private readonly Graphic graphic;
            private readonly PixelFIFO pixelFIFO;
            private FetcherState state;
            private sbyte tileID;
            private byte data0;
            private byte data1;
            private bool workFlag;

            internal Fetcher(Graphic graphic, Memory memory, PixelFIFO pixelFIFO)
            {
                this.graphic = graphic;
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
                        byte xAddress = (byte)(((memory.SCX / 8) + graphic.x) & 0x1F);
                        byte yAddress = (byte)((((memory.SCY + memory.LY) / 8) & 0x1F) * 0x20);
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

                if (state == FetcherState.Sleep)
                {
                    pixelFIFO.EnqueueData(data0, data1);
                    graphic.x++;
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
            }
        }
    }
}
