using System;

namespace Prism.Library
{
    public struct Packet
    {
        
    }
    
    public class MessageForKcp
    {
        public int conv;
        public Packet packet;
        
        public MessageForKcp(Span<byte> bytes)
        {
            conv = BitConverter.ToInt32(bytes);
            bytes.Slice(4);
            packet = GetPacketFromSpan(bytes);
        }

        private Packet GetPacketFromSpan(Span<byte> bytes)
        {
            //todo
            return new Packet();
        }
    }
}