using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace vcks_ilh
{    
    [DataContract]
    public abstract class MusicalObject
    {
        [DataMember]
        public double baseDuration;
        public abstract string ValueForDisplay {get;}
        public abstract double Duration { get;}
    }
}
