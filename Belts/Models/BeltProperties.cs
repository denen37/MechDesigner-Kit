using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltWindow.EventsModel;

namespace BeltWindow.Models
{
    public enum Material
    {
        Polyamide = 0,
        Leather,
        Urethane_Flat,
        Urethane_Round
    }

    public class BeltProperties
    {

        private string beltMaterial;

        public BeltProperties() { }

        public BeltProperties(string beltMat)
        {
            BeltMaterial = beltMat;
        }

        public string BeltMaterial 
        {
            get { return beltMaterial; }
            set { beltMaterial = value; }
        }
        /// <summary>
        /// The material the belt is made of.
        /// </summary>
        public int BeltMaterialId { get; set; }

        /// <summary>
        /// The amount of power the belt is to transmit 
        /// from one pulley to another.
        /// </summary>

        public string Specification { get; set; }

        public double PowerTransmitted { get; set; }

        /// <summary>
        /// The ratio of the speed of the big pulley to 
        /// speed of the small pulley.
        /// </summary>
        public double VelocityRatio { get; set; }

        /// <summary>
        /// The small pulley diameter.
        /// </summary>
        public double SmallPulleySize { get; set; }


        /// <summary>
        /// The big pulley diameter.
        /// </summary>
        public double BigPulleySize { get; set; }


        /// <summary>
        /// The service factor depends on hours of running, 
        /// type of shock load expected and nature of duty.
        /// </summary>
        public double ServiceFactor { get; set; } = 1;

        /// <summary>
        /// The ratio of the Nominal or design power 
        /// to the actual power transmitted.
        /// </summary>
        public Double DesignFactor { get; set; } = 1;

        /// <summary>
        /// The distance between the centres of the two pulleys.
        /// </summary>
        public Double CentreToCentreDistance { get; set; }

        /// <summary>
        /// How wide the belt is.
        /// </summary>
        public double BeltWidth { get; set; }

        /// <summary>
        /// The thickness of the belt cross-section.
        /// </summary>
        public double BeltThickness { get; set; }

        /// <summary>
        /// The centrifugal force becomes significant at high speeds.
        /// </summary>
        public double CentrifugalForce { get; set; }


        /// <summary>
        /// The tension of the belt when it is not running.
        /// </summary>
        public double InitialTension { get; set; }

        /// <summary>
        /// The tension of the slack side
        /// </summary>
        public double SlackTension { get; set; }

        /// <summary>
        /// The tension of the tight side.
        /// </summary>
        public double TightTension { get; set; }

        /// <summary>
        /// The amount of friction developed when the belt is running.
        /// </summary>
        public double FrictionDevelopment { get; set; }

        /// <summary>
        /// The amount of deflection of the belt from a straight line 
        /// under its own weight.
        /// </summary>
        public double Dip { get; set; }

        /// <summary>
        /// The cost of the belt in naira.
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// The total length of the belt.
        /// </summary>
        public double BeltLength { get; set; }

        /// <summary>
        /// The angle of the arc of contact small pulley.
        /// </summary>
        public double AngleOfContact_a { get; set; }

        /// <summary>
        /// The angle of the arc of contact big pulley.
        /// </summary>
        public double AngleOfContact_b { get; set; }


        /// <summary>
        /// The weight of the belt.
        /// </summary>
        public double BeltWeight { get; set; }

        /// <summary>
        /// The weight of one cubic metre of the belt material.
        /// </summary>
        public double SpecificWeight { get; set; }
        /// <summary>
        /// The angular velocity of the belt.
        /// </summary>
        public double AngularVelocity { get; set; }
        /// <summary>
        /// The Linear velocity of the small pulley.
        /// </summary>
        public double LinearVelocity_Small { get; set; }

        public double  Torque { get; set; }

        public double NetTension { get; set; }

        public double MinSmallPulleySize { get; set; }

        public double PulleyCorrectionFactor { get; set; } = 1;

        public double VelocityCorrectionFactor { get; set; } = 1;

        public double AllowableTension { get; set; }

        public int UnitId { get; set; }

        public double DesignPower { get; set; }

        public double MaxFriction { get; set; }

        public double LargestAllowableTension { get; set; }

        public bool IsOpen { get; set; } = true;

        public readonly string ChangeMaterialTxt = "You are attempting the change the belt material. Do you want to Clear all the previous data?";
        public readonly string BigPulleyIsTooSmallTxt = "The Big pulley cannot be smaller than the Small pulley!";
        public readonly string CentrDistTooSmallTxt = "The Centre distance is too small it must be at least 3 times the diameter of the Big pulley!";
        public readonly string SmallPulleyTooSmallTxt = "The Small pulley size cannot be less than the minimum pulley size!";
        public readonly string InvalidInputTxt = "Invalid Input!";
        public readonly string ServFactTooSmall = "Service factor should not be less than 1 ";
        public readonly string DesignFactTooSmall = "Design factor should not be less than 1 ";
        public readonly string ConfirmClearAll = "Are you sure you want to clear all?";

    }
}
