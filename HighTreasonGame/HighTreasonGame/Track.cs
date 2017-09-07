using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Track : BoardObject
    {
        public int Value {
            get; protected set;
        }

        public int MinValue
        {
            get; protected set;
        }

        public int MaxValue
        {
            get; protected set;
        }

        public Track(int _value, int _minValue, int _maxValue, Game _game, params Property[] _properties)
            : base(_game, _properties)
        {
            Properties.Add(Property.Track);

            Value = _value;
            MinValue = _minValue;
            MaxValue = _maxValue;
        }

        public void AddToValue(int value)
        {
            Value += value;
            Value = Math.Max(Math.Min(Value, MaxValue), MinValue);
        }

        public bool CanModify(int modValue)
        {
            return (Math.Sign(modValue) > 0) ? Value < MaxValue : Value > MinValue;
        }

        public virtual bool CanModifyByAction(int modValue)
        {
            return CanModify(modValue);
        }

        public override string ToString()
        {
            string outStr = "-";

            foreach (Property str in Properties)
            {
                outStr += str + " ";
            }

            outStr += " value=" + Value + " min=" + MinValue + " max=" + MaxValue;

            return outStr;
        }
    }
}
