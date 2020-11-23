using System.Collections.Generic;

namespace Lunchbox
{
  public class DebugInfo
  {
    public byte Opcode;
    public string Operand;
    public Dictionary<string, ushort> RegDict;

        private readonly Cpu cpuptr;
        private readonly Memory memptr;

    public DebugInfo(Cpu cpu, Memory memory)
        {
            cpuptr = cpu;
            memptr = memory;
            Update();
        }

    public void Update()
        {
            RegDict = cpuptr.RegDict;
            Opcode = memptr.Ram[cpuptr.PC];
        }
  }
}
