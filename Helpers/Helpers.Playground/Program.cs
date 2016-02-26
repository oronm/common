using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            GetMacsUsingTPLINKWDR4300();
            GetMacsUsingIPInfo();
        }

        static void GetMacsUsingTPLINKWDR4300()
        {
            var scanner = new Helpers.Network.WiFi.TpLink_WDR430.MACScanner("192.168.0.1", "admin", "admin");
            foreach (var mac in scanner.GetConnectedMacs())
            {
                Console.WriteLine(mac);
            }
            Console.ReadLine();
        }

        static void GetMacsUsingIPInfo()
        {
            var info = IPInfo.GetIPInfo();

            foreach (var ip in info)
            {
                Console.WriteLine("{0}\t{1}", ip.IPAddress, ip.MacAddress);
            }

            Console.ReadLine();
        }
    }
}
