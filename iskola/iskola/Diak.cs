using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    internal class Diak
    {

        public string Nev { get; set; }
        public int Kor { get; set; }
        public int Id { get; set; }

        public Diak(string nev, int kor, int id)
        {
            Nev = nev;
            Kor = kor;
            Id = id;
        }

        public void InformacioMegjelenitese()
        {
            Console.WriteLine($"Diák neve: {Nev}, Életkor: {Kor}");
        }
    }
}
