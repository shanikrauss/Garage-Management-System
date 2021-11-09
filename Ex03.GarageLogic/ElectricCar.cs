using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The class that inherits from the Car Department and consolidates under it const fields (relevant specifically to the electric car)
    //and methods relevant to an electric car.
    public class ElectricCar : Car
    {
        private const float k_MaxBatteryTimeHours = 3.2f;


        public ElectricCar(string i_License) : base(i_License)
        {
            m_Engine = new ElectricEngine(k_MaxBatteryTimeHours);
        }


        public override string ToString()
        {
            string electricCarArgsStr = base.ToString();

            return electricCarArgsStr;
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);
            m_Engine.UpdateInfo(i_VehicleArgsList[3], k_MaxBatteryTimeHours);
        }
    }
}
