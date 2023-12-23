using System;
using System.Net.Sockets;

namespace HTTPServer
{
	public class Response
	{
		private Byte[] data = null;
		private String status;
		private String mime;

		private Response(String status, String mime, Byte[] data)
		{
			this.data = data;
			this.status = status;
			this.mime = mime;
		}

		public static Response From(Request request)
		{
			if (request is null)
				return MakeNullRequest();

			if (request.Type.Equals("GET"))
			{
				String file = Environment.CurrentDirectory + Server.WEB_DIR + request.URL;
				FileInfo f = new FileInfo(file);
				if (f.Exists && f.Extension.Contains('.'))
				{
					return MakeFromFile(f);
				}
				else
				{
					DirectoryInfo directory = new DirectoryInfo(f + "/");
					FileInfo[] files = directory.GetFiles();
					foreach (var item in files)
					{
						String name = item.Name;
						if (name.Contains("default.html") || name.Contains("default.html")
							|| name.Contains("index.html") || name.Contains("index.htm"))
						{
                            return MakeFromFile(item);
                        }
                    }
				}
				if (!f.Exists)
				{
					return PageNotFound();
				}
            }
			else
			{
				return MethodNotAllowed();
			}
			return PageNotFound();
		}

		private static Response MakeFromFile(FileInfo f)
		{
            FileStream fileStream = f.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new byte[fileStream.Length];
            reader.Read(d, 0, (int)fileStream.Length);
			fileStream.Close();
            return new Response("200 OK", "text/html", new Byte[0]);
        }

		private static Response MakeNullRequest()
		{
			String file = Environment.CurrentDirectory + Server.MSG_DIR + "400.html";
			FileInfo fileInfo = new FileInfo(file);
			FileStream fileStream = fileInfo.OpenRead();
			BinaryReader reader = new BinaryReader(fileStream);
			Byte[] d = new byte[fileStream.Length];
			reader.Read(d, 0, (int)fileStream.Length);
            fileStream.Close();

            return new Response("400 Bad Request", "text/html", new Byte[0]);
		}

        private static Response PageNotFound()
        {
            String file = Environment.CurrentDirectory + Server.MSG_DIR + "404.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new byte[fileStream.Length];
            reader.Read(d, 0, (int)fileStream.Length);
            fileStream.Close();

            return new Response("404 Page Not Found", "text/html", new Byte[0]);
        }

        private static Response MethodNotAllowed()
        {
            String file = Environment.CurrentDirectory + Server.MSG_DIR + "405.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fileStream = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fileStream);
            Byte[] d = new byte[fileStream.Length];
            reader.Read(d, 0, (int)fileStream.Length);
            return new Response("405 Method Not Allowed", "text/html", new Byte[0]);
        }

        public void Post(NetworkStream stream)
		{
            StreamWriter writer = new StreamWriter(stream);
			writer.WriteLine(String.Format("“{0} {1}\r\nServer: {2}\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\n”"),
				Server.VERSION, status, Server.NAME, mime, data.Length);
            writer.Flush();

            stream.Write(data, 0, data.Length);
		}
	}
}

