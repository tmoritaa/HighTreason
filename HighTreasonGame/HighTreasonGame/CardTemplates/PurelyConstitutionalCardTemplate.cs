using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class PurelyConstitutionalCardTemplate : CardTemplate
    {
        public PurelyConstitutionalCardTemplate(JObject json)
            : base("\"A Purely Constitutional Movement\"", 3, json)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(new CardEffectPair(
                genRevealOrPeakCardChoice(new HashSet<Property>(), 3, true, this.CardInfo.JurySelectionInfos[0].Description,
                    null,
                    (List<BoardObject> remainingChoices, Dictionary<BoardObject, int> selected) =>
                    {
                        List<BoardObject> newChoices = new List<BoardObject>(remainingChoices);
                        foreach (BoardObject obj in selected.Keys)
                        {
                            if (obj.Properties.Contains(Property.Religion))
                            {
                                newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Religion)).ToList();
                            }

                            if (obj.Properties.Contains(Property.Occupation))
                            {
                                newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Occupation)).ToList();
                            }

                            if (obj.Properties.Contains(Property.Language))
                            {
                                newChoices = newChoices.Where(c => !c.Properties.Contains(Property.Language)).ToList();
                            }
                        }

                        return newChoices;
                    }),
                revealAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    (Game game, ChoiceHandler choiceHandler) =>
                    {
                        BoardChoices choices = new BoardChoices();
                        choices.NotCancelled = handleMomentOfInsightChoice(new Player.PlayerSide[] { Player.PlayerSide.Defense }, game, choiceHandler, out choices.MoIInfo);
                        return choices;
                    },
                    (Game game, BoardChoices choices) => 
                    {
                        game.GetInsanityTrack().AddToValue(1);
                        handleMomentOfInsight(game, choices);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) => 
                    {
                        List<BoardObject> options = game.FindBO(
                            (BoardObject htgo) =>
                            {
                                return (htgo.Properties.Contains(Property.Track)
                                && htgo.Properties.Contains(Property.Aspect)
                                && (htgo.Properties.Contains(Property.Farmer) || htgo.Properties.Contains(Property.French))
                                && ((Track)htgo).CanModify(-2));
                            });

                        options.Cast<AspectTrack>().ToList().ForEach(t => t.AddToValue(-2));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    (Game game, ChoiceHandler choiceHandler) =>
                    {
                        BoardChoices choices = new BoardChoices();
                        return choices;
                    },
                    (Game game, BoardChoices choices) => 
                    {
                        game.GetInsanityTrack().AddToValue(1);
                    }));
        }
    }
}
