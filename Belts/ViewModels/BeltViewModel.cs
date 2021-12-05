using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeltWindow.Models;
using System.Windows;
using BeltWindow.EventsModel;
using BeltWindow;

namespace BeltWindow.ViewModels
{
    public class BeltViewModel : Screen
    {
        private BeltProperties belt = new BeltProperties();
        private List<float> thicknessess = null;
        private BindableCollection<string> belts = new BindableCollection<string>();
        private UnitList unit = new UnitList(0);
        private MessagesModel msg = new MessagesModel();
        MsgBoxParam box;
        //private MessagesModel msg = new MessagesModel(this.belt);


        public BeltViewModel()
        {
            belts.Add("Polyamide");
            belts.Add("Leather");
            belts.Add("Urethane Flat");
            belts.Add("Urethane Round");

            ////Populate Specification dropdown.
            //GetSpecifications();

            ////Populate Thickness dropdown;
            //GetThicknessess();
        }
        
        public string SelectedBelt
        {
            set
            {
                if (value != null)
                {
                    belt = new BeltProperties(value);
                }

            }
        }

        public int SelectedBeltId
        {
            set
            {
                belt.BeltMaterialId = value;
            }
        }


        #region Belt Specification
        //Connect to Database and get specifications
        public void GetSpecifications()
        {
            //MessageBox.Show("You are attempting the change the belt material. Do you want to Clear all the previous data?",
            //    "Belt Materials");
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
            thicknessess = BeltDbConnection.GetBeltThicknessValues( belt.BeltMaterialId, belt.Specification );
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
                if (Invalid(value)) 
                    return;
                else
                {
                    if (unit.UnitId == 1 && value > 0)
                    {
                        belt.BeltWidth = UnitConverter.MMandInches(value, unit.UnitId - 1);
                    }
                    else
                        belt.BeltWidth = value;
                } 
            }
        }

        //Belt Materials
        public BindableCollection<string> BeltMaterials
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
                if (Invalid(value))
                    return;
                else
                {
                    if (value < belt.MinSmallPulleySize)
                    {
                        MsgBoxParam m = new MsgBoxParam(belt.SmallPulleyTooSmallTxt);
                        msg._belt_message(m);
                        return;
                    }
                    else
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
                if (Invalid(value)) 
                    return;
                else
                {
                    if (value < belt.SmallPulleySize)
                    {
                        MsgBoxParam m = new MsgBoxParam(belt.BigPulleyIsTooSmallTxt);
                        msg._belt_message(m);
                        return;
                    }
                    else
                    {
                        if (unit.UnitId == 1)
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
                if (Invalid(value)) return;
                else
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
                if (Invalid(value)) return;
                else
                {
                    if ((value * 1000) < (3 * belt.BigPulleySize))
                    {
                        box = new MsgBoxParam(belt.CentrDistTooSmallTxt);
                        msg._belt_message(box);
                        return;
                    }
                    if (unit.UnitId == 1 )
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
            set 
            {
                if (Invalid(value)) return;
                else
                {

                    if (value < 1)
                    {
                        box = new MsgBoxParam(belt.ServFactTooSmall);
                        msg._belt_message(box);
                        return;
                    }
                    belt.ServiceFactor = value;
                }
            }
        }

        public double DesignFactor
        {
            get { return belt.DesignFactor; }
            set
            {
                if (Invalid(value)) return;
                else
                {

                    if (value < 1)
                    {
                        box = new MsgBoxParam(belt.ServFactTooSmall);
                        msg._belt_message(box);
                        return;
                    }
                    belt.DesignFactor = value;
                }
            }
        }

        //Angular velocity textbox
        public double AngVel
        {
            get { return belt.AngularVelocity; }
            set
            {
                if (Invalid(value)) 
                    return;
                else
                {
                    belt.AngularVelocity = value;
                    if (VelocityRatio > 0 && SmallPulleyDiameter > 0)
                    {
                        BeltCalculations.CalculateLinearVelocity(belt);
                        NotifyOfPropertyChange(() => LinVel);
                    }
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
            NotifyOfPropertyChange(() => Dip);
            NotifyOfPropertyChange(() => FrictionDevelopment);
        }

        //Design power textbox
        public double PowerTransmitted
        {
            get { return belt.PowerTransmitted; }
            set
            {

                if (Invalid(value)) return;
                else
                {
                    belt.PowerTransmitted = value;
                }
            }
        }


        public double BeltWeight
        {
            get
            {
                double approxValue = Math.Round(belt.BeltWeight, 2);

                if (unit.UnitId == 1 && approxValue > 0)
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
                if (unit.UnitId == 1 && approxValue > 0)
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
                if (unit.UnitId == 1 && approxValue > 0)
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
                if (unit.UnitId == 1 && approxValue > 0)
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
                if (unit.UnitId == 1 && approxValue > 0)
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
                if (unit.UnitId == 1 && approxValue > 0)
                {
                    return UnitConverter.NewtonAndPound(approxValue, unit.UnitId);
                }
                return approxValue;
            }
        }


        public double FrictionDevelopment
        {
            get
            {
                double approxValue = Math.Round(belt.FrictionDevelopment, 2);
                return approxValue;
            }
        }


        public bool IsOpen
        {
            get { return belt.IsOpen; }
            set { belt.IsOpen = value; }
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

        public string WeightUnit
        {
            get { return unit.UnitWeightUnit; }
        }

        public bool Invalid(double input)
        {
            if ((input < 0))
            {
                box = new MsgBoxParam(belt.InvalidInputTxt);
                msg._belt_message(box);
                return true;
            }

            return false;
        }

        public void ClearAll()
        {
            box = new MsgBoxParam(belt.ConfirmClearAll, "Confirm");
            box.MsgButton = MessageBoxButton.YesNo;
            MessageBoxResult choice = MessageBox.Show(box.Message, box.Caption, box.MsgButton);

            if (choice == MessageBoxResult.Yes)
            {
                belt = new BeltProperties();
                Specification = null;
                Thicknessess = null;

                NotifyOfPropertyChange(() => BeltWeight);
                NotifyOfPropertyChange(() => TightTension);
                NotifyOfPropertyChange(() => SlackTension);
                NotifyOfPropertyChange(() => InitialTension);
                NotifyOfPropertyChange(() => CentrifugalForce);
                NotifyOfPropertyChange(() => Torque);
                NotifyOfPropertyChange(() => LargestAllowableTension);
                NotifyOfPropertyChange(() => Dip);
                NotifyOfPropertyChange(() => FrictionDevelopment);
                NotifyOfPropertyChange(() => Specification);
                NotifyOfPropertyChange(() => Thicknessess);
                NotifyOfPropertyChange(() => BeltWidth);
                NotifyOfPropertyChange(() => SmallPulleyDiameter);
                NotifyOfPropertyChange(() => BigPulleyDiameter);
                NotifyOfPropertyChange(() => Length);
                NotifyOfPropertyChange(() => VelocityRatio);
                NotifyOfPropertyChange(() => CtoCDistance);
                NotifyOfPropertyChange(() => AngleOfContactA);
                NotifyOfPropertyChange(() => AngleOfContactB);
                NotifyOfPropertyChange(() => AngVel);
                NotifyOfPropertyChange(() => LinVel);
                NotifyOfPropertyChange(() => PowerTransmitted);
                NotifyOfPropertyChange(() => ServiceFactor);
                NotifyOfPropertyChange(() => DesignFactor);
                NotifyOfPropertyChange(() => PulleyCorrectionFactor);
                NotifyOfPropertyChange(() => VelocityCorrectionFactor);


            }
        }
    }
}
