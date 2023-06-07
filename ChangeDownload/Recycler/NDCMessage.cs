using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ChangeScreens
{
    public abstract class NDCMessage
    {
        public const byte FS = (byte)'\x01C';
        public const byte GS = (byte)'\x01D';

        [DllImport("ssdsEXUTIL.DLL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        protected extern static IntPtr AllocateMessageBuffer(uint nNumChars);

        [DllImport("ssdsEXUTIL.DLL", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        protected extern static unsafe void FreeMessageBuffer(IntPtr pchMessage);

        public byte[] Data { set; get; }

        protected unsafe byte ReadByte(byte** ptr)
        {
            return *(*ptr)++;
        }

        protected unsafe ushort ReadUInt16(byte** ptr)
        {
            return BitConverter.ToUInt16(Read(ptr, 2), 0);
        }

        protected unsafe byte[] Read(byte** ptr, int len)
        {
            byte[] buffer = new byte[len];

            for (int i = 0; i < len; i++)
                buffer[i] = ReadByte(ptr);

	        return buffer;
        }

        protected unsafe void Discard(byte** ptr, int len)
        {
            for (int i = 0; i < len; i++)
                ReadByte(ptr);
        }

        public abstract byte[] ToByteArray();

        public abstract unsafe void GetFromMessagePointer(byte** message);

        public override string ToString()
        {
            return Encoding.ASCII.GetString(Data);
        }

        public static IntPtr AllocateBuffer(uint length)
        {
            try
            {
                return AllocateMessageBuffer(length);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error allocating memory with AANDC: {ex.Message}");

                return Marshal.AllocCoTaskMem((int)length);
            }
        }

        public static void FreeBuffer(IntPtr ptr)
        {
            try
            {
                FreeMessageBuffer(ptr);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error freeing memory with AANDC: {ex.Message}");

                Marshal.FreeCoTaskMem(ptr);
            }
        }

        public virtual IntPtr ToPointer()
        {
            var data = ToByteArray();
            var buffer = AllocateBuffer((uint)data.Length);

            Marshal.Copy(data, 0, buffer, data.Length);

            return buffer;
        }

        public unsafe void ToMessagePointer(byte** message, bool freePreviousMsgBuffer = true)
        {
            if (freePreviousMsgBuffer)
                FreeBuffer(new IntPtr((void*)*message));

            var ptr = ToPointer();

            *message = (byte*)ptr.ToPointer();
        }
    }
}
