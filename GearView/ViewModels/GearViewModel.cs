using Caliburn.Micro;
using GearWindow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GearWindow.ViewModels
{
    class GearViewModel : Conductor<object>, IHandle<Applications>
    {
        private GearAppViewModel _GearApp;
        IWindowManager _manager;
        IEventAggregator _events;
        private SimpleContainer _container;
        private GearDrive gearDrive;
        private UnitList unit = new UnitList(1);
        private BindableCollection<string> availableMaterials =
            new BindableCollection<string> { "Steel", "Malleable Iron", "Nodular Iron", "Cast Iron", "Aluminium Bronze", "Tin Bronze" };
        private BindableCollection<string> availableQualityNo =
            new BindableCollection<string> { "A12", "A11", "A10", "A9", "A8", "A7", "A6", "A5", "A4", "A3", "A2", "A1" };
        
        public GearViewModel(IWindowManager manager, IEventAggregator events, SimpleContainer container)
        {
            _manager = manager;
            _events = events;
            _events.SubscribeOnPublishedThread(this);
            _container = container;
            gearDrive = new GearDrive();
            gearDrive.NoOfLoadCyclesChanged += GearDrive_NoOfLoadCyclesChanged;
            gearDrive.pinion.AngVelChanged += Pinion_AngVelChanged;
            gearDrive.gear.AngVelChanged += Gear_AngVelChanged;
            gearDrive.pinion.CanCalcStressCycleFactors += Pinion_CanCalcStressCycleFactors;
            gearDrive.gear.CanCalcStressCycleFactors += Gear_CanCalcStressCycleFactors;
            gearDrive.pinion.CanCalcStressNumbers += Pinion_CanCalcStressNumbers;
            gearDrive.gear.CanCalcStressNumbers += Gear_CanCalcStressNumbers;
        }

        private void Gear_CanCalcStressNumbers(object sender, double e)
        {
            GearCalculations.CalcBendingStress(gearDrive.gear);
            GearCalculations.CalcContactStress(gearDrive.gear);

            NotifyOfPropertyChange(() => GearBendingStress);
            NotifyOfPropertyChange(() => GearPittingStress);
        }

        private void Pinion_CanCalcStressNumbers(object sender, double e)
        {
            GearCalculations.CalcBendingStress(gearDrive.pinion);
            GearCalculations.CalcContactStress(gearDrive.pinion);

            NotifyOfPropertyChange(() => PinionBendingStress);
            NotifyOfPropertyChange(() => PinionPittingStress);
        }

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
                Application = message.Application;
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
            get { return gearDrive.Power; }
            set
            {
                gearDrive.Power = value;
                NotifyOfPropertyChange(() => InputPower);
            }
        }

        //Input Speed
        public double InputSpeed
        {
            get { return gearDrive.pinion.AngVel; }
            set
            {
                gearDrive.pinion.AngVel = value;
                NotifyOfPropertyChange(() => InputSpeed);
            }
        }

        //Diametral Pitch
        public double DiametralPitch
        {
            get { return gearDrive.DiametralPitch; }
            set
            {
                gearDrive.DiametralPitch = value;
                NotifyOfPropertyChange(() => DiametralPitch);
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
            get { return gearDrive.pinion.PitchDiameter; }
            //set { gearDrive.pinion.PitchDiameter = value; }
        }

        public double GearPitchDiameter
        {
            get { return gearDrive.gear.PitchDiameter; }
            //set { gearDrive.gear.PitchDiameter = value; }
        }

        public double CenterDistance
        {
            get { return gearDrive.CenterDistance; }
        }

        public double PitchLineSpeed
        {
            get { return gearDrive.PitchLineVel; }
        }

        public double TransmittedLoad
        {
            get { return gearDrive.TangentialForce; }
            set { gearDrive.TangentialForce = value; }
        }

        public double RadialForce
        {
            get { return gearDrive.RadialForce; }
            set { gearDrive.RadialForce = value; }
        }

        public double NormalForce
        {
            get { return gearDrive.NormalForce; }
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
                return maxFaceWidth;
            }
        }

        public double FaceWidth
        {
            get
            {
                return gearDrive.FaceWidth;
            }
            set
            {
                gearDrive.FaceWidth = value;
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
                return ratio;
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
            get { return gearDrive.pinion.BendingStress; }
            set { gearDrive.pinion.BendingStress = value; }
        }

        public double GearBendingStress
        {
            get { return gearDrive.gear.BendingStress; }
            set { gearDrive.gear.BendingStress = value; }
        }

        public double PinionPittingStress
        {
            get { return gearDrive.pinion.ContactStress; }
            set { gearDrive.pinion.ContactStress = value; }
        }

        public void ShowContactStressCycleFactor()
        {
            ContactStressCycleViewModel viewModel = _container.GetInstance<ContactStressCycleViewModel>();
            ActivateItemAsync(viewModel);
            _manager.ShowWindowAsync(viewModel);
        }

        public double GearPittingStress
        {
            get { return gearDrive.gear.ContactStress; }
            set { gearDrive.gear.ContactStress = value; }
        }

        public void ShowContactStressCycleFactor2()
        {
            ShowContactStressCycleFactor();
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
    }
}
