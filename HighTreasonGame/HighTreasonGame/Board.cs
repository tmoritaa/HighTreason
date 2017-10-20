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

        public override string ToString()
        {
            string outStr = String.Empty;

            outStr += "*************************************************\n";
            outStr += "Evidence Tracks:\n";
            foreach (EvidenceTrack track in EvidenceTracks)
            {
                outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                outStr += track.ToString() + "\n";
            }
            outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
            outStr += "*************************************************\n";

            outStr += "*************************************************\n";
            outStr += "Aspect Tracks:\n";
            foreach (AspectTrack track in AspectTracks)
            {
                outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                outStr += track.ToString() + "\n";
            }
            outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
            outStr += "*************************************************\n";

            outStr += "*************************************************\n";
            outStr += "Juries:\n";
            foreach(Jury jury in Juries)
            {
                outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
                outStr += jury.ToString() + "\n";
            }
            outStr += "++++++++++++++++++++++++++++++++++++++++++++++++++++++\n";
            outStr += "*************************************************\n";

            return outStr;
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
