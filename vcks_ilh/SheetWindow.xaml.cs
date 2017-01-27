using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing.Text;
using System.IO;

namespace vcks_ilh
{

    public partial class SheetWindow : UserControl
    {
        static Dictionary<double, double> RESTS_MARGIN_TOP = new Dictionary<double, double>() { { Music.Durations.WHOLE, -26.5 }, { Music.Durations.HALF, -24.5 }, { Music.Durations.QUARTER, -23 }, { Music.Durations.EIGHT, -28 }, { Music.Durations.SIXTEENTH, -13 } };
        static double[] NOTES_MARGIN_TOP;

        const uint FONT_SIZE_PRIMARY = 80;
        const int PAGE_MARGIN_LEFT = 45;
        const int PAGE_MARGIN_TOP = 210;
        const uint SPACE_BETWEEN_STAFF = 140;
        const uint FONT_SIZE_FOR_AUTHOR = 17;
        const int PAGE_MARGIN_TOP_FOR_AUTHOR = 135;
        const int PAGE_MARGIN_RIGHT_FOR_AUTHOR = 35;
        const uint FONT_SIZE_FOR_TITLE = 70;
        const int PAGE_MARGIN_TOP_FOR_TITLE = 50;
        const int PAGE_MARGIN_LEFT_FOR_CLEF = PAGE_MARGIN_LEFT + 60;
        const uint FONT_SIZE_FOR_ACCIDENTALS = 30;
        const int PAGE_MARGIN_TOP_FOR_FLAT = PAGE_MARGIN_TOP + 52;
        const int PAGE_MARGIN_TOP_FOR_SHARP = PAGE_MARGIN_TOP + 55;
        const int PAGE_MARGIN_LEFT_FOR_ACCIDENTALS = PAGE_MARGIN_LEFT + 35;
        const uint SPACE_BETWEEN_ACCIDENTALS = 10;
        const int PAGE_MARGIN_LEFT_FOR_TIME_SIGNATURE = 20;
        const int PAGE_MARGIN_TOP_FOR_TIME_SIGNATURE = PAGE_MARGIN_TOP + 10;
        const uint FONT_SIZE_FOR_TIME_SIGNATURE = 33;
        const double SPACE_BETWEEN_SEMITONES = 6.5;

        static FontFamily f1 = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#ALOT Gutenberg B Normal");
        static FontFamily f2 = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#Segoe UI");
        static FontFamily f3 = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#Proclamate Heavy");
        static FontFamily f4 = new FontFamily(new Uri("pack://application:,,,/"), "./resources/#Symbola");

        static FontFamily noteFontForAccidentals = f4;
        static FontFamily noteFontForTimeSignature = f3;
        static FontFamily noteFontPrimary = f4;
        static FontFamily titleFont = f1;
        static FontFamily authorFont = f2;

        // for 80pt font
        const double QUARTER_NOTE_WIDTH = 22.03;
        const double EIGHT_NOTE_WIDTH = 35.82;
        const double WHOLE_NOTE_WIDTH = 54.45;
        const double REST_WIDTH = 21.406;

        uint countOfFlats, countOfSharps;
        double timeSignature;

        List<uint> notesWithAccidentalInKey;

        public bool IsLastPageDisplayed { get { return bd.activePageId == bd.BarCount / (BD.BARS_ON_STAFF * BD.STAFFS_ON_PAGE); } }

        public delegate void NewPageCreatedEventHandler(uint n);
        public event NewPageCreatedEventHandler NewPageCreated;

        public delegate void PageRemovedEventHandler(uint n);
        public event PageRemovedEventHandler PageRemoved;

        BD bd;

        public SheetWindow()
        {
            InitializeComponent();
            SetNotesTopMargin();
        }

        public void ExportToPNG(Song song, string path)
        {
            File.WriteAllBytes(path, GetPageMemoryStream(canvas).ToArray());
        }

        MemoryStream GetPageMemoryStream(Canvas c)
        {
            double dpi = 300;
            Rect rect = VisualTreeHelper.GetDescendantBounds(c);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(rect.Width * (dpi / 96)), (int)(rect.Height * (dpi / 96)), dpi, dpi, System.Windows.Media.PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(c);
                dc.DrawRectangle(vb, null, new Rect(new Point(), rect.Size));
            }
            rtb.Render(dv);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream ms = new MemoryStream();
            pngEncoder.Save(ms);
            ms.Close();
            return ms;
        }

