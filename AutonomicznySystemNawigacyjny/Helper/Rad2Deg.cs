using System;

namespace Helper
{
    public class Rad2Deg
    {

        public double[] rad2degGyro(double[] wejscie)
        {
            double[] wyjscie = new double[3];

            for (int i = 0; i < 3; i++)
            {
                wyjscie[i] = Math.Round(wejscie[i] * 180.0 / Math.PI, 4);
            }

            return wyjscie;
        }
    }
}
