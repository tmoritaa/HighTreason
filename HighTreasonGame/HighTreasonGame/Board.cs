using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HighTreasonGame
{
    public class Board
    {
        private Game game;

        public List<EvidenceTrack> EvidenceTracks
        {
            get; private set;
        }

        public List<AspectTrack> AspectTracks
        {
            get; private set;
        }

        public List<Jury> Juries
        {
            get; private set;
        }

        public Board(Game _game)
        {
            game = _game;
            Juries = new List<Jury>();
            initTracks();
            initJury();
        }

        // Copy constructor.
        public Board(Board board, Game _game)
        {
            game = _game;

            EvidenceTracks = new List<EvidenceTrack>();
            foreach (EvidenceTrack track in board.EvidenceTracks)
            {
                EvidenceTracks.Add(new EvidenceTrack(track, game));
            }

            AspectTracks = new List<AspectTrack>();
            foreach (AspectTrack track in board.AspectTracks)
            {
                AspectTracks.Add(new AspectTrack(track, game));
            }

            Juries = new List<Jury>();
            foreach (Jury jury in board.Juries)
            {
                Juries.Add(new Jury(jury, game));
            }
        }

        public bool CheckCloneEquality(Board board)
        {
            bool equal = true;

            equal &= !object.ReferenceEquals(this, board);

            if (!equal)
            {
                Console.WriteLine("Board reference test failed");
                return equal;
            }

            for (int i = 0; i < EvidenceTracks.Count; ++i)
            {
                equal &= EvidenceTracks[i].CheckCloneEquality(board.EvidenceTracks[i]);

                if (!equal)
                {
                    Console.WriteLine("Evidence Track " + EvidenceTracks[i] + " equality test failed");
                    return equal;
                }
            }

            for (int i = 0; i < AspectTracks.Count; ++i)
            {
                equal &= AspectTracks[i].CheckCloneEquality(board.AspectTracks[i]);

                if (!equal)
                {
                    Console.WriteLine("Aspect Track " + AspectTracks[i] + " equality test failed");
                    return equal;
                }
            }

            for (int i = 0; i < Juries.Count; ++i)
            {
                equal &= Juries[i].CheckCloneEquality(board.Juries[i]);

                if (!equal)
                {
                    Console.WriteLine("Jury " + Juries[i] + " equality test failed");
                    return equal;
                }
            }

            return equal;
        }

        public void RemoveJury(Jury jury)
        {
            Juries.Remove(jury);
            game.RemoveBoardObject(jury);
        }

        public EvidenceTrack GetInsanityTrack()
        {
            return EvidenceTracks.Find(t => t.Properties.Contains(Property.Insanity));
        }

        public EvidenceTrack GetGuiltTrack()
        {
            return EvidenceTracks.Find(t => t.Properties.Contains(Property.Guilt));
        }

        private void initTracks()
        {
            EvidenceTrack insanityTrack = new EvidenceTrack(game, Property.Insanity);
            EvidenceTrack guiltTrack = new EvidenceTrack(game, Property.Guilt);

            EvidenceTracks = new List<EvidenceTrack> { insanityTrack, guiltTrack };

            AspectTrack protestantTrack = new AspectTrack(5, game, Property.Religion, Property.Protestant);
            AspectTrack catholicTrack = new AspectTrack(3, game, Property.Religion, Property.Catholic);

            AspectTrack englishTrack = new AspectTrack(2, game, Property.Language, Property.English);
            AspectTrack frenchTrack = new AspectTrack(5, game, Property.Language, Property.French);

            AspectTrack farmerTrack = new AspectTrack(4, game, Property.Occupation, Property.Farmer);
            AspectTrack merchantTrack = new AspectTrack(5, game, Property.Occupation, Property.Merchant);
            AspectTrack govWorkerTrack = new AspectTrack(5, game, Property.Occupation, Property.GovWorker);

            AspectTracks = new List<AspectTrack> { protestantTrack, catholicTrack, englishTrack, frenchTrack, farmerTrack, merchantTrack, govWorkerTrack };
        }

        private void initJury()
        {
            // Generate list of markers.
            Dictionary<Property, int> aspectToNumMap = new Dictionary<Property, int>() {
                { Property.Protestant , GameConstants.NUM_PROTESTANT_MARKERS },
                { Property.Catholic, GameConstants.NUM_CATHOLIC_MARKERS },
                { Property.English , GameConstants.NUM_ENGLISH_MARKERS },
                { Property.French , GameConstants.NUM_FRENCH_MARKERS },
                { Property.Farmer , GameConstants.NUM_FARMER_MARKERS },
                { Property.Merchant , GameConstants.NUM_MERCHANT_MARKERS },
                { Property.GovWorker , GameConstants.NUM_GOVWORKER_MARKERS }
            };

            List<Property> religionAspectMarkers = new List<Property>();
            List<Property> languageAspectMarkers = new List<Property>();
            List<Property> occupationAspectMarkers = new List<Property>();
            foreach (Property aspect in new Property[] { Property.Protestant, Property.Catholic })
            {
                for (int i = 0; i < aspectToNumMap[aspect]; ++i)
                {
                    religionAspectMarkers.Add(aspect);
                }
            }
            foreach (Property aspect in new Property[] { Property.English, Property.French })
            {
                for (int i = 0; i < aspectToNumMap[aspect]; ++i)
                {
                    languageAspectMarkers.Add(aspect);
                }
            }
            foreach (Property aspect in new Property[] { Property.Farmer, Property.Merchant, Property.GovWorker })
            {
                for (int i = 0; i < aspectToNumMap[aspect]; ++i)
                {
                    occupationAspectMarkers.Add(aspect);
                }
            }

            // Create juries.
            List<int> jurySwaySpaces = new List<int>() { 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6 }; // Length of action point list will also determine number of juries. Should be 12.
            Debug.Assert(jurySwaySpaces.Count == GameConstants.NUM_TOTAL_JURY, "");

            int id = 0;
            foreach (int swaySpaces in jurySwaySpaces)
            {
                int religionIdx = GlobalRandom.GetRandomNumber(0, religionAspectMarkers.Count);
                int languageIdx = GlobalRandom.GetRandomNumber(0, languageAspectMarkers.Count);
                int occupationIdx = GlobalRandom.GetRandomNumber(0, occupationAspectMarkers.Count);

                Juries.Add(new Jury(id, swaySpaces, swaySpaces - 3, game, religionAspectMarkers[religionIdx], languageAspectMarkers[languageIdx], occupationAspectMarkers[occupationIdx]));

                religionAspectMarkers.RemoveAt(religionIdx);
                languageAspectMarkers.RemoveAt(languageIdx);
                occupationAspectMarkers.RemoveAt(occupationIdx);

                ++id;
            }
        }
    }
}
