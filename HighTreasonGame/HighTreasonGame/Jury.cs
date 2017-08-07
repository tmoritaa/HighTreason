using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Jury : HTGameObject
    {
        public class JuryAspect : HTGameObject
        {
            private Dictionary<Player.PlayerSide, bool> seenStatus = new Dictionary<Player.PlayerSide, bool>();

            public Jury Owner
            {
                get; private set;
            }

            public Property Trait
            {
                get; private set;
            }

            public Property Aspect
            {
                get; private set;
            }

            public bool IsFullyRevealed
            {
                get
                {
                    return seenStatus[Player.PlayerSide.Prosecution] && seenStatus[Player.PlayerSide.Defense];
                }
            }
            
            public JuryAspect(Game game, Jury _owner, Property trait, Property aspect) 
                : base(game, trait, aspect)
            {
                Properties.Add(Property.Jury);
                Properties.Add(Property.Aspect);

                Trait = trait;
                Aspect = aspect;

                seenStatus.Add(Player.PlayerSide.Prosecution, false);
                seenStatus.Add(Player.PlayerSide.Defense, false);

                Owner = _owner;
            }

            public void Revealed()
            {
                seenStatus[Player.PlayerSide.Prosecution] = true;
                seenStatus[Player.PlayerSide.Defense] = true;
            }

            public void Peeked(Player.PlayerSide side)
            {
                seenStatus[side] = true;
            }

            public override string ToString()
            {
                string outStr = "Owner Id = " + Owner.Id + "\n";

                foreach (Property str in Properties)
                {
                    outStr += str + " ";
                }

                outStr += "\n";

                foreach (Player.PlayerSide side in seenStatus.Keys)
                {
                    outStr += side + " seen=" + seenStatus[side] + "\n";
                }

                return outStr;
            }
        }

        public int Id
        {
            get; private set;
        }

        private static int nextUniqueId = 0;

        private SwayTrack track;
        private int actionPoints;
        private List<JuryAspect> aspects = new List<JuryAspect>();

        public Jury(int swayMax, int _actionPoints, Game game, Property religionAspect, Property languageAspect, Property occupationAspect)
            : base(game, Property.Jury, Property.Religion, Property.Language, Property.Occupation)
        {
            Id = nextUniqueId;
            ++nextUniqueId;

            actionPoints = _actionPoints;

            track = new SwayTrack(-swayMax, swayMax, game, Property.Jury);

            aspects.Add(new JuryAspect(game, this, Property.Religion, religionAspect));
            aspects.Add(new JuryAspect(game, this, Property.Language, languageAspect));
            aspects.Add(new JuryAspect(game, this, Property.Occupation, occupationAspect));
        }

        public override void RemoveChildrenHTGameObjects()
        {
            game.RemoveHTGameObject(track);

            foreach (JuryAspect aspect in aspects)
            {
                game.RemoveHTGameObject(aspect);
            }
        }

        public override string ToString()
        {
            string outStr = "Id = " + Id + "\n";
            foreach (JuryAspect aspect in aspects)
            {
                outStr += "---------------------------------------\n";
                outStr += aspect;
            }
            outStr += "---------------------------------------\n";

            outStr += track.ToString() + "\n";
            outStr += "Action Points=" + actionPoints + "\n";

            return outStr;
        }
    }
}
