using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class Iskola
    {
        public string Nev { get; set; }

        public string Cim { get; set; }

        public List<Diak> Diakok { get; set; }

        public Iskola(string nev, string cim)
        {
            Nev = nev;
            Cim = cim;
            Diakok = new List<Diak>();
        }

        public void AddDiak(Diak diak)
        {
            Diakok.Add(diak);
        }

        public void InformacioMegjelenitese()
        {
            Console.WriteLine($"Iskola neve: {Nev}, Címe: {Cim}");
            Console.WriteLine("Diákok:");
            foreach (var diak in Diakok)
            {
                diak.InformacioMegjelenitese();
            }
        }
    }
}