using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex03.GarageLogic
{
    //The class that inherits from an Exception class, throws errors relevant to exceptions from a defined range of values.
    public class ValueOutOfRangeException : Exception
    {
        private readonly float r_MaxValue;
        private readonly float r_MinValue;
        private readonly string r_ObjectThrowType;


        public ValueOutOfRangeException(float i_MinValue, float i_MaxValue, string i_ObjectThrowType) : base("Value out of range")
        {
            r_MinValue = i_MinValue;
            r_MaxValue = i_MaxValue;
            r_ObjectThrowType = i_ObjectThrowType;
        }


        public string ObjectThrowType
        {
            get
            {
                return r_ObjectThrowType;
            }
        }


        public float MaxValue
        {
            get
            {
                return r_MaxValue;
            }
        }


        public float MinValue
        {
            get
            {
                return r_MinValue;
            }
           
        }
    }
}
