using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public enum Property
    {
        Religion,
        Language,
        Occupation,
        Protestant,
        Catholic,
        English,
        French,
        Farmer,
        Merchant,
        GovWorker,
        Insanity,
        Guilt,
        Sway,
        Jury,
        Aspect,
        Evidence,
        Track,
    }

    public enum PlayerSide
    {
        Prosecution,
        Defense
    }

    public static class GameConstants
    {
        #region Number Constants
        public static int NUM_PROTESTANT_MARKERS = 11;
        public static int NUM_CATHOLIC_MARKERS = 7;
        public static int NUM_RELIGION_MARKERS = NUM_PROTESTANT_MARKERS + NUM_CATHOLIC_MARKERS;

        public static int NUM_ENGLISH_MARKERS = 13;
        public static int NUM_FRENCH_MARKERS = 5;
        public static int NUM_LANGUAGE_MARKERS = NUM_ENGLISH_MARKERS + NUM_FRENCH_MARKERS;

        public static int NUM_FARMER_MARKERS = 9;
        public static int NUM_MERCHANT_MARKERS = 6;
        public static int NUM_GOVWORKER_MARKERS = 3;
        public static int NUM_OCCUPATION_MARKERS = NUM_FARMER_MARKERS + NUM_MERCHANT_MARKERS + NUM_GOVWORKER_MARKERS;

        public static int NUM_JURY = 12;
        #endregion
    }
}
