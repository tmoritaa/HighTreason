using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class CardTemplateManager
    {
        private static CardTemplateManager instance = null;
        public static CardTemplateManager Instance {
            get {
                if (instance == null)
                {
                    instance = new CardTemplateManager();
                }

                return instance;
            }
        }

        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }
        
        public void Test()
        {
            CardTemplates["John W. Astley"].SummationEvent(0, new TestChoiceHandler());
        }

        private CardTemplateManager()
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates();
        }

        private void generateCardTemplates()
        {
            CardTemplates.Add("John W. Astley", 
                new CardTemplate("John W. Astley",
                2, 
                (int gameId, IChoiceHandler choiceHandler) => {
                    List<HTGameObject> choices = Game.GetGameFromId(gameId).GetHTGOFromCondition((HTGameObject htgo) =>
                    {
                        return (htgo.properties.Contains(Property.Jury) 
                        && htgo.properties.Contains(Property.Religion) 
                        && htgo.properties.Contains(Property.Occupation));
                    });

                    // TODO: implement.
                }, 
                (int gameId, IChoiceHandler choiceHandler) => {
                    Game game = Game.GetGameFromId(gameId);

                    List<HTGameObject> choices = game.GetHTGOFromCondition((HTGameObject htgo) =>
                    {
                        return (htgo.properties.Contains(Property.Track) 
                        && htgo.properties.Contains(Property.Aspect) 
                        && ((Track)htgo).CanIncrease());
                    });

                    AspectTrack track = choiceHandler.ChooseAspectTrack(choices);
                    track.AddToValue(1);

                    List<HTGameObject> guiltTrack = game.GetHTGOFromCondition((HTGameObject htgo) =>
                    {
                        return (htgo.properties.Contains(Property.Guilt));
                    });
                    System.Diagnostics.Debug.Assert(guiltTrack.Count == 1, "Guilt track search failed");
                    ((EvidenceTrack)guiltTrack[0]).AddToValue(1);
                },
                (int gameId, IChoiceHandler choiceHandler) => {
                    List<HTGameObject> choices = Game.GetGameFromId(gameId).GetHTGOFromCondition((HTGameObject htgo) =>
                    {
                        return (htgo.properties.Contains(Property.Track)
                        && htgo.properties.Contains(Property.Aspect)
                        && !htgo.properties.Contains(Property.French)
                        && ((Track)htgo).CanIncrease());
                    });

                    List<AspectTrack> chosenTracks = new List<AspectTrack>();
                    for (int i = 0; i < 3; ++i)
                    {
                        AspectTrack track = choiceHandler.ChooseAspectTrack(choices);
                        chosenTracks.Add(track);
                        choices.Remove(track);
                    }
                    chosenTracks.ForEach(t => t.AddToValue(1));
                }));
        }

    }
}
