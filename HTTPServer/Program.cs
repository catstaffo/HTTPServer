using System;
using System.Collections.Generic;
using System.Text;

namespace HTTPServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port 8080");
            Server server = new Server(8080);
            server.Start();
        }
    }
}
