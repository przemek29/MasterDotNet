
namespace Filtering
{
    public class Komplementarny
    {
        public double wynik { get; set; }

        public double oblicz(double kat, double predkoscKatowa, double przyspieszenieKatowe, double czas)
        {
            var angle = 0.98 * (kat + predkoscKatowa * czas) + 0.02 * przyspieszenieKatowe;
            wynik = angle;

            return angle;
        }
    }
}
