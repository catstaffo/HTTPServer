using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace HTTPServer
{
	public class Server
	{
		public const String MSG_DIR = "/root/msg/";
		public const String WEB_DIR = "/root/web/";
		public const String VERSION = "HTTP/1.1";
		public const String NAME = "Catstaffo HTTP Server";

		private bool running = false;
		private TcpListener listener;

		public Server(int port)
		{
			listener = new TcpListener(System.Net.IPAddress.Any, port);
		}

		public void Start()
		{
			Thread serverThread = new Thread(new ThreadStart(Run));
			serverThread.Start();
		}

		private void Run()
		{
			running = true;
			listener.Start();

			while (running)
			{
				Console.WriteLine("Waiting for connection");
				TcpClient client = listener.AcceptTcpClient();

				Console.WriteLine("Client connected");
				HandleClient(client);

				client.Close();
			}
			running = false;
			listener.Stop();

		}

		private void HandleClient(TcpClient client)
		{
			StreamReader reader = new StreamReader(client.GetStream());

			String msg = "";

			while(reader.Peek() != -1)
			{
				msg += reader.ReadLine() + "\n";
			}

			Debug.WriteLine("Requests: \n" + msg);
			Request req = Request.GetRequest(msg);
			Response resp = Response.From(req);
			resp.Post(client.GetStream());
		}
	}
}

