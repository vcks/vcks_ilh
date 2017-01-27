using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vcks_ilh
{
    public static class Music
    {
        public static uint[] possibleNotesRange = { 61, 87 };
        public static uint[] notesWithAccidentals = { 61, 63, 66, 68, 70, 73, 75, 78, 80, 82, 85, 87 };
        public enum Accidentals {none, flat, sharp, natural }; // Знаки альтерации
        public static class Durations
        {
            public const double WHOLE = 4, HALF = 2, QUARTER = 1, EIGHT = 0.5, SIXTEENTH = 0.25;
        }

        public static class TimeSignatures
        {
            public const double twoOf4 = 2, threeOf4 = 3, fourOf4 = 4, fiveOf8 = 2.5, sevenOf8 = 3.5;
        }

        public static Dictionary<double, string[]> TimeSignaturesForDisplay = new Dictionary<double, string[]>()  {
            {2, new string[] {"2","4"} },
            {3, new string[] {"3","4"} },
            {4, new string[] {"4","4"} },
            {2.5, new string[] {"5","8"} },
            {3.5, new string[] {"7","8"} }
        };

        public static Dictionary<double, string> NotesForDisplay = new Dictionary<double, string>()  {
            {Durations.WHOLE, Symbols.WHOLE_NOTE},
            {Durations.HALF, Symbols.HALF_NOTE},
            { Durations.QUARTER, Symbols.QUARTER_NOTE},
            {Durations.EIGHT, Symbols.EIGHT_NOTE},
        };

        public static Dictionary<int, string> AccidentalsForDisplay = new Dictionary<int, string>()  {
            {0, string.Empty},
            {1, Symbols.FLAT_SIGN},
            {2, Symbols.SHARP_SIGN},
            {3, Symbols.NATURAL_SIGN},
        };

        public static Dictionary<double, string> RestsForDisplay = new Dictionary<double, string>()  {
            {Durations.WHOLE, Symbols.WHOLE_REST},
            {Durations.HALF, Symbols.HALF_REST},
            { Durations.QUARTER, Symbols.QUARTER_REST},
            {Durations.EIGHT, Symbols.EIGHT_REST},
            {Durations.SIXTEENTH, Symbols.SIXTEENTH_REST},
        };        

        public static uint[] flatKeys = { 83, 88, 81, 86, 79, 84, 77 };
        public static uint[] sharpKeys = { 89, 84, 91, 86, 81, 88, 83 };
    }
}
