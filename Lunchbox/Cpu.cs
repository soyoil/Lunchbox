using System;
using System.Collections.Generic;

namespace Lunchbox
{
    internal partial class Cpu
    {
        // 8bit registers
        private byte A;
        private byte B;
        private byte C;
        private byte D;
        private byte E;
        private Flags F;
        private byte H;
        private byte L;

        // 16bit registers
        private ushort SP;
        internal ushort PC { get; set; }

        internal bool IME;

        // Pair registers
        private ushort AF
        {
            get
            {
                return (ushort)(A * 0x100 + F);
            }
            set
            {
                A = (byte)(value / 0x100);
                F = (Flags)(value % 0x100);
            }
        }
        private ushort BC
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
        private ushort DE
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
        private ushort HL
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

        internal Dictionary<string, ushort> RegDict
        {
            get
            {
                var dict = new Dictionary<string, ushort>
                {
                    { "A", A },
                    { "B", B },
                    { "C", C },
                    { "D", D },
                    { "E", E },
                    { "H", H },
                    { "L", L }
                };
                return dict;
            }
        }

        // Flags set
        [Flags]
        private enum Flags
        {
            Z = 1 << 7,
            N = 1 << 6,
            H = 1 << 5,
            C = 1 << 4,
        };

        private readonly Action[] ops;

        private readonly Memory memory;

        private int JumpOpCycle;
        private bool isHalted;

        // Constructor
        internal Cpu(Memory memoryPtr = null)
        {
            A = B = C = D = E = H = L = 0;
            F = 0;
            PC = 0;
            memory = memoryPtr;
            ops = new Action[0x100];
            JumpOpCycle = 0;
            isHalted = false;
            RegisterOps();
        }

        private void SetFlag(Flags flag, bool isSet)
        {
            if (isSet)
            {
                F |= flag;
            }
            else
            {
                F &= ~flag;
            }
        }

        private bool GetFlag(Flags flag) => F.HasFlag(flag);

        public int Run()
        {
            if (isHalted)
            {
                if (memory.IF != 0) isHalted = false;
                return 4;
            }

            if (memory.IF != 0 && IME)
            {
                Push(PC);
                PC = ResolveIF();
            }

            byte opcode = memory[PC];
            ops[opcode]();
            PC++;
            return opcycle[opcode] == 0 ? JumpOpCycle * 4 : opcycle[opcode] * 4;
        }

        internal void TestRun(ushort endAddr)
        {
            do
            {
                byte opcode = memory[PC];
                ops[opcode]();
            } while (PC++ != endAddr);
        }

        private ushort ResolveIF()
        {
            IME = false;
            if (memory.IF.HasFlag(Memory.IFReg.IsRequestedVBlankInterrupt))
            {
                memory.IF &= ~Memory.IFReg.IsRequestedVBlankInterrupt;
                return 0x0040;
            }
            else if (memory.IF.HasFlag(Memory.IFReg.IsRequestedSTATInterrupt))
            {
                memory.IF &= ~Memory.IFReg.IsRequestedSTATInterrupt;
                return 0x0048;
            }
            else if (memory.IF.HasFlag(Memory.IFReg.IsRequestedTimerInterrupt))
            {
                memory.IF &= ~Memory.IFReg.IsRequestedTimerInterrupt;
                return 0x0050;
            }
            else if (memory.IF.HasFlag(Memory.IFReg.IsRequestedSerialInterrupt))
            {
                memory.IF &= ~Memory.IFReg.IsRequestedSerialInterrupt;
                return 0x0058;
            }
            else if (memory.IF.HasFlag(Memory.IFReg.IsRequestedJoypadInterrupt))
            {
                memory.IF &= ~Memory.IFReg.IsRequestedJoypadInterrupt;
                return 0x0060;
            }
            else
            {
                return PC;
            }
        }

        private ushort GetTwoBitesFromRam() => (ushort)(memory[++PC] + memory[++PC] * 0x100);

        private void Push(ushort value)
        {
            memory[--SP] = (byte)(value >> 8);
            memory[--SP] = (byte)(value & 0xFF);
        }

        private ushort Pop() => (ushort)(memory[SP++] + (memory[SP++] << 8));

        private void Add(byte value, bool isADC = false)
        {
            int result = A + value + Convert.ToInt32(isADC);
            SetFlag(Flags.Z, (byte)result == 0);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, (A & 0xF) + (value & 0xF) + Convert.ToInt32(isADC) > 0xF);
            SetFlag(Flags.C, result > 0xFF);
            A = (byte)result;
        }

