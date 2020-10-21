using System;
using System.Collections.Generic;
using System.Text;

namespace Lunchbox
{
    class Cpu
    {
        // 8bit registers
        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte F { get; set; }
        public byte H { get; set; }
        public byte L { get; set; }

        // 16bit registers
        public ushort SP { get; set; }
        public ushort PC { get; set; }

        // Pair registers
        public ushort AF {
            get { 
                return (ushort)(A * 0x100 + F);
            }
            set {
                A = (byte)(value / 0x100);
                F = (byte)(value % 0x100);
            }
        }
        public ushort BC
        {
            get
            {
                return (ushort)(B * 0x100 + C);
            }
            set
            {
                B = (byte)(value / 0x100);
                C = (byte)(value % 0x100);
            }
        }
        public ushort DE
        {
            get
            {
                return (ushort)(D * 0x100 + E);
            }
            set
            {
                D = (byte)(value / 0x100);
                E = (byte)(value % 0x100);
            }
        }
        public ushort HL
        {
            get
            {
                return (ushort)(H * 0x100 + L);
            }
            set
            {
                H = (byte)(value / 0x100);
                L = (byte)(value % 0x100);
            }
        }

        // Flags set
        [Flags]
        enum Flags
        {
            Z = 1 << 7,
            N = 1 << 6,
            H = 1 << 5,
            C = 1 << 4,
        }

        // Constructor
        public Cpu()
        {

        }
    }
}
