using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace vcks_ilh
{
    [DataContract]
    [KnownType (typeof(Bar))]    
    public class Song
    {
        [DataMember]
        public string title;
        [DataMember]
        public string author;
        [DataMember]
        public double timeSignature;
        [DataMember]
        public uint temp;
        [DataMember]
        public uint countOfFlats;
        [DataMember]
        public uint countOfSharps;

        [DataMember]
        List<Bar> bars = new List<Bar>();
                
        public delegate void ChangedEventHandler(Bar bar, bool switchPageIfFull = true);
        public event ChangedEventHandler Changed;

        public delegate void BarRemovedEventHandler(Song song);
        public event BarRemovedEventHandler BarRemoved;

        public Song(string title, string author, double timeSignature, uint temp, uint countOfFlats, uint countOfSharps)
        {
            this.title = title;
            this.author = author;
            this.temp = temp;
            this.timeSignature = timeSignature;
            this.countOfFlats = countOfFlats;
            this.countOfSharps = countOfSharps;
            bars.Add(new Bar(timeSignature));
        }

        public void ChangeTemp(uint temp)
        {
            this.temp = temp;
        }

        public void AddMusicalObject(MusicalObject[] mo)
        {
            if (bars.Last().Duration == timeSignature)
                bars.Add(new Bar(timeSignature));

            if (bars.Last().Duration + mo[0].Duration <= timeSignature)
            {
                bars.Last().AddMusicalObject(mo);
                if (Changed != null) Changed(bars.Last());
            }
            else
            {
                bars.Last().Close();
                if (Changed != null) Changed(bars.Last());
                AddMusicalObject(mo);
            }
        }

        public void RemoveLastMusicalObject()
        {
            if (bars.Count > 0)
            {
                if (bars.First().Duration!=0)
                {
                    if (bars.Last().Duration == 0)
                    {
                        if (bars.Count > 1)
                        {
                            bars.Remove(bars.Last());
                            bars.Last().RemoveLastMusicalObject();
                            if (BarRemoved != null) BarRemoved(this);
                        }
                    }
                    else
                    {
                        if (bars.Last().IsFull)
                        {
                            bars.Last().RemoveLastMusicalObject();
                            if (BarRemoved != null) BarRemoved(this);
                        }
                        else
                        {
                            bars.Last().RemoveLastMusicalObject();
                            if (Changed != null) Changed(bars.Last());
                        }
                    }
                }
            }
        }

        public IEnumerable<Bar> GetBars()
        {
            return bars;
        }

        public IEnumerable<MusicalObject[]> GetAllMusicalObjectsInLine()
        {
            List<MusicalObject[]> mo = new List<MusicalObject[]>();
            foreach(var b in bars)
                mo.AddRange(b.GetContent());
            return mo;
        }

    }
}
