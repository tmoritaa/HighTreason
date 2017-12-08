using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Card
    {
        public CardTemplate Template
        {
            get; private set;
        }

        public CardHolder CardHolder
        {
            get; set;
        }

        public bool CanBePlayed
        {
            get
            {
                return CardHolder.Id == CardHolder.HolderId.Hand;
            }
        }

        public bool BeingPlayed = false;

        public bool Revealed = false;

        public Card(CardTemplate _cardTemplate)
        {
            Template = _cardTemplate;
        }

        public bool PlayAsEvent(Game game, Player choosingPlayer, int idx, ChoiceHandler choiceHandler)
        {
            GameState.GameStateType curStateType = game.CurState.StateType;

            CardTemplate.CardChoice cardChoice = null;
            CardTemplate.CardEffect cardEffect = null;

            if (curStateType == GameState.GameStateType.JurySelection)
            {
                cardChoice = Template.SelectionEvents[idx].CardChoice;
                cardEffect = Template.SelectionEvents[idx].CardEffect;
            }
            else if (curStateType == GameState.GameStateType.TrialInChief)
            {
                cardChoice = Template.TrialEvents[idx].CardChoice;
                cardEffect = Template.TrialEvents[idx].CardEffect;
            }
            else if (curStateType == GameState.GameStateType.Summation)
            {
                cardChoice = Template.SummationEvents[idx].CardChoice;
                cardEffect = Template.SummationEvents[idx].CardEffect;
            }

            System.Diagnostics.Debug.Assert(cardChoice != null && cardEffect != null, "Card choice or card effect is null. Should never happen");

            BoardChoices choices = cardChoice(game, choosingPlayer, choiceHandler);

            if (choices.NotCancelled)
            {
                cardEffect(game, choosingPlayer, choices);
            }

            return choices.NotCancelled;
        }

        public bool PlayAsAction(Game game, Player choosingPlayer, ChoiceHandler choiceHandler)
        {
            bool isSummation = game.CurState.StateType == GameState.GameStateType.Summation;

            int modValue = (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
            List<BoardObject> choices = game.FindBO(
                (BoardObject bo) =>
                {
                    return (bo.Properties.Contains(Property.Track) &&
                    ((bo.Properties.Contains(Property.Jury) && bo.Properties.Contains(Property.Sway))
                    || (!isSummation && bo.Properties.Contains(Property.Aspect))))
                    && ((Track)bo).CanModifyByAction(modValue);
                }).ToList();

            int actionPtsForState = isSummation ? 2 : Template.ActionPts;

            BoardChoices boardChoices;
            choiceHandler.ChooseBoardObjects(choices,
                HTUtility.GenActionValidateChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionFilterChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionChoicesCompleteFunc(actionPtsForState, null),
                game,
                choosingPlayer,
                "Select usage for " + actionPtsForState + " action points",
                out boardChoices);

            if (boardChoices.NotCancelled)
            {
                foreach (BoardObject bo in boardChoices.SelectedObjs.Keys)
                {
                    if (bo.GetType() == typeof(AspectTrack))
                    {
                        ((AspectTrack)bo).ModTrackByAction(modValue * boardChoices.SelectedObjs[bo]);
                    }
                    else
                    {
                        ((Track)bo).AddToValue(modValue * boardChoices.SelectedObjs[bo]);
                    }
                }
            }

            return boardChoices.NotCancelled;
        }
    }
}
