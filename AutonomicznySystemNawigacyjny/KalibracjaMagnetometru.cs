using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomiczny_System_Nawigacyjny
{
    public class KalibracjaMagnetometru
    {
        private readonly double _vendorMaxX;
        private readonly double _vendorMaxY;
        private readonly double _vendorMaxZ;

        public double MaxX { get; private set; }
        public double MaxY { get; private set; }
        public double MaxZ { get; private set; }

        public double MinX { get; private set; }
        public double MinY { get; private set; }
        public double MinZ { get; private set; }

        public double Wektor3D
        {
            get
            {
                var tmp1 = Math.Sqrt(
                    Math.Pow((MaxX - MinX), 2)
                    + Math.Pow((MaxY - MinY), 2)
                    + Math.Pow((MaxZ - MinZ), 2));
                return Math.Round(tmp1, 2);
            }
        }

        public double GainX
        {
            get
            {
                return Math.Round(Wektor3D / (MaxX - MinX), 2);
            }
        }

        public double GainY
        {
            get
            {
                return Math.Round(Wektor3D / (MaxY - MinY), 2);
            }
        }

        public double GainZ
        {
            get
            {
                return Math.Round(Wektor3D / (MaxZ - MinZ), 2);
            }
        }

        public double OffsetX
        {
            get
            {
                return Math.Round(((0 - MaxX) + (0 - MinX)) / 2, 2);
            }
        }

        public double OffsetY
        {
            get
            {
                return Math.Round(((0 - MaxY) + (0 - MinY)) / 2, 2);
            }
        }

        public double OffsetZ
        {
            get
            {
                return Math.Round(((0 - MaxZ) + (0 - MinZ)) / 2, 2);
            }
        }

        public KalibracjaMagnetometru(double vendorMaxX, double vendorMaxY, double vendorMaxZ)
        {
            _vendorMaxX = vendorMaxX;
            _vendorMaxY = vendorMaxY;
            _vendorMaxZ = vendorMaxZ;
        }

        public void AddValues(double x, double y, double z)
        {
            if (Math.Abs(x) > _vendorMaxX)
                x = _vendorMaxX;

            if (Math.Abs(y) > _vendorMaxY)
                y = _vendorMaxY;

            if (Math.Abs(z) > _vendorMaxZ)
                z = _vendorMaxZ;

            if (x > MaxX)
                MaxX = x;

            if (y > MaxY)
                MaxY = y;

            if (z > MaxZ)
                MaxZ = z;

            if (x < MinX)
                MinX = x;

            if (y < MinY)
                MinY = y;

            if (z < MinZ)
                MinZ = z;
        }
    }
}

