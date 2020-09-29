using System;
using System.Dynamic;

namespace Opg5ConcurentEchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            s.Start();
            Console.ReadLine();
        }
    }
}
