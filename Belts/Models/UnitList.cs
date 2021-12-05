using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltWindow.Models
{
    public class UnitList
    {
        private List<string> Length = new List<string> { "mm", "in" };

        private List<string> Force = new List<string> { "N", "lbf" };

        private List<string> MajorLength = new List<string> { "m", "ft" };

        private List<string> Torque = new List<string> { "Nm", "lbf.in" };

        private List<string> LinearVelocity = new List<string> { "m/s", "ft/min" };

        private List<string> UnitWeight = new List<string> { "N/m", "lbf/in" };

        private List<string> Power = new List<string> { "KW", "hp" };

        private string AngularSpeed = "rpm";

        private int unitId;

        public UnitList(int Id)
        {
            UnitId = Id;
        }

        public int UnitId
        {
            get { return unitId; }
            set { unitId = value; }
        }

        public string PowerUnit 
        { 
            get 
            {
                if (UnitId < Power.Count)
                {
                    return Power[UnitId];
                }

                return Power[0];
            } 
        }

        public string AngularSpeedUnit { get { return AngularSpeed; } }

        public string AngleUnit { get; set; } = "deg";

        public string LengthUnit 
        {
            get
            {
                if (UnitId < Length.Count)
                {
                    return Length[UnitId];
                }

                return Length[0];
            }
        }

        public string MajorLengthUnit 
        {

            get
            {
                if (UnitId < MajorLength.Count)
                {
                    return MajorLength[UnitId];
                }

                return MajorLength[0];
            }
        }

        public string ForceUnit
        {
            get
            {
                if (UnitId < Force.Count)
                {
                    return Force[UnitId];
                }

                return Force[0];
            }
        }

        public string TorqueUnit
        {
            get
            {
                if (UnitId < Torque.Count)
                {
                    return Torque[UnitId];
                }

                return Torque[0];
            }
        }

        public string LinearVelocityUnit
        {
            get 
            {
                if (UnitId < LinearVelocity.Count)
                {
                    return LinearVelocity[UnitId];
                }

                return LinearVelocity[0];
            }
        }

        public string UnitWeightUnit
        {
            get 
            {

                if (UnitId < UnitWeight.Count)
                {
                    return UnitWeight[UnitId];
                }

                return UnitWeight[0];
            }
        }

    }
}
