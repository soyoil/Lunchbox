namespace Lunchbox
{
    internal partial class Cpu
    {
        private void RegisterOps()
        {
            ops[0x40] = () => { };
            ops[0x41] = () => B = C;
            ops[0x42] = () => B = D;
            ops[0x43] = () => B = E;
            ops[0x44] = () => B = H;
            ops[0x45] = () => B = L;
            ops[0x47] = () => B = A;

            ops[0x48] = () => C = B;
            ops[0x49] = () => { };
            ops[0x4A] = () => C = D;
            ops[0x4B] = () => C = E;
            ops[0x4C] = () => C = H;
            ops[0x4D] = () => C = L;
            ops[0x4F] = () => C = A;

            ops[0x50] = () => D = B;
            ops[0x51] = () => D = C;
            ops[0x52] = () => { };
            ops[0x53] = () => D = E;
            ops[0x54] = () => D = H;
            ops[0x55] = () => D = L;
            ops[0x57] = () => D = A;

            ops[0x58] = () => E = B;
            ops[0x59] = () => E = C;
            ops[0x5A] = () => E = D;
            ops[0x5B] = () => { };
            ops[0x5C] = () => E = H;
            ops[0x5D] = () => E = L;
            ops[0x5F] = () => E = A;

            ops[0x60] = () => H = B;
            ops[0x61] = () => H = C;
            ops[0x62] = () => H = D;
            ops[0x63] = () => H = E;
            ops[0x64] = () => { };
            ops[0x65] = () => H = L;
            ops[0x67] = () => H = A;

            ops[0x68] = () => L = B;
            ops[0x69] = () => L = C;
            ops[0x6A] = () => L = D;
            ops[0x6B] = () => L = E;
            ops[0x6C] = () => L = H;
            ops[0x6D] = () => { };
            ops[0x6F] = () => L = A;

            ops[0x78] = () => A = B;
            ops[0x79] = () => A = C;
            ops[0x7A] = () => A = D;
            ops[0x7B] = () => A = E;
            ops[0x7C] = () => A = H;
            ops[0x7D] = () => A = L;
            ops[0x7F] = () => { };

            ops[0x06] = () => B = memory[++PC];
            ops[0x0E] = () => C = memory[++PC];
            ops[0x16] = () => D = memory[++PC];
            ops[0x1E] = () => E = memory[++PC];
            ops[0x26] = () => H = memory[++PC];
            ops[0x2E] = () => L = memory[++PC];
            ops[0x3E] = () => A = memory[++PC];

            ops[0x46] = () => B = memory[HL];
            ops[0x4E] = () => C = memory[HL];
            ops[0x56] = () => D = memory[HL];
            ops[0x5E] = () => E = memory[HL];
            ops[0x66] = () => H = memory[HL];
            ops[0x6E] = () => L = memory[HL];
            ops[0x7E] = () => A = memory[HL];

            ops[0x70] = () => memory[HL] = B;
            ops[0x71] = () => memory[HL] = C;
            ops[0x72] = () => memory[HL] = D;
            ops[0x73] = () => memory[HL] = E;
            ops[0x74] = () => memory[HL] = H;
            ops[0x75] = () => memory[HL] = L;
            ops[0x77] = () => memory[HL] = A;

            ops[0x36] = () => memory[HL] = memory[++PC];

            ops[0x0A] = () => A = memory[BC];
            ops[0x1A] = () => A = memory[DE];
            ops[0xFA] = () => A = memory[GetTwoBitesFromRam()];

            ops[0x02] = () => memory[BC] = A;
            ops[0x12] = () => memory[DE] = A;
            ops[0xEA] = () => memory[GetTwoBitesFromRam()] = A;

            ops[0xF0] = () => A = memory[0xFF00 + memory[++PC]];
            ops[0xE0] = () => memory[0xFF00 + memory[++PC]] = A;
            ops[0xF2] = () => A = memory[0xFF00 + C];
            ops[0xE2] = () => memory[0xFF00 + C] = A;

            ops[0x22] = () => memory[HL++] = A;
            ops[0x32] = () => memory[HL--] = A;
            ops[0x2A] = () => A = memory[HL++];
            ops[0x3A] = () => A = memory[HL--];

            ops[0x01] = () => BC = GetTwoBitesFromRam();
            ops[0x11] = () => DE = GetTwoBitesFromRam();
            ops[0x21] = () => HL = GetTwoBitesFromRam();
            ops[0x31] = () => SP = GetTwoBitesFromRam();

            ops[0xF9] = () => SP = HL;

            ops[0x08] = () =>
            {
                memory[++PC] = (byte)(SP & 0xFF);
                memory[++PC] = (byte)(SP >> 4);
            };

            ops[0xC5] = () => Push(BC);
            ops[0xD5] = () => Push(DE);
            ops[0xE5] = () => Push(HL);
            ops[0xF5] = () => Push(AF);

            ops[0xC1] = () => BC = Pop();
            ops[0xD1] = () => DE = Pop();
            ops[0xE1] = () => HL = Pop();
            ops[0xF1] = () => AF = Pop();

            ops[0x80] = () => Add(B);
            ops[0x81] = () => Add(C);
            ops[0x82] = () => Add(D);
            ops[0x83] = () => Add(E);
            ops[0x84] = () => Add(H);
            ops[0x85] = () => Add(L);
            ops[0x87] = () => Add(A);
            ops[0xC6] = () => Add(memory[++PC]);
            ops[0x86] = () => Add(memory[HL]);

            ops[0x88] = () => Add(B, true);
            ops[0x89] = () => Add(C, true);
            ops[0x8A] = () => Add(D, true);
            ops[0x8B] = () => Add(E, true);
            ops[0x8C] = () => Add(H, true);
            ops[0x8D] = () => Add(L, true);
            ops[0x8F] = () => Add(A, true);
            ops[0xCE] = () => Add(memory[++PC], true);
            ops[0x8E] = () => Add(memory[HL], true);

            ops[0x90] = () => Sub(B);
            ops[0x91] = () => Sub(C);
            ops[0x92] = () => Sub(D);
            ops[0x93] = () => Sub(E);
            ops[0x94] = () => Sub(H);
            ops[0x95] = () => Sub(L);
            ops[0x97] = () => Sub(A);
            ops[0xD6] = () => Sub(memory[++PC]);
            ops[0x96] = () => Sub(memory[HL]);

            ops[0x98] = () => Sub(B, true);
            ops[0x99] = () => Sub(C, true);
            ops[0x9A] = () => Sub(D, true);
            ops[0x9B] = () => Sub(E, true);
            ops[0x9C] = () => Sub(H, true);
            ops[0x9D] = () => Sub(L, true);
            ops[0x9F] = () => Sub(A, true);
            ops[0xDE] = () => Sub(memory[++PC], true);
            ops[0x9E] = () => Sub(memory[HL], true);

            ops[0xA0] = () => And(B);
            ops[0xA1] = () => And(C);
            ops[0xA2] = () => And(D);
            ops[0xA3] = () => And(E);
            ops[0xA4] = () => And(H);
            ops[0xA5] = () => And(L);
            ops[0xA7] = () => And(A);
            ops[0xE6] = () => And(memory[++PC]);
            ops[0xA6] = () => And(memory[HL]);

            ops[0xB0] = () => Or(B);
            ops[0xB1] = () => Or(C);
            ops[0xB2] = () => Or(D);
            ops[0xB3] = () => Or(E);
            ops[0xB4] = () => Or(H);
            ops[0xB5] = () => Or(L);
            ops[0xB7] = () => Or(A);
            ops[0xF6] = () => Or(memory[++PC]);
            ops[0xB6] = () => Or(memory[HL]);

            ops[0xA8] = () => Xor(B);
            ops[0xA9] = () => Xor(C);
            ops[0xAA] = () => Xor(D);
            ops[0xAB] = () => Xor(E);
            ops[0xAC] = () => Xor(H);
            ops[0xAD] = () => Xor(L);
            ops[0xAF] = () => Xor(A);
            ops[0xEE] = () => Xor(memory[++PC]);
            ops[0xAE] = () => Xor(memory[HL]);

            ops[0xB8] = () => Cp(B);
            ops[0xB9] = () => Cp(C);
            ops[0xBA] = () => Cp(D);
            ops[0xBB] = () => Cp(E);
            ops[0xBC] = () => Cp(H);
            ops[0xBD] = () => Cp(L);
            ops[0xBF] = () => Cp(A);
            ops[0xFE] = () => Cp(memory[++PC]);
            ops[0xBE] = () => Cp(memory[HL]);

            ops[0x04] = () => B = Increment(B);
            ops[0x0C] = () => C = Increment(C);
            ops[0x14] = () => D = Increment(D);
            ops[0x1C] = () => E = Increment(E);
            ops[0x24] = () => H = Increment(H);
            ops[0x2C] = () => L = Increment(L);
            ops[0x34] = () => memory[HL] = Increment(memory[HL]);
            ops[0x3C] = () => A = Increment(A);

            ops[0x05] = () => B = Decrement(B);
            ops[0x0D] = () => C = Decrement(C);
            ops[0x15] = () => D = Decrement(D);
            ops[0x1D] = () => E = Decrement(E);
            ops[0x25] = () => H = Decrement(H);
            ops[0x2D] = () => L = Decrement(L);
            ops[0x35] = () => memory[HL] = Decrement(memory[HL]);
            ops[0x3D] = () => A = Decrement(A);

            ops[0x09] = () => Add(BC);
            ops[0x19] = () => Add(DE);
            ops[0x29] = () => Add(HL);
            ops[0x39] = () => Add(SP);

            ops[0x03] = () => BC++;
            ops[0x13] = () => DE++;
            ops[0x23] = () => HL++;
            ops[0x33] = () => SP++;

            ops[0x0B] = () => BC--;
            ops[0x1B] = () => DE--;
            ops[0x2B] = () => HL--;
            ops[0x3B] = () => SP--;

            ops[0xE8] = () => SP = AddSP();
            ops[0xF8] = () => HL = AddSP();

            ops[0x27] = () => Daa();

            ops[0x2F] = () =>
            {
                A ^= 0xFF;
                SetFlag(Flags.N, true);
                SetFlag(Flags.H, true);
            };
            ops[0x3F] = () =>
            {
                SetFlag(Flags.N, false);
                SetFlag(Flags.H, false);
                SetFlag(Flags.C, GetFlag(Flags.C) ^ true);
            };
            ops[0x37] = () =>
            {
                SetFlag(Flags.N, false);
                SetFlag(Flags.H, false);
                SetFlag(Flags.C, true);
            };

            ops[0x00] = () => { };
            ops[0x76] = () => 
            { 
                // HALT, TODO
            };
            ops[0x10] = () =>
            {
                // STOP, TODO
            };

            ops[0xF3] = () => IME = false;
            ops[0xFB] = () => IME = true;

            ops[0x07] = () => RotateLeft(ref A, true, false);
            ops[0x17] = () => RotateLeft(ref A, false, false);

            ops[0xC3] = () => AbsoluteJump();
            ops[0xDA] = () => AbsoluteJump(Flags.C, true);
            ops[0xD2] = () => AbsoluteJump(Flags.C, false);
            ops[0xCA] = () => AbsoluteJump(Flags.Z, true);
            ops[0xC2] = () => AbsoluteJump(Flags.Z, false);
            ops[0xE9] = () => PC = (ushort)(HL - 1);

            ops[0x18] = () => RelativeJump();
            ops[0x38] = () => RelativeJump(Flags.C, true);
            ops[0x30] = () => RelativeJump(Flags.C, false);
            ops[0x28] = () => RelativeJump(Flags.Z, true);
            ops[0x20] = () => RelativeJump(Flags.Z, false);

            ops[0xCD] = () => Call();
            ops[0xDC] = () => Call(Flags.C, true);
            ops[0xD4] = () => Call(Flags.C, false);
            ops[0xCC] = () => Call(Flags.Z, true);
            ops[0xC4] = () => Call(Flags.Z, false);

            ops[0xC9] = () => Ret();
            ops[0xD8] = () => Ret(Flags.C, true);
            ops[0xD0] = () => Ret(Flags.C, false);
            ops[0xC8] = () => Ret(Flags.Z, true);
            ops[0xC0] = () => Ret(Flags.Z, false);

            ops[0xD9] = () =>
            {
                IME = true;
                Ret();
            };

            ops[0xC7] = () => Rst(0x00);
            ops[0xCF] = () => Rst(0x08);
            ops[0xD7] = () => Rst(0x10);
            ops[0xDF] = () => Rst(0x18);
            ops[0xE7] = () => Rst(0x20);
            ops[0xEF] = () => Rst(0x28);
            ops[0xF7] = () => Rst(0x30);
            ops[0xFF] = () => Rst(0x38);

            ops[0xCB] = () => PrefixCB();
        }
    }
}
