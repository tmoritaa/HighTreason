using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class AspectTrack : Track
    {
        private const int MAX_TIMES_MODABLE = 3;

        private SwayTrack swayTrack;

        public AspectTrack(int _value, Game _game, params Property[] _properties) 
            : base(_value, 1, 10, _game, _properties)
        {
            Properties.Add(Property.Aspect);
            swayTrack = new SwayTrack(0, 3, _game, _properties);
        }

        public void ModTrackByAction(int modValue)
        {
            System.Diagnostics.Debug.Assert(swayTrack.Value != swayTrack.MaxValue, "Sway track of Aspect track should never be full when being modified.");

            swayTrack.AddToValue(1);
            AddToValue(modValue);
        }

        public override bool CanModifyByAction(int modValue)
        {
            return (swayTrack.Value != swayTrack.MaxValue) && base.CanModifyByAction(modValue);
        }

        public override void RemoveChildrenHTGameObjects()
        {
            game.RemoveHTGameObject(swayTrack);
        }

        public override string ToString()
        {
            string outStr = "-" + base.ToString() + "\n";

            outStr += "\t" + swayTrack.ToString();

            return outStr;
        }

    }
}
