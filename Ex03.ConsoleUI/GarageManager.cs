using System;
using System.Text;
using Ex03.GarageLogic;
using System.Collections.Generic;
using System.Threading;


namespace Ex03.ConsoleUI
{
    //The class presents to the console and manages the garage system.
    //Displays the various functions that can be performed in the garage system and uses the logic classes to perform them.
    //Receives inputs from the user about the requested actions.
    //Contains an enum of the various functions that can be performed in the garage's system.
    public class GarageManager
    {
        private const int k_ShowAllVehicle = 4;
        private const int k_NumberOfMilSecondsConsoleSleeps = 5000;
        private GarageLogic.Garage m_Garage;


        public GarageManager()
        {
            m_Garage = new Garage();
        }


        private enum eUserSelectAction
        {
            AddNewVehicle = 1,
            ShowLicenseList,
            ChangeStatus,
            InflateWheelsToMax,
            Refuel,
            ChargeBattery,
            ShowVehicleInfo,
            Exit
        }


        public void Manager() 
        {
            bool continueProgram = true;

            while (continueProgram)
            {
                PrintMessages.PrintMenu();
                eUserSelectAction inputAction = userMenuInput();

                try
                {
                    switch(inputAction)
                    {
                        case eUserSelectAction.AddNewVehicle:
                            AddNewVehicle();
                            break;

                        case eUserSelectAction.ShowLicenseList:
                            showLicenseList();
                            break;

                        case eUserSelectAction.ChangeStatus:
                            changeStatus();
                            break;

                        case eUserSelectAction.InflateWheelsToMax:
                            inflateWheelsToMax();
                            break;

                        case eUserSelectAction.Refuel:
                            refuel();
                            break;

                        case eUserSelectAction.ChargeBattery:
                            chargeBattery();
                            break;

                        case eUserSelectAction.ShowVehicleInfo:
                            showVehicleInfo();
                            break;

                        case eUserSelectAction.Exit:
                            continueProgram = !continueProgram;
                            break;
                        default:
                            Console.WriteLine("Not valid option");
                            break;
                    }
                }
                catch(ValueOutOfRangeException outRangeEx)
                {
                    Console.WriteLine(
                        "{0}, {1} needs to be between {2} - {3}",
                        outRangeEx.Message,
                        outRangeEx.ObjectThrowType,
                        outRangeEx.MinValue,
                        outRangeEx.MaxValue);
                }
                catch(FormatException formatEx)
                {
                    Console.WriteLine(formatEx.Message);
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(argEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Something went wrong... Please try again :)");
                }

                Thread.Sleep(k_NumberOfMilSecondsConsoleSleeps);
                Console.Clear();
            }
        }


        private void AddNewVehicle()
        {
            PrintMessages.VehicleTypeMsg();
            string vehicleTypeStr = Console.ReadLine();
            VehicleCreator.eVehicleType vehicleType = (VehicleCreator.eVehicleType)Enum.Parse(typeof(VehicleCreator.eVehicleType), vehicleTypeStr);

            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();
            bool exist = m_Garage.CheckIfLicenseExist(license);
            
            if(!exist)
            {
                Vehicle newVehicle = VehicleCreator.Create(license, vehicleType);

                List<string> requestedArgsListVehicle = new List<string>();
                List<string> requestedArgsListGarage = new List<string>();

                m_Garage.GetVehicleRequestedArgsList(newVehicle, requestedArgsListVehicle);
                m_Garage.GetRequestedArgsList(requestedArgsListGarage);

                List<string> inputVehicleArgsList = new List<string>();
                List<string> inputGarageArgsList = new List<string>();

                printAndGetRequestedArgsList(requestedArgsListGarage, inputGarageArgsList);
                printAndGetRequestedArgsList(requestedArgsListVehicle, inputVehicleArgsList);

                m_Garage.UpdateVehicleInfo(newVehicle, inputVehicleArgsList);
                m_Garage.AddVehicle(newVehicle, inputGarageArgsList[0], inputGarageArgsList[1]);
            }
            else
            {
                Console.WriteLine("Vehicle already exists, changing status to 'in progress'");
                m_Garage.UpdateVehicleStatus(license, Garage.eVehicleStatus.InProgress);
            }
        }


        private void printAndGetRequestedArgsList(List<string> i_RequestedArgsList, List<string> io_InputArgsList)
        {
            foreach (string arg in i_RequestedArgsList)
            {
                string msg = string.Format("Please enter {0}", arg);

                Console.WriteLine(msg);
                string input = Console.ReadLine();

                io_InputArgsList.Add(input);
            }
        }


        private void showLicenseList()
        {
            StringBuilder licenseList;
            PrintMessages.ShowMsg();
            int userInput = int.Parse(Console.ReadLine());
            if (userInput == k_ShowAllVehicle)
            {
                licenseList = m_Garage.GetLicenseListOfAllVehicleInGarage();
            }
            else
            {
                Garage.eVehicleStatus status = (Garage.eVehicleStatus)userInput;
                licenseList = m_Garage.GetLicenseListAccordingStatus(status);
            }

            Console.WriteLine(licenseList);
        }


        private void changeStatus()
        {
            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();

            PrintMessages.StatusMsg();
            int statusInt = int.Parse(Console.ReadLine());

            Garage.eVehicleStatus status = (Garage.eVehicleStatus)statusInt;
            m_Garage.UpdateVehicleStatus(license, status);
        }


        private void inflateWheelsToMax()
        {
            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();

            m_Garage.FillAirPressureToMax(license);
        }


        private void refuel()
        {
            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();

            PrintMessages.FuelTypeMsg();
            string fuelTypeStr = Console.ReadLine();

            FuelEngine.eFuelType fuelType = (FuelEngine.eFuelType)Enum.Parse(typeof(FuelEngine.eFuelType), fuelTypeStr);

            PrintMessages.FuelAmountMsg();
            string fuelAmountStr = Console.ReadLine();
            int fuelAmount = int.Parse(fuelAmountStr);

            m_Garage.FillFuel(license, fuelType, fuelAmount);
        }


        private void chargeBattery()
        {
            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();

            PrintMessages.BatteryAmountMsg();
            string amountInMinutesStr = Console.ReadLine();
            int amountInMinutes;
            bool isValid = int.TryParse(amountInMinutesStr, out amountInMinutes);

            if(!isValid)
            {
                throw new FormatException("Minutes to charge battery must be a positive integer");
            }

            float amountInHours = minutesToHours(amountInMinutes);

            m_Garage.ChargeBattery(license, amountInHours);
        }


        private void showVehicleInfo()
        {
            PrintMessages.LicenseMsg();
            string license = Console.ReadLine();
            string vehicleInfo = m_Garage.GetVehicleInfo(license);

            Console.WriteLine(vehicleInfo);
        }


        private float minutesToHours(int i_Minutes)
        {
            float hours = i_Minutes / 60f;

            return hours;
        }


        private eUserSelectAction userMenuInput()
        {
            string userInputStr = Console.ReadLine();
            eUserSelectAction action;
            bool isValid = Enum.TryParse<eUserSelectAction>(userInputStr, out action);

            while(!isValid)
            {
                Console.WriteLine("Invalid input, try again");
                userInputStr = Console.ReadLine();
                isValid = Enum.TryParse<eUserSelectAction>(userInputStr, out action);
            }

            return action;
        }
    }
}
