using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    [CardTemplateAttribute]
    public class CouncilExovedateCardTemplate : CardTemplate
    {
        public CouncilExovedateCardTemplate()
            : base("Council of the Exovedate", 2, Player.PlayerSide.Defense)
        {}

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true, this.CardInfo.JurySelectionInfos[0].Description), 
                revealAllAspects));

            SelectionEvents.Add(
                new CardEffectPair(genRevealOrPeakCardChoice(new HashSet<Property>(), 1, false, this.CardInfo.JurySelectionInfos[1].Description), 
                peekAllAspects));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, game);
                        findAspectTracksWithProp(game, Property.French)[0].AddToValue(modValue);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, game);
                        findAspectTracksWithProp(game, Property.English)[0].AddToValue(modValue);
                    }));

            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(1, game);
                        findAspectTracksWithProp(game, Property.Merchant, Property.Farmer).ForEach(t => t.AddToValue(modValue));
                    }));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, game);
                        findAspectTracksWithProp(game, Property.French)[0].AddToValue(modValue);
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(2, game);
                        findAspectTracksWithProp(game, Property.English)[0].AddToValue(modValue);
                    }));

            SummationEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    (Game game, BoardChoices choices) =>
                    {
                        int modValue = calcModValueBasedOnSide(1, game);
                        findAspectTracksWithProp(game, Property.Merchant, Property.Farmer).ForEach(t => t.AddToValue(modValue));
                    }));
        }
    }
}
