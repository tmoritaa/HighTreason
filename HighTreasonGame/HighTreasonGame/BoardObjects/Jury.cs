using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Jury : BoardObject
    {
        public class JuryAspect : BoardObject
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

            public bool IsRevealed
            {
                get
                {
                    return seenStatus[Player.PlayerSide.Prosecution] && seenStatus[Player.PlayerSide.Defense];
                }
            }

            public bool IsPeeked
            {
                get
                {
                    return seenStatus[Player.PlayerSide.Prosecution] ^ seenStatus[Player.PlayerSide.Defense];
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

            public void Reveal()
            {
                seenStatus[Player.PlayerSide.Prosecution] = true;
                seenStatus[Player.PlayerSide.Defense] = true;
            }

            public void Peek(Player.PlayerSide side)
            {
                seenStatus[side] = true;
            }

            public bool IsVisibleToPlayer(Player.PlayerSide side)
            {
                return seenStatus[side];
            }

            public int CalculateGuiltScore(Game game)
            {
                AspectTrack track = (AspectTrack)game.FindBO(
                    (BoardObject htgo) =>
                    {
                        return (htgo.Properties.Contains(Property.Track) && htgo.Properties.Contains(Property.Aspect)
                        && htgo.Properties.Contains(Trait) && htgo.Properties.Contains(Aspect));
                    })[0];

                return track.Value;
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

        public SwayTrack SwayTrack
        {
            get; private set;
        }

        public int ActionPoints
        {
            get; private set;
        }

        public List<JuryAspect> Aspects
        {
            get; private set;
        }

        public Jury(int id, int swayMax, int _actionPoints, Game game, Property religionAspect, Property languageAspect, Property occupationAspect)
            : base(game, Property.Jury, Property.Religion, Property.Language, Property.Occupation)
        {
            Id = id;

            ActionPoints = _actionPoints;

            SwayTrack = new SwayTrack(-swayMax, swayMax, game, Property.Jury, Property.Religion, Property.Language, Property.Occupation, religionAspect, languageAspect, occupationAspect);

            Aspects = new List<JuryAspect>();
            Aspects.Add(new JuryAspect(game, this, Property.Religion, religionAspect));
            Aspects.Add(new JuryAspect(game, this, Property.Language, languageAspect));
            Aspects.Add(new JuryAspect(game, this, Property.Occupation, occupationAspect));
        }

        public void RevealAllTraits()
        {
            Aspects.ForEach(a => a.Reveal());
        }

        public int CalculateGuiltScore()
        {
            int guiltScore = 0;

            foreach (JuryAspect aspect in Aspects)
            {
                guiltScore += aspect.CalculateGuiltScore(game);
            }

            if (SwayTrack.IsLockedByProsecution)
            {
                guiltScore *= 2;
            }
            else if (SwayTrack.IsLockedByDefense)
            {
                guiltScore /= 2;
            }
            
            guiltScore += SwayTrack.Value;

            return guiltScore;
        }

        public override void RemoveChildrenBoardObjects()
        {
            game.RemoveBoardObject(SwayTrack);

            foreach (JuryAspect aspect in Aspects)
            {
                game.RemoveBoardObject(aspect);
            }
        }

        public override string ToString()
        {
            string outStr = "Id = " + Id + "\n";
            outStr += SwayTrack.ToString() + "\n";
            outStr += "Action Points=" + ActionPoints + "\n";
            foreach (JuryAspect aspect in Aspects)
            {
                outStr += "---------------------------------------\n";
                outStr += aspect;
            }
            outStr += "---------------------------------------\n";

            return outStr;
        }
    }
}
