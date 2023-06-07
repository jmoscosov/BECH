using ChangeScreens;
using System.Runtime.InteropServices;
using System.Text;

namespace ChangeScreensTest
{
    internal class Program
    {
        private const char FS = '\x1C';
        private const char GS = '\x1D';
        private const char FF = '\x0D';
        private const char SI = '\x0F';
        private const char ESC = '\x1B';

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
                // Data = Encoding.ASCII.GetBytes("11" + FS + "000" + FS + FS + FS + "1:" + FS + ";621996103726280044=1911101152019" + FS + "" + FS + "" + FS + "BA      " + FS + "00000000" + FS + "9:7=669>>40" + FS + "3034" + FS + "" + FS + "" + FS + "1%B621996103726280044^/                         ^191110115200019000000" + FS + "" + FS + "2291910000000000000000000000000000000000000000000000" + FS + "w0A020B030C040D050E01")
              //  Data = Encoding.ASCII.GetBytes("3" + FS + FS + FS + "11" + FS + "014" + FF + SI + "@@" + ESC + "P2013" + ESC + "*" + ESC + "[27m" + ESC + "[80m" + SI + "@AEPOR FAVOR SELECCIONE" + SI + "BJEL PRODUCTO" + SI + "CECON QUE DESE0 OPERAR" + SI + "E@CUENTA" + SI + "E1CUENTA" + SI + "F@CORRIENTE" + SI + "H@LINEA" + SI + "H1TARJETA DE" + SI + "I@DE CREDITO" + SI + "I1CREDITO" + SI + "K@CUENTA DE" + SI + "K1CAMBIO NUMERO" + SI + "L@AHORRO" + SI + "L1SECRETO" + SI + "N@EXTRANJEROS /" + SI + "O@FOREIGN CLIENT" + SI + "O1TARJETA PREPAGO" + ESC + "(1" + SI + "E7 VISTA /" + SI + "F1CUENTA RUT" + FS + "017" + FF + SI + "@@" + ESC + "P2013" + ESC + "*" + ESC + "[27m" + ESC + "[80m" + SI + "@HTARJETA DE CREDITO" + SI + "CBOPERACION QUE DESEA REALIZAR" + SI + "HA AVANCE EN" + SI + "H1  CONSULTA" + SI + "IA EFECTIVO" + SI + "I1  DE SALDO" + SI + "KA CAMBIO NUMERO" + SI + "LA SECRETO" + SI + "N1  SALIR" + FS)
                Data = Encoding.ASCII.GetBytes("3" + FS + FS + FS + "12" + FS + "000A010002130006002002001155" + FS + "001Z011131002001000000000000" + FS + "002_010010010010010010190545" + FS + "008W000000145000000145000000" + FS + "009Z000000100000000200000000" + FS + "010B011121121000133132359003" + FS + "011Y012141137008009700036012" + FS + "014Y014141137016015012255130" + FS + "030X030141137032031033245000" + FS + "910Y910041135912911005012000")
                //000A010002130006002002001155001Z011131002001000000000000002_010010010010010010190545008W000000145000000145000000009Z000000100000000200000000010B011121121000133132359003011Y012141137008009700036012012Z000000000000000300000000013D017000000000000000000000014Y014141137016015012255130015Z100000940400000700400200016W191203025558011115171030017_014014521014521014190546018Y017141137019020012106000019W000185000137000025026000020Z000330000000000940130000021Z100200000000000000000000022F026141137255255023022188023G536461000000000001001000024E024141137255013121255000025B020141137000000020095003026X059141137027169033255000027W185185185022185185185185029F028141137255255185029187030X030141137032031033247000031Z000000000000000050015005032W475486041000474473472470033Y027141137035034234110000034Z080220320000920720820000035W185185084358039185029000036F036141137255185036255187038E038141137255255013121000039D7830000080060010000000005E438EBE
            };

