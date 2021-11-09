using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex03.GarageLogic
{
    //The abstract class inherits from the Vehicle class and combines fields and methods relevant to the Car.
    //Contains enum that combines possible car colors.
    public abstract class Car : Vehicle
    {
        private const int k_WheelsAmount = 4;
        private const float k_WheelMaxAirPressure = 32;
        private const int k_MaxPossibleDoor = 5;
        private const int k_MinPossibleDoor = 2;
        private eColor m_Color; 
        private int m_NumberOfDoors;


        public enum eColor
        {
            Black = 1,
            White,
            Silver,
            Red
        }


        protected Car(string i_License) : base(i_License, k_WheelsAmount, k_WheelMaxAirPressure)
        { }
        

        public eColor Color
        {
            get
            {
                return m_Color;
            }
        }


        public int NumberOfDoors
        {
            get
            {
                return m_NumberOfDoors;
            }
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);

            if (!Enum.IsDefined(typeof(eColor), i_VehicleArgsList[4]))
            {
                throw new ArgumentException("Not Valid Color");
            }

            int doorAmount;
            bool isValidDoorAmount = int.TryParse(i_VehicleArgsList[5], out doorAmount);

            if (!isValidDoorAmount)
            {
                throw new FormatException("Amount of doors must be a positive integer number");
            }

            if (doorAmount > k_MaxPossibleDoor || doorAmount < k_MinPossibleDoor)
            {
                throw new ValueOutOfRangeException(k_MinPossibleDoor, k_MaxPossibleDoor, "car");
            }

            m_NumberOfDoors = doorAmount;
            m_Color = (eColor)Enum.Parse(typeof(eColor), i_VehicleArgsList[4]);
        }


        public override void GetArgsList(List<string> io_List)
        {
            base.GetArgsList(io_List);
            string[] colorStr = Enum.GetNames(typeof(eColor));
            StringBuilder colorOptions = new StringBuilder();

            foreach (string color in colorStr)
            {
                colorOptions.Append(string.Format("{0}, ", color));
            }

            string colors = string.Format("Color:  {0}", colorOptions);
            string doors = string.Format("Number of doors {0} - {1}", k_MinPossibleDoor, k_MaxPossibleDoor);

            io_List.Add(colors);
            io_List.Add(doors);
        }


        public override string ToString()
        {
            string vehicleArgsStr = base.ToString();
            string wheelArgsStr = string.Format(@"Wheels Amount: {0}
 Wheel max air pressure: {1}
 color: {2}
 Number of doors: {3}{4}", k_WheelsAmount, k_WheelMaxAirPressure, m_Color.ToString(), NumberOfDoors, Environment.NewLine);
            string str = string.Format("{0} {1}", vehicleArgsStr, wheelArgsStr);

            return str;
        }
    }
}
