using Caliburn.Micro;
using GearWindow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace GearWindow.ViewModels
{
    class GearViewModel : Conductor<object>, IHandle<Applications>
    {
        private GearAppViewModel _GearApp;
        IWindowManager _manager;
        IEventAggregator _events;
        private SimpleContainer _container;
        private GearDrive gearDrive;
        private UnitList unit;
        private UnitSystem myUnit = UnitSystem.English;
        private string hardnessUnit;
        private bool isSelected = false;
        private BindableCollection<string> availableMaterials =
            new BindableCollection<string> { "Steel", "Malleable Iron", "Nodular Iron", "Cast Iron", "Aluminium Bronze", "Tin Bronze" };
        private BindableCollection<string> availableQualityNo =
            new BindableCollection<string> { "A12", "A11", "A10", "A9", "A8", "A7", "A6", "A5", "A4", "A3", "A2", "A1" };
        private BindableCollection<string> _heatTreaments = 
            new BindableCollection<string> { "Through Hardened", "Case Hardened", "Flame and Induction Hardened", "Nitrided"};
        
        public GearViewModel(IWindowManager manager, IEventAggregator events, SimpleContainer container)
        {
            _manager = manager;
            _events = events;
            _events.SubscribeOnPublishedThread(this);
            _container = container;
            gearDrive = new GearDrive();
            unit = new UnitList((int)myUnit);
            gearDrive.NoOfLoadCyclesChanged += GearDrive_NoOfLoadCyclesChanged;
            gearDrive.pinion.AngVelChanged += Pinion_AngVelChanged;
            gearDrive.gear.AngVelChanged += Gear_AngVelChanged;
            gearDrive.pinion.CanCalcStressCycleFactors += Pinion_CanCalcStressCycleFactors;
            gearDrive.gear.CanCalcStressCycleFactors += Gear_CanCalcStressCycleFactors;
            gearDrive.CanCalcStressNumbers += GearDrive_CanCalcStressNumbers;
            gearDrive.pinion.CanCalcAllBendingStress += Pinion_CanCalcAllBendingStress;
            gearDrive.gear.CanCalcAllBendingStress += Gear_CanCalcAllBendingStress;
            gearDrive.pinion.CanCalcAllContactStress += Pinion_CanCalcAllContactStress;
            gearDrive.gear.CanCalcAllContactStress += Gear_CanCalcAllContactStress;
            gearDrive.pinion.ReCalcStressFactor += Pinion_ReCalcStressFactor;
            gearDrive.gear.ReCalcStressFactor += Gear_ReCalcStressFactor;
            gearDrive.ReCalcStressNumbers += GearDrive_ReCalcStressNumbers;

        }

        public void ToImperial()
        {
            IsImperial = true;
        }

        public void ToMetric()
        {
            IsMetric = true;
        }
        public bool IsMetric
        {
            get { return myUnit == UnitSystem.Metric; }
            set 
            {
                if (value == true)
                {
                    myUnit = UnitSystem.Metric;
                    unit = new UnitList((int)myUnit);
                    NotifyAll();
                }
            }
        }

        public bool IsImperial
        {
            get { return myUnit == UnitSystem.English; }
            set
            {
                if (value == true)
                {
                    myUnit = UnitSystem.English;
                    unit = new UnitList((int)myUnit);
                    NotifyAll();
                }
            }
        }

        public void ClearAll()
        {
            gearDrive = new GearDrive();
            NotifyAll();
        }

        public void NotifyAll()
        {
            Type type = Type.GetType(this.ToString());
            var propNames = type.GetProperties();
            foreach (var property in propNames)
            {
                NotifyOfPropertyChange(property.Name);
            }
        }

        public string Jugdement
        {
            get
            {
                string conclusion = null;
                if (PBendingSafetyFactor == 0 ||
                    GBendingSafetyFactor == 0 ||
                    PContactSafetyFactor == 0 ||
                    GContactSafetyFactor == 0)
                {
                    return conclusion;
                }
                else
                {
                    if (PBendingSafetyFactor < 1)
                    {
                        conclusion = $"The Gear Design Will Not Be Able To Withstand the bending stress since " +
                            $"the Pinion Bending Safety Factor is Less than 1\nConsider selecting another diametral pitch";
                        
                    }
                    else if(GBendingSafetyFactor < 1)
                    {
                        conclusion = $"The Gear Design Will Not Be Able To Withstand the bending stress since " +
                            $"the Gear Bending Safety Factor is Less than 1\nConsider selecting another diametral pitch ";
                    }
                    else if (PContactSafetyFactor < 1)
                    {
                        conclusion = $"The Gear Design Will Not Be Able To Withstand the contact stress since " +
                            $"the Pinion Contact Safety Factor is Less than 1\nConsider selecting a larger face width";
                    }
                    else if (GContactSafetyFactor < 1)
                    {
                        conclusion = $"The Gear Design Will Not Be Able To Withstand the contact stress since " +
                            $"the Gear Contact Safety Factor is Less than 1\nConsider selecting a larger face width";
                    }
                    else
                    {
                        conclusion = $"The Gear Design is Satisfactory!\n You may want to iterate again to obtain more alternatives";
                    }

                    return conclusion;
                }
            }
        }
        private void GearDrive_ReCalcStressNumbers(object sender, double e)
        {
            Gear_CanCalcAllBendingStress(sender, e);
            Gear_CanCalcAllContactStress(sender, e);
            Pinion_CanCalcAllBendingStress(sender, e);
            Pinion_CanCalcAllContactStress(sender, e);
        }

        private void Gear_ReCalcStressFactor(object sender, double e)
        {
            GearDrive_CanCalcStressNumbers(sender, e);
        }

        private void Pinion_ReCalcStressFactor(object sender, double e)
        {
            GearDrive_CanCalcStressNumbers(sender, e);
        }

        public BindableCollection<string> HeatTreatmentMethods
        {
            get { return _heatTreaments; }
            //set { myVar = value; }
        }

        public int HeatTreatment
        {
            set
            {
                switch (value)
                {
                    case 0:
                        gearDrive.HardnessType = HardnessMethod.Through_hardened;
                        hardnessUnit = unit.HardnessUnit[0];
                        IsSelected = true;
                        NotifyOfPropertyChange(() => IsSelected);
                        NotifyOfPropertyChange(() => HardnessUnit);
                        break;
                    case 1:
                        gearDrive.HardnessType = HardnessMethod.Case_carburized;
                        hardnessUnit = unit.HardnessUnit[1];
                        IsSelected = true;
                        NotifyOfPropertyChange(() => IsSelected);
                        NotifyOfPropertyChange(() => HardnessUnit);
                        break;
                    case 2:
                        gearDrive.HardnessType = HardnessMethod.FlameOrInduction_hardened;
                        hardnessUnit = unit.HardnessUnit[1];
                        IsSelected = true;
                        NotifyOfPropertyChange(() => IsSelected);
                        NotifyOfPropertyChange(() => HardnessUnit);
                        break;
                    case 3:
                        gearDrive.HardnessType = HardnessMethod.Nitrided;
                        hardnessUnit = unit.HardnessUnit[1];
                        IsSelected = true;
                        NotifyOfPropertyChange(() => IsSelected);
                        NotifyOfPropertyChange(() => HardnessUnit);
                        break;
                    default:
                        break;
                }
            }

        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }


        public string HardnessUnit
        {
            get { return hardnessUnit; }
            set { hardnessUnit = value; }
        }

        public int HardnessValue
        {
            get { return gearDrive.Hardness; }
            set 
            {
                if (gearDrive.HardnessType == HardnessMethod.Through_hardened)
                {
                    gearDrive.Hardness = value;
                }
                else
                {
                    gearDrive.CaseHardness = value;
                }
            }
        }


        private void Gear_CanCalcAllContactStress(object sender, double e)
        {
            GearCalculations.CalcAdjustedContactStress(gearDrive.gear);
            NotifyOfPropertyChange(() => GAllowableContactStress);
            NotifyOfPropertyChange(() => GContactSafetyFactor);
            NotifyOfPropertyChange(() => Jugdement);
        }

        private void Pinion_CanCalcAllContactStress(object sender, double e)
        {
            GearCalculations.CalcAdjustedContactStress(gearDrive.pinion);
            NotifyOfPropertyChange(() => PAllowableContactStress);
            NotifyOfPropertyChange(() => PContactSafetyFactor);
            NotifyOfPropertyChange(() => Jugdement);
        }

        private void Gear_CanCalcAllBendingStress(object sender, double e)
        {
            GearCalculations.CalcAdjustedBendingStress(gearDrive.gear);
            NotifyOfPropertyChange(() => GAllowableBendingStress);
            NotifyOfPropertyChange(() => GBendingSafetyFactor);
            NotifyOfPropertyChange(() => Jugdement);
        }

        private void Pinion_CanCalcAllBendingStress(object sender, double e)
        {
            GearCalculations.CalcAdjustedBendingStress(gearDrive.pinion);
            NotifyOfPropertyChange(() => PAllowableBendingStress);
            NotifyOfPropertyChange(() => PBendingSafetyFactor);
            NotifyOfPropertyChange(() => Jugdement);
        }

        private void GearDrive_CanCalcStressNumbers(object sender, double e)
        {
            GearCalculations.CalcBendingStress(gearDrive.pinion);
            GearCalculations.CalcBendingStress(gearDrive.gear);
            GearCalculations.CalcContactStress(gearDrive.pinion);
            GearCalculations.CalcContactStress(gearDrive.gear);

            NotifyOfPropertyChange(() => PinionBendingStress);
            NotifyOfPropertyChange(() => GearBendingStress);
            NotifyOfPropertyChange(() => PinionPittingStress);
            NotifyOfPropertyChange(() => GearPittingStress);
            NotifyOfPropertyChange(() => PBendingSafetyFactor);
            NotifyOfPropertyChange(() => GBendingSafetyFactor);
            NotifyOfPropertyChange(() => PContactSafetyFactor);
            NotifyOfPropertyChange(() => GContactSafetyFactor);
            NotifyOfPropertyChange(() => Jugdement);
        }

        //private void Gear_CanCalcStressNumbers(object sender, double e)
        //{
        //    GearCalculations.CalcBendingStress(gearDrive.gear);
        //    GearCalculations.CalcContactStress(gearDrive.gear);

        //    NotifyOfPropertyChange(() => GearBendingStress);
        //    NotifyOfPropertyChange(() => GearPittingStress);
        //}

        //private void Pinion_CanCalcStressNumbers(object sender, double e)
        //{
        //    GearCalculations.CalcBendingStress(gearDrive.pinion);
        //    GearCalculations.CalcContactStress(gearDrive.pinion);

        //    NotifyOfPropertyChange(() => PinionBendingStress);
        //    NotifyOfPropertyChange(() => PinionPittingStress);
        //}

        private void Gear_CanCalcStressCycleFactors(object sender, double e)
        {
            GearCalculations.CalcBendingStrengthStressCycleFactor(gearDrive.gear);
            GearCalculations.CalcContactStrengthStressCycleFactor(gearDrive.gear);
            NotifyOfPropertyChange(() => GBendingStressCycleFactor);
            NotifyOfPropertyChange(() => GContactStressCycleFactor);
        }

        private void Pinion_CanCalcStressCycleFactors(object sender, double e)
        {
            GearCalculations.CalcBendingStrengthStressCycleFactor(gearDrive.pinion);
            GearCalculations.CalcContactStrengthStressCycleFactor(gearDrive.pinion);
            NotifyOfPropertyChange(() => PBendingStressCycleFactor);
            NotifyOfPropertyChange(() => PContactStressCycleFactor);
        }

        private void Gear_AngVelChanged(object sender, double e)
        {
            NotifyOfPropertyChange(() => GearNoOfLoadCycles);
        }

        private void Pinion_AngVelChanged(object sender, double e)
        {
            NotifyOfPropertyChange(() => PinionNoOfLoadCycles);
        }

        private void GearDrive_NoOfLoadCyclesChanged(object sender, double e)
        {
            NotifyOfPropertyChange(() => PinionNoOfLoadCycles);
            NotifyOfPropertyChange(() => GearNoOfLoadCycles);
        }

        public Task HandleAsync(Applications message, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
             {
                // Update the Application Textbox
                //Application = message.Application;
                //Update the DesignLife textbox.
                DesignLife = (double)message.Average;
                //Close the form;
                _GearApp.TryCloseAsync(false);

                //TODO - Investigate why the form is refusing to close.
            });
        }

        public double DesignLife
        {
            get { return gearDrive.DesignLife; }
            set
            {
                gearDrive.DesignLife = value;
                NotifyOfPropertyChange(() => DesignLife);
            }
        }

        public void ShowDesignLife()
        {
            LoadApplication();
        }


        public string Application
        {
            get { return gearDrive.Application; }
            set
            {
                gearDrive.Application = value;
                NotifyOfPropertyChange(() => Application);
            }
        }


        public void LoadApplication()
        {
            _GearApp = _container.GetInstance<GearAppViewModel>();

            ActivateItemAsync(_GearApp);

            _manager.ShowWindowAsync(_GearApp);
        }

        //Input Power
        public double InputPower
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.WattsandHp(gearDrive.Power, myUnit);
                }
                return gearDrive.Power;
            }
            set
            {
                if (myUnit == UnitSystem.Metric)
                {
                    gearDrive.Power = UnitConverter.WattsandHp(value, UnitSystem.English);
                }
                else
                {
                    gearDrive.Power = value;
                }
                
                NotifyOfPropertyChange(() => InputPower);
            }
        }

        //Input Speed
        public double InputSpeed
        {
            get 
            {
                return gearDrive.pinion.AngVel; 
            }
            set
            {
                gearDrive.pinion.AngVel = value;
                NotifyOfPropertyChange(() => InputSpeed);
            }
        }

        //Diametral Pitch
        public double DiametralPitch
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return  UnitConverter.ModuleToDiaPitch(gearDrive.DiametralPitch, myUnit);
                }
                return gearDrive.DiametralPitch;
            }
            set
            {
                if (myUnit == UnitSystem.English)
                {
                    gearDrive.DiametralPitch = value; 
                }
                else
                {
                    gearDrive.DiametralPitch = UnitConverter.ModuleToDiaPitch(value, myUnit);
                }
                NotifyOfPropertyChange(() => DiametralPitch);
            }
        }

        public string ModuleOrDiaPitch

        {
            get
            {
                if (myUnit == UnitSystem.English)
                {
                    return "Diametral Pitch";
                }
                else
                {
                    return "Module";
                }
            }
            
        }


        //Show Fig 1 - Graph of diametral pitch
        public void ShowDPGraph()
        {
            DPGraphViewModel dPGraph = _container.GetInstance<DPGraphViewModel>();

            ActivateItemAsync(dPGraph);

            _manager.ShowWindowAsync(dPGraph);
        }

        //Number of Pinion Teeth
        public double NoOfPinionTeeth
        {
            get { return gearDrive.pinion.ToothCount; }
            set
            {
                gearDrive.pinion.ToothCount = value;
                NotifyOfPropertyChange(() => NoOfPinionTeeth);
            }
        }

        //Desired Output Speed
        public double GearSpeed
        {
            get
            {
                double speed = gearDrive.gear.AngVel;
                return speed;
            }
            set
            {
                gearDrive.gear.AngVel = value;

                NotifyOfPropertyChange(() => GearSpeed);
                NoOfGearTeeth = GearCalculations.CalcNoGearTeeth(gearDrive);
            }
        }

        //Computed No. of Gear Teeth
        public double NoOfGearTeeth
        {
            get { return gearDrive.gear.ToothCount; }
            set
            {
                gearDrive.gear.ToothCount = value;
                NotifyOfPropertyChange(() => NoOfGearTeeth);
                ChosenNoOfGearTeeth = (int)Math.Round(value);
            }
        }

        //Enter Chosen No. of Gear Teeth
        public int ChosenNoOfGearTeeth
        {
            get { return (int)gearDrive.gear.ToothCount; }
            set
            {
                gearDrive.gear.ToothCount = value;
                NotifyOfPropertyChange(() => ChosenNoOfGearTeeth);
            }
        }

        //Actual Output Speed
        public double ActualGearSpeed
        {
            get
            {
                return gearDrive.gear.AngVel;
            }
            set
            {
                gearDrive.gear.AngVel = value;
                NotifyOfPropertyChange(() => ActualGearSpeed);
            }
        }

        //Computed Data
        public void ComputeGeometry()
        {
            ActualGearSpeed = GearCalculations.CalcOutputSpeed(gearDrive);
            GearCalculations.CalcTagentialForce(gearDrive);
            GearCalculations.CalcNormalForce(gearDrive);
            GearCalculations.CalcRadialForce(gearDrive);
           
            NotifyOfPropertyChange(() => GearRatio);
            NotifyOfPropertyChange(() => PinionPitchDiameter);
            NotifyOfPropertyChange(() => GearPitchDiameter);
            NotifyOfPropertyChange(() => CenterDistance);
            NotifyOfPropertyChange(() => PitchLineSpeed);
            NotifyOfPropertyChange(() => TransmittedLoad);
            NotifyOfPropertyChange(() => MinFaceWidth);
            NotifyOfPropertyChange(() => NomFaceWidth);
            NotifyOfPropertyChange(() => MaxFaceWidth);
            NotifyOfPropertyChange(() => FaceWidth);
            NotifyOfPropertyChange(() => FaceWidthToPinionDiameter);
            NotifyOfPropertyChange(() => NormalForce);
            NotifyOfPropertyChange(() => RadialForce);
            CalcGeometryFactors();
            CalcAlignmentFactor();
        }

        //Gear Ratio
        public double GearRatio
        {
            get { return gearDrive.GearRatio; }
            set { gearDrive.GearRatio = value; }
        }

        public double PinionPitchDiameter
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return  UnitConverter.MMandInches(gearDrive.pinion.PitchDiameter, myUnit);
                }
                return gearDrive.pinion.PitchDiameter; 
            }
            //set { gearDrive.pinion.PitchDiameter = value; }
        }

        public double GearPitchDiameter
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return   UnitConverter.MMandInches(gearDrive.gear.PitchDiameter, myUnit);
                }
                return gearDrive.gear.PitchDiameter; 
            }
            //set { gearDrive.gear.PitchDiameter = value; }
        }

        public double CenterDistance
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return  UnitConverter.MMandInches(gearDrive.CenterDistance, myUnit);
                }
                return gearDrive.CenterDistance; 
            }
        }

        public double PitchLineSpeed
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return  UnitConverter.ConvLinvel(gearDrive.PitchLineVel, myUnit);
                }
                return gearDrive.PitchLineVel; 
            }
        }

        public double TransmittedLoad
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.NewtonAndPound(gearDrive.TangentialForce, myUnit);
                }
                return gearDrive.TangentialForce;
            }
            //set { gearDrive.TangentialForce = value; }
        }

        public double RadialForce
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.NewtonAndPound(gearDrive.RadialForce, myUnit);
                }
                return gearDrive.RadialForce; 
            }
            set { gearDrive.RadialForce = value; }
        }

        public double NormalForce
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.NewtonAndPound(gearDrive.NormalForce, myUnit);
                }
                return gearDrive.NormalForce; 
            }
            set { gearDrive.NormalForce = value; }
        }


        public double MinFaceWidth
        {
            get
            {
                double minFaceWidth = 0;
                if (DiametralPitch > 0)
                {
                    minFaceWidth = Math.Round(8 / DiametralPitch, 3);
                }
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.MMandInches(minFaceWidth, myUnit);
                }
                return minFaceWidth;
            }
        }

        public double NomFaceWidth
        {
            get
            {
                double nomFaceWidth = 0;
                if (DiametralPitch > 0)
                {
                    nomFaceWidth = Math.Round(12 / DiametralPitch, 3);
                }
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.MMandInches(nomFaceWidth, myUnit);
                }
                return nomFaceWidth;
            }
        }

        public double MaxFaceWidth
        {
            get
            {
                double maxFaceWidth = 0;
                if (DiametralPitch > 0)
                {
                    maxFaceWidth = Math.Round(16 / DiametralPitch, 3);
                }
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.MMandInches(maxFaceWidth, myUnit);
                }
                return maxFaceWidth;
            }
        }

        public double FaceWidth
        {
            get
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.MMandInches(gearDrive.FaceWidth, myUnit);
                }
                return gearDrive.FaceWidth;
            }
            set
            {
                if (myUnit == UnitSystem.English)
                {
                    gearDrive.FaceWidth = value;
                }
                else
                {
                    gearDrive.DiametralPitch = UnitConverter.MMandInches(value, UnitSystem.English);
                }
                
                NotifyOfPropertyChange(() => FaceWidth);
                NotifyOfPropertyChange(() => FaceWidth);
            }
        }

        public double FaceWidthToPinionDiameter
        {
            get
            {
                double ratio = 0;

                if (gearDrive.FaceWidth > 0 && gearDrive.pinion.PitchDiameter > 0)
                {
                    ratio = gearDrive.FaceWidth / gearDrive.pinion.PitchDiameter;
                }
                return Math.Round(ratio, 2);
            }
        }

        public string RecommendedRatio
        {
            get { return "1.5 - 2.0"; }
        }

        public BindableCollection<string> AvailableMaterials
        {
            get { return availableMaterials; }
        }

        public string PinionMaterial
        {
            set { gearDrive.pinion.Material = value; }
        }

        public void PinionMaterialChanged()
        {
            if (gearDrive.pinion.Material != gearDrive.gear.Material)
            {
                ElasticCoefficient = GearDbConnection.GetElasticCoefficient(gearDrive.pinion.Material, gearDrive.gear.Material);
            }
        }

        public string GearMaterial
        {
            set { gearDrive.gear.Material = value; }
        }

        public void GearMaterialChanged()
        {
            ElasticCoefficient = GearDbConnection.GetElasticCoefficient(gearDrive.gear.Material, gearDrive.gear.Material);
        }

        public int ElasticCoefficient
        {
            get { return gearDrive.ElasticCoefficient; }
            set
            {
                gearDrive.ElasticCoefficient = value;
                NotifyOfPropertyChange(() => ElasticCoefficient);
            }
        }

        public BindableCollection<string> AvailableQualityNo
        {
            get { return availableQualityNo; }
        }

        public string QualityNumber
        {
            set { gearDrive.QualityNumber = ConvertToInt(value); }
        }

        public void ShowQualityNumbers()
        {
            QualityNoViewModel qualityNo = _container.GetInstance<QualityNoViewModel>();
            ActivateItemAsync(qualityNo);
            _manager.ShowWindowAsync(qualityNo);
        }


        public int ConvertToInt(string quality)
        {
            string No = quality.Remove(0, 1);
            int convNo = int.Parse(No);
            return convNo;
        }

        public void FindDynamicFactor()
        {
            GearCalculations.CalcDynamicFactor(gearDrive);
            
            NotifyOfPropertyChange(() => DynamicFactor);
        }

        public void CalcGeometryFactors()
        {
            GearCalculations.CalcBendingStressGeometryFactor(gearDrive.pinion);
            GearCalculations.CalcBendingStressGeometryFactor(gearDrive.gear);
            GearCalculations.CalcContactStressGeometryfactor(gearDrive);

            NotifyOfPropertyChange(() => PinionBGeometryFactor);
            NotifyOfPropertyChange(() => GearBGeometryFactor);
            NotifyOfPropertyChange(() => ContactGeometryFactor);
        }
        public double DynamicFactor
        {
            get { return gearDrive.DynamicFactor; }
            //set {  gearDrive.DynamicFactor = value; }
        }

        public double PinionBGeometryFactor
        {
            get 
            { 
                return gearDrive.pinion.BendingStressGeometryFactor; 
            }
            set 
            { 
                gearDrive.pinion.BendingStressGeometryFactor = value;
                NotifyOfPropertyChange(() => PinionBGeometryFactor);
            }
        }

        public void ShowBendingGeometry()
        {
            BendingGeometryViewModel viewModel = _container.GetInstance<BendingGeometryViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }

        public double GearBGeometryFactor
        {
            get { return gearDrive.gear.BendingStressGeometryFactor; }
            set
            {
                gearDrive.gear.BendingStressGeometryFactor = value;
                NotifyOfPropertyChange(() => PinionBGeometryFactor);
            }
        }

        public void ShowBendingGeometry2()
        {
            ShowBendingGeometry();
        }

        public double ContactGeometryFactor
        {
            get { return gearDrive.ContactStressGeometryFactor; }
            set 
            { 
                gearDrive.ContactStressGeometryFactor = value;
                NotifyOfPropertyChange(() => ContactGeometryFactor);
            }
        }

        public void ShowPittingGeometry()
        {
            ContactGeometryViewModel viewModel = _container.GetInstance<ContactGeometryViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }
        public bool IsCrowned
        {
            get { return gearDrive.IsCrowned; }
            set 
            {
                gearDrive.IsCrowned = value;
            }
        }

        public bool IsLapped
        {
            get { return gearDrive.IsLapped; }
            set { gearDrive.IsLapped = value; }
        }

        public bool IsCentered
        {
            get { return gearDrive.IsCentered; }
            set { gearDrive.IsCentered = value; }
        }

        public bool IsOpen
        {
            get { return gearDrive.Accuracy == Condition.Open; }
            set 
            {
                if (value == true)
                {
                    gearDrive.Accuracy = Condition.Open;
                }
            }
        }

        public bool IsCommercial
        {
            get { return gearDrive.Accuracy == Condition.Commercial; }
            set
            {
                if (value == true)
                {
                    gearDrive.Accuracy = Condition.Commercial;
                }
            }
        }

        public bool IsPrecision
        {
            get { return gearDrive.Accuracy == Condition.Precision; }
            set
            {
                if (value == true)
                {
                    gearDrive.Accuracy = Condition.Precision;
                }
            }
        }

        public bool IsExtraPrecision
        {
            get { return gearDrive.Accuracy == Condition.ExtraPrecision; }
            set
            {
                if (value == true)
                {
                    gearDrive.Accuracy = Condition.ExtraPrecision;
                } 
            }

        }

        public double PinionPropFactor
        {
            get 
            { 
                return gearDrive.PinionProportionFactor; 
            }
            //set { myVar = value; }
        }

        public double MeshAlignFactor
        {
            get { return gearDrive.MeshAlignmentFactor; }
        }

        public double AlignmentFactor
        {
            get { return gearDrive.LoadDistributionFactor; }
        }

        public void CalcAlignmentFactor()
        {
            NotifyOfPropertyChange(() => PinionPropFactor);
            NotifyOfPropertyChange(() => MeshAlignFactor);
            NotifyOfPropertyChange(() => AlignmentFactor);
        }

        public double SizeFactor
        {
            get { return gearDrive.SizeFactor; }
            set { gearDrive.SizeFactor = value; }
        }

        public void ShowSizeFactors()
        {
            SizeFactorViewModel viewModel = _container.GetInstance<SizeFactorViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }
        public double OverloadFactor
        {
            get { return gearDrive.OverLoadFactor; }
            set { gearDrive.OverLoadFactor = value; }
        }

        public void ShowOverloadFactors()
        {
            OverloadFactorViewModel viewModel = _container.GetInstance<OverloadFactorViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }
        public double PinionRimFactor
        {
            get { return gearDrive.pinion.RimThicknessFactor; }
            set { gearDrive.pinion.RimThicknessFactor = value; }
        }

        public void ShowRimThicknessFactor()
        {
            RimThicknessViewModel viewModel = _container.GetInstance<RimThicknessViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }

        public void ShowRimThicknessFactor2()
        {
            ShowRimThicknessFactor();
        }
        public double GearRimFactor
        {
            get { return gearDrive.gear.RimThicknessFactor; }
            set { gearDrive.gear.RimThicknessFactor = value; }
        }

        public double HardnessRatioFactor
        {
            get { return gearDrive.HardnessRatioFactor; }
            set { gearDrive.HardnessRatioFactor = value; }
        }

        public int TemperatureFactor
        {
            get { return gearDrive.TemperatureFactor; }
            set { gearDrive.TemperatureFactor = value; }
        }

        public double SafetyFactor
        {
            get { return gearDrive.SafetyFactor; }
            set { gearDrive.SafetyFactor = value; }
        }

        public double ReliabilityFactor
        {
            get { return gearDrive.ReliabilityFactor; }
            set { gearDrive.ReliabilityFactor = value; }
        }

        public void ShowReliabilityFactors()
        {
            ReliabilityFactorViewModel viewModel = _container.GetInstance<ReliabilityFactorViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }
        public string PinionNoOfLoadCycles
        {
            get { return gearDrive.pinion.LoadCyclesToPower; }
            //set { gearDrive.pinion.LoadCycles = value; }
        }

        public string GearNoOfLoadCycles
        {
            get { return gearDrive.gear.LoadCyclesToPower; }
            //set { gearDrive.gear.LoadCycles = value; }
        }

        public double PBendingStressCycleFactor
        {
            get { return gearDrive.pinion.BendingStressCycleFactor; }
            set { gearDrive.pinion.BendingStressCycleFactor = value; }
        }

        public void ShowBendingStressCycle()
        {
            BendingStressCycleViewModel viewModel = _container.GetInstance<BendingStressCycleViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }

        public double GBendingStressCycleFactor
        {
            get { return gearDrive.gear.BendingStressCycleFactor; }
            set { gearDrive.gear.BendingStressCycleFactor = value; }
        }

        public void ShowBendingStressCycle2()
        {
            ShowBendingStressCycle();
        }
        public double PContactStressCycleFactor
        {
            get { return gearDrive.pinion.PittingStressCycleFactor; }
            set { gearDrive.pinion.PittingStressCycleFactor = value; }
        }

        public double GContactStressCycleFactor
        {
            get { return gearDrive.gear.PittingStressCycleFactor; }
            set { gearDrive.gear.PittingStressCycleFactor = value; }
        }

        public double PinionBendingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.pinion.BendingStress, myUnit);
                }
                return gearDrive.pinion.BendingStress; 
            }
            //set { gearDrive.pinion.BendingStress = value; }
        }

        public double PAllowableBendingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.pinion.AdjustedBendingStress, myUnit);
                }
                return gearDrive.pinion.AdjustedBendingStress; 
            }
            //set { gearDrive.pinion.AdjustedBendingStress = value; }
        }
        public double GearBendingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.gear.BendingStress, myUnit);
                }
                return gearDrive.gear.BendingStress;
            }
            //set { gearDrive.gear.BendingStress = value; }
        }

        public double GAllowableBendingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.gear.AdjustedBendingStress, myUnit);
                }
                return gearDrive.gear.AdjustedBendingStress; 
            }
            set { gearDrive.gear.AdjustedBendingStress = value; }
        }
        public double PinionPittingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.pinion.ContactStress, myUnit);
                }
                return gearDrive.pinion.ContactStress; 
            }
           // set { gearDrive.pinion.ContactStress = value; }
        }

        public double PAllowableContactStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.pinion.AdjustedContactStress, myUnit);
                }
                return gearDrive.pinion.AdjustedContactStress; 
            }
            //set { gearDrive.pinion.AdjustedContactStress = value; }
        }

        public void ShowContactStressCycleFactor()
        {
            ContactStressCycleViewModel viewModel = _container.GetInstance<ContactStressCycleViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }

        public double GearPittingStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.gear.ContactStress, myUnit);
                }
                return gearDrive.gear.ContactStress; 
            }
           // set { gearDrive.gear.ContactStress = value; }
        }

        public double GAllowableContactStress
        {
            get 
            {
                if (myUnit == UnitSystem.Metric)
                {
                    return UnitConverter.ConvStressMag(gearDrive.gear.AdjustedContactStress, myUnit);
                }
                return gearDrive.gear.AdjustedContactStress; 
            }
            //set { gearDrive.gear.AdjustedContactStress = value; }
        }
        public void ShowContactStressCycleFactor2()
        {
            ShowContactStressCycleFactor();
        }

        public double PBendingSafetyFactor
        {
            get 
            {
                double Sf = 0;
                if (gearDrive.pinion.BendingStress != 0)
                {
                    Sf = gearDrive.pinion.AdjustedBendingStress / gearDrive.pinion.BendingStress; 
                }
                return Math.Round(Sf, 2);
            }
        }

        public double GBendingSafetyFactor
        {
            get 
            {
                double Sf = 0;
                if (gearDrive.gear.BendingStress != 0)
                {
                    Sf = gearDrive.gear.AdjustedBendingStress / gearDrive.gear.BendingStress; 
                }
                return Math.Round(Sf, 2); 
            }
        }

        public double PContactSafetyFactor
        {
            get 
            {
                double Sf = 0;
                if (gearDrive.pinion.ContactStress != 0)
                {
                    Sf = gearDrive.pinion.AdjustedContactStress / gearDrive.pinion.ContactStress; 
                }
                return Math.Round(Sf, 2);
            }
        }

        public double GContactSafetyFactor
        {
            get 
            {
                double Sf = 0;
                if (gearDrive.gear.ContactStress != 0)
                {
                    Sf = gearDrive.gear.AdjustedContactStress / gearDrive.gear.ContactStress;
                }
                return Math.Round(Sf, 2);
            }
        }

        public string PowerUnit
        {
            get { return unit.PowerUnit; }
        }

        public string SpeedUnit
        {
            get { return unit.AngularSpeedUnit; }
        }

        public string LinearSpeedUnit
        {
            get { return unit.LinearVelocityUnit; }
        }


        public string LengthUnit
        {
            get { return unit.LengthUnit; }
        }

        public string ForceUnit
        {
            get { return unit.ForceUnit; }
        }

        public string StressUnit
        {
            get
            {
               return $"({unit.StressUnit})";
            }
        }
    }
}
