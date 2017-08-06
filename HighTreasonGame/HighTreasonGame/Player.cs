using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Player
    {
        public enum PlayerSide
        {
            Prosecution,
            Defense
        }

        public class CardUsageParams
        {
            public enum UsageType
            {
                Event,
                Action,
            }

            public CardTemplate card;
            public UsageType usage;
            public List<object> misc = new List<object>();
        }

        public PlayerSide Side 
        {
            get; private set;
        }

        private int gameId;
        private IChoiceHandler choiceHandler;

        List<CardTemplate> hand = new List<CardTemplate>();

        List<CardTemplate> cardsForSummation = new List<CardTemplate>();

        public Player(PlayerSide _side, IChoiceHandler _choiceHandler, int _gameId)
        {
            gameId = _gameId;
            Side = _side;
            choiceHandler = _choiceHandler;
        }

        public void SetupHand(List<CardTemplate> _hand)
        {
            hand = _hand;
        }

        public void PlayCard(GameState.StateType curStateType)
        {
            CardUsageParams cardUsage = choiceHandler.ChooseCardAndUsage(hand);

            if (cardUsage.usage == CardUsageParams.UsageType.Event)
            {
                int idx = (int)cardUsage.misc[0];

                CardTemplate.EffectType effectType = Utility.ConvertToEffectTypeFromStateType(curStateType);

                cardUsage.card.PlayAsEvent(effectType, gameId, idx, choiceHandler);
            }
            else if (cardUsage.usage == CardUsageParams.UsageType.Action)
            {
                // TODO: implement.
            }

            discardCard(cardUsage.card);
        }

        public bool MustPass()
        {
            return hand.Count <= 2;
        }

        public void AddHandToSummation()
        {
            cardsForSummation.AddRange(hand);
            hand.Clear();
        }

        private void discardCard(CardTemplate card)
        {
            hand.Remove(card);
            Game.GetGameFromId(gameId).Discards.Add(card);
        }
    }
}
