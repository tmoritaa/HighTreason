using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Track : HTGameObject
    {
        public int Value {
            get; protected set;
        }

        protected string name;

        protected int minValue = 0;
        protected int maxValue = 0;

        public Track(Game _game, HashSet<string> _properties, int _value, int _minValue, int _maxValue)
            : base(_game, _properties)
        {
            Value = _value;
            minValue = _minValue;
            maxValue = _maxValue;
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
