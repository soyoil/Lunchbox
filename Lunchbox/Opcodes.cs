namespace Lunchbox
{
    public partial class Cpu
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

            ops[0x06] = () => B = memory.Ram[++PC];
            ops[0x0E] = () => C = memory.Ram[++PC];
            ops[0x16] = () => D = memory.Ram[++PC];
            ops[0x1E] = () => E = memory.Ram[++PC];
            ops[0x26] = () => H = memory.Ram[++PC];
            ops[0x2E] = () => L = memory.Ram[++PC];
            ops[0x3E] = () => A = memory.Ram[++PC];

            ops[0x46] = () => B = memory.Ram[HL];
            ops[0x4E] = () => C = memory.Ram[HL];
            ops[0x56] = () => D = memory.Ram[HL];
            ops[0x5E] = () => E = memory.Ram[HL];
            ops[0x66] = () => H = memory.Ram[HL];
            ops[0x6E] = () => L = memory.Ram[HL];
            ops[0x7E] = () => A = memory.Ram[HL];

            ops[0x70] = () => memory.Ram[HL] = B;
            ops[0x71] = () => memory.Ram[HL] = C;
            ops[0x72] = () => memory.Ram[HL] = D;
            ops[0x73] = () => memory.Ram[HL] = E;
            ops[0x74] = () => memory.Ram[HL] = H;
            ops[0x75] = () => memory.Ram[HL] = L;
            ops[0x77] = () => memory.Ram[HL] = A;

            ops[0x36] = () => memory.Ram[HL] = memory.Ram[++PC];

            ops[0x0A] = () => A = memory.Ram[BC];
            ops[0x1A] = () => A = memory.Ram[DE];
            ops[0xFA] = () => A = memory.Ram[getTwoBitesFromRam()];

            ops[0x02] = () => memory.Ram[BC] = A;
            ops[0x12] = () => memory.Ram[DE] = A;
            ops[0xEA] = () => memory.Ram[getTwoBitesFromRam()] = A;

            ops[0xF0] = () => A = memory.Ram[0xFF00 + memory.Ram[++PC]];
            ops[0xE0] = () => memory.Ram[0xFF00 + memory.Ram[++PC]] = A;
            ops[0xF2] = () => A = memory.Ram[0xFF00 + C];
            ops[0xE2] = () => memory.Ram[0xFF00 + C] = A;

            ops[0x22] = () => memory.Ram[HL + 1] = A;
            ops[0x32] = () => memory.Ram[HL - 1] = A;
            ops[0x2A] = () => A = memory.Ram[HL + 1];
            ops[0x3A] = () => A = memory.Ram[HL - 1];

            ops[0x01] = () => BC = getTwoBitesFromRam();
            ops[0x11] = () => DE = getTwoBitesFromRam();
            ops[0x21] = () => HL = getTwoBitesFromRam();
            ops[0x31] = () => SP = getTwoBitesFromRam();

            ops[0xF9] = () => SP = HL;

            ops[0x08] = () =>
            {
                memory.Ram[++PC] = (byte)(SP & 0xFF);
                memory.Ram[++PC] = (byte)(SP >> 4);
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
            ops[0xC6] = () => Add(memory.Ram[++PC]);
            ops[0x86] = () => Add(memory.Ram[HL]);

            ops[0x88] = () => Add(B, true);
            ops[0x89] = () => Add(C, true);
            ops[0x8A] = () => Add(D, true);
            ops[0x8B] = () => Add(E, true);
            ops[0x8C] = () => Add(H, true);
            ops[0x8D] = () => Add(L, true);
            ops[0x8F] = () => Add(A, true);
            ops[0xCE] = () => Add(memory.Ram[++PC], true);
            ops[0x8E] = () => Add(memory.Ram[HL], true);

            ops[0x90] = () => Sub(B);
            ops[0x91] = () => Sub(C);
            ops[0x92] = () => Sub(D);
            ops[0x93] = () => Sub(E);
            ops[0x94] = () => Sub(H);
            ops[0x95] = () => Sub(L);
            ops[0x97] = () => Sub(A);
            ops[0xD6] = () => Sub(memory.Ram[++PC]);
            ops[0x96] = () => Sub(memory.Ram[HL]);

            ops[0x98] = () => Sub(B, true);
            ops[0x99] = () => Sub(C, true);
            ops[0x9A] = () => Sub(D, true);
            ops[0x9B] = () => Sub(E, true);
            ops[0x9C] = () => Sub(H, true);
            ops[0x9D] = () => Sub(L, true);
            ops[0x9F] = () => Sub(A, true);
            ops[0xDE] = () => Sub(memory.Ram[++PC], true);
            ops[0x9E] = () => Sub(memory.Ram[HL], true);

            ops[0xA0] = () => And(B);
            ops[0xA1] = () => And(C);
            ops[0xA2] = () => And(D);
            ops[0xA3] = () => And(E);
            ops[0xA4] = () => And(H);
            ops[0xA5] = () => And(L);
            ops[0xA7] = () => And(A);
            ops[0xE6] = () => And(memory.Ram[++PC]);
            ops[0xA6] = () => And(memory.Ram[HL]);

            ops[0xB0] = () => Or(B);
            ops[0xB1] = () => Or(C);
            ops[0xB2] = () => Or(D);
            ops[0xB3] = () => Or(E);
            ops[0xB4] = () => Or(H);
            ops[0xB5] = () => Or(L);
            ops[0xB7] = () => Or(A);
            ops[0xF6] = () => Or(memory.Ram[++PC]);
            ops[0xB6] = () => Or(memory.Ram[HL]);

            ops[0xA8] = () => Xor(B);
            ops[0xA9] = () => Xor(C);
            ops[0xAA] = () => Xor(D);
            ops[0xAB] = () => Xor(E);
            ops[0xAC] = () => Xor(H);
            ops[0xAD] = () => Xor(L);
            ops[0xAF] = () => Xor(A);
            ops[0xEE] = () => Xor(memory.Ram[++PC]);
            ops[0xAE] = () => Xor(memory.Ram[HL]);

            ops[0xB8] = () => Cp(B);
            ops[0xB9] = () => Cp(C);
            ops[0xBA] = () => Cp(D);
            ops[0xBB] = () => Cp(E);
            ops[0xBC] = () => Cp(H);
            ops[0xBD] = () => Cp(L);
            ops[0xBF] = () => Cp(A);
            ops[0xFE] = () => Cp(memory.Ram[++PC]);
            ops[0xBE] = () => Cp(memory.Ram[HL]);

            ops[0x04] = () => B = Increment(B);
            ops[0x0C] = () => C = Increment(C);
            ops[0x14] = () => D = Increment(D);
            ops[0x1C] = () => E = Increment(E);
            ops[0x24] = () => H = Increment(H);
            ops[0x2C] = () => L = Increment(L);
            ops[0x34] = () => memory.Ram[HL] = Increment(memory.Ram[HL]);
            ops[0x3C] = () => A = Increment(A);

            ops[0x05] = () => B = Decrement(B);
            ops[0x0D] = () => C = Decrement(C);
            ops[0x15] = () => D = Decrement(D);
            ops[0x1D] = () => E = Decrement(E);
            ops[0x25] = () => H = Decrement(H);
            ops[0x2D] = () => L = Decrement(L);
            ops[0x35] = () => memory.Ram[HL] = Decrement(memory.Ram[HL]);
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
        }
    }
}
