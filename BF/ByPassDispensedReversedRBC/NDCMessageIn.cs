using System;
using System.IO;

namespace ByPass
{
    public class NDCMessageIn : NDCMessage
    {
        public byte OriginatorSystemAddress { set; get; }
        public byte TCode { set; get; }
        public byte Offset { set; get; }
        public byte[] SeverityData { set; get; }
        public byte[] MaintenanceData { set; get; }

        public override unsafe void GetFromMessagePointer(byte** message)
        {
            // Make a copy of the message pointer
            byte* p = &**message;

            // Read originator's system address
            OriginatorSystemAddress = ReadByte(&p);

            // Discard 1 reserved byte
            Discard(&p, 1);

            // Read message length
            ushort msgLen = ReadUInt16(&p);

            // Read severity length
            byte sevLen = ReadByte(&p);

            // Read maintenance length
            byte manLen = ReadByte(&p);

            // Discard 2 reserved bytes
            Discard(&p, 2);

            // Read T-Code
            TCode = ReadByte(&p);

            // Read Offset
            Offset = ReadByte(&p);

            // Discard offset if any
            if (Offset > 1)
            {
                Discard(&p, Offset - 1);
            }

            int realLen = msgLen - (Offset + 1);

            if (realLen > 0)
            {
                // Read message data
                Data = Read(&p, realLen);
            }

            // Read severity data if any
            if (sevLen > 0)
            {
                SeverityData = Read(&p, sevLen);
            }

            // Read maintenance data if any
            if (manLen > 0)
            {
                MaintenanceData = Read(&p, manLen);
            }
        }

        public override byte[] ToByteArray()
        {
            var ms = new MemoryStream();

            ms.WriteByte(OriginatorSystemAddress);
            ms.WriteByte(0);

            var len = (ushort)(Data.Length + Offset + 1);

            ms.Write(BitConverter.GetBytes(len), 0, 2);
            ms.WriteByte((byte)(SeverityData == null ? 0 : SeverityData.Length));
            ms.WriteByte((byte)(MaintenanceData == null ? 0 : MaintenanceData.Length));
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(TCode);
            ms.WriteByte(Offset);

            if (Offset > 1)
            {
                var offsetBytes = new byte[Offset - 1];

                ms.Write(offsetBytes, 0, offsetBytes.Length);
            }

            if (Data != null && Data.Length > 0)
            {
                ms.Write(Data, 0, Data.Length);
            }

            if (SeverityData != null && SeverityData.Length > 0)
            {
                ms.Write(SeverityData, 0, SeverityData.Length);
            }

            if (MaintenanceData != null && MaintenanceData.Length > 0)
            {
                ms.Write(MaintenanceData, 0, MaintenanceData.Length);
            }

            return ms.ToArray();
        }

        public static unsafe NDCMessageIn FromMessagePointer(byte** message)
        {
            var msgIn = new NDCMessageIn();

            msgIn.GetFromMessagePointer(message);

            return msgIn;
        }
    }
}
