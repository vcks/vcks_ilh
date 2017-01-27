using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.Runtime.Serialization;

namespace vcks_ilh
{
    [DataContract]
    public  class Note : MusicalObject
    {
        public override string ValueForDisplay { get { return Music.NotesForDisplay[baseDuration] + (hasDot ? Symbols.DOT : string.Empty); } }
        public override double Duration { get { return baseDuration * (hasDot ? 1.5 : 1.0); } }
        public bool HasDot { get { return hasDot; } }

        [DataMember]
        public uint code;
        [DataMember]
        bool hasDot;

        static Dictionary<uint, string> NoteNames = new Dictionary<uint, string>()  {
           {61,"C#"},
           {62,"D"},
           {63,"D#"},
           {64,"E"},
           {65,"F"},
           {66,"F#"},
           {67,"G"},
           {68,"G#"},
           {69,"A"},
           {70,"A#"},
           {71,"B"}
        };

        public  Note(uint code, double duration, bool hasDot=false)
        {
            this.code = code;
            this.baseDuration = duration;
            this.hasDot = hasDot;
        }        

        public static uint[] GetThisNoteInAllOctaves(uint code)
        {
            List<uint> notes = new List<uint>();
            for (int i = (int)Music.possibleNotesRange[0]; i <= Music.possibleNotesRange[1]; i++)
                if (Math.Abs(i - code) % 12 == 0) notes.Add((uint)i);
            return notes.ToArray();
        }

        public static string GetNoteName(uint code)
        {
            uint magicVar = 0;
            for (uint i = code; i > NoteNames.Last().Key; i -= 12) magicVar = i-12;
            return NoteNames[magicVar];
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", code, baseDuration);
        }
    }
}
