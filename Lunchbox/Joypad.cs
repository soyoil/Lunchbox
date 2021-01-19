using System;
using System.Collections.Generic;

namespace Lunchbox
{
    public class Joypad
    {
        private readonly Memory memory;
        public byte PushedKeyDictionary;

        public enum Keys : byte
        {
            Right = 1,
            Left = 1 << 1,
            Up = 1 << 2,
            Down = 1 << 3,
            A = 1 << 4,
            B = 1 << 5,
            Select = 1 << 6,
            Start = 1 << 7,
        }

        internal Joypad(Memory memory)
        {
            this.memory = memory;
            PushedKeyDictionary = 0;
        }

        internal void SetJoypadInterrupt()
        {
            if (PushedKeyDictionary != 0) memory.IF |= Memory.IFReg.IsRequestedJoypadInterrupt;
            else memory.IF &= ~Memory.IFReg.IsRequestedJoypadInterrupt;
        }
    }
}
