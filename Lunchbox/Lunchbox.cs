using System;

namespace Lunchbox
{
    public class Lunchbox
    {
        Cpu cpu;
        Memory memory;

        // Constructor
        public Lunchbox()
        {
            memory = new Memory();
            cpu = new Cpu(memory);
        }

        public void run()
        {
            cpu.run();
        }
    }
}
