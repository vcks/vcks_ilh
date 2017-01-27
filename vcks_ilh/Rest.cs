using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace vcks_ilh
{
    [DataContract]
    class Rest : MusicalObject
    {
        public override string ValueForDisplay { get { return Music.RestsForDisplay[baseDuration]; } }
        public override double Duration { get { return baseDuration; } }

        public Rest(double duration)
        {
            this.baseDuration = duration;
        }

        public override string ToString()
        {
            return string.Format("D = {0}", baseDuration);
        }
    }
}
