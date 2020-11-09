using System.Collections.Generic;

namespace Lunchbox
{
  public class DebugInfo
  {
    public byte Opcode;
    public string Operand;
    public Dictionary<string, ushort> RegState;
  }
}
