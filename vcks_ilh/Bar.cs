using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace vcks_ilh
{
    [DataContract]
    [KnownType(typeof(Note))]
    [KnownType(typeof(Rest))]
    [KnownType(typeof(MusicalObject))]
    public class Bar
    {
        [DataMember]
        List<MusicalObject[]> musicalObjects = new List<MusicalObject[]>();
                
        public double Duration { get { return musicalObjects.Sum(z =>z[0].Duration); } }

        [DataMember]
        public double timeSignature;

        public bool IsFull { get { return Duration == timeSignature; } }

        public Bar(double timeSignature)
        {
            this.timeSignature = timeSignature;
        }

        public void Close()
        {
            if(Duration!=timeSignature)
            {
                AddMusicalObject(new MusicalObject[] { GetAppropriateRest(timeSignature-Duration) });
                Close();
            }
        }

        public void AddMusicalObject(MusicalObject[] mo)
        {
            musicalObjects.Add(mo);
            CheckAndReplceRests();
        }

        public void RemoveLastMusicalObject()
        {
            musicalObjects.Remove(musicalObjects.Last());
        }

        void CheckAndReplceRests()
        {
            List<MusicalObject[]> temporaryMusicalObjects = new List<MusicalObject[]>();
            

            for (int i = 0; i < musicalObjects.Count; i++)
            {
                if (musicalObjects[i][0].GetType() == typeof(Rest))
                {
                    List<MusicalObject[]> musicalObjectsForRemoving = new List<MusicalObject[]>();
                    double d = 0;

                    for (int j = i; j < musicalObjects.Count; j++)
                        if (musicalObjects[j][0].GetType() == typeof(Rest)) musicalObjectsForRemoving.Add(musicalObjects[j]);
                        else break;

                    if (musicalObjectsForRemoving.Count>1)
                    {
                        d =  musicalObjectsForRemoving.Sum(x => x[0].Duration);
                        List<MusicalObject[]> rests = new List<MusicalObject[]>();

                        while (d != 0)
                        {
                            Rest r = GetAppropriateRest(d);
                            rests.Add(new MusicalObject[]{ r });
                            d -= r.Duration;
                        }

                        temporaryMusicalObjects.AddRange(rests);
                        i += musicalObjectsForRemoving.Count-1;
                    }
                    else temporaryMusicalObjects.AddRange(musicalObjectsForRemoving);
                }
                else temporaryMusicalObjects.Add(musicalObjects[i]);
            }
            musicalObjects = temporaryMusicalObjects;
        }

        Rest GetAppropriateRest(double duration)
        {
            double d = 0;
            if (duration >= Music.Durations.SIXTEENTH) d = Music.Durations.SIXTEENTH;
            if (duration >= Music.Durations.EIGHT) d = Music.Durations.EIGHT;
            if (duration >= Music.Durations.QUARTER) d = Music.Durations.QUARTER;
            if (duration >= Music.Durations.QUARTER) d = Music.Durations.QUARTER;
            if (duration >= Music.Durations.HALF) d = Music.Durations.HALF;
            if (duration >= Music.Durations.WHOLE) d = Music.Durations.WHOLE;
            return new Rest(d);
        }

        public IEnumerable<MusicalObject[]> GetContent()
        {
            return musicalObjects;
        }
    }
}
