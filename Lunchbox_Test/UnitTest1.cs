using Xunit;

namespace Lunchbox_Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var lunchbox = new Lunchbox.Lunchbox()
                .InjectToMemory(0x0, new byte[] { 0xFA, 0x46, 0x10, 0x6F })
                .InjectToMemory(0x1046, 0x7E)
                .TestRun(0x3);
            Assert.Equal(0x7E, lunchbox.debugInfo.RegDict["L"]);
        }

        [Fact]
        public void LDTest()
        {
            var GB = new Lunchbox.Lunchbox()
                .InjectToMemory(0x0, new byte[] { 0x06, 0x56 })
                .TestRun(0x1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["B"]);
        }

        [Fact]
        public void AddTest()
        {
            var GB = new Lunchbox.Lunchbox()
                .InjectToMemory(0x0, new byte[] { 0x3E, 0x14, 0x06, 0x05, 0x80 })
                .TestRun(0x4);
            Assert.Equal(0x19, GB.debugInfo.RegDict["A"]);
        }
    }
}
