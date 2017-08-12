using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighTreasonGame.ChoiceHandlers
{
    public class ConsolePlayerChoiceHandler : IChoiceHandler
    {
        public bool ChooseCardAndUsage(List<CardTemplate> cards, Game game, out Player.CardUsageParams outCardUsage)
        {
            outCardUsage = new Player.CardUsageParams();
            bool inputHandled = false;
            while (!inputHandled)
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
                    bool goBack;
                    if (handleGenericCases(tokens, game, out goBack))
                    {
                        if (goBack)
                        {
                            return false;
                        }
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

            return true;
        }

        public bool ChooseAspectTracks(List<HTGameObject> choices, int numChoices, Game game, out List<AspectTrack> outAspectTracks)
        {
            outAspectTracks = new List<AspectTrack>();

            if (choices.Count == 0)
            {
                Console.WriteLine("No choices");
                return true;
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

                int expectedTokenLength = Math.Min(numChoices, choices.Count);

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
                    else if (tokens.Length == expectedTokenLength)
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

            outAspectTracks = aspects;

            return true;
        }

        public bool ChooseJuryAspects(List<List<HTGameObject>> choicesList, List<int> numChoicesList, Game game, out List<Jury.JuryAspect> outJuryAspects)
        {
            outJuryAspects = new List<Jury.JuryAspect>();

            if (choicesList.Count == 0)
            {
                Console.WriteLine("No choices");
                return true;
            }

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

                    int numValidChoices = Math.Min(numChoices, choices.Count);

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
                        else if (tokens.Length == numValidChoices)
                        {
                            foreach (string idxStr in tokens)
                            {
                                int choiceIdx = Int32.Parse(idxStr);

                                if (choiceIdx >= choices.Count || chosenAspects.Contains((Jury.JuryAspect)choices[choiceIdx]))
                                {
                                    throw new Exception();
                                }

                                chosenAspects.Add((Jury.JuryAspect)choices[choiceIdx]);
                            }

                            if (chosenAspects.Count == numValidChoices)
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
                        outJuryAspects.AddRange(chosenAspects);
                        break;
                    }
                }
            }

            return true;
        }

        public bool ChooseJuryToDismiss(List<Jury> juries, Game game, out List<Jury> outJury)
        {
            outJury = new List<Jury>();
            bool inputHandled = false;
            while (!inputHandled)
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
                    bool goBack;
                    if (handleGenericCases(tokens, game, out goBack))
                    {
                        if (goBack)
                        {
                            return false;
                        }
                    }
                    else if (tokens.Length == 1)
                    {
                        int juryId = Int32.Parse((tokens[0]));

                        if (!juries.Exists(j => j.Id == juryId))
                        {
                            throw new Exception();
                        }

                        outJury.Add(juries.Find(j => j.Id == juryId));
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

            return true;
        }

        public bool ChooseMomentOfInsightUse(Game game, out BoardChoices.MomentOfInsightInfo outMoIInfo)
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