        void SetNotesTopMargin()
        {
            NOTES_MARGIN_TOP = new double[120];

            double i = SPACE_BETWEEN_SEMITONES; // расстояние между линиями нотного стана

            NOTES_MARGIN_TOP[52] = i * 13; //E2
            NOTES_MARGIN_TOP[53] = i * 12; //F2
            NOTES_MARGIN_TOP[54] = i * 12; //F2#
            NOTES_MARGIN_TOP[55] = i * 11; //G2
            NOTES_MARGIN_TOP[56] = i * 11; //G2#
            NOTES_MARGIN_TOP[57] = i * 10; //A2
            NOTES_MARGIN_TOP[58] = i * 10; //A2#
            NOTES_MARGIN_TOP[59] = i * 9; //B2

            NOTES_MARGIN_TOP[60] = i * 8; //C3
            NOTES_MARGIN_TOP[61] = i * 8; //C3#
            NOTES_MARGIN_TOP[62] = i * 7; //D3
            NOTES_MARGIN_TOP[63] = i * 7; //D3#
            NOTES_MARGIN_TOP[64] = i * 6; //E3
            NOTES_MARGIN_TOP[65] = i * 5; //F3
            NOTES_MARGIN_TOP[66] = i * 5; //F3#
            NOTES_MARGIN_TOP[67] = i * 4; //G3
            NOTES_MARGIN_TOP[68] = i * 4; //G3#
            NOTES_MARGIN_TOP[69] = i * 3; //A3
            NOTES_MARGIN_TOP[70] = i * 3; //A3#
            NOTES_MARGIN_TOP[71] = i * 2; //B3

            NOTES_MARGIN_TOP[72] = i * 1; //C4
            NOTES_MARGIN_TOP[73] = i * 1; //C4#
            NOTES_MARGIN_TOP[74] = i * 0; //D4
            NOTES_MARGIN_TOP[75] = i * 0; //D4#
            NOTES_MARGIN_TOP[76] = i * -1; //E4
            NOTES_MARGIN_TOP[77] = i * -2; //F4
            NOTES_MARGIN_TOP[78] = i * -2; //F4#
            NOTES_MARGIN_TOP[79] = i * -3; //G4
            NOTES_MARGIN_TOP[80] = i * -3; //G4#
            NOTES_MARGIN_TOP[81] = i * -4; //A4
            NOTES_MARGIN_TOP[82] = i * -4; //A4#
            NOTES_MARGIN_TOP[83] = i * -5; //B4

            NOTES_MARGIN_TOP[84] = i * -6; //C5
            NOTES_MARGIN_TOP[85] = i * -6; //C5#
            NOTES_MARGIN_TOP[86] = i * -7; //D5
            NOTES_MARGIN_TOP[87] = i * -7; //D5#

            NOTES_MARGIN_TOP[88] = i * -8; //E5
            NOTES_MARGIN_TOP[89] = i * -9; //F5
            NOTES_MARGIN_TOP[90] = i * -9; //F5#
            NOTES_MARGIN_TOP[91] = i * -10; //G5
            NOTES_MARGIN_TOP[92] = i * -10; //G5#
            NOTES_MARGIN_TOP[93] = i * -11; //A5
            NOTES_MARGIN_TOP[94] = i * -11; //A5#
        }

