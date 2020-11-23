using System;

namespace Lunchbox
{
    public class Lunchbox
    {
        Cpu cpu;
        Memory memory;
        public DebugInfo debugInfo;

        // Constructor
        public Lunchbox()
        {
            memory = new Memory();
            cpu = new Cpu(memory);
            debugInfo = new DebugInfo(cpu, memory);
        }

        public void run()
        {
            cpu.run();
        }

        public Lunchbox TestRun(ushort endAddr)
        {
            cpu.TestRun(endAddr);
            debugInfo.Update();
            return this;
        }

        public Lunchbox InjectToMemory(ushort addr, byte value)
        {
            memory.Ram[addr] = value;
            return this;
        }

        public Lunchbox InjectToMemory(ushort startAddr, byte[] valueArray)
        {
            foreach (var value in valueArray)
            {
                memory.Ram[startAddr++] = value;
            }
            return this;
        }
    }
}
