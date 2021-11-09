using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The class inherits from the Motorcycle class and consolidates under it const fields
    //(relevant specifically to the motorcycle that prevents electricity) and methods that are relevant for an electric vehicle.
    public class ElectricMotorcycle : Motorcycle
    {
        private const float k_MaxBatteryTimeHours = 1.8f;


        public ElectricMotorcycle(string i_License)
            : base(i_License)
        {
            m_Engine = new ElectricEngine(k_MaxBatteryTimeHours);
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);
            m_Engine.UpdateInfo(i_VehicleArgsList[3], k_MaxBatteryTimeHours);
        }


        public override string ToString()
        {
            string electricMotorcycleStr = base.ToString();

            return electricMotorcycleStr;
        }
    }
}
