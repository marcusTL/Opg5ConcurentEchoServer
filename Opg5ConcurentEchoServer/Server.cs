using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Opg1Lib;

namespace Opg5ConcurentEchoServer
{
    class Server
    {
        private static readonly List<Cykel> _cykler = new List<Cykel>()
        {
            new Cykel(1, "black", 50, 3),
            new Cykel(2, "white", 80, 4),
            new Cykel(3, "green", 120, 6),
            new Cykel(4, "blue", 100, 4)
        };
        public void Start()
        {
            //Establishing a server + port
            TcpListener server = new TcpListener(IPAddress.Loopback,4646);
            server.Start();

            //Looping connecting to a client
            while (true)
            {
                TcpClient socket = server.AcceptTcpClient();

                Task.Run((() =>
                {
                    TcpClient tempSocket = socket;
                    DoClinet(tempSocket);
                }));

            }
        }

        public void DoClinet(TcpClient socket)
        {
            StreamReader sr = new StreamReader(socket.GetStream());
            StreamWriter sw = new StreamWriter(socket.GetStream());


            //Asking the Client the method to use along with possible relevant info
            string start = "HentAlle, Hent Num., Gem";
            sw.WriteLine(start);
            sw.Flush();

            //receiving the response
            string strRet = sr.ReadLine();
            Console.WriteLine(strRet);

            //determining and executing the right method
            if (strRet == "HentAlle")
            {
                HentAlle();
            }

            else
            {
                if (strRet.Contains("Hent"))
                {
                    string[] a = strRet.Split(" ");
                    string tempid = a[1].ToString();
                    int id = Convert.ToInt32(tempid);

                    Hent(id);
                }
                else
                {
                    if (strRet.Contains("Gem"))
                    {
                        string[] a = strRet.Split(" ");
                        string obj = a[1].ToString();

                        Gem(obj);
                    }
                }
            }


            void HentAlle()
            {
                string strSend = "";
                //going through each object, adding them to the same string
                foreach (Cykel c in _cykler)
                {
                    strSend = strSend + "  " + JsonConvert.SerializeObject(c);

                }
                //Sending the final string
                sw.WriteLine(strSend);
                sw.Flush();
            }

            void Hent(int id)
            {
                //Finding the right object
                Cykel c1 = _cykler.Find(cykel => cykel.Id == id);
                //Converting into JsonString
                string send = JsonConvert.SerializeObject(c1);

                sw.WriteLine(send);
                sw.Flush();

            }

            void Gem(string s)
            {
                //string fx = {"ID":123,"Farve":"gul","Gear":7,"Pris":4700} 
                Cykel c1 = JsonConvert.DeserializeObject<Cykel>(s);
                _cykler.Add(c1);

                //Local Confirmation
                Console.WriteLine("Object Saved");
            }
        }

    }
}
