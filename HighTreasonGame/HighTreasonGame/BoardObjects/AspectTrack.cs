using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class AspectTrack : Track
    {
        private const int MAX_TIMES_MODABLE = 3;

        public int TimesAffectedByAction
        {
            get; private set;
        }

        private Property[] uniqueProps;

        public AspectTrack(int _value, Game _game, params Property[] _properties) 
            : base(_value, 1, 10, _game, _properties)
        {
            uniqueProps = _properties;
            Properties.Add(Property.Aspect);
            TimesAffectedByAction = 0;
        }

        public void ModTrackByAction(int modValue)
        {
            System.Diagnostics.Debug.Assert(TimesAffectedByAction < MAX_TIMES_MODABLE, "Sway track of Aspect track should never be full when being modified.");

            TimesAffectedByAction += 1;
            AddToValue(modValue);
        }

        public override bool CanModifyByAction(int modValue)
        {
            return (TimesAffectedByAction < MAX_TIMES_MODABLE) && base.CanModifyByAction(modValue);
        }

        public void ResetTimesAffected()
        {
            TimesAffectedByAction = 0;
        }

        public override string ToString()
        {
            string outStr = "";

            foreach (Property prop in uniqueProps)
            {
                outStr += prop + " ";
            }
            outStr += "Aspect Track";

            return outStr;
        }

    }
}
