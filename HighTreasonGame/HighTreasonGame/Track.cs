using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Track
    {
        public int Value {
            get; protected set;
        }

        protected string name;

        protected int minValue = 0;
        protected int maxValue = 0;

        protected HashSet<string> properties;

        public Track(int _value, int _minValue, int _maxValue, HashSet<string> _properties)
        {
            Value = _value;
            minValue = _minValue;
            maxValue = _maxValue;
            properties = new HashSet<string>(_properties);
        }

        public void addToValue(int value)
        {
            Value += value;
            Value = Math.Max(Math.Min(Value, maxValue), minValue);
        }

        public override string ToString()
        {
            string outStr = String.Empty;

            foreach (string str in properties)
            {
                outStr += str + " ";
            }

            outStr += " value=" + Value + " min=" + minValue + " max=" + maxValue;

            return outStr;
        }
    }
}
