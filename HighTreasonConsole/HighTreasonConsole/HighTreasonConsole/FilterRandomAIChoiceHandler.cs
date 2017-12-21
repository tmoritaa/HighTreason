using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HighTreasonGame;

namespace HighTreasonConsole
{
    public class FilterRandomAIChoiceHandler : RandomAIChoiceHandler
    {
        public FilterRandomAIChoiceHandler() : base()
        {
        }

        public override void ChoosePlayerAction(List<Card> cards, Game game, Player choosingPlayer, out PlayerActionParams outCardUsage)
        {
            outCardUsage = new PlayerActionParams();

            while (true)
            {
                // Pick card
                Card card = cards[random.Next() % cards.Count];
                outCardUsage.card = card;

                List<CardTemplate.CardEffectPair> pairs = card.Template.SelectionEvents;

                if (game.CurState.StateType == GameState.GameStateType.JurySelection)
                {
                    pairs = card.Template.SelectionEvents;
                }
                else if (game.CurState.StateType == GameState.GameStateType.TrialInChief)
                {
                    pairs = card.Template.TrialEvents;
                }
                else if (game.CurState.StateType == GameState.GameStateType.Summation)
                {
                    pairs = card.Template.SummationEvents;
                }

                List<int> validIndices = new List<int>();
                for (int i = 0; i < pairs.Count; ++i)
                {
                    if (pairs[i].Selectable(game, choosingPlayer))
                    {
                        if (game.CurState.StateType == GameState.GameStateType.TrialInChief
                            && (card.Template.CardInfo.TrialInChiefInfos[i].Type.Equals("neutral")
                                || card.Template.CardInfo.TrialInChiefInfos[i].Type.Equals(choosingPlayer.Side.ToString().ToLower())))
                        {
                            validIndices.Add(i);
                        }
                        else if (game.CurState.StateType == GameState.GameStateType.Summation
                            && (card.Template.CardInfo.SummationInfos[i].Type.Equals("neutral")
                                || card.Template.CardInfo.SummationInfos[i].Type.Equals(choosingPlayer.Side.ToString().ToLower())))
                        {
                            validIndices.Add(i);
                        }
                        else
                        {
                            validIndices.Add(i);
                        }
                    }
                }

                if (game.CurState.StateType != GameState.GameStateType.JurySelection)
                {
                    validIndices.Add(pairs.Count);
                }

                if (validIndices.Count == 0)
                {
                    continue;
                }

                int idx = validIndices[random.Next() % validIndices.Count];
                if (idx == pairs.Count)
                {
                    outCardUsage.usage = PlayerActionParams.UsageType.Action;
                }
                else
                {
                    outCardUsage.usage = PlayerActionParams.UsageType.Event;
                    outCardUsage.eventIdx = idx;
                }

                break;
            }
        }
    }
}
