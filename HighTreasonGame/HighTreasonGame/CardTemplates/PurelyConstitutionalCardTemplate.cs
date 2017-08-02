using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame.CardTemplates
{
    public class PurelyConstitutionalCardTemplate : CardTemplate
    {
        public PurelyConstitutionalCardTemplate()
            : base("\"A Purely Constitutional Movement\"", 3)
        {}

        protected override void addSelectionEvents()
        {
            SelectionEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {



                });
        }

        protected override void addTrialEvents()
        {
            TrialEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    handleInsanity(game, 1);
                    handleMomentOfInsight();
                });

            TrialEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    handleAspectTrackChange(game, choiceHandler, 0, -2,
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.Farmer)
                            && ((Track)htgo).CanIncrease());
                        });

                    handleAspectTrackChange(game, choiceHandler, 0, -2,
                        (HTGameObject htgo) => 
                        {
                            return (htgo.properties.Contains(Property.Track)
                            && htgo.properties.Contains(Property.Aspect)
                            && htgo.properties.Contains(Property.French)
                            && ((Track)htgo).CanIncrease());
                        });
                });
        }

        protected override void addSummationEvents()
        {
            SummationEvents.Add(
                (int gameId, IChoiceHandler choiceHandler) => 
                {
                    Game game = Game.GetGameFromId(gameId);
                    handleInsanity(game, 1);
                });
        }
    }
}
