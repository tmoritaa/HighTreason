﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame.CardTemplates
{
    public class FatherVitalCardTemplate : CardTemplate
    {
        public FatherVitalCardTemplate(JObject json)
            : base("Father Vital Fourmond", 2, json)
        { }

        protected override void addSelectionEventsAndChoices()
        {
            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>(), 2, true));
            SelectionEvents.Add(revealAllAspects);

            SelectionEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 1, false));
            SelectionEvents.Add(peekAllAspects);
        }

        protected override void addTrialEventsAndChoices()
        {
            TrialEventChoices.Add(doNothingChoice);

            TrialEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetInsanityTrack().AddToValue(1);
                });

            TrialEventChoices.Add(genRevealOrPeakCardChoice(new HashSet<Property>() { Property.Religion }, 3, false));
            TrialEvents.Add(peekAllAspects);
        }

        protected override void addSummationEventsAndChoices()
        {
            SummationEventChoices.Add(doNothingChoice);

            SummationEvents.Add(
                (Game game, BoardChoices choices) =>
                {
                    game.GetInsanityTrack().AddToValue(1);
                });
        }
    }
}
