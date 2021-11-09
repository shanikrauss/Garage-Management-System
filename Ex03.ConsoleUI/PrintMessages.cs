using System;
using System.Collections.Generic;
using System.Text;
using Ex03.GarageLogic;

namespace Ex03.ConsoleUI
{
    //The class prints messages to the console screen for the user, for the purpose of receiving inputs from the user.
    public struct PrintMessages
    {
        public static void LicenseMsg()
        {
            Console.WriteLine("Please enter a license");
        }


        public static void FuelTypeMsg()
        {
            string[] fuelTypes = Enum.GetNames(typeof(FuelEngine.eFuelType));
            string msg = string.Format(
                @"Please enter fuel type, options are:
{0} - PRESS 1,
{1} - PRESS 2,
{2} - PRESS 3,
{3} - PRESS 4", fuelTypes);

            Console.WriteLine(msg);
        }


        public static void VehicleTypeMsg()
        {
            List<string> vehicleTypeList = VehicleCreator.GetVehicleList();
            StringBuilder msg = new StringBuilder();

            Console.WriteLine("Please choose vehicle type, options are: ");
            int i = 1;

            foreach (string vehicleType in vehicleTypeList)
            {
                msg.AppendLine(string.Format("{0} - PRESS {1}", vehicleType, i));
                i++;
            }

            Console.WriteLine(msg);
        }


        public static void FuelAmountMsg()
        {
            Console.WriteLine("Please enter fuel amount to refuel in litters: ");
        }


        public static void BatteryAmountMsg()
        {
            Console.WriteLine("Please enter how many minutes to charge battery: ");
        }


        public static void StatusMsg()
        {
            string[] statusType = Enum.GetNames(typeof(Garage.eVehicleStatus));
            string msg = string.Format(
                @"Please choose vehicle's status, options are:
{0} - PRESS 1,
{1} - PRESS 2,
{2} - PRESS 3", statusType);

            Console.WriteLine(msg);
        }


        public static void ShowMsg()
        {
            StatusMsg();
            Console.WriteLine("All - PRESS 4");
        }


        public static void PrintMenu()
        {
            string menu = string.Format(@"Please choose an action: (enter number of action)
1. Add new vehicle to garage
2. Show License list of vehicle in garage
3. Change vehicle status
4. Inflate vehicle wheels to max
5. Refuel
6. Charge battery
7. Show vehicle info
8. Exit");

            Console.WriteLine(menu);
        }
    }
}