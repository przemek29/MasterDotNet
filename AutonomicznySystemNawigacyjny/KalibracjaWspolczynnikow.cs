using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class KalibracjaWspolczynnikow
    {
        public double Suma { get; set; }

        public double X { get; set; }

        public double Srednia
        {
            get
            {
                return Math.Round(Suma / licznik, 4);
            }

            set {; }
        }

        public int licznik { get; set; }

        public KalibracjaWspolczynnikow()
        {

            Suma = 0;

        }

        public void kalibruj(double X, int licznik)
        {
            this.licznik = licznik;
            this.X = X;

            Suma += X;
        }

        public void zeruj()
        {
            X = 0;

            Srednia = 0;

            Suma = 0;

        }
    }
}
