using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    //The abstract class combines fields and methods relevant to Engine.
    public abstract class Engine
    {
        private const float k_MinPercent=0.1f;
        private const float k_MaxPercent = 100f;
        protected float m_EnergyPercentLeft;
        
        
       public float EnergyPercentLeft
        {
            get
            {
                return m_EnergyPercentLeft;
            }
            set
            {
                if(value < k_MinPercent || value > k_MaxPercent)
                {
                    throw new ValueOutOfRangeException(k_MinPercent, k_MaxPercent, "energy percent left in engine");
                }

                m_EnergyPercentLeft = value;
            }
        }


       public abstract void UpdateInfo(string i_CurrentEnergy, float i_MaxEnergyPossible);

       
       public abstract void GetArgsList(List<string> i_List);


        protected float CalcEnergyPercentLeft(float i_CurrentEnergy, float i_MaxEnergyPossible)
       {
           float percent = i_CurrentEnergy / i_MaxEnergyPossible * 100;

           return percent;
       }


       public override string ToString()
       {
           string engineArgsStr = string.Format("Energy percent left: {0}%{1}", m_EnergyPercentLeft, Environment.NewLine);

           return engineArgsStr;
       }
    }
}
