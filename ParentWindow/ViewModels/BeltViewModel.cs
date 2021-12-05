using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltWindow.Models;

namespace ParentWindow.ViewModels
{
    public class BeltViewModel : Screen
    {
        private BeltProperties belt = new BeltProperties();
        private List<float> thicknessess = null;
        private BindableCollection<BeltProperties> belts = new BindableCollection<BeltProperties>();
        private UnitList unit = new UnitList(1);


        public BeltViewModel()
        {
            belts.Add(new BeltProperties("Polyamide"));
            belts.Add(new BeltProperties("Leather"));
            belts.Add(new BeltProperties("Urethane Flat"));
            belts.Add(new BeltProperties("Urethane Round"));

            ////Populate Specification dropdown.
            //GetSpecifications();

            ////Populate Thickness dropdown;
            //GetThicknessess();
        }

        public BeltProperties SelectedBelt
        {
            set
            {
                if (value != null)
                {
                    belt = value;
                }
            }
        }

        #region Belt Specification
        //Connect to Database and get specifications
        public void GetSpecifications()
        {
            List<string> specifications = BeltDbConnection.GetBeltSpecification(belt.BeltMaterialId);
            Specification = new BindableCollection<string>(specifications);
            NotifyOfPropertyChange(() => Specification);
        }

        //Specification Dropdown
        public BindableCollection<string> Specification { get; set; }

        //Update the BeltProperties Model
        public string SelectedSpecification
        {
            set { belt.Specification = value; }
        }
        #endregion

        #region Belt Thickness
        //Connect to database and get thicknessess.
        public void GetThicknessess()
        {
            thicknessess = BeltDbConnection.GetBeltThicknessValues(belt.BeltMaterialId, belt.Specification);
            NotifyOfPropertyChange(() => Thicknessess);
        }

        //Thickness Dropdown
        public List<float> Thicknessess
        {
            get
            {
                //Selected unit is inches;
                if (unit.UnitId == 1 && thicknessess != null)
                {
                    List<float> convertedThickness = new List<float>();
                    foreach (var thickness in thicknessess)
                    {
                        convertedThickness.Add((float)UnitConverter.MMandInches((double)thickness, unit.UnitId));
                    }

                    return convertedThickness;
                }
                return thicknessess; 
            }
            set { thicknessess = value; }
        }

        //Update the BeltProperties Model
        public float SelectedThickness
        {
            set 
            {
                if (unit.UnitId == 1)
                {
                    //convert to mm and store in model
                    double tempholder = UnitConverter.MMandInches((double)value, unit.UnitId - 1);
                    belt.BeltThickness = Math.Round(tempholder, 2);
                }
                else
                belt.BeltThickness = Math.Round(value, 2);
            }
        }
        #endregion

        public double BeltWidth
        {
            get 
            {
                if (unit.UnitId == 1 && belt.BeltWidth > 0)
                {
                    return UnitConverter.MMandInches(belt.BeltWidth, unit.UnitId);
                }
                return belt.BeltWidth;
            }
            set
            {
                if (unit.UnitId == 1 && value > 0)
                {
                    belt.BeltWidth = UnitConverter.MMandInches(value, unit.UnitId - 1);
                }
                else
                belt.BeltWidth = value;    
            }
        }

        //Belt Materials
        public BindableCollection<BeltProperties> BeltMaterials
        {
            get { return belts; }
            set { belts = value; }
        }

        //Small Pulley Diameter.
        public double SmallPulleyDiameter
        {
            get
            {

                if (unit.UnitId == 1 && belt.SmallPulleySize > 0)
                {
                    return UnitConverter.MMandInches(belt.SmallPulleySize, unit.UnitId);
                }
                return belt.SmallPulleySize;
            }
            set
            {
                if (unit.UnitId == 1 && value > 0)
                {
                    belt.SmallPulleySize = UnitConverter.MMandInches(value, unit.UnitId - 1);
                }
                else
                    belt.SmallPulleySize = value;
                NotifyOfPropertyChange(() => SmallPulleyDiameter);
            }
        }

