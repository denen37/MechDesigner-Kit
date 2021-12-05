using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace GearWindow.Models
{
    public static class UnitConverter
    {
        /// <summary>
        /// Convert from MM to inches and vice versa.
        /// First argument is magnitude of the original unit, 
        /// Second Argument is index Destination unit.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="belt"></param>
        /// <returns></returns>
        public static double MMandInches (double length, UnitSystem convertTo)
        {
            double output = 0;

            if (convertTo == UnitSystem.English)
            {
                output = 25.4 * length;
            }
            else
            {
                output = length / 25.4;
            }

            return Round(output, 2);
        }

        public static double MtrAndFoot(double length, UnitSystem convertTo)
        {
            double output = 0;

            if (convertTo == UnitSystem.Metric)
            {
                output = 0.3048 * length;
            }
            else
            {
                output = length / 0.3048;
            }

            return Round(output, 2);
        }

        public static double NewtonAndPound (double force, UnitSystem convertTo)
        {
            double output = 0;

            if (convertTo == UnitSystem.Metric)
            {
                output = force * 4.45;
            }
            else if (convertTo == UnitSystem.English)
            {
                output = force / 4.45;
            }

            return Round(output, 2);
        }

        public static double ConvertLengthToBaseUnit(double mag, int selectedIndex)
        {
            if (selectedIndex != 0)
            {
                return MMandInches(mag, 0);
            }
            else
            {
                return mag;
            }
        }

        public static double WattsandHp (double mag, UnitSystem convertTo)
        {
            if (convertTo == UnitSystem.Metric)
            {
                return mag * 746;
            }
            else
            {
                return mag / 746;
            }
           
        }

        public static double MetPowerandImpPower(double mag, UnitSystem convertTo)
        {
            double output;
            if (convertTo == UnitSystem.Metric)
            {
                output = mag * 1.35;
            }
            else
            {
                output = mag / 1.35;
            }

            return Round(output, 2);
        }
        public static double ConvertPowerToBaseUnit (double mag, int selectedIndex)
        {
            if (selectedIndex == 1)
            {
                return MetPowerandImpPower(mag, 0);
            }
            else if (selectedIndex == 2)
            {
                return WattsandHp(mag, 0);
            }
            else
            {
                return mag;
            }
        }

        public static double Rpm_and_Rad(double mag, int convertTo)
        {
            double output;
            if (convertTo == 0)
            {
                output = ((mag * 60) / (2 * PI));
            }
            else
            {
                output = ((mag * 2 * PI) / 60);
            }

            return Round(output, 2);
        }

        public static double AngVelToBaseUnit (double mag, int selectedIndex)
        {
            if (selectedIndex != 0)
            {
                return Rpm_and_Rad(mag, 0);
            }
            else
            {
                return mag;
            }
        }
    }
}
