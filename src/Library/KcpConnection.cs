using System.Net;
using System.Net.Sockets.Kcp;

namespace Prism.Library
{
    public class KcpConnection
    {
        Kcp _kcp;
        private EndPoint remoteEp;
        public KcpConnection(uint conv, EndPoint remoteEp)
        {
            _kcp = new Kcp(conv, () =>
            {
                //todo send call back
            });
        }
    }
}