using ByPass;
using System.Runtime.InteropServices;
using System.Text;

namespace RbcMessageTest
{
    internal class Program
    {
        private const char FS = '\x1C';
        private const char GS = '\x1D';

        public static unsafe void Main(string[] args)
        {
            var msgIn = new NDCMessageIn()
            {
                //Data = Encoding.ASCII.GetBytes("3" + FS + FS + FS + "1A" + FS + FS + "00002010010200203001040400500107001120001500177385390017000045000" + FS + "00030010300200503060040300503006030070050803009030100059003088030970309803099030")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "JAA0" + GS + "B0" + GS + "C0" + GS + "D0" + GS + "E00000" + GS + "G0" + GS + "H0" + GS + "L0" + GS + "M0" + GS + "O0" + GS + "Q0" + GS + "q0" + GS + "w0" + GS + "y0")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "HA0000" + FS + "B35" + FS + "CC00" + GS + "D0A" + GS + "E03" + GS + "G07" + GS + "H80" + GS + "LC7" + GS + "M04" + GS + "O00" + GS + "P01" + GS + "Q02" + GS + "R02" + GS + "S00" + GS + "Z02" + GS + "[01" + GS + "q005B" + GS + "w04" + GS + "y48")
                //Data = Encoding.ASCII.GetBytes("12" + FS + "000" + FS + "" + FS + "w0!!!!!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ % " + FS + "00010002000300040" + FS + "00" + FS + "00010002000300040")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "IAD1" + GS + "E12111" + GS + "G1111" + GS + "H111" + GS + "M1" + GS + "w1234")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "HA0001" + FS + "B00" + FS + "CC00" + GS + "D0A" + GS + "E03" + GS + "G07" + GS + "H80" + GS + "LC7" + GS + "M04" + GS + "O00" + GS + "P01" + GS + "Q02" + GS + "R02" + GS + "S00" + GS + "Z02" + GS + "[01" + GS + "q005B" + GS + "w04" + GS + "X00")
                //Data = Encoding.ASCII.GetBytes("12" + FS + "000" + FS + FS + "w0'#$%!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~!/ " + FS + "0" + FS + "00" + FS + "0")
                  Data = Encoding.ASCII.GetBytes("11" + FS + "000" + FS + FS + FS + "1:" + FS + ";621996103726280044=1911101152019" + FS + "" + FS + "" + FS + "BA      " + FS + "00000000" + FS + "9:7=669>>40" + FS + "3034" + FS + "" + FS + "" + FS + "1%B621996103726280044^/                         ^191110115200019000000" + FS + "" + FS + "2291910000000000000000000000000000000000000000000000" + FS + "w0A020B030C040D050E01")

            };

            var message = msgIn.ToByteArray();
            var ptr = NDCMessage.AllocateBuffer((uint)message.Length);

            Marshal.Copy(message, 0, ptr, message.Length);

            byte* nativePtr = (byte*)ptr.ToPointer();

            ByPass.SendCountersBF.Incoming(&nativePtr);

            //var message = "11" + FS + FS + FS + "w019902880377";

            //Recycler.Recycler.ProcessMessage(ref message);
        }
    }
}
