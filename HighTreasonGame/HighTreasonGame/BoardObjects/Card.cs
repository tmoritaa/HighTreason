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

        // Copy constructor
        public Card(Card card, CardHolder holder)
        {
            Template = card.Template;
            CardHolder = holder;
            
            BeingPlayed = card.BeingPlayed;
            Revealed = card.Revealed;
        }

        public bool CheckCloneEquality(Card card)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, card);
            if (!equal)
            {
                Console.WriteLine("Card references test failed");
                return equal;
            }

            equal &= this.Template == card.Template;
            if (!equal)
            {
                Console.WriteLine("Card Template was not equal");
                return equal;
            }

            equal &= CardHolder.Id == card.CardHolder.Id;
            if (!equal)
            {
                Console.WriteLine("Card Cardholder Id was not equal");
                return equal;
            }

            equal &= BeingPlayed == card.BeingPlayed;
            equal &= Revealed == card.Revealed;
            if (!equal)
            {
                Console.WriteLine("Card Vars was not equal");
                return equal;
            }

            return equal;
        }

        public CardTemplate.CardEffectPair GetEventEffectPair(Game game, int idx)
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

            return new CardTemplate.CardEffectPair(cardChoice, cardEffect);
        }

        public Action PerformActionChoice(Game game, Player choosingPlayer)
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

            return new Action(
                ChoiceHandler.ChoiceType.BoardObjects,
                choosingPlayer.ChoiceHandler,
                choices,
                HTUtility.GenActionValidateChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionFilterChoicesFunc(actionPtsForState, null),
                HTUtility.GenActionChoicesCompleteFunc(actionPtsForState, null),
                game,
                choosingPlayer,
                "Select usage for " + actionPtsForState + " action points");
        }

        public void PerformAction(Player choosingPlayer, BoardChoices boardChoices)
        {
            int modValue = (choosingPlayer.Side == Player.PlayerSide.Prosecution) ? 1 : -1;
            string str = "";

            foreach (BoardObject bo in boardChoices.SelectedObjs.Keys)
            {
                str += bo + " modified by " + modValue * boardChoices.SelectedObjs[bo] + "\n";
                if (bo.GetType() == typeof(AspectTrack))
                {
                    ((AspectTrack)bo).ModTrackByAction(modValue * boardChoices.SelectedObjs[bo]);
                }
                else
                {
                    ((Track)bo).AddToValue(modValue * boardChoices.SelectedObjs[bo]);
                }
            }
            FileLogger.Instance.Log(str);
        }

        public override string ToString()
        {
            return Template.Name;
        }
    }
}
