using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class DoctorJamesCardTemplate : CardTemplate
    {
        public DoctorJamesCardTemplate(JObject json)
            : base("Doctor James Wallace", 2, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Occupation }, 3, true, this.CardInfo.JurySelectionPairs[0].Description),
                    revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(
                    genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 2, true, this.CardInfo.JurySelectionPairs[1].Description),
                    revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, ChoiceHandler choiceHandler) =>
                    {
                        BoardChoices choices = new BoardChoices();

                        choices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Prosecution }, game, choiceHandler, out choices.MoIInfo);

                        return choices;
                    },
                    (Game game, BoardChoices choices) =>
                    {
                        game.GetGuiltTrack().AddToValue(-1);
                        handleMomentOfInsight(game, choices);
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && (htgo.Properties.Contains(Property.English) || htgo.Properties.Contains(Property.Protestant)));
                            });

                        options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(2));
                    }));
        }
    }
}
