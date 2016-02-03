using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filtering
{
    public class KalmanFilter
    {
        public double Xk { get; set; }
        public double[] K = new double[2];
        public double[,] P = new double[2, 2];

        public double Q_angle { get; set; }
        public double Q_bias { get; set; }
        public double R_measure { get; set; }

        public double K_angle { get; set; }
        public double K_bias { get; set; }
        public double K_rate { get; set; }

        public double S { get; set; }
        public double y { get; set; }

        public double dt { get; set; }

        public KalmanFilter(double angle, double bias, double pomiar, double czestotliwosc)
        {

            Q_angle = angle;
            Q_bias = bias;
            R_measure = pomiar;

            K_angle = 0;
            K_bias = 0;

            P[0, 0] = 0;
            P[0, 1] = 0;
            P[1, 0] = 0;
            P[1, 1] = 0;

            dt = czestotliwosc;
        }

        public double update(double kat, double predKatowa, double dt)
        {

            K_rate = predKatowa - K_bias;
            K_angle += dt * K_rate;

            P[0, 0] += dt * (P[1, 1] + P[0, 1]) + Q_angle * dt;
            P[0, 1] -= dt * P[1, 1];
            P[1, 0] -= dt * P[1, 1];
            P[1, 1] += Q_bias * dt;

            S = P[0, 0] + R_measure;

            K[0] = P[0, 0] / S;
            K[1] = P[1, 0] / S;

            y = kat - K_angle;

            K_angle += K[0] * y;
            K_bias += K[1] * y;

            P[0, 0] -= K[0] * P[0, 0];
            P[0, 1] -= K[0] * P[0, 1];
            P[1, 0] -= K[1] * P[0, 0];
            P[1, 1] -= K[1] * P[0, 1];

            return K_angle;
        }
    }
}
