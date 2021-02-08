using System;
using System.Net;

namespace Prism.Library
{
    public struct Packet
    {
        
    }
    
    public class MessageForKcp
    {
        public uint conv;
        public Packet packet;
        public EndPoint remoteEp;
        
        public MessageForKcp(Span<byte> bytes, EndPoint remoteEp)
        {
            this.conv = BitConverter.ToUInt32(bytes);
            this.remoteEp = remoteEp;
            bytes.Slice(4);
            this.packet = GetPacketFromSpan(bytes);
        }

        private Packet GetPacketFromSpan(Span<byte> bytes)
        {
            //todo
            return new Packet();
        }
    }
}