using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class SwayTrack : Track
    {
        public bool IsLocked
        {
            get
            {
                return IsLockedByProsecution || IsLockedByDefense;
            }
        }

        public bool IsLockedByProsecution
        {
            get
            {
                return Value == MaxValue;
            }
        }

        public bool IsLockedByDefense
        {
            get
            {
                return Value == MinValue;
            }
        }

        public SwayTrack(int min, int max, Game game, params Property[] _properties)
            : base(0, min, max, game, _properties)
        {
            Properties.Add(Property.Sway);
        }

        public override bool CanModifyByAction(int modValue)
        {
            return !IsLocked && base.CanModifyByAction(modValue);
        }
    }
}