            var msgOut = new NDCMessageOut()
            {
                //Data = Encoding.ASCII.GetBytes("3" + FS + FS + FS + "1A" + FS + FS + "00002010010200203001040400500107001120001500177385390017000045000" + FS + "00030010300200503060040300503006030070050803009030100059003088030970309803099030")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "JAA0" + GS + "B0" + GS + "C0" + GS + "D0" + GS + "E00000" + GS + "G0" + GS + "H0" + GS + "L0" + GS + "M0" + GS + "O0" + GS + "Q0" + GS + "q0" + GS + "w0" + GS + "y0")
                Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "HA0000" + FS + "B35" + FS + "CC00" + GS + "D0A" + GS + "E03" + GS + "G07" + GS + "H80" + GS + "LC7" + GS + "M04" + GS + "O00" + GS + "P01" + GS + "Q02" + GS + "R02" + GS + "S00" + GS + "Z02" + GS + "[01" + GS + "q005B" + GS + "w04" + GS + "y48")
                //Data = Encoding.ASCII.GetBytes("12" + FS + "000" + FS + "" + FS + "w0!!!!!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ % " + FS + "00010002000300040" + FS + "00" + FS + "00010002000300040")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "IAD1" + GS + "E12111" + GS + "G1111" + GS + "H111" + GS + "M1" + GS + "w1234")
                //Data = Encoding.ASCII.GetBytes("22" + FS + "000" + FS + FS + "F" + FS + "HA0001" + FS + "B00" + FS + "CC00" + GS + "D0A" + GS + "E03" + GS + "G07" + GS + "H80" + GS + "LC7" + GS + "M04" + GS + "O00" + GS + "P01" + GS + "Q02" + GS + "R02" + GS + "S00" + GS + "Z02" + GS + "[01" + GS + "q005B" + GS + "w04" + GS + "X00")
                //Data = Encoding.ASCII.GetBytes("12" + FS + "000" + FS + FS + "w0'#$%!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~!/ " + FS + "0" + FS + "00" + FS + "0")
                //Data = Encoding.ASCII.GetBytes("11" + FS + "000" + FS + FS + FS + "1:" + FS + ";621996103726280044=1911101152019" + FS + "" + FS + "" + FS + "BCBB    " + FS + "00000000" + FS + "9:7=669>>40" + FS + "3034" + FS + "" + FS + "" + FS + "1%B621996103726280044^/                         ^191110115200019000000" + FS + "w05300" + FS + "2291910000000000000000000000000000000000000000000000" + FS + "w0A020B030C040D050E01")

            };
            //string sData = "11" + FS + "000009130" + FS + FS + "146A9262" + FS + "14" + FS + "" + FS + "" + FS + "" + FS + "BCBB    " + FS + "00000000" + FS + "" + FS + "" + FS + "" + FS + "" + FS + "1%B621996103726280044^/                         ^191110115200019000000" + FS + "w05126" + FS + "2291910000000000000000000000000000000000000000000000" + FS + "w0A020B030C040D050E01";
           /* string sData = "3" + FS + FS + FS + "11" + FS + "014" + FF + SI + "@@" + ESC + "P2013" + ESC + "*" + ESC + "[27m" + ESC + "[80m" + SI + "@AEPOR FAVOR SELECCIONE" + SI + "BJEL PRODUCTO" + SI + "CECON QUE DESEA OPERAR" + SI + "E@CUENTA" + SI + "E1CUENTA" + SI + "F@CORRIENTE" + SI + "H@LINEA" + SI + "H1TARJETA DE" + SI + "I@DE CREDITO" + SI + "I1CREDITO" + SI + "K@CUENTA DE" + SI + "K1CAMBIO NUMERO" + SI + "L@AHORRO" + SI + "L1SECRETO" + SI + "N@EXTRANJEROS /" + SI + "O@FOREIGN CLIENT" + SI + "O1TARJETA PREPAGO" + ESC + "(1" + SI + "E7 VISTA /" + SI;
            sData += sData + "F1CUENTA RUT" + FS + "017" + FF + SI + "@@" + ESC + "P2013" + ESC + "*" + ESC + "[27m" + ESC + "[80m" + SI + "@HTARJETA DE CREDITO" + SI + "CBOPERACION QUE DESEA REALIZAR" + SI + "HA AVANCE EN" + SI + "H1  CONSULTA" + SI + "IA EFECTIVO" + SI + "I1  DE SALDO" + SI + "KA CAMBIO NUMERO" + SI + "LA SECRETO" + SI + "N1  SALIR" + FS;
            */
            var message = msgIn.ToByteArray();
            //var message = msgOut.ToByteArray();
            var ptr = NDCMessage.AllocateBuffer((uint)message.Length);




            Marshal.Copy(message, 0, ptr, message.Length);

            byte* nativePtr = (byte*)ptr.ToPointer();

            ChangeScreens.ChangeDL.Incoming(&nativePtr);
            //Recycler.Recycler.Outgoing(&nativePtr);
            //var message = "11" + FS + FS + FS + "w019902880377";

            //Recycler.Recycler.ProcessMessage(ref sData);
        }
    }
}
