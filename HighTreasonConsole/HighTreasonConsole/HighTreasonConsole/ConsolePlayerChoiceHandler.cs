/*using System;
using System.Collections.Generic;
using System.Linq;

using HighTreasonGame;
using HighTreasonGame.GameStates;

namespace HighTreasonConsole
{
    public class ConsolePlayerChoiceHandler : ChoiceHandler
    {
        public ConsolePlayerChoiceHandler()
            : base(Player.PlayerType.Human)
        {}

        public override void ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage)
        {
            outCardUsage = new Player.CardUsageParams();
            bool inputHandled = false;

            bool actionIsValid = (game.CurState.GetType() != typeof(JurySelectionState));
            while (!inputHandled)
            {
                Console.WriteLine("Current player=" + game.CurPlayer.Side);
                Console.WriteLine("Hand:");
                for (int i = 0; i < cards.Count; ++i)
                {
                    CardTemplate card = cards[i];
                    Console.WriteLine(i + " " + card.Name + " eventNum=" + card.GetNumberOfEventsInState(game.CurState.GetType()));
                }

                if (!actionIsValid)
                {
                    Console.WriteLine("Please choose card and usage => <card idx> <usage idx>");
                }
                else
                {
                    Console.WriteLine("Please choose card and usage => <card idx> <action or event> <usage idx>");
                }

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    bool goBack;
                    if (handleGenericCases(tokens, game, out goBack))
                    {
                        // Do nothing.
                    }
                    else if ((actionIsValid && tokens.Length == 3) || (!actionIsValid && tokens.Length == 2))
                    {
                        int cardIdx = Int32.Parse((tokens[0]));
                        string usage = actionIsValid ? tokens[1] : "event";
                        int usageIdx = actionIsValid ? Int32.Parse(tokens[2]) : Int32.Parse(tokens[1]);

                        if (!(cardIdx < cards.Count 
                            && (usage.Equals("action") 
                            || (usage.Equals("event") && usageIdx < cards[cardIdx].GetNumberOfEventsInState(game.CurState.GetType())))))
                        {
                            throw new Exception();
                        }

                        outCardUsage.card = cards[cardIdx];

                        if (usage == "action")
                        {
                            outCardUsage.usage = Player.CardUsageParams.UsageType.Action;
                        }
                        else if (usage == "event")
                        {
                            outCardUsage.usage = Player.CardUsageParams.UsageType.Event;
                        }
                        
                        outCardUsage.misc.Add(usageIdx);

                        inputHandled = true;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            }

            return;
        }

        public override void ChooseBoardObjects(List<BoardObject> choices, Func<Dictionary<BoardObject, int>, bool> validateChoices, Func<List<BoardObject>, Dictionary<BoardObject, int>, List<BoardObject>> filterChoices, Func<Dictionary<BoardObject, int>, bool> choicesComplete, Game game, out BoardChoices boardChoice)
        {
            boardChoice = new BoardChoices();

            if (choices.Count == 0)
            {
                Console.WriteLine("No choices");
                return;
            }

            Dictionary<BoardObject, int> selected = new Dictionary<BoardObject, int>();
            List<BoardObject> remainingChoices = new List<BoardObject>(choices);

            bool inputComplete = false;
            while (!inputComplete)
            {
                if (remainingChoices.Count <= 0)
                {
                    inputComplete = true;
                    continue;
                }

                Console.WriteLine("Choices:");
                for (int i = 0; i < remainingChoices.Count; ++i)
                {
                    Console.Write(i + " " + remainingChoices[i].ToString());
                }
                
                Console.WriteLine("Please choose object => <idx>");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    {
                        bool goBack;
                        if (handleGenericCases(tokens, game, out goBack))
                        {
                            if (goBack)
                            {
                                boardChoice.NotCancelled = false;
                                return;
                            }
                        }
                    }
                    
                    if (tokens.Length == 1)
                    {
                        int idx = Int32.Parse(tokens[0]);

                        if (idx >= remainingChoices.Count)
                        {
                            throw new Exception();
                        }

                        BoardObject obj = remainingChoices[idx];
                        if (!selected.ContainsKey(obj))
                        {
                            selected[obj] = 0;
                        }
                        selected[obj] += 1;

                        bool valid = validateChoices(selected);

                        if (!valid)
                        {
                            selected[obj] -= 1;
                            if (selected[obj] < 0)
                            {
                                selected.Remove(obj);
                            }

                            throw new Exception();
                        }

                        bool complete = choicesComplete(selected);

                        if (complete)
                        {
                            inputComplete = true;
                            break;
                        }

                        remainingChoices = filterChoices(remainingChoices, selected);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            }

            boardChoice.SelectedObjs = selected;
        }

        public override bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
        {
            outMoIInfo = new BoardChoices.MomentOfInsightInfo();
            bool inputComplete = false;

            while (!inputComplete)
            {
                Console.WriteLine("Current player=" + game.CurPlayer.Side);
                Console.WriteLine("Hand:");
                for (int i = 0; i < game.CurPlayer.Hand.Count; ++i)
                {
                    CardTemplate card = game.CurPlayer.Hand[i];
                    Console.WriteLine(i + " " + card.Name);
                }
                Console.WriteLine("CardsInSummation:");
                List<CardTemplate> allSummationCards = game.CurPlayer.SummationDeck.AllCards;
                for (int i = 0; i < allSummationCards.Count; ++i)
                {
                    CardTemplate card = allSummationCards[i];
                    Console.WriteLine(i + " " + card.Name);
                }

                Console.WriteLine("Please choose how to use moment of insight => <swap or reveal> <hand card idx if swap> <summation card idx if swap>");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    bool goBack;
                    if (handleGenericCases(tokens, game, out goBack))
                    {
                        if (goBack)
                        {
                            return false;
                        }
                    }
                    else if (tokens.Length == 1 || tokens.Length == 3)
                    {
                        string action = tokens[0];

                        if (action != "swap" && action != "reveal")
                        {
                            throw new Exception();
                        }

                        BoardChoices.MomentOfInsightInfo.MomentOfInsightUse moiUse =
                            (action.Equals("swap")) ? BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap : BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Reveal;

                        if (moiUse == BoardChoices.MomentOfInsightInfo.MomentOfInsightUse.Swap)
                        {
                            int handIdx = Int32.Parse(tokens[1]);
                            int summationIdx = Int32.Parse(tokens[2]);

                            if (handIdx >= game.CurPlayer.Hand.Count || summationIdx >= allSummationCards.Count)
                            {
                                throw new Exception();
                            }

                            outMoIInfo.Use = moiUse;
                            outMoIInfo.HandCard = game.CurPlayer.Hand[handIdx];
                            outMoIInfo.SummationCard = allSummationCards[summationIdx];

                            inputComplete = true;
                        }
                        else
                        {
                            outMoIInfo.Use = moiUse;

                            inputComplete = true;
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Console.WriteLine("Invalid input. Try again.");
                }
            }

            return true;
        }

        private bool handleGenericCases(string[] tokens, Game game, out bool goBack)
        {
            goBack = false;
            bool handled = true;

            string command = tokens[0];

            if (command.Equals("help"))
            {
                Console.WriteLine("Available Commands - track, player, jury, discard");
            }
            else if (command.Equals("track"))
            {
                Console.WriteLine("Evidence Tracks:");
                foreach (EvidenceTrack track in game.Board.EvidenceTracks)
                {
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine(track);
                }
                Console.WriteLine("---------------------------------------------------");

                Console.WriteLine("Aspect Tracks:");
                foreach (AspectTrack track in game.Board.AspectTracks)
                {
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine(track);
                }
                Console.WriteLine("---------------------------------------------------");
            }
            else if (command.Equals("player"))
            {
                Console.WriteLine("Current player = " + game.CurPlayer.Side);
                Console.WriteLine("Hand:");
                foreach (CardTemplate card in game.CurPlayer.Hand)
                {
                    Console.WriteLine(card.Name);
                }

                Console.WriteLine("Summation:");
                foreach (CardTemplate card in game.CurPlayer.SummationDeck.HiddenCards)
                {
                    Console.WriteLine(card.Name);
                }
                foreach (CardTemplate card in game.CurPlayer.SummationDeck.RevealedCards)
                {
                    Console.WriteLine(card.Name + " - revealed");
                }

                Console.WriteLine("Opponent Summation:");
                foreach(CardTemplate card in game.GetOtherPlayer().SummationDeck.RevealedCards)
                {
                    Console.WriteLine(card.Name + " - revealed");
                }
            }
            else if (command.Equals("jury"))
            {
                List<Jury> juries = game.Board.Juries;

                Console.WriteLine("Juries:");
                foreach (Jury jury in juries)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine(jury.SwayTrack);
                    Console.WriteLine("Action Points = " + jury.ActionPoints);
                    Console.WriteLine("Aspects:");
                    foreach (Jury.JuryAspect aspect in jury.Aspects)
                    {
                        string outStr = "- " + aspect.Trait;
                        if (aspect.IsVisibleToPlayer(game.CurPlayer.Side))
                        {
                            outStr += " " + aspect.Aspect;
                        }

                        if (aspect.IsRevealed)
                        {
                            outStr += " revealed";
                        }
                        else if (aspect.IsPeeked)
                        {
                            outStr += " peeked";
                        }
                        Console.WriteLine(outStr);
                    }
                }
                Console.WriteLine("---------------------------------------");
            }
            else if (command.Equals("discard"))
            {
                Console.WriteLine("Discards:");
                foreach (CardTemplate card in game.Discards)
                {
                    Console.WriteLine(card.Name);
                }
            }
            else if (command.Equals("back"))
            {
                goBack = true;
            }
            else
            {
                handled = false;
            }

            return handled;
        }
    }
}
*/