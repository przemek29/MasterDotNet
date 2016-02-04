using Sensors;
using Calkowanie;
using Helper;
namespace Obliczanie
{
    public class PredkosciLiniowe
    {
        public double[] V1 { get; private set; } = new double[3];
        public double[] V2 { get; private set; } = new double[3];

        private readonly MetodaTrapezow _integracja1 = new MetodaTrapezow(3);
        private readonly MetodaTrapezow _integracja2 = new MetodaTrapezow(3);
        private readonly DzielenieWektorow _dziel = new DzielenieWektorow();

        public const double stalaGrawitacji = 9.8123; //dla Warszawy

        public PredkosciLiniowe()
        {
            for (var i = 0; i < 3; i++)
            {
                V1[i] = 0.0;
                V2[i] = 0.0;
            }
        }

        public void oblicz(double[] wektorPrzyspieszen1, double[] wektorPrzyspieszen2, double czas)
        {
            wektorPrzyspieszen1[2] = wektorPrzyspieszen1[2] / stalaGrawitacji;
            wektorPrzyspieszen2[2] = wektorPrzyspieszen2[2] / stalaGrawitacji;

            _integracja1.calkuj(wektorPrzyspieszen1, czas);
            _integracja2.calkuj(wektorPrzyspieszen2, czas);

            this.V1 = _integracja1.Calka;
            this.V2 = _integracja2.Calka;
        }

        public void zeruj()
        {
            _integracja1.zerujWszystkie();
            _integracja2.zerujWszystkie();
        }

    }
}
