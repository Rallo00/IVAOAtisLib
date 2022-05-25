using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVAOAtisLibTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(">> Search for ATIS from IVAO (insert ICAO code): ");
            Console.ForegroundColor = ConsoleColor.White;
            string ICAO = Console.ReadLine();
            GetATIS(ICAO);


            Console.ReadKey();
        }

        static async void GetATIS(string ICAO)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<< Getting ATIS. Please wait...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">> Response");
            Console.ForegroundColor = ConsoleColor.White;
            IVAOAtisLib.Atis response = IVAOAtisLib.GetAtis(ICAO).Result;
            Console.WriteLine($"ICAO Code: {response.ICAOCode}");
            Console.WriteLine($"Callsign: {response.Callsign}");
            Console.WriteLine($"Revision: {response.Revision}");
            Console.WriteLine($"Timestamp (UTC): {response.TimestampZulu}");
            Console.WriteLine($"Timestamp (Local): {response.Timestamp}");
            Console.WriteLine($"ATIS: {response.Value}");
        }
    }
}