        void DrawStaff()
        {
            string stringForDrawing = string.Empty;

            for (int i = 0; i < 15; i++) stringForDrawing += Symbols.FIVE_LINE_STAFF;
            stringForDrawing = Symbols.SINGLE_BARLINE + stringForDrawing + Symbols.SINGLE_BARLINE;

            for (int i = 0; i < 7; i++)
            {
                double marginForTimeSign = PAGE_MARGIN_LEFT_FOR_ACCIDENTALS;

                TextBlock tb = new TextBlock()
                {
                    FontFamily = noteFontPrimary,
                    Text = stringForDrawing,
                    FontSize = FONT_SIZE_PRIMARY,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                Canvas.SetTop(tb, PAGE_MARGIN_TOP + i * SPACE_BETWEEN_STAFF);
                Canvas.SetLeft(tb, PAGE_MARGIN_LEFT);
                canvas.Children.Add(tb);

                TextBlock tbc = new TextBlock()
                {
                    FontFamily = noteFontPrimary,
                    Text = Symbols.G_CLEF,
                    FontSize = FONT_SIZE_PRIMARY,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                Canvas.SetTop(tbc, PAGE_MARGIN_TOP + i * SPACE_BETWEEN_STAFF);
                Canvas.SetLeft(tbc, PAGE_MARGIN_LEFT_FOR_CLEF);
                canvas.Children.Add(tbc);

                for (int z = 0; z < countOfFlats; z++)
                {
                    TextBlock tbf = new TextBlock()
                    {
                        FontFamily = noteFontForAccidentals,
                        Text = Symbols.FLAT_SIGN,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        FontSize = FONT_SIZE_FOR_ACCIDENTALS
                    };
                    Canvas.SetTop(tbf, PAGE_MARGIN_TOP_FOR_FLAT + i * SPACE_BETWEEN_STAFF + NOTES_MARGIN_TOP[Music.flatKeys[z]]);
                    Canvas.SetLeft(tbf, PAGE_MARGIN_LEFT_FOR_ACCIDENTALS + z * SPACE_BETWEEN_ACCIDENTALS);
                    marginForTimeSign = PAGE_MARGIN_LEFT_FOR_ACCIDENTALS + z * SPACE_BETWEEN_ACCIDENTALS;
                    canvas.Children.Add(tbf);
                }

                for (int z = 0; z < countOfSharps; z++)
                {
                    TextBlock tbs = new TextBlock()
                    {
                        FontFamily = noteFontForAccidentals,
                        Text = Symbols.SHARP_SIGN,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        FontSize = FONT_SIZE_FOR_ACCIDENTALS
                    };
                    Canvas.SetTop(tbs, PAGE_MARGIN_TOP_FOR_SHARP + i * SPACE_BETWEEN_STAFF + NOTES_MARGIN_TOP[Music.sharpKeys[z]]);
                    Canvas.SetLeft(tbs, PAGE_MARGIN_LEFT_FOR_ACCIDENTALS + z * SPACE_BETWEEN_ACCIDENTALS);
                    marginForTimeSign = PAGE_MARGIN_LEFT_FOR_ACCIDENTALS + z * SPACE_BETWEEN_ACCIDENTALS;
                    canvas.Children.Add(tbs);
                }

                if (i == 0)
                {
                    bd.SET_PAGE_MARGIN_LEFT_FOR_1ST_BAR_N_STAFF = marginForTimeSign + 25;
                    marginForTimeSign += PAGE_MARGIN_LEFT_FOR_TIME_SIGNATURE;
                    bd.SET_PAGE_MARGIN_LEFT_FOR_1ST_BAR_1ST_STAFF = marginForTimeSign + 25;

                    TextBlock tbts1 = new TextBlock()
                    {
                        FontFamily = noteFontForTimeSignature,
                        Text = Music.TimeSignaturesForDisplay[timeSignature][0],
                        FontSize = FONT_SIZE_FOR_TIME_SIGNATURE,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    Canvas.SetTop(tbts1, PAGE_MARGIN_TOP_FOR_TIME_SIGNATURE + i * SPACE_BETWEEN_STAFF);
                    Canvas.SetLeft(tbts1, marginForTimeSign);
                    canvas.Children.Add(tbts1);

                    TextBlock tbts2 = new TextBlock()
                    {
                        FontFamily = noteFontForTimeSignature,
                        Text = Music.TimeSignaturesForDisplay[timeSignature][1],
                        FontSize = FONT_SIZE_FOR_TIME_SIGNATURE,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    Canvas.SetTop(tbts2, PAGE_MARGIN_TOP_FOR_TIME_SIGNATURE + i * SPACE_BETWEEN_STAFF + 27);
                    Canvas.SetLeft(tbts2, marginForTimeSign);
                    canvas.Children.Add(tbts2);
                }
            }
        }

        void SwitchToBlankPage()
        {
            bd.activePageId++;
            ClearSheet();
            DrawStaff();
            if(NewPageCreated!=null) NewPageCreated(bd.activePageId + 1);
        }

        public uint SwitchToNextPage(Song song)
        {
            if(bd.isPageFull)
            {
                bd.activePageId++;
                DrawPage(song);
            }
            return bd.activePageId +1;
        }

        public uint SwitchToPreviousPage(Song song)
        {
            if (bd.activePageId > 0)
            {
                bd.activePageId--;
                DrawPage(song);
            }
            return bd.activePageId + 1;
        }

        void DrawPage(Song song)
        {
            int skip = (int)(BD.BARS_ON_STAFF * BD.STAFFS_ON_PAGE * bd.activePageId);
            bd.Reset(skip);
            ClearSheet();
            DrawStaff();
            if (bd.activePageId == 0) DrawHead(song);
            foreach (Bar b in song.GetBars().Skip(skip).Take((int)(BD.BARS_ON_STAFF * BD.STAFFS_ON_PAGE))) DrawBar(b, false);
        }

        void DrawHead(Song song)
        {
            TextBlock tbt = new TextBlock()
            {
                FontFamily = titleFont,
                Text = song.title,
                TextTrimming=TextTrimming.CharacterEllipsis,
                FontSize = FONT_SIZE_FOR_TITLE,
                Width = canvas.Width,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetTop(tbt, PAGE_MARGIN_TOP_FOR_TITLE);
            canvas.Children.Add(tbt);

            TextBlock tba = new TextBlock()
            {
                FontFamily = authorFont,
                Text = "Words and Music by" + Environment.NewLine + song.author,
                TextTrimming = TextTrimming.CharacterEllipsis,
                FontSize = FONT_SIZE_FOR_AUTHOR,
                Width = canvas.Width-80,
                TextAlignment = TextAlignment.Right
            };
            Canvas.SetTop(tba, PAGE_MARGIN_TOP_FOR_AUTHOR);
            Canvas.SetRight(tba, PAGE_MARGIN_RIGHT_FOR_AUTHOR);
            canvas.Children.Add(tba);
        }

        public void CreateNewSheet(Song song)
        {
            bd = new BD(canvas, song.timeSignature);
            countOfFlats = song.countOfFlats;
            countOfSharps = song.countOfSharps;
            timeSignature = song.timeSignature;
            notesWithAccidentalInKey = new List<uint>();

            foreach (var el in Music.flatKeys.Take((int)countOfFlats))
                notesWithAccidentalInKey.AddRange(Note.GetThisNoteInAllOctaves(el));

            foreach (var el in Music.sharpKeys.Take((int)countOfSharps))
                notesWithAccidentalInKey.AddRange(Note.GetThisNoteInAllOctaves(el));

            ClearSheet();
            DrawHead(song);
            DrawStaff();
        }

        public Visual GetSheetView()
        {
            return canvas;
        }

        void ClearSheet()
        {
            canvas.Children.Clear();
        }

        TextBlock DrawAccidentals(Note note, Music.Accidentals accidental)
        {
            if (bd.accidentalsInBar.ContainsKey(note.code)) bd.accidentalsInBar[note.code] = accidental;
            else bd.accidentalsInBar.Add(note.code, accidental);

            TextBlock tbs = new TextBlock()
            {
                FontFamily = noteFontForAccidentals,
                Text = Music.AccidentalsForDisplay[(int)accidental],
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = FONT_SIZE_FOR_ACCIDENTALS
            };

            double signLeft=15;

            if (note.baseDuration == Music.Durations.EIGHT) signLeft = 11;

            Canvas.SetTop(tbs, bd.GetNoteTop(note.code)+55);
            Canvas.SetLeft(tbs, bd.GetNoteLeft(note.baseDuration)-signLeft);
            canvas.Children.Add(tbs);
            return tbs;
        }

        TextBlock DrawLedgerLine(Note note)
        {
            TextBlock tb = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontFamily = noteFontPrimary,
                FontSize = FONT_SIZE_PRIMARY,
                Width = note.baseDuration == Music.Durations.WHOLE ? QUARTER_NOTE_WIDTH + 6 : QUARTER_NOTE_WIDTH
            };
            Canvas.SetTop(tb, bd.GetBarlineTop() + 65);
            Canvas.SetLeft(tb, bd.GetNoteLeft(note.baseDuration));

            if (note.code < 64)
            {
                tb.Text = Symbols.FOUR_LINE_STAFF;
            }
            else
            {
                if (note.code < 67) tb.Text = Symbols.THREE_LINE_STAFF;
                else tb.Text = note.code < 71 ? Symbols.TWO_LINE_STAFF : Symbols.ONE_LINE_STAFF;
            }

            canvas.Children.Add(tb);
            return tb;
        }

        void DrawSingleNote(Note note)
        {
            List<TextBlock> tbs = new List<TextBlock>();

            TextBlock tb = new TextBlock()
            {
                FontFamily = noteFontPrimary,
                Text = note.ValueForDisplay,
                FontSize = FONT_SIZE_PRIMARY,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            Note n = new Note(note.code, note.baseDuration, note.HasDot); ;

            if (countOfFlats>0)
            {
                if (Music.notesWithAccidentals.Contains(note.code))
                {
                    n = new Note(note.code + 1, note.baseDuration, note.HasDot);
                    if (notesWithAccidentalInKey.Contains(n.code))
                    {
                        if (bd.IsItActiveAccidental(n.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(n.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(n, Music.Accidentals.flat));
                }
                else
                {
                    n = new Note(note.code, note.baseDuration, note.HasDot);
                    if (notesWithAccidentalInKey.Contains(n.code))
                    {
                        if (!bd.IsItActiveAccidental(n.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(n.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                }
            }

            if (countOfSharps > 0)
            {

                if (Music.notesWithAccidentals.Contains(note.code))
                {
                    n = new Note(note.code - 1, note.baseDuration, note.HasDot);
                    if (notesWithAccidentalInKey.Contains(n.code))
                    {
                        if (bd.IsItActiveAccidental(n.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(n.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(n, Music.Accidentals.sharp));
                }
                else
                {
                    n = new Note(note.code, note.baseDuration, note.HasDot);
                    if (notesWithAccidentalInKey.Contains(n.code))
                    {
                        if (!bd.IsItActiveAccidental(n.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(n.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(n, Music.Accidentals.natural));
                }
            }

            if (countOfSharps == countOfFlats)
            {
                if (Music.notesWithAccidentals.Contains(note.code))
                {
                    if (!bd.IsItActiveAccidental(note.code - 1, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(new Note(note.code - 1, note.baseDuration, note.HasDot), Music.Accidentals.sharp));
                }
                else
                    if (bd.IsItActiveAccidental(note.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(note, Music.Accidentals.natural));
            }

            Canvas.SetTop(tb, bd.GetNoteTop(n.code));
            Canvas.SetLeft(tb, bd.GetNoteLeft(n.baseDuration));
            canvas.Children.Add(tb);
            
            tbs.Add(tb);

            if (n.code < 74) tbs.Add(DrawLedgerLine(n));

            bd.NextNote(tbs.ToArray(), n.Duration);
        }

        void DrawInterval(MusicalObject[] mo)
        {
            Note topNote = ((Note)mo[0]).code > ((Note)mo[1]).code ? (Note)mo[0] : (Note)mo[1];
            Note underNote = ((Note)mo[0]).code > ((Note)mo[1]).code ? (Note)mo[1] : (Note)mo[0];

            topNote = new Note(topNote.code, topNote.baseDuration, topNote.HasDot);
            underNote = new Note(underNote.code, underNote.baseDuration, underNote.HasDot);

            List<TextBlock> tbs = new List<TextBlock>();

            TextBlock tb1 = new TextBlock()
            {
                FontFamily = noteFontPrimary,
                Text = Music.NotesForDisplay[topNote.baseDuration],
                FontSize = FONT_SIZE_PRIMARY,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            if (countOfFlats > 0)
            {
                if (Music.notesWithAccidentals.Contains(topNote.code))
                {
                    topNote = new Note(topNote.code + 1, topNote.baseDuration, topNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(topNote.code))
                    {
                        if (bd.IsItActiveAccidental(topNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(topNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.flat));
                }
                else
                {
                    topNote = new Note(topNote.code, topNote.baseDuration, topNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(topNote.code))
                    {
                        if (!bd.IsItActiveAccidental(topNote.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(topNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                }
            }

            if (countOfSharps > 0)
            {

                if (Music.notesWithAccidentals.Contains(topNote.code))
                {
                    topNote = new Note(topNote.code - 1, topNote.baseDuration, topNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(topNote.code))
                    {
                        if (bd.IsItActiveAccidental(topNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(topNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.sharp));
                }
                else
                {
                    topNote = new Note(topNote.code, topNote.baseDuration, topNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(topNote.code))
                    {
                        if (!bd.IsItActiveAccidental(topNote.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(topNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
                }
            }

            if (countOfSharps == countOfFlats)
            {
                if (Music.notesWithAccidentals.Contains(topNote.code))
                {
                    if (!bd.IsItActiveAccidental(topNote.code - 1, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(new Note(topNote.code - 1, topNote.baseDuration, topNote.HasDot), Music.Accidentals.sharp));
                }
                else
                    if (bd.IsItActiveAccidental(topNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(topNote, Music.Accidentals.natural));
            }

            Canvas.SetTop(tb1, bd.GetNoteTop(topNote.code));
            Canvas.SetLeft(tb1, bd.GetNoteLeft(topNote.baseDuration));
            canvas.Children.Add(tb1);


            string underNoteValue = underNote.ValueForDisplay;
            double underNoteLeft = bd.GetNoteLeft(underNote.baseDuration);

            if (countOfFlats > 0)
            {
                if (Music.notesWithAccidentals.Contains(underNote.code))
                {
                    underNote = new Note(underNote.code + 1, underNote.baseDuration, underNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(underNote.code))
                    {
                        if (bd.IsItActiveAccidental(underNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(underNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.flat));
                }
                else
                {
                    underNote = new Note(underNote.code, underNote.baseDuration, underNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(underNote.code))
                    {
                        if (!bd.IsItActiveAccidental(underNote.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(underNote.code, Music.Accidentals.flat)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                }
            }

            if (countOfSharps > 0)
            {

                if (Music.notesWithAccidentals.Contains(underNote.code))
                {
                    underNote = new Note(underNote.code - 1, underNote.baseDuration, underNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(underNote.code))
                    {
                        if (bd.IsItActiveAccidental(underNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                    }
                    else
                        if (!bd.IsItActiveAccidental(underNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.sharp));
                }
                else
                {
                    underNote = new Note(underNote.code, underNote.baseDuration, underNote.HasDot);
                    if (notesWithAccidentalInKey.Contains(underNote.code))
                    {
                        if (!bd.IsItActiveAccidental(underNote.code, Music.Accidentals.natural)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                    }
                    else
                        if (bd.IsItActiveAccidental(underNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
                }
            }

            if (countOfSharps == countOfFlats)
            {
                if (Music.notesWithAccidentals.Contains(underNote.code))
                {
                    if (!bd.IsItActiveAccidental(underNote.code - 1, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(new Note(underNote.code - 1, underNote.baseDuration, underNote.HasDot), Music.Accidentals.sharp));
                }
                else
                    if (bd.IsItActiveAccidental(underNote.code, Music.Accidentals.sharp)) tbs.Add(DrawAccidentals(underNote, Music.Accidentals.natural));
            }


            if (underNote.baseDuration == Music.Durations.EIGHT)
            {
                if (Math.Abs(NOTES_MARGIN_TOP[topNote.code] - NOTES_MARGIN_TOP[underNote.code]) <= SPACE_BETWEEN_SEMITONES * 7)
                    underNoteValue = new Note(underNote.code,Music.Durations.QUARTER,underNote.HasDot).ValueForDisplay;
                if (Math.Abs(NOTES_MARGIN_TOP[topNote.code] - NOTES_MARGIN_TOP[underNote.code]) < SPACE_BETWEEN_SEMITONES * 2)
                {
                    underNoteLeft += 12;
                    underNoteValue = Symbols.NOTEHEAD_BLACK + (underNote.HasDot?Symbols.DOT:string.Empty);
                }
            }

            if (underNote.baseDuration == Music.Durations.QUARTER)
            {
                if (Math.Abs(NOTES_MARGIN_TOP[topNote.code] - NOTES_MARGIN_TOP[underNote.code]) < SPACE_BETWEEN_SEMITONES * 2)
                {
                    underNoteLeft += 12;
                    underNoteValue = Symbols.NOTEHEAD_BLACK+ (underNote.HasDot ? Symbols.DOT : string.Empty);
                }
            }

            if (underNote.baseDuration == Music.Durations.HALF)
            {
                if (Math.Abs(NOTES_MARGIN_TOP[topNote.code] - NOTES_MARGIN_TOP[underNote.code]) < SPACE_BETWEEN_SEMITONES * 2)
                {
                    underNoteLeft += 12;
                    underNoteValue = Symbols.VOID_NOTEHEAD + (underNote.HasDot ? Symbols.DOT : string.Empty);
                }
            }

            if (underNote.baseDuration == Music.Durations.WHOLE)
            {
                if (Math.Abs(NOTES_MARGIN_TOP[topNote.code] - NOTES_MARGIN_TOP[underNote.code]) < SPACE_BETWEEN_SEMITONES * 2)
                {
                    underNoteLeft += 14;
                    underNoteValue = Symbols.WHOLE_NOTE + (underNote.HasDot ? Symbols.DOT : string.Empty);
                }
            }

            TextBlock tb2 = new TextBlock()
            {
                FontFamily = noteFontPrimary,
                Text = underNoteValue,
                FontSize = FONT_SIZE_PRIMARY,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetTop(tb2, bd.GetNoteTop(underNote.code));
            Canvas.SetLeft(tb2, underNoteLeft);
            canvas.Children.Add(tb2);

            tbs.Add(tb1);
            tbs.Add(tb2);

            if (underNote.code < 74) tbs.Add(DrawLedgerLine(underNote));
            bd.NextNote(tbs.ToArray(), topNote.Duration);
        }

        void DrawRest(MusicalObject mo)
        {
            Rest rest = mo as Rest;
            TextBlock tb = new TextBlock()
            {
                FontFamily = noteFontPrimary,
                Text = rest.ValueForDisplay,
                FontSize = FONT_SIZE_PRIMARY,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetTop(tb, bd.GetRestTop(rest.baseDuration));
            Canvas.SetLeft(tb, bd.GetRestLeft(rest.baseDuration));
            canvas.Children.Add(tb);
            bd.NextNote(new TextBlock[] { tb }, rest.Duration);
        }

        void DrawBarline()
        {
            TextBlock tb = new TextBlock()
            {
                FontFamily = noteFontPrimary,
                Text = Symbols.SINGLE_BARLINE,
                FontSize = FONT_SIZE_PRIMARY,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Canvas.SetTop(tb, bd.GetBarlineTop());
            Canvas.SetLeft(tb, bd.GetBarlineLeft());
            canvas.Children.Add(tb);
            bd.NextNote(new TextBlock[] { tb }, 0);
        }

        public void DrawBar(Bar bar, bool switchPageIfItIsFull=true)
        {
            bd.ClearBar();            

            foreach (var mo in bar.GetContent())
            {
                if (mo[0].GetType() == typeof(Note))
                {
                        if (mo.Length == 1) DrawSingleNote(mo[0] as Note); else DrawInterval(mo);
                }
                else
                {
                    DrawRest(mo[0]);
                }
            }
            if (bar.IsFull)
            {
                if (bd.IsBarlineNeeded) DrawBarline();
                bd.NextBar();
                if (switchPageIfItIsFull && bd.isPageFull)  SwitchToBlankPage();
            }
        }

        public void DrawEntireSong(Song song)
        {
            foreach (Bar b in song.GetBars()) DrawBar(b);
        }


        public void RemoveLastBar(Song song)
        {
            if (bd.BarCount % (BD.BARS_ON_STAFF * BD.STAFFS_ON_PAGE) == 0)
            {
                bd.activePageId--;
                if (PageRemoved != null) PageRemoved(bd.activePageId + 1);
            }
            DrawPage(song);
        }
      
    }
}
