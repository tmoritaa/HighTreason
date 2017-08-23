using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class AspectTrack : Track
    {
        private const int MAX_TIMES_MODABLE = 3;

        private int timesAffectedByAction = 0;
        public int TimesAffectedByAction
        {
            get; private set;
        }

        public AspectTrack(int _value, Game _game, params Property[] _properties) 
            : base(_value, 1, 10, _game, _properties)
        {
            Properties.Add(Property.Aspect);
        }

        public void ModTrackByAction(int modValue)
        {
            System.Diagnostics.Debug.Assert(timesAffectedByAction < MAX_TIMES_MODABLE, "Sway track of Aspect track should never be full when being modified.");

            timesAffectedByAction += 1;
            AddToValue(modValue);
        }

        public override bool CanModifyByAction(int modValue)
        {
            return (timesAffectedByAction < MAX_TIMES_MODABLE) && base.CanModifyByAction(modValue);
        }

        public override string ToString()
        {
            string outStr = "-" + base.ToString() + "\n";

            outStr += "\t" + "times modified=" + timesAffectedByAction;

            return outStr;
        }

    }
}
