using Calkowanie;

namespace Obliczanie
{
    public class Droga
    {
        public double[] S1 { get; private set; } = new double[3];
        public double[] S2 { get; private set; } = new double[3];

        private readonly MetodaTrapezow _integracja1 = new MetodaTrapezow(3);
        private readonly MetodaTrapezow _integracja2 = new MetodaTrapezow(3);

        public Droga()
        {
            for (var i = 0; i < 3; i++)
            {
                S1[i] = 0;
                S2[i] = 0;
            }
        }

        public void oblicz(double[] wektorPredkosci1, double[] wektorPredkosci2, double czas)
        {
            _integracja1.calkuj(wektorPredkosci1, czas);
            _integracja2.calkuj(wektorPredkosci2, czas);

            this.S1 = _integracja1.Calka;
            this.S2 = _integracja2.Calka;
        }

        public void zeruj()
        {
            _integracja1.zerujWszystkie();
            _integracja2.zerujWszystkie();
        }

    }
}
