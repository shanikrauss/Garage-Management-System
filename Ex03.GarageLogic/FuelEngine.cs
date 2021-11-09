using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The class inherits from the Engine class and combines fields and methods that are relevant
    //for a fuel-driven engine. Contains enum of different fuel types.
    public class FuelEngine : Engine
    {
        private const float k_MinAmountFuelLiters = 0.1f;
        private readonly eFuelType r_FuelType; 
        private readonly float r_MaxAmountFuelLiters;
        private float m_CurrentAmountFuelLiters;


        public FuelEngine(eFuelType i_FuelType, float i_MaxAmountFuelLiters) : base()
        {
            r_FuelType = i_FuelType;
            r_MaxAmountFuelLiters = i_MaxAmountFuelLiters;
        }


        public enum eFuelType
        {
            Soler = 1,
            Octan95,
            Octan96,
            Octan98
        }


        public float MaxAmountFuelLiters
        {
            get
            {
                return r_MaxAmountFuelLiters;
            }
        }


        public float CurrentAmountFuelLiters
        {
            get
            {
                return m_CurrentAmountFuelLiters;
            }
            set
            {
                if (value < k_MinAmountFuelLiters || value > r_MaxAmountFuelLiters)
                {
                    throw new ValueOutOfRangeException(k_MinAmountFuelLiters, r_MaxAmountFuelLiters, "fuel engine");
                }

                m_CurrentAmountFuelLiters = value;
                EnergyPercentLeft = CalcEnergyPercentLeft(m_CurrentAmountFuelLiters, r_MaxAmountFuelLiters);
            }
        }


        public eFuelType FuelType
        {
            get
            {
                return r_FuelType;
            }
        }


        public override void UpdateInfo(string i_CurrentAmountFuelLiters, float i_MaxAmountFuelLiters)
        {
            float inputCurrAmountOfFuel;
            bool isValid = float.TryParse(i_CurrentAmountFuelLiters, out inputCurrAmountOfFuel);

            if (!isValid)
            {
                throw new FormatException("Current amount of fuel must be a positive number");
            }

            CurrentAmountFuelLiters = inputCurrAmountOfFuel;
        }


        public override void GetArgsList(List<string> io_List)
        {
            io_List.Add("Amount of Fuel in Liters");
        }


        public void Refuel(eFuelType i_Type, float i_QuantityLitersToAdd)
        {
            if(i_Type != r_FuelType)
            {
                throw new ArgumentException("Fuel type doesn't match vehicle fuel");
            }

            float newAmount = m_CurrentAmountFuelLiters + i_QuantityLitersToAdd;

            if (newAmount > r_MaxAmountFuelLiters)
            {
                float maxToFuel = r_MaxAmountFuelLiters - m_CurrentAmountFuelLiters;

                throw new ValueOutOfRangeException(0, maxToFuel, "fuel engine");
            }

            CurrentAmountFuelLiters = newAmount;
        }
        

        public override string ToString()
        {
            string engineArgsStr = base.ToString();
            string fuelEngineArgsStr = string.Format(@"Fuel type: {0}
 Current amount of fuel in liters: {1}
 Max amount of fuel in liters: {2}", r_FuelType.ToString(), m_CurrentAmountFuelLiters, r_MaxAmountFuelLiters, Environment.NewLine);
            string mergedFuelCarStr = string.Format("{0} {1}", engineArgsStr, fuelEngineArgsStr);

            return mergedFuelCarStr;
        }
    }
}
