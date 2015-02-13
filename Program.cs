using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace AkafoeWOL
{
    class Program
    {
        private static IPAddress convertBroadcast(string _BroadcastAddress)
        {
            string[] Octets = _BroadcastAddress.Split(new Char[] { '.' });
            IPAddress BroadcastAddr = IPAddress.Parse(_BroadcastAddress);
            return BroadcastAddr;
        }

        private static int WakeFunction(string MAC_ADDRESS, IPAddress BroadcastAddress)
        {
            int position = 0;
            byte[] buffer = new byte[1024];

            UdpClient WOL = new UdpClient();
            WOL.Connect(BroadcastAddress, 7);
            WOL.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
            for (int i = 0; i < 6; i++)
            {
                buffer[position++] = 0xFF;
            }
            for (int i = 0; i < 16; i++)
            {
                int x = 0;
                for (int j = 0; j < 6; j++)
                {
                    buffer[position++] = byte.Parse(MAC_ADDRESS.Substring(x, 2), NumberStyles.HexNumber);
                    x+=3;
                }
            }
            return WOL.Send(buffer, 1024);
        }

        private static Boolean isMacOk(String MacAddress)
        {
            if (Regex.IsMatch(MacAddress, @"((([a-f]|[0-9]|[A-F]){2}\:){5}([a-f]|[0-9]|[A-F]){2}\b)|((([a-f]|[0-9]|[A-F]){2}\-){5}([a-f]|[0-9]|[A-F]){2}\b)"))
            {
                return true;
            }
            return false;
        }

        private static Boolean isBroadcastOk(String BroadcastAddress)
        {
            if (Regex.IsMatch(BroadcastAddress, @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b"))
            {
                return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            if (args != null) {
                Console.WriteLine("AkafoeWOL v1.0 - (C)2013 Thorsten Schröpel\n");
                if ((args.Length != 2) || (!isMacOk(args[0])) || (!isBroadcastOk(args[1])))
                {
                    Console.WriteLine("Parameter:");
                    Console.WriteLine("          AkafoeWOL.exe [Mac-Adresse] [Subnetzmaske]\n");
                    Console.WriteLine("Mac-Adresse: im Format 00-00-00-00-00-00 oder 00:00:00:00:00:00");
                    Console.WriteLine("Subnetzmaske: zum Beispiel 192.168.116.255");
                    if (Process.GetCurrentProcess().MainWindowTitle != "")
                    {
                        Console.ReadKey();
                    }
                }
                else
                {
                    WakeFunction(args[0], convertBroadcast(args[1]));
                }
            }
        }
    }
}
