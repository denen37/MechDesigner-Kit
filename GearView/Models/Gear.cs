using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GearWindow.Models
{
    public class Gear
    {
        private double _toothCount;
        private double _pitchDiameter;
        private double _outsideDiameter;
        private double _rootDiameter;
        private double _torque;
        private GearDrive _drive;
        private double _bendingStress;
        private double _loadCycles;
        private double _bendingStressGeometryFactor;
        private double _adjustedContactStress;
        private double _adjustedBendingStress;
        private double _fatigueStressConcFactor;
        private double _rimThicknessFactor = 1;
        private double _backupRatio;
        private double _bendingStressCycle = 1;
        private double _pittingStressCycle;
        private double _contactStress;
        private double _angVel;

        public event EventHandler<double> AngVelChanged;
        public event EventHandler<double> CanCalcStressCycleFactors;
        public event EventHandler<double> ReCalcStressFactor;
        public event EventHandler<double> CanCalcAllBendingStress;
        public event EventHandler<double> CanCalcAllContactStress;
        public Gear() { }
       
        public Gear(GearDrive drive)
        {
            _drive = drive;
        }

        public double ToothCount
        {
            get 
            {
                if (_toothCount <= 0)
                {
                    if (_drive.DiametralPitch > 0 && _pitchDiameter > 0)
                    {
                        _toothCount = _drive.DiametralPitch * _pitchDiameter;
                    }
                }
                return _toothCount; 
            }
            set { _toothCount = value; }
        }

        public double AngVel 
        {
            get
            {
                return _angVel;
            }
            set
            {
                _angVel = value;
                if (Drive.DesignLife != 0)
                {
                    AngVelChanged?.Invoke(this, _angVel);
                }
            }
        }

        public double PitchDiameter
        {
            get 
            {
                if (_pitchDiameter <= 0)
                {
                    if (_toothCount > 0 && _drive.DiametralPitch > 0)
                    {
                        _pitchDiameter = Math.Round(_toothCount / _drive.DiametralPitch, 2);
                    }
                }

                return _pitchDiameter; 
            }
            set { _pitchDiameter = value; }
        }

        public double OutsideDiameter
        {
            get { return _outsideDiameter; }
            set { _outsideDiameter = value; }
        }

        public double RootDiameter
        {
            get { return _rootDiameter; }
            set { _rootDiameter = value; }
        }

        public int numOfLoadAppPerRev { get; set; } = 1;

        public double Torque
        {
            get 
            {
                _torque = 63000 * _drive.Power / AngVel;
                return _torque; 
            }
            set { _torque = value; }
        }

        public double BendingStress
        {
            get { return _bendingStress; }
            set { _bendingStress = value; }
        }

        public double BendingStressGeometryFactor
        {
            get { return _bendingStressGeometryFactor; }
            set 
            {
                _bendingStressGeometryFactor = value;
                ReCalcStressFactor?.Invoke(this, value);
            }
        }

        public double LoadCycles
        {
            get
            {
                
                    _loadCycles = 60 * _drive.DesignLife * numOfLoadAppPerRev * AngVel;
                
                return _loadCycles;
            }
            set { _loadCycles = value; }
        }

        public string LoadCyclesToPower
        {
            get
           {
                string loadCycle = null;
                int pow = 0;
                
                if (LoadCycles != 0 )
                {
                    string word = LoadCycles.ToString();
                    string[] words = word.Split('.');
                    pow = words[0].Length - 1;

                    double coeff = Math.Round(LoadCycles / (Math.Pow(10, pow)), 2);

                    loadCycle = $"{coeff} X 10{pow.ToSuperscriptNumber()} Cycles";
                }

                CanCalcStressCycleFactors?.Invoke(this, _loadCycles);

                return loadCycle;
            }
        }
        public double AdjustedContactStress
        {
            get { return _adjustedContactStress; }
            set { _adjustedContactStress = value; }
        }

        public double AdjustedBendingStress
        {
            get { return _adjustedBendingStress; }
            set { _adjustedBendingStress = value; }
        }

        public string Material { get; set; }

        //Do not use this property unless absolutely neccesary
        public GearDrive Drive
        {
            get { return _drive; }
            private set { _drive = value; }
        }

        public double FatigueStressConcFactor
        {
            get { return _fatigueStressConcFactor; }
            set { _fatigueStressConcFactor = value; }
        }

        public double RimThicknessFactor
        {
            get
            {
                if (_rimThicknessFactor <= 0)
                {
                    if (_backupRatio >= 1.2)
                    {
                        _rimThicknessFactor = 1.6 * Math.Log(2.242 / _backupRatio);
                    }
                }
                return _rimThicknessFactor;
            }
            set 
            { 
                _rimThicknessFactor = value;
            }
        }

        public double BackupRatio
        {
            get
            {
                if (_backupRatio <= 0)
                {
                    _backupRatio = 1.2;
                }
                return _backupRatio;
            }
            set { _backupRatio = value; }
        }

        public double BendingStressCycleFactor
        {
            get { return _bendingStressCycle; }
            set 
            {
                _bendingStressCycle = value;
                CanCalcAllBendingStress?.Invoke(this, _bendingStressCycle);
            }
        }

        public double PittingStressCycleFactor
        {
            get { return _pittingStressCycle; }
            set 
            { 
                _pittingStressCycle = value;
                CanCalcAllContactStress?.Invoke(this, _pittingStressCycle);
            }
        }

        public double ContactStress
        {
            get { return _contactStress; }
            set { _contactStress = value; }
        }

    }
}
