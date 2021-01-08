namespace Lunchbox
{
    public class Lunchbox
    {
        private readonly Cpu cpu;
        private readonly Memory memory;
        public Graphic graphic;
        public DebugInfo debugInfo;
        private int tick = 0;

        // Constructor
        public Lunchbox(string filepath)
        {
            memory = new Memory(filepath);
            cpu = new Cpu(memory);
            graphic = new Graphic(memory);
            debugInfo = new DebugInfo(cpu, memory);
        }

        public void Run()
        {
            if (tick-- == 0) tick = cpu.Run();
            // debugInfo.Update();
            graphic.Update();
        }

        public Lunchbox TestRun()
        {
            for (int i = 0; i < 4; i++)
            {
                if (tick++ % 4 == 0) cpu.Run();
                graphic.Update();
            }
            debugInfo.Update();
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
