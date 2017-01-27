using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using NAudio.Midi;

namespace vcks_ilh
{
    class Player
    {
        uint countOfFlats;
        uint countOfSharps;
        int temp;

        static Thread playingSong;
        public static bool IsSongPlaying { get { return playingSong == null ? false : playingSong.IsAlive; } }

        int channel = 1;
        static MidiOut midiOut;

        public Player(Song song)
        {
            try
            {
                countOfFlats = song.countOfFlats;
                countOfSharps = song.countOfSharps;
                temp = (int)song.temp;
                if (midiOut == null) midiOut = new MidiOut(0);
                SetVolumeAndIntsrument();
            }
            catch (Exception e) { Logger.Log(e.Message); }
        }

        void SetVolumeAndIntsrument()
        {
            try
            {
                midiOut.Send(NAudio.Midi.MidiMessage.ChangePatch(24, 1).RawData);
                midiOut.Volume = -2048;
            }
            catch (Exception e) { Logger.Log(e.Message); }
        }
        public void Close()
        {
            StopPlayingSong();
            midiOut.Close();
            midiOut.Dispose();
        }

        public void StopPlayingSong()
        {
            if (playingSong != null)
                if(playingSong.IsAlive)
                    playingSong.Abort();
        }

        public void PlayEntireSong(IEnumerable<MusicalObject[]> song)
        {
            playingSong = new Thread(() =>
            {
                foreach (var mo in song) Play(mo);
            });
            playingSong.Start();
        }

        public void PlaySingleMusicalObject(MusicalObject[] mo)
        {
            Thread playingNote = new Thread(() => Play(mo));
            playingNote.Start();
        }

        void Play(MusicalObject[] mo)
        {
            try
            {
                if (mo[0].GetType() == typeof(Note))
                {
                    if (mo.Length == 1)
                    {
                        int ch = channel;
                        Note note = (Note)mo[0];
                        uint noteForPlaying = note.code;
                        midiOut.Send(MidiMessage.StartNote((int)noteForPlaying, 127, ch).RawData);
                        Thread.Sleep((int)((60d / temp * note.Duration) * 1000));
                        midiOut.Send(MidiMessage.StopNote((int)noteForPlaying, 0, ch).RawData);
                    }
                    else
                    {
                        int ch1 = channel;
                        int ch2 = channel+1;
                        Note note1 = (Note)mo[0];
                        Note note2 = (Note)mo[1];
                        uint note1ForPlaying = note1.code;
                        uint note2ForPlaying = note2.code;
                        midiOut.Send(MidiMessage.StartNote((int)note1ForPlaying, 127, ch1).RawData);
                        midiOut.Send(MidiMessage.StartNote((int)note2ForPlaying, 127, ch2).RawData);
                        Thread.Sleep((int)((60d / temp * note1.Duration) * 1000));
                        midiOut.Send(MidiMessage.StopNote((int)note1ForPlaying, 0, ch1).RawData);
                        midiOut.Send(MidiMessage.StopNote((int)note2ForPlaying, 0, ch2).RawData);
                    }
                }
                else Thread.Sleep((int)((60d / temp * mo[0].Duration) * 1000));
            }
            catch(ThreadAbortException threadAbortException) {}
            catch (Exception e) { Logger.Log(e.Message); }
        }
    }
}
