using System;

namespace Filtracje
{
    public class Komplementarny
    {
        public double roll1 { get; set; }
        public double roll2 { get; set; }

        public double pitch1 { get; set; }
        public double pitch2 { get; set; }

        public double oblicz(double kat, double predkoscKatowa, double przyspieszenieKatowe, double czas)
        {
            var angle = 0.98 * (kat + predkoscKatowa * czas) + 0.02 * przyspieszenieKatowe;
            return Math.Round(angle);
        }
    }
}
