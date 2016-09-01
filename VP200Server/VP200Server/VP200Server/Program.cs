using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Threading.Tasks;

namespace VP200Server
{
    class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 2048;
        private const int PORT = 100;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        static void Main()
        {
            Console.Title = "VP200-Server";
            SetupServer();
            Console.ReadLine(); // fecha o programa quando enter é pressionado
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Levantando servidor...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.201"), 9101)); //você precisa configurar a porta do vp200 antes
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Setup do servidor concluído");
            Console.WriteLine("Pressione ENTER para fechar a aplicação");
        }

        /// <summary>
        /// fecha todos os clientes conectados.
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) 
            {
                return;
            }

            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Cliente conectado, esperando por requisições...");
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket current = (Socket)AR.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(AR);
            }
            catch (SocketException)
            {
                Console.WriteLine("Cliente disconectado");
                current.Close();
                clientSockets.Remove(current);
                return;
            }

            byte[] recBuf = new byte[received];
            Array.Copy(buffer, recBuf, received);
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Dado recebido: " + text);

            List<byte> listCommands = new List<byte>();

            //primeiro exemplo de retorno
            listCommands.AddRange (exemploRetorno1());


            //segundo exemplo de resposta, apra usa-lo comente a linha acima e descomente essa linha abaixo 
            //listCommands.AddRange(exemploRetorno2());

            current.Send(listCommands.ToArray()); //envia o buffer de comandos para o socket 
                
            Console.WriteLine("Dados enviados para o leitor");
            

            current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
        }

        private static List<byte> exemploRetorno1()
        {
            //montagem da resposta no terminal em um buffer de bytes, 
            //afim de ser enviado para o terminal de uma só vez

            List<byte> listCommands = new List<byte>();
            listCommands.AddRange(VP200Commands.normalFontSize());
            listCommands.AddRange(VP200Commands.clearScreen());
            listCommands.AddRange(VP200Commands.text("Queijo"));
            listCommands.AddRange(VP200Commands.enter());
            listCommands.AddRange(VP200Commands.text("500g"));
            listCommands.AddRange(VP200Commands.largeFontSize());
            listCommands.AddRange(VP200Commands.alignRightBottom());
            listCommands.AddRange(VP200Commands.text("R$ 5.99"));

            return listCommands;
        }

        private static List<byte> exemploRetorno2()
        {
            //montagem da resposta no terminal em um buffer de bytes, 
            //afim de ser enviado para o terminal de uma só vez
            List<byte> listCommands = new List<byte>();
            listCommands.AddRange(VP200Commands.largeFontSize());
            listCommands.AddRange(VP200Commands.clearScreen());
            listCommands.AddRange(VP200Commands.alignCenterTop());
            listCommands.AddRange(VP200Commands.text("Oferta Especial!"));
            listCommands.AddRange(VP200Commands.normalFontSize());
            listCommands.AddRange(VP200Commands.alignCenter());
            listCommands.AddRange(VP200Commands.text("Agua 500 ml"));
            listCommands.AddRange(VP200Commands.largeFontSize());
            listCommands.AddRange(VP200Commands.alignCenterBottom());
            listCommands.AddRange(VP200Commands.text("R$ 0.69"));


            return listCommands;
        }
    }
}

