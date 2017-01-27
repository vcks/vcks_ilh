using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;

namespace vcks_ilh
{
    public partial class SheetWindow
    {
        class BD
        {
            public const uint BARS_ON_STAFF = 2;
            public const uint STAFFS_ON_PAGE = 7;

             double PAGE_RIGHT_BORDER = 863;

            const double noteMarginInBar = 30;
            const double noteStartPosition = noteMarginInBar/2;

            double PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF;
            double PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF;

            public double SET_PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF { set { PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF = value; } }
            public double SET_PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF { set { PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF = value; } }

            double BAR_1ST_STAFF_WIDTH { get { return (PAGE_RIGHT_BORDER - PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF) / BARS_ON_STAFF; } }
            double BAR_N_STAFF_WIDTH { get { return (PAGE_RIGHT_BORDER - PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF) / BARS_ON_STAFF; } }

             uint barCount;
            public  uint BarCount { get { return barCount; } }

             uint noteCount;
            public  uint NoteCount { get { return noteCount; } }

            List<TextBlock> musicalObjectsInCurrentBar;

            public Dictionary<uint, Music.Accidentals> accidentalsInBar;

            Canvas canvas;

            double timeSignature;

            double currentLeft;

            public uint activePageId;
            public bool isPageFull { get { return BarCount == (activePageId + 1) * (BARS_ON_STAFF * STAFFS_ON_PAGE); } }

            double BarWidth { get { return barCount < BARS_ON_STAFF ? BAR_1ST_STAFF_WIDTH : BAR_N_STAFF_WIDTH; } }

             double GetNoteSpace(double duration)
            {
                return (BarWidth - noteMarginInBar) / (timeSignature / duration);
            }

            public  bool IsBarlineNeeded { get { return (barCount % BARS_ON_STAFF) != BARS_ON_STAFF - 1; } }

            public  double GetNoteLeft(double duration)
            {
                double noteWidth = duration == Music.Durations.QUARTER ? QUARTER_NOTE_WIDTH : EIGHT_NOTE_WIDTH;
                double left = (barCount < BARS_ON_STAFF ? PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF : PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF) + BarWidth * (barCount % BARS_ON_STAFF) + currentLeft + GetNoteSpace(duration) / 2 - noteWidth / 2;
                return left; 
            }

            public  double GetNoteTop(uint code)
            {
                return PAGE_MARGIN_TOP + ((barCount % (STAFFS_ON_PAGE*BARS_ON_STAFF)) / BARS_ON_STAFF) * SPACE_BETWEEN_STAFF + NOTES_MARGIN_TOP[code];
            }

            public  double GetRestLeft(double duration)
            {
                return (barCount < BARS_ON_STAFF ? PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF : PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF) + BarWidth * (barCount % BARS_ON_STAFF) + currentLeft + GetNoteSpace(duration) / 2 - REST_WIDTH / 2;
            }

            public  double GetRestTop(double duration)
            {
                return PAGE_MARGIN_TOP + ((barCount % (STAFFS_ON_PAGE * BARS_ON_STAFF)) / BARS_ON_STAFF) * SPACE_BETWEEN_STAFF + RESTS_MARGIN_TOP[duration];
            }

            public  double GetBarlineLeft()
            {
                return (barCount < BARS_ON_STAFF ? PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF : PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF) + BarWidth * (barCount % BARS_ON_STAFF + 1);
            }

            public  double GetBarlineTop()
            {
                return PAGE_MARGIN_TOP + ((barCount % (STAFFS_ON_PAGE * BARS_ON_STAFF)) / BARS_ON_STAFF) * SPACE_BETWEEN_STAFF;
            }

            public bool IsItActiveAccidental(uint code,Music.Accidentals accidental)
            {
                if (!accidentalsInBar.ContainsKey(code)) return false;
                else return accidentalsInBar[code] == accidental?true:false;
            }

            public  void Reset(int skip=0)
            {
                Init(skip);
            }
            
            public void Init(int skip=0)
            {
                barCount = (uint)skip;
                noteCount = 0;
                currentLeft = noteStartPosition;
                musicalObjectsInCurrentBar = new List<TextBlock>();
                accidentalsInBar = new Dictionary<uint, Music.Accidentals>();
            }

            public BD(Canvas cnv, double tS)
            {
                canvas = cnv;
                timeSignature = tS;
                activePageId = 0;
                Init();
            }

            public  void NextPage()
            {
                noteCount = 0;
                currentLeft = noteStartPosition;
                musicalObjectsInCurrentBar = new List<TextBlock>();
            }

            public  void NextBar()
            {
                accidentalsInBar = new Dictionary<uint, Music.Accidentals>();
                musicalObjectsInCurrentBar.Clear();
                barCount++;
            }

            public  void NextNote(TextBlock[] tbs, double duration)
            {
                foreach (var tb in tbs)
                    musicalObjectsInCurrentBar.Add(tb);
                noteCount++;
                currentLeft += GetNoteSpace(duration);
            }

            public  void ClearBar()
            {
                accidentalsInBar = new Dictionary<uint, Music.Accidentals>();
                noteCount = 0;
                currentLeft = noteStartPosition;
                foreach (var el in musicalObjectsInCurrentBar) canvas.Children.Remove(el);
            }

        }
    }
}
