using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Track(int _value, int _minValue, int _maxValue, Game _game, params Property[] _properties)
            : base(_game, _properties)
        {
            properties.Add(Property.Track);

            Value = _value;
            minValue = _minValue;
            maxValue = _maxValue;
        }

        public void AddToValue(int value)
        {
            Value += value;
            Value = Math.Max(Math.Min(Value, maxValue), minValue);
        }

        public bool CanIncrease()
        {
            return Value < maxValue;
        }

        public bool CanDecrease()
        {
            return Value > minValue;
        }

        public override string ToString()
        {
            string outStr = "-";

            foreach (Property str in properties)
            {
                outStr += str + " ";
            }

            outStr += " value=" + Value + " min=" + minValue + " max=" + maxValue;

            return outStr;
        }
    }
}
