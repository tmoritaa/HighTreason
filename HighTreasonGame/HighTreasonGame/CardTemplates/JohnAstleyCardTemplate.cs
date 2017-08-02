using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.CardTemplates
{
    public class JohnAstleyCardTemplate : CardTemplate
    {
        public JohnAstleyCardTemplate() 
            : base("John W. Astley", 2)
        {}

        protected override void addSelectionEvents()
        {
            SelectionEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    List<HTGameObject> choices = Game.GetGameFromId(gameId).GetHTGOFromCondition(
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Religion));
                        });

                    // TODO: implement.
                });

            SelectionEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    List<HTGameObject> choices = Game.GetGameFromId(gameId).GetHTGOFromCondition(
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Jury)
                            && htgo.properties.Contains(Property.Occupation));
                        });

                    // TODO: implement.
                });
        }

        protected override void addTrialEvents()
        {
            TrialEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    
                    handleAspectTrackChange(game, choiceHandler, 1, 1,
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && ((Track)htgo).CanIncrease());
                        });

                    handleGuilt(game, 1);
                });
        }

        protected override void addSummationEvents()
        {
            SummationEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    handleAspectTrackChange(game, choiceHandler, 3, 1,
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && !htgo.properties.Contains(Property.French)
                            && ((Track)htgo).CanIncrease());
                        });
                });
        }
    }
}
