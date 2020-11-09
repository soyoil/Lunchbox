using System;
namespace Lunchbox
{
    public class Memory
    {
        public byte[] Ram;
        public Memory()
        {
            Ram = new byte[0x10000];
        }
    }
}
