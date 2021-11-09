using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The class that inherits from Car class and consolidates const fields (relevant specifically to a fuel car)
    //and methods that are relevant for a car that drives fuel.
    public class FuelCar : Car
    {
        private const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Octan95;
        private const float k_MaxAmountFuelLiters = 45;


        public FuelCar(string i_License) : base(i_License)
        {
            m_Engine = new FuelEngine(k_FuelType, k_MaxAmountFuelLiters);
        }


        public override string ToString()
        {
            string fuelCarArgsStr = base.ToString();

            return fuelCarArgsStr;
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);
            m_Engine.UpdateInfo(i_VehicleArgsList[3], k_MaxAmountFuelLiters);
        }
    }
}
