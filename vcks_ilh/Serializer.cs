using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.Serialization;

namespace vcks_ilh
{
    static class Serializer
    {
        public static void Serialize(Song song, string path)
        {
            try
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(Song));
                using (Stream fs = new FileStream(path, FileMode.Create)) dcs.WriteObject(fs, song);
            }
            catch (Exception e) { Logger.Log(e.Message); }
        }

        public static Song Deserialize(string path)
        {
            try
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(Song));
                using (Stream fs = new FileStream(path, FileMode.Open)) return (Song)dcs.ReadObject(fs);
            }
            catch (Exception e) { Logger.Log(e.Message); return null; }
        }

    }
}
