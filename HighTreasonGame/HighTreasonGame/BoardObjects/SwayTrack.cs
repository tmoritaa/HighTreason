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

        private Jury owner;

        public SwayTrack(int min, int max, Game game, Jury _owner, params Property[] _properties)
            : base(0, min, max, game, _properties)
        {
            owner = _owner;
            Properties.Add(Property.Sway);
        }

        // Copy constructor
        public SwayTrack(SwayTrack track, Jury _owner, Game game)
            : base(track.Value, track.MinValue, track.MaxValue, game, track.Properties.ToArray())
        {
            owner = _owner;
        }

        public override bool CheckCloneEquality(BoardObject track)
        {
            bool equal = base.CheckCloneEquality(track);

            equal &= owner.Id == ((SwayTrack)track).owner.Id;

            return equal;
        }

        public override bool CanModifyByAction(int modValue)
        {
            return !IsLocked && base.CanModifyByAction(modValue);
        }

        public override string ToString()
        {
            return owner + " Sway Track";
        }
    }
}
