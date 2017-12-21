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

        public override bool CheckCloneEquality(BoardObject bo)
        {
            bool equal = base.CheckCloneEquality(bo);

            Track track = (Track)bo;

            equal &= Value == track.Value;
            equal &= MaxValue == track.MaxValue;
            equal &= MinValue == track.MinValue;            

            return equal;
        }

        public void AddToValue(int value)
        {
            Value += value;
            Value = Math.Max(Math.Min(Value, MaxValue), MinValue);
        }

        public void ResetValue()
        {
            Value = 0;
        }

        public bool CanModify(int modValue)
        {
            return (Math.Sign(modValue) > 0) ? Value < MaxValue : Value > MinValue;
        }

        public virtual bool CanModifyByAction(int modValue)
        {
            return CanModify(modValue);
        }
    }
}
