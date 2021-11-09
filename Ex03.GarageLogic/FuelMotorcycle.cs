using System;
using System.Collections.Generic;
using System.Text;


namespace Ex03.GarageLogic
{
    //The class inherits from the Motorcycle class and consolidates under it const fields (relevant specifically to a fuel-driven motorcycle)
    //and methods relevant to a fuel-driven vehicle.
    public class FuelMotorcycle : Motorcycle
    {
        private const FuelEngine.eFuelType k_FuelType = FuelEngine.eFuelType.Octan98;
        private const float k_MaxAmountFuelLiters = 6;


        public FuelMotorcycle(string i_License) : base(i_License)
        {
            m_Engine = new FuelEngine(k_FuelType, k_MaxAmountFuelLiters);
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);
            m_Engine.UpdateInfo(i_VehicleArgsList[3], k_MaxAmountFuelLiters);
        }


        public override string ToString()
        {
            string fuelMotorcycleArgsStr = base.ToString();

            return fuelMotorcycleArgsStr;
        }
    }
}
