﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets.Kcp;
using System.Net.Sockets;
using System.Threading;
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
        private Dictionary<uint, KcpConnection> _connections = new Dictionary<uint, KcpConnection>();
        private byte[] _buffer = new byte[1024];
        private EndPoint _remoteEP = new IPEndPoint(IPAddress.Any, 8888);
        private Queue<MessageForKcp> _msgQueue = new Queue<MessageForKcp>();
        
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
            var msgProcessor = Task.Run(ProcessMsg);

            await Task.WhenAll(listener, msgProcessor);
        }
        
        private void Listen()
        {
            while (true)
            {
                if (ServerStatus != ServerStatus.Running)
                    return;

                _socket.ReceiveFrom(_buffer, ref _remoteEP);
                _msgQueue.Enqueue(new MessageForKcp(_buffer, _remoteEP));
            }
        }

        private void ProcessMsg()
        {
            while (true)
            {
                if (_msgQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var msg = _msgQueue.Dequeue();
                if (!_connections.TryGetValue(msg.conv, out var con))
                {
                    CreateConnection(msg.conv, msg.remoteEp);
                }
                    
                //todo process msg
            } 
        }

        private void CreateConnection(uint conv, EndPoint remoteEp)
        {
            _connections.Add(conv, new KcpConnection(conv, remoteEp));
        }
    }
}