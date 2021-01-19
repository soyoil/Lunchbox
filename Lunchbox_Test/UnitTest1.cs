using Xunit;

namespace Lunchbox_Test
{
    public class UnitTest1
    {
        /*
        [Fact]
        public void Test1()
        {
            var lunchbox = new Lunchbox.Lunchbox()
                .InjectToMemory(0x0, new byte[] { 0xFA, 0x46, 0x10, 0x6F })
                .InjectToMemory(0x1046, 0x7E)
                .TestRun();
            Assert.Equal(0x7E, lunchbox.debugInfo.RegDict["L"]);
        }
        */

        [Fact]
        public void Test_LD_A()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x3E, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["A"]);
        }

        [Fact]
        public void Test_LD_B()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x06, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["B"]);
        }

        [Fact]
        public void Test_LD_C()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x0E, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["C"]);
        }

        [Fact]
        public void Test_LD_D()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x16, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["D"]);
        }

        [Fact]
        public void Test_LD_E()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x1E, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["E"]);
        }

        [Fact]
        public void Test_LD_H()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x26, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["H"]);
        }

        [Fact]
        public void Test_LD_L()
        {
            var GB = new Lunchbox.Lunchbox("");
            GB.InjectToMemory(0x0, new byte[] { 0x2E, 0x56 }).TestRun(1);
            Assert.Equal(0x56, GB.debugInfo.RegDict["L"]);
        }

        [Fact]
        public void AddTest()
        {
            var GB = new Lunchbox.Lunchbox("")
                .InjectToMemory(0x0, new byte[] { 1, 0, 0x12, 0xc5, 0xF1, 0xf5, 0xd1, 0x79, 0xbb })
                .TestRun(8);
            Assert.Equal(0x12, GB.debugInfo.RegDict["D"]);
        }
        /*
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
*/
    }
}
