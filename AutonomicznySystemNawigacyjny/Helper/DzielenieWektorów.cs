
namespace Helper
{
    public class DzielenieWektorow
    {
        public double[] wynik { get; set; }

        public double[] dziel(double[] dzielna, int dzielnik)
        {
            for (var i = 0; i < 3; i++)
            {
                wynik[i] = dzielna[i] / dzielnik;
            }
            return wynik;
        }
    }
}