        private void Add(ushort value)
        {
            var result = HL + value;
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, (HL & 0xF) + (value & 0xF) > 0xF);
            SetFlag(Flags.C, result > 0xFFFF);
            HL = (ushort)result;
        }

        private ushort AddSP()
        {
            var e = memory[++PC];
            var result = SP + e;
            SetFlag(Flags.Z, false);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, (HL & 0xF) + (e & 0xF) > 0xF);
            SetFlag(Flags.C, result > 0xFFFF);
            return (ushort)result;
        }

        private void Sub(byte value, bool isSBC = false) => A = Cp(value, isSBC);

        private byte Cp(byte value, bool isSBC = false)
        {
            int result = A - value - Convert.ToInt32(isSBC);
            SetFlag(Flags.Z, result == 0);
            SetFlag(Flags.N, true);
            SetFlag(Flags.H, (A & 0xF) + (value & 0xF) + Convert.ToInt32(isSBC) < 0);
            SetFlag(Flags.C, result < 0);
            return (byte)result;
        }

        private void And(byte value)
        {
            A = (byte)(A & value);
            SetFlag(Flags.Z, A == 0);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, true);
            SetFlag(Flags.C, false);
        }

        private void Or(byte value)
        {
            A = (byte)(A | value);
            SetFlag(Flags.Z, A == 0);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, false);
            SetFlag(Flags.C, false);
        }

        private void Xor(byte value)
        {
            A = (byte)(A ^ value);
            SetFlag(Flags.Z, A == 0);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, false);
            SetFlag(Flags.C, false);
        }

        private byte Increment(byte value)
        {
            SetFlag(Flags.Z, value == 0xFF);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, (value & 0xF) == 0xF);
            return (byte)(value + 1);
        }

        private byte Decrement(byte value)
        {
            SetFlag(Flags.Z, value == 1);
            SetFlag(Flags.N, true);
            SetFlag(Flags.H, (value & 0xF) == 0);
            return (byte)(value - 1);
        }

        private void Daa()
        {
            int a = A;
            int collection = 0;
            Flags setFlagC = 0;
            if (GetFlag(Flags.H) || (!GetFlag(Flags.N) && (a & 0xF) > 9))
                collection |= 0x6;
            if (GetFlag(Flags.C) || (!GetFlag(Flags.N) && a > 0x99))
            {
                collection |= 0x60;
                setFlagC = Flags.C;
            }
            a += GetFlag(Flags.N) ? -collection : collection;
            a &= 0xFF;
            Flags setFlagZ = a == 0 ? Flags.Z : 0;
            F &= ~(Flags.Z | Flags.H | Flags.C);
            F |= setFlagC | setFlagZ;
            A = (byte)a;
        }

        private void RotateLeft(int selector, bool circular, bool onlyShift)
        {
            byte result = GetValueFromSelector(selector);
            byte top = (byte)(result >> 7);
            result = (byte)(result << 1);
            if (onlyShift)
            {
                SetFlag(Flags.Z, result == 0);
            }
            else
            {
                result += circular ? top : Convert.ToByte(F.HasFlag(Flags.C));
                SetFlag(Flags.Z, false);
            }
            SetFlag(Flags.C, Convert.ToBoolean(top));
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, false);
            SetValueFromSelector(selector, result);
        }

        private void AbsoluteJump(Flags flag = 0, bool isTrue = false)
        {
            if (flag == 0 || F.HasFlag(flag) == isTrue)
            {
                PC = (ushort)(GetTwoBitesFromRam() - 1);
                JumpOpCycle = 4;
            }
            else
            {
                PC += 2;
                JumpOpCycle = 3;
            }
        }

        private void RelativeJump(Flags flag = 0, bool isTrue = false)
        {
            if (flag == 0 || F.HasFlag(flag) == isTrue)
            {
                PC = (ushort)(++PC + (sbyte)memory[PC]);
                JumpOpCycle = 3;
            }
            else
            {
                PC++;
                JumpOpCycle = 2;
            }
        }

        private void Call(Flags flag = 0, bool isTrue = false)
        {
            if (flag == 0 || F.HasFlag(flag) == isTrue)
            {
                Push((ushort)(PC + 2));
                PC = (ushort)(GetTwoBitesFromRam() - 1);
                JumpOpCycle = 6;
            }
            else
            {
                PC += 2;
                JumpOpCycle = 4;
            }
        }

        private void Ret(Flags flag = 0, bool isTrue = false)
        {
            if (flag == 0 || F.HasFlag(flag) == isTrue)
            {
                PC = Pop();
                JumpOpCycle = 5;
            }
            else
                JumpOpCycle = 2;
        }

        private void Rst(byte addr)
        {
            Push(PC);
            PC = (ushort)(addr & 0xFF - 1);
        }

        private void Bit(int selector, int place)
        {
            SetFlag(Flags.Z, GetValueFromSelector(selector) >> place == 0);
            SetFlag(Flags.N, false);
            SetFlag(Flags.H, true);
        }

        private void Set(int selector, int place) => SetValueFromSelector(selector, (byte)(GetValueFromSelector(selector) | 1 << place));

        private void Reset(int selector, int place) => SetValueFromSelector(selector, (byte)(GetValueFromSelector(selector) & ~(1 << place)));

        private byte GetValueFromSelector(int selector) => selector switch
        {
            0 => B,
            1 => C,
            2 => D,
            3 => E,
            4 => H,
            5 => L,
            6 => memory[HL],
            7 => A,
            _ => 0
        };

        private byte SetValueFromSelector(int selector, byte value) => selector switch
        {
            0 => B = value,
            1 => C = value,
            2 => D = value,
            3 => E = value,
            4 => H = value,
            5 => L = value,
            6 => memory[HL] = value,
            7 => A = value,
            _ => 0
        };

        private void PrefixCB()
        {
            byte subop = memory[++PC];

            if (subop <= 0x07) RotateLeft(subop, true, false);
            else if (subop <= 0x0F) return; // RotateRight(subop % 0x08, true, false);
            else if (subop <= 0x17) RotateLeft(subop % 0x10, false, false);
            else if (subop <= 0x1F) return; // RotateRight(subop % 0x18, false, false);
            else if (subop <= 0x27) RotateLeft(subop % 0x20, false, true);
            else if (subop <= 0x2F) return; // RotateRight(subop % 0x28, false, true);
            else if (subop <= 0x37) return; // swap
            else if (subop <= 0x3F) return; // srl
            else if (subop <= 0x7F) Bit((subop & 0xF) % 8, (subop - 0x40) / 8);
            else if (subop <= 0xBF) Reset((subop & 0xF) % 8, (subop - 0x80) / 8);
            else if (subop <= 0xFF) Set((subop & 0xF) % 8, (subop - 0xC0) / 8);
        }
    } 
}
