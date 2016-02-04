using Sensors;
using System;

namespace Obliczanie
{
    public class PrzyspieszeniaLiniowe
    {
        public double[] A1 { get; private set; } = new double[3];
        public double[] A2 { get; private set; } = new double[3];

        public const double stalaGrawitacji = 9.8123; //dla Warszawy

        public PrzyspieszeniaLiniowe()
        {
            for (var i = 0; i < 3; i++)
            {
                A1[i] = 0.0;
                A2[i] = 0.0;
            }
        }

        public void oblicz(Akcelerometr A1, Akcelerometr A2)
        {
            this.A1[0] = Math.Round(A1.akcelKal[0] * stalaGrawitacji, 2);
            this.A1[1] = Math.Round(A1.akcelKal[1] * stalaGrawitacji, 2);
            this.A1[2] = Math.Round(A1.akcelKal[2] * stalaGrawitacji, 2);

            this.A2[0] = Math.Round(A2.akcelKal[0] * stalaGrawitacji, 2);
            this.A2[1] = Math.Round(A2.akcelKal[1] * stalaGrawitacji, 2);
            this.A2[2] = Math.Round(A2.akcelKal[2] * stalaGrawitacji, 2);
        }
    }
}
