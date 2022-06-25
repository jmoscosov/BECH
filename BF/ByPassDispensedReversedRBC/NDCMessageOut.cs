using System;
using System.IO;

namespace ByPass
{
    public class NDCMessageOut : NDCMessage
    {
		public byte OriginatorSystemAddress { set; get; }
        public byte Category { set; get; }

        public override unsafe void GetFromMessagePointer(byte** message)
        {
            // Make a copy of the message pointer
            byte* p = &**message;

            // Read originator's system address
            OriginatorSystemAddress = ReadByte(&p);

            // Discard 2 reserved bytes
            Discard(&p, 2);

            // Read total message length
            ushort msgLen = ReadUInt16(&p);

            // Discard 30 bytes
            Discard(&p, 30);

            int realLen = msgLen - 30;

            if (realLen > 0)
            {
                // Read message data
                Data = Read(&p, realLen);

                // Read message category
                Category = Data[0];
            }
        }

        public override byte[] ToByteArray()
        {
            var ms = new MemoryStream();

            ms.WriteByte(OriginatorSystemAddress);
            ms.WriteByte(0);
            ms.WriteByte(0);

            var len = (ushort)(Data.Length + 30);

            ms.Write(BitConverter.GetBytes(len), 0, 2);

            var reservedBytes = new byte[30];

            ms.Write(reservedBytes, 0, reservedBytes.Length);

            if (Data != null && Data.Length > 0)
            {
                ms.Write(Data, 0, Data.Length);
            }

            return ms.ToArray();
        }

        public static unsafe NDCMessageOut FromMessagePointer(byte** message)
        {
            var msgOut = new NDCMessageOut();

            msgOut.GetFromMessagePointer(message);

            return msgOut;
        }

    }
}
