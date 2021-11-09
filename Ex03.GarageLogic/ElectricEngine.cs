using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    //The class inherits from the Engine class and combines fields and methods that are relevant for a motor
    //that drives electricity.
    public class ElectricEngine : Engine
    {
        private const float k_MinBatteryTimeHours = 0.1f;
        private float m_BatteryRemainingTimeHours;
        private readonly float r_MaxBatteryTimeHours;


        public ElectricEngine(float i_MaxBatteryTimeHours) : base()
        {
            r_MaxBatteryTimeHours = i_MaxBatteryTimeHours;
        }


        public override void GetArgsList(List<string> io_List)
        {
            io_List.Add("Battery Remaining Time in Hours");
        }


        public float MaxBatteryTimeHours
        {
            get
            {
                return r_MaxBatteryTimeHours;
            }
        }


        public float BatteryRemainingTimeHours
        {
            get
            {
                return m_BatteryRemainingTimeHours;
            }
            set
            {
                if (value < k_MinBatteryTimeHours || value > r_MaxBatteryTimeHours)
                {
                    throw new ValueOutOfRangeException(k_MinBatteryTimeHours, r_MaxBatteryTimeHours, "electric engine");
                }

                m_BatteryRemainingTimeHours = value;
                EnergyPercentLeft = CalcEnergyPercentLeft(m_BatteryRemainingTimeHours, r_MaxBatteryTimeHours);
            }
        }


        public void ChargeBattery(float i_HoursToAddToCurrCharge)
        {
            float newBatteryTime = m_BatteryRemainingTimeHours + i_HoursToAddToCurrCharge;

            if(newBatteryTime > r_MaxBatteryTimeHours)
            {
                float maxCharge = r_MaxBatteryTimeHours - i_HoursToAddToCurrCharge;

                throw new ValueOutOfRangeException(0, maxCharge, "electric engine");
            }

            BatteryRemainingTimeHours = newBatteryTime;
        }


        public override string ToString()
        {
            string engineArgsStr = base.ToString();
            string electricEngineArgsStr = string.Format(
                @"Battery remaining time in hours: {0}
 Max battery time in hours: {1}{2}",m_BatteryRemainingTimeHours, r_MaxBatteryTimeHours, Environment.NewLine);
            string mergedElectricStr = string.Format("{0} {1}", engineArgsStr, electricEngineArgsStr);

            return mergedElectricStr;
        }


        public override void UpdateInfo(string i_BatteryTimeStr, float i_MaxBatteryTimeHours)
        {
            float inputBatteryTime;
            bool isValid = float.TryParse(i_BatteryTimeStr, out inputBatteryTime);

            if (!isValid)
            {
                throw new FormatException("Battery time must be a positive number");
            }

            BatteryRemainingTimeHours = inputBatteryTime;
        }
    }
}
