using System.Collections.Generic;

namespace Lunchbox
{
    public class Lunchbox
    {
        private readonly Cpu cpu;
        private readonly Memory memory;
        public Graphic graphic;
        public Joypad joypad;
        public DebugInfo debugInfo;

        private int tick = 0;

        // Constructor
        public Lunchbox(string filepath)
        {
            memory = new Memory(filepath);
            cpu = new Cpu(memory);
            graphic = new Graphic(memory);
            joypad = new Joypad(memory);
            debugInfo = new DebugInfo(cpu, memory);
            memory.SetJoypadPtr(joypad);
        }

        public void Run()
        {
            if (tick-- == 0)
            {
                joypad.SetJoypadInterrupt();
                tick = cpu.Run();
            }
            // debugInfo.Update();
            graphic.Update();
        }

        public Lunchbox TestRun(ushort stopPlace)
        {
            memory[0xFF50] = 1;
            do
            {
                cpu.Run();
            } while (cpu.PC <= stopPlace);
            debugInfo.Update();
            // graphic.Update();
            return this;
        }

        public Lunchbox InjectToMemory(ushort addr, byte value)
        {
            memory[addr] = value;
            return this;
        }

        public Lunchbox InjectToMemory(ushort startAddr, byte[] valueArray)
        {
            foreach (var value in valueArray)
            {
                memory[startAddr++] = value;
            }
            return this;
        }
    }
}
