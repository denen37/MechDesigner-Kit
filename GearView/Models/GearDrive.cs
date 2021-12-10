using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearWindow.Models
{
    public enum UnitSystem
    { 
       Metric,
       English
    }

    public enum Condition
    {
        Open,
        Commercial,
        Precision,
        ExtraPrecision
    }

    public enum HardnessMethod
    {
        Case_carburized,
        Nitrided,
        Through_hardened,
        FlameOrInduction_hardened
    }

    public enum Grade
    {
        Grade1,
        Grade2
    }

    public class GearDrive
    {
        private UnitSystem unit;
        private double _addendum;
        private double _dedendum;
        private double _clearance;
        private double _wholeDepth;
        private double _workingDepth;
        private double _faceWidth;
        private double _pressureAngle;
        private double _toothThickness;
        private double _centerDistance;
        private double _gearRatio;
        private double _circularPitch;
        private double _diametralPitch;
        private double _pitchLineVel;
        private double _module;
        private double _power;
        private double _pinionProprotionFactor;
        private double _meshAlignmentFactor;
        private Condition _manufacAccuracy;
        private double _crowningFactor;
        private bool _crowned = false;
        private bool _adjustedAtAssembly = false;
        private bool isCentered = true;
        private double _adjustmentFactor;
        private double _pinionMountingFactor;
        private double _loadDistributionFactor;
        private double _dynamicFactor = 1;
        private double _maxVelocity;
        private double _reliabilityFactor = 1;
        private double _contactStressGeometryFactor;
        private int _elasticCoefficient = 2300;
        private int _qualityNumber;
        private double _safetyFactor = 1;
        private double _designLife;
        private HardnessMethod _hardnessType = HardnessMethod.Case_carburized;
        private Grade grade = Grade.Grade1;
        private double _allowableContactStress;
        private double _allowableBendingStress;
        private double _filletRaduis;
        private double _hardnessRatio = 1;
        private int _temperature = 1;
        private int alignmentFactorsRequest = 0;
        private double _overloadFactor = 1;
        private double _sizeFactor = 1;
        private double _serviceFactor = 1;
        private int _caseHardness = 50;
        private int _hardness = 140;
        private int places = 2;

        public event EventHandler<double> NoOfLoadCyclesChanged;
        public event EventHandler<double> CanCalcStressNumbers;
        public event EventHandler<double> ReCalcStressNumbers;


        //private readonly GearDrive drive = this;
        public Gear pinion;
        public Gear gear;

        public GearDrive()
        {
            GearDrive drive = this;
            pinion = new Gear(this);
            gear = new Gear(this);
            unit = UnitSystem.English;
            Accuracy = Condition.Commercial;
        }

        public double TangentialForce { get; set; }

        public double NormalForce { get; set; }

        public double RadialForce { get; set; }

        public double OverLoadFactor 
        {
            get { return _overloadFactor; }
            set 
            {
                _overloadFactor = value;
                CanCalcStressNumbers?.Invoke(this, _overloadFactor);
            } 
        }

        public double SizeFactor 
        {
            get { return _sizeFactor; }
            set 
            { 
                _sizeFactor = value;
                CanCalcStressNumbers?.Invoke(this, _overloadFactor);
            }
        } 

        public int Hardness 
        {
            get { return _hardness; }
            set 
            {
                _hardness = value;
                ReCalcStressNumbers?.Invoke(this, value);
            }
        }

        public int CaseHardness
        { get { return _caseHardness; }
            set 
            { 
                _caseHardness = value;
                ReCalcStressNumbers?.Invoke(this, value);
            } 
        }

        public string Application { get; set; }

        public double ServiceFactor 
        {
            get { return _serviceFactor; } 
            set
            { 
                _serviceFactor = value;
                CanCalcStressNumbers?.Invoke(this, _serviceFactor);
            }
        }

        public double PinionProportionFactor 
        {
            get
            {
                if (alignmentFactorsRequest > 0)
                {
                    double F = _faceWidth;
                    double d = pinion.PitchDiameter;
                    double A = 0;
                    if (unit == UnitSystem.Metric)
                    {
                        F = UnitConverter.MMandInches(_faceWidth, UnitSystem.English);
                        d = UnitConverter.MMandInches(pinion.PitchDiameter, UnitSystem.English);
                    }

                    if (d != 0) A = (F / 10 * d);
                    if (A < 0.05 && d != 0)
                        A = 0.05;

                    if (F <= 1)
                        _pinionProprotionFactor = A - 0.025;

                    if (F > 1 && F <= 17)
                        _pinionProprotionFactor = A - 0.0375 + 0.0125 * F;

                    if (F > 17 && F < 40)
                        _pinionProprotionFactor = A - 0.1109 + 0.0207 * F + Math.Pow(F, 2);

                    alignmentFactorsRequest++;
                }
                else alignmentFactorsRequest++;
                
                 
                return Math.Round(_pinionProprotionFactor, places);
            } 
            set { _pinionProprotionFactor = value; } 
        }

        public Condition Accuracy
        {
            get
            {
                return _manufacAccuracy;
            }
            set
            {
                _manufacAccuracy = value;
            }
        }

        public UnitSystem Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public double MeshAlignmentFactor
        {
            get 
            {
                if (alignmentFactorsRequest > 1)
                {

                    double F = _faceWidth;

                    switch (_manufacAccuracy)
                    {
                        case Condition.Open:
                            _meshAlignmentFactor = 0.247 + 0.0167 * F - (0.765 * Math.Pow(10, -4) * Math.Pow(F, 2));
                            break;

                        case Condition.Commercial:
                            _meshAlignmentFactor = 0.127 + 0.0158 * F - (1.093 * Math.Pow(10, -4) * Math.Pow(F, 2));
                            break;

                        case Condition.Precision:
                            _meshAlignmentFactor = 0.0675 + 0.0128 * F - (0.926 * Math.Pow(10, -4) * Math.Pow(F, 2));
                            break;

                        case Condition.ExtraPrecision:
                            _meshAlignmentFactor = 0.038 + 0.0102 * F - (0.822 * Math.Pow(10, -4) * Math.Pow(F, 2));
                            break;

                        default:
                            break;
                    } 
                }
                
                return Math.Round(_meshAlignmentFactor, places); 
            }
            set { _meshAlignmentFactor = value; }
        }

        public double CrownFactor
        {
            get 
            {
                if (_crowned == false)
                {
                    _crowningFactor = 1;
                }
                else
                {
                    _crowningFactor = 0.8;
                }
                return _crowningFactor; 
            }
            set { _crowningFactor = value; }
        }

        public bool IsCrowned
        {
            get { return _crowned; }
            set { _crowned = value; }
        }

        public bool IsLapped
        {
            get { return _adjustedAtAssembly; }
            set { _adjustedAtAssembly = value; }
        }

        public double AdjustmentFactor
        {
            get 
            {
                if (_adjustedAtAssembly == false)
                {
                    _adjustmentFactor = 1;
                }
                else
                {
                    _adjustmentFactor = 0.8;
                }

                return _adjustmentFactor; 
            }
            set { _adjustmentFactor = value; }
        }

        public double PinionMountingFactor
        {
            get 
            {
                if (isCentered == true)
                {
                    _pinionMountingFactor = 1;
                }
                else
                { _pinionMountingFactor = 1.1; }
                return _pinionMountingFactor; 
            }
            //set { _pinionMountingFactor = value; }
        }

        public bool IsCentered
        {
            get { return isCentered; }
            set { isCentered = value; }
        }

        public double LoadDistributionFactor
        {
            get 
            {
                if (alignmentFactorsRequest > 1)
                {


                    _loadDistributionFactor = 1 + CrownFactor *
                        (PinionProportionFactor * PinionMountingFactor + MeshAlignmentFactor * AdjustmentFactor);

                }
                return Math.Round(_loadDistributionFactor, places); 
            }
        }

        public int QualityNumber
        {
            get 
            {
                if (_qualityNumber <= 0)
                {
                    _qualityNumber = 9;
                }
                return _qualityNumber; 
            }
            set { _qualityNumber = value; }
        }

        public double Addendum
        {
            get 
            {
                if (unit == UnitSystem.English)
                {
                    _addendum = 1 / _diametralPitch;
                }
                else
                {
                    _addendum = _module;
                }

                return _addendum; 
            }
        }

        public double Dedendum
        {
            get 
            {
                if (unit == UnitSystem.English)
                {
                    // Coarse Pitch.
                    if (_diametralPitch < 20)
                    {
                        _dedendum = 1.25 / _diametralPitch;
                    }
                    // fine pitch.
                    else
                    {
                        _dedendum = 1.2 / _diametralPitch + 0.002;
                    }
                }
                else
                {
                    _dedendum = 1.25 * _diametralPitch;
                }

                return _dedendum; 
            }
        }

        public double Clearance
        {
            get 
            {
                if (unit == UnitSystem.English)
                {
                    // Coarse Pitch.
                    if (_diametralPitch < 20)
                    {
                        _clearance = 0.25 / _diametralPitch;
                    }
                    // fine pitch.
                    else
                    {
                        _clearance = 0.2 / _diametralPitch + 0.002;
                    }
                }
                else
                {
                    _clearance = 0.25 * _diametralPitch;
                }

                return _clearance; 
            }
        }

        public double FilletRaduis
        {
            get 
            { 
                if (_diametralPitch > 0 )
                {
                    _filletRaduis = 0.3 / _diametralPitch;
                }
                return _filletRaduis; 
            }
            set { _filletRaduis = value; }
        }

        public double WholeDepth
        {
            get 
            {
                _wholeDepth = _addendum + _dedendum;

                return _wholeDepth; 
            }
        }

        public double WorkingDepth
        {
            get 
            {
                _workingDepth = 2 * _addendum;

                return _workingDepth; 
            }
        }

        public double FaceWidth
        {
            get 
            {
                if (unit == UnitSystem.English && _diametralPitch > 0)
                {
                    _faceWidth = 12 / _diametralPitch;
                }
                if (unit == UnitSystem.Metric && _module > 0)
                {
                    _faceWidth = 12 * _module;
                }
                return _faceWidth; 
            }
            set { _faceWidth = value; }
        }

        public double PressureAngle
        {
            get 
            {
                _pressureAngle = 20;

                return _pressureAngle; 
            }
            set { _pressureAngle = value; }
        }

        public double ToothThickness
        {
            get
            {
                if (_circularPitch <= 0)
                {
                    double c = CircularPitch;
                }

                _toothThickness = _circularPitch / 2;

                return _toothThickness; 
            }
        }

        public double CenterDistance
        {
            get 
            {
                _centerDistance = Math.Round((pinion.PitchDiameter + gear.PitchDiameter) / 2, 2);

                return _centerDistance; 
            }
        }

        public double GearRatio
        {
            get { return _gearRatio; }
            set { _gearRatio = value; }
        }

        private double _velocityRatio;

        public double VelocityRatio
        {
            get { return _velocityRatio; }
            set { _velocityRatio = value; }
        }

        public double CircularPitch
        {
            get 
            {
                _circularPitch = Math.PI * pinion.PitchDiameter / pinion.ToothCount;

                return _circularPitch; 
            }
            set { _circularPitch = value; }
        }

        public double DiametralPitch
        {
            get 
            {
                if (_diametralPitch <= 0)
                {
                    if (pinion.PitchDiameter > 0 && pinion.ToothCount > 0)
                    {
                        _diametralPitch = pinion.ToothCount / pinion.PitchDiameter;
                    }
                }

                return _diametralPitch;
            }
            set { _diametralPitch = value; }
        }

        public double PitchLineVel
        {
            get 
            {
                if (unit == UnitSystem.English)
                {
                    _pitchLineVel = Math.Round((Math.PI * gear.PitchDiameter * gear.AngVel) / 12, 2);
                }
                else
                {
                    _pitchLineVel = Math.Round((Math.PI * gear.PitchDiameter * gear.AngVel) / 60000, 2);
                }
                return _pitchLineVel; 
            }
        }

        public double Module
        {
            get 
            {
                if (_module <= 0)
                {
                    if (pinion.PitchDiameter > 0 && pinion.ToothCount > 0)
                    {
                        _module = pinion.PitchDiameter / pinion.ToothCount;
                    }
                    if (gear.PitchDiameter > 0 && gear.ToothCount > 0)
                    {
                        _module = pinion.PitchDiameter / pinion.ToothCount;
                    }
                }

                return _module; 
            }
            set { _module = value; }
        }

        public double Power
        {
            get {  return _power; }
            set { _power = value; }
        }

        public double DynamicFactor
        {
            get 
            {
                if (_dynamicFactor <= 0)
                {
                    _dynamicFactor = 1;
                }

                return _dynamicFactor;
            }
            set 
            { 
                _dynamicFactor = value;
                CanCalcStressNumbers?.Invoke(this, _dynamicFactor);
            }
        }

        public double MaxVelocity
        {
            get { return _maxVelocity; }
            set { _maxVelocity = value; }
        }

        public double ReliabilityFactor
        {
            get { return _reliabilityFactor; }
            set { _reliabilityFactor = value; }
        }

        public double ContactStressGeometryFactor
        {
            get { return _contactStressGeometryFactor; }
            set 
            {
                _contactStressGeometryFactor = value;
                CanCalcStressNumbers?.Invoke(this, value);
            }
        }


        public int ElasticCoefficient
        {
            get { return _elasticCoefficient; }
            set { _elasticCoefficient = value; }
        }

        public double SafetyFactor
        {
            get { return _safetyFactor; }
            set { _safetyFactor = value; }
        }

        public double DesignLife
        {
            get { return _designLife; }
            set 
            { 
                _designLife = value;

                if (pinion.AngVel != 0)
                {
                    NoOfLoadCyclesChanged?.Invoke(this, _designLife);
                }
            }
        }

        public HardnessMethod HardnessType
        {
            get { return _hardnessType; }
            set { _hardnessType = value; }
        }

        public Grade SteelGrade
        {
            get { return grade; }
            set { grade = value; }
        }


        public double AllowableContactStress
        {
            get { return _allowableContactStress; }
            set { _allowableContactStress = value; }
        }

        public double AllowableBendingStress
        {
            get { return _allowableBendingStress; }
            set { _allowableBendingStress = value; }
        }

        public double HardnessRatioFactor
        {
            get { return _hardnessRatio; }
            set { _hardnessRatio = value; }
        }

        public int TemperatureFactor
        {
            get { return _temperature; }
            set { _temperature = value; }
        }

    }
}
