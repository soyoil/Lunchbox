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
                .TestRun();
            Assert.Equal(0x7E, lunchbox.debugInfo.RegDict["L"]);
        }

        [Fact]
        public void LDTest()
        {
            var GB = new Lunchbox.Lunchbox();
            GB.InjectToMemory(0x0, new byte[] { 0x06, 0x56 }).TestRun();
            Assert.Equal(0x56, GB.debugInfo.RegDict["B"]);
        }

        [Fact]
        public void AddTest()
        {
            var GB = new Lunchbox.Lunchbox()
                .InjectToMemory(0x0, new byte[] { 0x3E, 0x53, 0x06, 0x41, 0x80, 0x27 })
                .TestRun();
            Assert.Equal(0x94, GB.debugInfo.RegDict["A"]);
        }

        [Fact]
        public void BootTest()
        {
            var GB = new Lunchbox.Lunchbox();
            while (GB.graphic.displayData.color == 0) 
                GB.TestRun();
            while (GB.debugInfo.Address < 0x200)
                GB.TestRun();
            Assert.Equal(0x0C, GB.debugInfo.RegDict["C"]);
        }
    }
}
