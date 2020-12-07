using System;

namespace Lunchbox
{
    public class Lunchbox
    {
        private readonly Cpu cpu;
        private readonly Memory memory;
        public Graphic graphic;
        public DebugInfo debugInfo;
        private int tick;

        // Constructor
        public Lunchbox()
        {
            memory = new Memory();
            cpu = new Cpu(memory);
            graphic = new Graphic(memory);
            debugInfo = new DebugInfo(cpu, memory);
        }

        public void Run()
        {
            if (tick++ % 4 == 0) cpu.run();
            // debugInfo.Update();
            graphic.Update();
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
