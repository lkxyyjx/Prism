using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Net.Sockets;
using System.Threading.Tasks;
using Prism.Library;

namespace Prism.Server
{
    enum ServerStatus
    {
        Default = 0,
        Initialized,
        Running,
        Disposed
    }
    
    public class KcpServer
    {
        private ServerStatus ServerStatus { get; set; }
        private static KcpServer Instance { get; set; }
        private Socket _socket;
        private Dictionary<int, KcpConnection> _connections = new Dictionary<int, KcpConnection>();
        private byte[] _buffer = new byte[1024];
        private EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 8888);

        public static KcpServer GetServer()
        {
            if (Instance == null || Instance.ServerStatus == ServerStatus.Disposed)
            {
                Instance = new KcpServer();
                Instance.Initialize();
            }

            return Instance;
        }
        
        private KcpServer()
        {
            ServerStatus = ServerStatus.Default;
        }

        private void Initialize()
        {
            ServerStatus = ServerStatus.Initialized;
        }

        private async void Run()
        {
            if (ServerStatus == ServerStatus.Running)
                return;
            IPAddress any = IPAddress.Any;
            _socket = new Socket(any.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
            _socket.Bind(new IPEndPoint(any, 8888));

            ServerStatus = ServerStatus.Running;
            
            var listener = Task.Run(Listen);

            await Task.WhenAll(listener);
        }
        
        private void Listen()
        {
            if (ServerStatus != ServerStatus.Running)
                return;

            
            _socket.ReceiveFrom(_buffer, ref remoteEP);
            //todo：process msg
        }
    }
}