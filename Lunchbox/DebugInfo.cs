using System.Collections.Generic;

namespace Lunchbox
{
    public class DebugInfo
    {
        public byte Opcode;
        public ushort Address;
        public string Operand;
        public Dictionary<string, ushort> RegDict;

        private readonly Cpu _cpu;
        private readonly Memory _memory;

        internal DebugInfo(Cpu cpu, Memory memory)
        {
            _cpu = cpu;
            _memory = memory;
            Update();
        }

        public void Update()
        {
            RegDict = _cpu.RegDict;
            Opcode = _memory[_cpu.PC];
            Address = _cpu.PC;
        }
    }
}
