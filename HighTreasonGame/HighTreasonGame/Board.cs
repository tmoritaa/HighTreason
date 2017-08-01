using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public class Board
    {
        private int gameId;

        private List<EvidenceTrack> evidenceTracks = new List<EvidenceTrack>();

        private List<AspectTrack> aspectTracks = new List<AspectTrack>();

        private List<Jury> juries = new List<Jury>();

        public Board(int _gameId)
        {
            gameId = _gameId;
            initTracks();
            initJury();
        }

        public override string ToString()
        {
            string printStr = String.Empty;

            List<Track> tracks = new List<Track>();
            tracks.AddRange(evidenceTracks);
            tracks.AddRange(aspectTracks);

            printStr += "======================================================\n";
            printStr += "Tracks:\n";
            foreach (Track track in tracks)
            {
                printStr += track.ToString() + "\n";
            }
            printStr += "======================================================\n";

            printStr += "Juries:\n";
            foreach(Jury jury in juries)
            {
                printStr += jury.ToString() + "\n";
                printStr += "----------------------------------------------------\n";
            }
            printStr += "======================================================\n";

            return printStr;
        }

        private void initTracks()
        {
            EvidenceTrack insanityTrack = new EvidenceTrack(gameId, Property.Insanity);
            EvidenceTrack guiltTrack = new EvidenceTrack(gameId, Property.Guilt);

            evidenceTracks = new List<EvidenceTrack> { insanityTrack, guiltTrack };

            AspectTrack protestantTrack = new AspectTrack(5, gameId, Property.Religion, Property.Protestant);
            AspectTrack catholicTrack = new AspectTrack(3, gameId, Property.Religion, Property.Catholic);

            AspectTrack englishTrack = new AspectTrack(2, gameId, Property.Language, Property.English);
            AspectTrack frenchTrack = new AspectTrack(5, gameId, Property.Language, Property.French);

            AspectTrack farmerTrack = new AspectTrack(4, gameId, Property.Occupation, Property.Farmer);
            AspectTrack merchantTrack = new AspectTrack(5, gameId, Property.Occupation, Property.Merchant);
            AspectTrack govWorkerTrack = new AspectTrack(5, gameId, Property.Occupation, Property.GovWorker);

            aspectTracks = new List<AspectTrack> { protestantTrack, catholicTrack, englishTrack, frenchTrack, farmerTrack, merchantTrack, govWorkerTrack };
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
            Random rand = new Random();
            List<int> jurySwaySpaces = new List<int>() { 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6 }; // Length of action point list will also determine number of juries. Should be 12.
            Debug.Assert(jurySwaySpaces.Count == GameConstants.NUM_JURY, "");
            foreach (int swaySpaces in jurySwaySpaces)
            {
                int religionIdx = rand.Next(0, religionAspectMarkers.Count);
                int languageIdx = rand.Next(0, languageAspectMarkers.Count);
                int occupationIdx = rand.Next(0, occupationAspectMarkers.Count);

                juries.Add(new Jury(swaySpaces, swaySpaces - 3, gameId, religionAspectMarkers[religionIdx], languageAspectMarkers[languageIdx], occupationAspectMarkers[occupationIdx]));

                religionAspectMarkers.RemoveAt(religionIdx);
                languageAspectMarkers.RemoveAt(languageIdx);
                occupationAspectMarkers.RemoveAt(occupationIdx);
            }
        }
    }
}
