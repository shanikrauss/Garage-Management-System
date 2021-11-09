using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    //The abstract class inherits from the Vehicle class and combines fields and methods relevant to Motorcycle.
    //Contains an enum that combines the possible license types.
    public abstract class Motorcycle : Vehicle
    {
        private const int k_WheelsAmount = 2;
        private const float k_WheelMaxAirPressure = 30;
        private eLicenseType m_LicenseType;
        private int m_EngineVolumeCC;


        public enum eLicenseType
        {
            A = 1,
            AA,
            B1,
            BB
        }


        protected Motorcycle(string i_License) : base(i_License, k_WheelsAmount, k_WheelMaxAirPressure)
        { }


        public eLicenseType LicenseType
        {
            get
            {
                return m_LicenseType;
            }
        }


        public int EngineVolumeCC
        {
            get
            {
                return m_EngineVolumeCC;
            }
        }


        public override void UpdateInfo(List<string> i_VehicleArgsList)
        {
            base.UpdateInfo(i_VehicleArgsList);

            if (!Enum.IsDefined(typeof(eLicenseType), i_VehicleArgsList[4]))
            {
                throw new ArgumentException("Not Valid motorcycle license type");
            }

            int engineVolumeCC;
            bool isValid = int.TryParse(i_VehicleArgsList[5], out engineVolumeCC);

            if (!isValid)
            {
                throw new FormatException("Engine volume CC must be a positive integer number");
            }

            m_EngineVolumeCC = engineVolumeCC;
            m_LicenseType =(eLicenseType)Enum.Parse(typeof(eLicenseType), i_VehicleArgsList[4]);
        }


        public override void GetArgsList(List<string> io_List)
        {
            base.GetArgsList(io_List);
            string[] licenseStr = Enum.GetNames(typeof(eLicenseType));
            StringBuilder licenseOptions = new StringBuilder();

            foreach (string type in licenseStr)
            {
                licenseOptions.Append(string.Format("{0}, ", type));
            }

            string licenseType = string.Format("License Types: {0}", licenseOptions);

            io_List.Add(licenseType);
            io_List.Add("Engine Volume CC");
        }


        public override string ToString()
        {
            string vehicleArgsStr = base.ToString();
            string motorcycleArgsStr = string.Format(@"Wheels Amount: {0}
 Wheel max air pressure: {1}
 License type: {2}
 Engine volume CC: {3}{4}", k_WheelsAmount, k_WheelMaxAirPressure, m_LicenseType.ToString(), m_EngineVolumeCC, Environment.NewLine);
            string mergedMotorcycleArgsStr = string.Format("{0} {1}", vehicleArgsStr, motorcycleArgsStr);

            return mergedMotorcycleArgsStr;
        }
    }
}
