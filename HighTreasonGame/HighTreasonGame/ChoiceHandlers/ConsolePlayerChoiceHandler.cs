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
            Console.WriteLine("Current player=" + game.CurPlayer.Side);
            Console.WriteLine("Hand:");
            for (int i = 0; i < cards.Count; ++i)
            {
                CardTemplate card = cards[i];
                Console.WriteLine(i + " " + card.Name + " eventNum=" + card.GetNumberOfEventsInState(game.CurState.GetType()));
            }

            Console.WriteLine("Please choose card and usage => <card idx> <action or event> <usage idx>");

            Player.CardUsageParams cardUsage = null;
            while (true)
            {
                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (tokens.Length == 3)
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

            List<AspectTrack> aspects = new List<AspectTrack>();
            bool inputComplete = false;
            while (!inputComplete)
            {
                aspects.Clear();

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (tokens.Length == numChoices)
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

                if (inputComplete)
                {
                    break;
                }
            }

            return aspects;
        }

        public List<Jury.JuryAspect> ChooseJuryAspects(List<HTGameObject> choices, int numChoices, Game game)
        {
            if (choices.Count == 0)
            {
                Console.WriteLine("No choices");
                return new List<Jury.JuryAspect>();
            }

            Console.WriteLine("JuryAspects:");

            foreach (HTGameObject htgo in choices)
            {
                Jury.JuryAspect juryAspect = (Jury.JuryAspect)htgo;
                Console.WriteLine("JuryId=" + juryAspect.Owner.Id + " Trait=" + juryAspect.Trait);
            }

            Console.WriteLine("Please pick " + numChoices + " jury aspects => <jury id> <religion or language or occupation> ... <jury id> <religion or language or occupation>");

            List<Jury.JuryAspect> aspects = new List<Jury.JuryAspect>();

            bool inputComplete = false;
            while (!inputComplete)
            {
                aspects.Clear();

                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (tokens.Length == numChoices * 2)
                    {
                        for (int i = 0; i < tokens.Length; i += 2)
                        {
                            int juryId = Int32.Parse(tokens[i]);
                            string aspectType = tokens[i + 1];

                            if (choices.Exists(c => ((Jury.JuryAspect)c).Owner.Id == juryId) 
                                && (aspectType.Equals("religion") || aspectType.Equals("language") || aspectType.Equals("occupation")))
                            {
                                Property property = Property.Religion;
                                if (aspectType.Equals("religion"))
                                {
                                    property = Property.Religion;
                                }
                                else if (aspectType.Equals("language"))
                                {
                                    property = Property.Language;
                                }
                                else if (aspectType.Equals("occupation"))
                                {
                                    property = Property.Occupation;
                                }

                                List<HTGameObject> chosen = choices.FindAll(c => ((Jury.JuryAspect)c).Owner.Id == juryId && c.Properties.Contains(property));

                                if (chosen.Count == 0 || aspects.Contains((Jury.JuryAspect)chosen[0]))
                                {
                                    throw new Exception();
                                }

                                System.Diagnostics.Debug.Assert(chosen.Count == 1, "Jury Aspect chosen from ConsolePlayerChoiceHandler has more than 1 valid aspect.");

                                aspects.Add((Jury.JuryAspect)chosen[0]);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }

                        if (aspects.Count == numChoices)
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
                    break;
                }
            }

            return aspects;
        }

        public Jury ChooseJuryToDismiss(List<Jury> juries, Game game)
        {
            Console.WriteLine("Juries:");
            foreach (Jury jury in juries)
            {
                Console.Write(jury);
            }

            Console.WriteLine("Please choose jury to dismiss => <jury id>");

            Jury chosenJury = null;
            while (true)
            {
                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                try
                {
                    if (tokens.Length == 1)
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

        public string ChooseMomentOfInsightUse(Game game)
        {
            return string.Empty;
        }
    }
}
