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
        private Game game;

        private List<EvidenceTrack> evidenceTracks = new List<EvidenceTrack>();

        private List<AspectTrack> aspectTracks = new List<AspectTrack>();

        private List<Jury> juries = new List<Jury>();

        public Board(Game game)
        {
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
            EvidenceTrack insanityTrack = new EvidenceTrack(game, new HashSet<string>() { GameConstants.PROP_INSANITY });
            EvidenceTrack guiltTrack = new EvidenceTrack(game, new HashSet<string>() { GameConstants.PROP_GUILT });

            evidenceTracks = new List<EvidenceTrack> { insanityTrack, guiltTrack };

            AspectTrack protestantTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_RELIGION, GameConstants.PROP_PROTESTANT }, 5);
            AspectTrack catholicTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_RELIGION, GameConstants.PROP_CATHOLIC }, 3);

            AspectTrack englishTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_LANGUAGE, GameConstants.PROP_ENGLISH }, 2);
            AspectTrack frenchTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_LANGUAGE, GameConstants.PROP_FRENCH }, 5);

            AspectTrack farmerTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_OCCUPATION, GameConstants.PROP_FARMER }, 4);
            AspectTrack merchantTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_OCCUPATION, GameConstants.PROP_MERCHANT }, 5);
            AspectTrack govWorkerTrack = new AspectTrack(game, new HashSet<string>() { GameConstants.PROP_OCCUPATION, GameConstants.PROP_GOVWORKER }, 5);

            aspectTracks = new List<AspectTrack> { protestantTrack, catholicTrack, englishTrack, frenchTrack, farmerTrack, merchantTrack, govWorkerTrack };
        }

        private void initJury()
        {
            // Generate list of markers.
            Dictionary<string, int> aspectToNumMap = new Dictionary<string, int>() {
                { GameConstants.PROP_PROTESTANT , GameConstants.NUM_PROTESTANT_MARKERS },
                { GameConstants.PROP_CATHOLIC, GameConstants.NUM_CATHOLIC_MARKERS },
                { GameConstants.PROP_ENGLISH , GameConstants.NUM_ENGLISH_MARKERS },
                { GameConstants.PROP_FRENCH , GameConstants.NUM_FRENCH_MARKERS },
                { GameConstants.PROP_FARMER , GameConstants.NUM_FARMER_MARKERS },
                { GameConstants.PROP_MERCHANT , GameConstants.NUM_MERCHANT_MARKERS },
                { GameConstants.PROP_GOVWORKER , GameConstants.NUM_GOVWORKER_MARKERS }
            };

            List<string> religionAspectMarkers = new List<string>();
            List<string> languageAspectMarkers = new List<string>();
            List<string> occupationAspectMarkers = new List<string>();
            foreach (string aspect in new string[] { GameConstants.PROP_PROTESTANT, GameConstants.PROP_CATHOLIC })
            {
                for (int i = 0; i < aspectToNumMap[aspect]; ++i)
                {
                    religionAspectMarkers.Add(aspect);
                }
            }
            foreach (string aspect in new string[] { GameConstants.PROP_ENGLISH, GameConstants.PROP_FRENCH })
            {
                for (int i = 0; i < aspectToNumMap[aspect]; ++i)
                {
                    languageAspectMarkers.Add(aspect);
                }
            }
            foreach (string aspect in new string[] { GameConstants.PROP_FARMER, GameConstants.PROP_MERCHANT, GameConstants.PROP_GOVWORKER })
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

                juries.Add(new Jury(game, swaySpaces, swaySpaces - 3, religionAspectMarkers[religionIdx], languageAspectMarkers[languageIdx], occupationAspectMarkers[occupationIdx]));

                religionAspectMarkers.RemoveAt(religionIdx);
                languageAspectMarkers.RemoveAt(languageIdx);
                occupationAspectMarkers.RemoveAt(occupationIdx);
            }
        }
    }
}
