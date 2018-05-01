using System.Net.Sockets;

namespace ImageService.Communication
{
    public interface IClientHandler
    {
        void HandleClient(TcpClient client);
    }
}