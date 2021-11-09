using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ex03.GarageLogic
{
    //The class combines fields and methods relevant to the wheel.
    public class Wheel
    {
        private const int k_MinPressure = 1;
        private readonly float r_MaxAirPressure;
        private string m_ManufacturerName;
        private float m_CurrAirPressure;


        public Wheel(float i_MaxAirPressure)
        {
            r_MaxAirPressure = i_MaxAirPressure;
        }


        public Wheel(string i_ManufacturerName, float i_CurrAirPressure, float i_MaxAirPressure)
        {
            m_ManufacturerName = i_ManufacturerName;
            m_CurrAirPressure = i_CurrAirPressure;
            r_MaxAirPressure = i_MaxAirPressure;
        }


        public float MaxAirPressure
        {
            get
            {
                return r_MaxAirPressure;
            }
        }


        public string ManufacturerName
        {
            get
            {
                return m_ManufacturerName;
            }
        }


        public float CurrAirPressure
        {
            get
            {
                return m_CurrAirPressure;
            }
            set
            {
                if (value < k_MinPressure || value > r_MaxAirPressure)
                {
                    throw new ValueOutOfRangeException(k_MinPressure, r_MaxAirPressure, "wheel air pressure");
                }

                m_CurrAirPressure = value;
            }
        }


        public override string ToString()
        {
            string wheelArgsStr = string.Format(@"Wheel manufacturer: {0}
 Max air pressure Name: {1}
 Current air pressure: {2}{3}", m_ManufacturerName, r_MaxAirPressure, m_CurrAirPressure, Environment.NewLine);

            return wheelArgsStr;
        }


        public void InflateWheel(float i_AirToAdd)
        {
            if(m_CurrAirPressure + i_AirToAdd > r_MaxAirPressure)
            {
                float maxAir = r_MaxAirPressure - m_CurrAirPressure;

                throw new ValueOutOfRangeException(0, maxAir, "wheel");
            }

            m_CurrAirPressure += i_AirToAdd;
        }


        public void InflateWheelToMax()
        {
            float airToAdd = r_MaxAirPressure - m_CurrAirPressure;

            InflateWheel(airToAdd);
        }


        public void GetArgsList(List<string> io_List)
        {
            io_List.Add("Wheel Manufacturer name");
            io_List.Add("Current wheel air pressure");
        }


        public void UpdateInfo(string i_ManufacturerName, string i_CurrAirPressure)
        {
            float wheelPressure;
            bool isValid = float.TryParse(i_CurrAirPressure, out wheelPressure);

            if (!isValid)
            {
                throw new FormatException("Wheel pressure must be a positive number");
            }
            
            CurrAirPressure = wheelPressure;
            m_ManufacturerName = i_ManufacturerName;
        }
    }
}
