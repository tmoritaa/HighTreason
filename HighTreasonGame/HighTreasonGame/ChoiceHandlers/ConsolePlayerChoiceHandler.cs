using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.ChoiceHandlers
{
    public class ConsolePlayerChoiceHandler : IChoiceHandler
    {
        public Player.CardUsageParams ChooseCardAndUsage(List<CardTemplate> cards, Game game)
        {
            Player.CardUsageParams cardUsage = null;
            while (true)
            {
                Console.WriteLine("Current player=" + game.CurPlayer.Side);
                Console.WriteLine("Hand:");
                for (int i = 0; i < cards.Count; ++i)
                {
                    CardTemplate card = cards[i];
                    Console.WriteLine(i + " " + card.Name + " eventNum=" + card.GetNumberOfEventsInState(game.CurState.GetType()));
                }
                Console.WriteLine("Please choose card and usage => <card idx> <action or event> <usage idx>");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (handleGenericCases(tokens, game))
                    {
                        // Do nothing since case already handled.
                    }
                    else if (tokens.Length == 3)
                    {
                        int cardIdx = Int32.Parse((tokens[0]));
                        string usage = tokens[1];
                        int usageIdx = Int32.Parse(tokens[2]);

                        if (!(cardIdx < cards.Count 
                            && (usage.Equals("action") 
                            || (usage.Equals("event") && usageIdx < cards[cardIdx].GetNumberOfEventsInState(game.CurState.GetType())))))
                        {
                            throw new Exception();
                        }

                        cardUsage = new Player.CardUsageParams();
                        cardUsage.card = cards[cardIdx];

                        if (usage == "action")
                        {
                            cardUsage.usage = Player.CardUsageParams.UsageType.Action;
                        }
                        else if (usage == "event")
                        {
                            cardUsage.usage = Player.CardUsageParams.UsageType.Event;
                        }
                        
                        cardUsage.misc.Add(usageIdx);
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

                if (cardUsage != null)
                {
                    break;
                }
            }

            return cardUsage;
        }

        public List<AspectTrack> ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game)
        {
            if (choices.Count == 0)
            {
                Console.WriteLine("No choices");
                return new List<AspectTrack>();
            }

            List<AspectTrack> aspects = new List<AspectTrack>();
            bool inputComplete = false;
            while (!inputComplete)
            {
                Console.WriteLine("Affectable Aspect Tracks:");
                for (int i = 0; i < choices.Count; ++i)
                {
                    AspectTrack track = (AspectTrack)choices[i];

                    Console.Write(i + " ");
                    foreach (Property str in track.Properties)
                    {
                        Console.Write(str + " ");
                    }

                    Console.Write(" value=" + track.Value + " min=" + track.MinValue + " max=" + track.MaxValue + "\n");
                }

                Console.WriteLine("Please choose " + numChoices + " aspects to affect => <aspect idx> ... <aspect idx>");

                aspects.Clear();

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (handleGenericCases(tokens, game))
                    {
                        // Do nothing since already handled.
                    }
                    else if (tokens.Length == numChoices)
                    {
                        foreach (string token in tokens)
                        {
                            int idx = Int32.Parse(token);

                            if (idx >= choices.Count)
                            {
                                throw new Exception();
                            }

                            AspectTrack track = (AspectTrack)choices[idx];
                            if (aspects.Contains(track))
                            {
                                throw new Exception();
                            }
                            aspects.Add(track);
                        }

                        inputComplete = true;
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

            return aspects;
        }

        public List<Jury.JuryAspect> ChooseJuryAspects(List<List<HTGameObject>> choicesList, List<int> numChoicesList, Game game)
        {
            if (choicesList.Count == 0)
            {
                Console.WriteLine("No choices");
                return new List<Jury.JuryAspect>();
            }

            List<Jury.JuryAspect> aspects = new List<Jury.JuryAspect>();

            for (int c = 0; c < choicesList.Count; ++c)
            {
                List<HTGameObject> choices = choicesList[c];
                int numChoices = numChoicesList[c];

                if (choices.Count == 0)
                {
                    continue;
                }

                List<Jury.JuryAspect> chosenAspects = new List<Jury.JuryAspect>();
                bool inputComplete = false;
                while (true)
                {
                    Console.WriteLine("JuryAspects:");
                    for (int i = 0; i < choices.Count; ++i)
                    {
                        Jury.JuryAspect juryAspect = (Jury.JuryAspect)choices[i];
                        Console.WriteLine(i + " JuryId=" + juryAspect.Owner.Id + " Trait=" + juryAspect.Trait);
                    }
                    Console.WriteLine("Please pick " + numChoices + " jury aspects => <idx> ... <idx>");

                    chosenAspects.Clear();

                    string input = Console.ReadLine();
                    string[] tokens = input.Split(' ');

                    try
                    {
                        if (handleGenericCases(tokens, game))
                        {
                            // Do nothing since case already handled.
                        }
                        else if (tokens.Length == numChoices)
                        {
                            foreach (string idxStr in tokens)
                            {
                                int choiceIdx = Int32.Parse(idxStr);

                                if (choiceIdx >= choices.Count || aspects.Contains((Jury.JuryAspect)choices[choiceIdx]))
                                {
                                    throw new Exception();
                                }

                                chosenAspects.Add((Jury.JuryAspect)choices[choiceIdx]);
                            }

                            if (chosenAspects.Count == numChoices)
                            {
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

                    if (inputComplete)
                    {
                        aspects.AddRange(chosenAspects);
                        break;
                    }
                }
            }

            return aspects;
        }

        public Jury ChooseJuryToDismiss(List<Jury> juries, Game game)
        {
            Jury chosenJury = null;
            while (true)
            {
                Console.WriteLine("Juries:");
                foreach (Jury jury in juries)
                {
                    Console.Write(jury);
                }
                Console.WriteLine("Please choose jury to dismiss => <jury id>");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (handleGenericCases(tokens, game))
                    {
                        // Do nothing since case already handled.
                    }
                    else if (tokens.Length == 1)
                    {
                        int juryId = Int32.Parse((tokens[0]));

                        if (!juries.Exists(j => j.Id == juryId))
                        {
                            throw new Exception();
                        }

                        chosenJury = juries.Find(j => j.Id == juryId);
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

                if (chosenJury != null)
                {
                    break;
                }
            }

            return chosenJury;
        }

        private bool handleGenericCases(string[] tokens, Game game)
        {
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
                    Console.WriteLine("---------------------------------------------------\n");
                    Console.WriteLine(track);
                }
                Console.WriteLine("---------------------------------------------------\n");

                Console.WriteLine("Aspect Tracks:");
                foreach (AspectTrack track in game.Board.AspectTracks)
                {
                    Console.WriteLine("---------------------------------------------------\n");
                    Console.WriteLine(track);
                }
                Console.WriteLine("---------------------------------------------------\n");
            }
            else if (command.Equals("player"))
            {
                Console.WriteLine("Current player = " + game.CurPlayer.Side);
                Console.WriteLine("Hand:");
                foreach (CardTemplate card in game.CurPlayer.Hand)
                {
                    Console.WriteLine(card.Name);
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
                        else if (aspect.IsPeaked)
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
            else
            {
                handled = false;
            }

            return handled;
        }

        void IChoiceHandler.ChooseMomentOfInsightUse(Game game, BoardChoices outBoardChoices)
        {
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
                for (int i = 0; i < game.CurPlayer.CardsForSummation.Count; ++i)
                {
                    CardTemplate card = game.CurPlayer.CardsForSummation[i];
                    Console.WriteLine(i + " " + card.Name);
                }

                Console.WriteLine("Please choose how to use moment of insight => <swap or reveal> <hand card idx if swap> <summation card idx if swap>");

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (handleGenericCases(tokens, game))
                    {
                        // Do nothing since case already handled.
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

                            if (handIdx >= game.CurPlayer.Hand.Count || summationIdx >= game.CurPlayer.CardsForSummation.Count)
                            {
                                throw new Exception();
                            }

                            outBoardChoices.MoIInfo.Use = moiUse;
                            outBoardChoices.MoIInfo.HandCard = game.CurPlayer.Hand[handIdx];
                            outBoardChoices.MoIInfo.SummationCard = game.CurPlayer.CardsForSummation[summationIdx];

                            inputComplete = true;
                        }
                        else
                        {
                            outBoardChoices.MoIInfo.Use = moiUse;

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

            return;
        }
    }
}