        //Big Pulley Diameter.
        public double BigPulleyDiameter
        {
            get
            {

                if (unit.UnitId == 1 && belt.BigPulleySize > 0)
                {
                    return UnitConverter.MMandInches(belt.BigPulleySize, unit.UnitId);
                }

                return belt.BigPulleySize;
            }
            set
            {
                if (unit.UnitId == 1 && value > 0)
                {
                    belt.BigPulleySize = UnitConverter.MMandInches(value, unit.UnitId - 1);
                }
                else
                    belt.BigPulleySize = value;

                //Big pulley and Small pulley has value.
                if (belt.SmallPulleySize > 0 && belt.BigPulleySize > 0)
                {
                    belt.VelocityRatio = belt.BigPulleySize / belt.SmallPulleySize;
                }
                else { belt.VelocityRatio = 0; }

                NotifyOfPropertyChange(() => CanChangeVelocityRatio);
                NotifyOfPropertyChange(() => VelocityRatio);
            }
        }

        //is V.R textbox enabled?
        public bool CanChangeVelocityRatio
        {
            get
            {
                return belt.BigPulleySize > 0;
            }
        }

        ////Is Big Pulley textbox enabled?
        //public bool CanChangeBigPulley
        //{
        //    get
        //    {
        //        return belt.VelocityRatio > 0;
        //    }
        //}
        //Velocity Ratio
        public double VelocityRatio
        {
            get
            {
                double approxValue = Math.Round(belt.VelocityRatio, 2);
                return approxValue;
            }
            set
            {
                belt.VelocityRatio = value;

                //Small pulley and velocity ratio has value.
                if (belt.SmallPulleySize > 0 && belt.VelocityRatio > 0)
                {
                    belt.BigPulleySize = belt.SmallPulleySize * belt.VelocityRatio;
                }
                else { belt.BigPulleySize = 0; }
                NotifyOfPropertyChange(() => BigPulleyDiameter);
                NotifyOfPropertyChange(() => CanChangeVelocityRatio);
            }
        }

        //Centre to Centre Distance;
        public double CtoCDistance
        {
            get 
            {

                if (unit.UnitId == 1 && belt.CentreToCentreDistance > 0)
                {
                    return UnitConverter.MtrAndFoot(belt.CentreToCentreDistance, unit.UnitId);
                }
                return belt.CentreToCentreDistance;
            }
            set
            {
                if (unit.UnitId == 1 && value > 0)
                {
                    belt.CentreToCentreDistance = UnitConverter.MtrAndFoot(value, unit.UnitId - 1);
                }
                else
                    belt.CentreToCentreDistance = value;

                BeltCalculations.CalculateLength(belt);
                BeltCalculations.CalculateAnglesOfContact(belt);

                NotifyOfPropertyChange(() => Length);
                NotifyOfPropertyChange(() => AngleOfContactA);
                NotifyOfPropertyChange(() => AngleOfContactB);
            }
        }

