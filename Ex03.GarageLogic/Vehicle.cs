using System;
using System.Collections.Generic;
using System.Text;

namespace Ex03.GarageLogic
{
    //The abstract class combines general fields and methods relevant to a Vehicle,
    //contains the Engine object, and an array of wheels.
    public abstract class Vehicle
    {
        private readonly string r_LicenseNumber;
        private string m_ModelName;
        private List<Wheel> m_Wheels;
        protected Engine m_Engine;


        protected Vehicle(string i_License, int i_WheelsAmount, float i_WheelMaxAirPressure)
        {
            r_LicenseNumber = i_License;
            m_Wheels = new List<Wheel>(i_WheelsAmount);

            for (int i = 0; i < i_WheelsAmount; i++)
            {
                m_Wheels.Add(new Wheel(i_WheelMaxAirPressure));
            }

            m_Engine = null;
        }


        public Engine Engine
        {
            get
            {
                return m_Engine;
            }
        }


        public string ModelName
        {
            get
            {
                return m_ModelName;
            }
        }


        public string LicenseNumber
        {
            get
            {
                return r_LicenseNumber;
            }
        }


        public virtual void UpdateInfo(List<string> i_VehicleArgsList)
        {
            m_ModelName = i_VehicleArgsList[0];

            foreach (Wheel wheel in m_Wheels)
            {
                wheel.UpdateInfo(i_VehicleArgsList[1], i_VehicleArgsList[2]);
            }
        }


        public virtual void GetArgsList(List<string> io_List)
        {
            io_List.Add("Model Name");
            m_Wheels[0].GetArgsList(io_List);
            m_Engine.GetArgsList(io_List);
        }


        public override string ToString()
        {
            string vehicleArgsStr = string.Format(@"license: {0}
 Model Name: {1}{2}", r_LicenseNumber, m_ModelName, Environment.NewLine);
            string engineArgsStr = m_Engine.ToString();
            string wheelArgsStr = m_Wheels[0].ToString();
            string mergedVehicleArgsStr = string.Format("{0} {1} {2}", vehicleArgsStr, engineArgsStr, wheelArgsStr);

            return mergedVehicleArgsStr;
        }


        public void InflateAllWheelToMax()
        {
            foreach (Wheel wheel in m_Wheels)
            {
                wheel.InflateWheelToMax();
            }
        }
    }
}
