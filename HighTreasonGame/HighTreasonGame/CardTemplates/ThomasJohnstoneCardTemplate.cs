using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    [CardTemplateAttribute]
    public class ThomasJohnstoneCardTemplate : CardTemplate
    {
        public ThomasJohnstoneCardTemplate()
            : base("Thomas Cooke Johnstone", 2, Player.PlayerSide.Defense, true)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEvents.Add(genAttorneyJurySelectPeekEffectPair(2, 1, 0));
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEvents.Add(
                new CardEffectPair(
                    doNothingChoice,
                    doNothingEffect,
                    (Game game, Player choosingPlayer) => { return false; }));

            TrialEvents.Add(genAttorneyTrialAspectClearEffectPair(2, 1));

            TrialEvents.Add(genAttorneyTrialAddSwayEffectPair(2));
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEvents.Add(
                new CardEffectPair(doNothingChoice, doNothingEffect, (Game game, Player choosingPlayer) => { return false; }));

            SummationEvents.Add(genAttorneySummationAddSwayEffectPair(1));

            SummationEvents.Add(genAttorneySummationClearSwayEffectPair(2));
        }
    }
}