        public double Length
        {
            get
            {
                double approxValue = Math.Round(belt.BeltLength, 2);

                if (unit.UnitId == 1 && approxValue > 0)
                {
                    return UnitConverter.MtrAndFoot(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        public double AngleOfContactA
        {
            get
            {
                double approxValue = Math.Round(belt.AngleOfContact_a, 2);
                return approxValue;
            }
        }

        public double AngleOfContactB
        {
            get
            {
                double approxValue = Math.Round(belt.AngleOfContact_b, 2);
                return approxValue;
            }
        }

        public double ServiceFactor
        {
            get { return belt.ServiceFactor; }
            set { belt.ServiceFactor = value; }
        }

        public double DesignFactor
        {
            get { return belt.DesignFactor; }
            set { belt.DesignFactor = value; }
        }

        //Angular velocity textbox
        public double AngVel
        {
            get { return belt.AngularVelocity; }
            set
            {
                belt.AngularVelocity = value;
                if (VelocityRatio > 0 && SmallPulleyDiameter > 0)
                {
                    BeltCalculations.CalculateLinearVelocity(belt);
                    NotifyOfPropertyChange(() => LinVel);
                }
            }
        }

        //Linear velocity textbox
        public double LinVel
        {
            get
            {
                double approxValue = Math.Round(belt.LinearVelocity_Small, 2);
                return approxValue;
            }
        }

        public void CalculateForces()
        {
            BeltCalculator.BeltManager(belt);
            NotifyOfPropertyChange(() => BeltWeight);
            NotifyOfPropertyChange(() => TightTension);
            NotifyOfPropertyChange(() => SlackTension);
            NotifyOfPropertyChange(() => InitialTension);
            NotifyOfPropertyChange(() => CentrifugalForce);
            NotifyOfPropertyChange(() => Torque);
            NotifyOfPropertyChange(() => LargestAllowableTension);
        }

        //Design power textbox
        public double PowerTransmitted
        {
            get { return belt.PowerTransmitted; }
            set
            {
                belt.PowerTransmitted = value;
            }
        }


        public double BeltWeight
        {
            get
            {
                double approxValue = Math.Round(belt.BeltWeight, 2);

                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        public double CentrifugalForce
        {
            get
            {
                double approxValue = Math.Round(belt.CentrifugalForce, 2);
                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        public double InitialTension
        {
            get
            {
                double approxValue = Math.Round(belt.InitialTension, 2);
                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }

        }

        public double TightTension
        {
            get
            {
                double approxValue = Math.Round(belt.TightTension, 2);
                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        public double SlackTension
        {
            get
            {
                double approxValue = Math.Round(belt.SlackTension, 2);
                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        //TODO set UnitConverter for torque and dip;
        public double Torque
        {
            get
            {
                double approxValue = Math.Round(belt.Torque, 2);
                return approxValue;
            }
        }

        public double Dip
        {
            get
            {
                double approxValue = Math.Round(belt.Dip, 2);
                return approxValue;
            }
        }

        public double VelocityCorrectionFactor
        {
            get
            {
                double approxValue = Math.Round(belt.VelocityCorrectionFactor, 2);
                return approxValue;
            }
        }

        public double PulleyCorrectionFactor
        {
            get
            {
                double approxValue = Math.Round(belt.PulleyCorrectionFactor, 2);
                return approxValue;
            }
        }

        public double LargestAllowableTension
        {
            get
            {
                double approxValue = Math.Round(belt.LargestAllowableTension, 2);
                if (unit.UnitId == 0 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }

        public void GetBeltData()
        {
            BeltDbConnection.GetAllBeltData(belt);
            belt.SmallPulleySize = belt.MinSmallPulleySize;
            NotifyOfPropertyChange(() => SmallPulleyDiameter);
        }

        public void GetPulleyCorrectionFactor()
        {
            belt.PulleyCorrectionFactor = BeltDbConnection.GetPulleyCorrectionFactor(belt);
            NotifyOfPropertyChange(() => PulleyCorrectionFactor);
        }

        public void GetVelocityCorrectionFactor()
        {
            belt.VelocityCorrectionFactor = BeltDbConnection.GetVelocityCorrectionFactor(belt);

            NotifyOfPropertyChange(() => VelocityCorrectionFactor);
        }

        public void GetCorrectionFactors()
        {
            GetPulleyCorrectionFactor();
            GetVelocityCorrectionFactor();
        }

        public string LengthUnit
        {
            get { return unit.LengthUnit; }
        }

        public string MajorLengthUnit
        {
            get { return unit.MajorLengthUnit; }
        }

        public string AngleUnit
        {
            get { return unit.AngleUnit; }
        }

        public string ForceUnit
        {
            get { return unit.ForceUnit; }
        }

        public string AngVelUnit
        {
            get { return unit.AngularSpeedUnit; }
        }

        public string PowerUnit
        {
            get { return unit.PowerUnit; }
        }

        public string TorqueUnit
        {
            get { return unit.TorqueUnit; }
        }

        public string LinVelUnit
        {
            get { return unit.LinearVelocityUnit; }
        }
    }
}
