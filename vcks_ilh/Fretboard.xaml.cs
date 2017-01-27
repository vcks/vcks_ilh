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

namespace vcks_ilh
{
    public partial class Fretboard : UserControl
    {
        uint FIRST_STRING_OPEN_NOTE=62, SECOND_STRING_OPEN_NOTE=67;
        uint firstStringFret, secondStringFret; // Выбранные ноты обеих струн

        int[,] strings = { { 0, 30 }, { 60, 88 } };
        int[,] frets = {
         { 0,0},
         {5,65},
         {68,135},
         {139,198},
         {204,260},
         {266,315},
         {321,372},
         {378,425},
         {431,476},
         {482,527},
         {533,576},
         {582,625},
         {631,669},
         {675,713},
         {719,754},
         {758,791},
         {797,826},
         {832,861},
         {867,898},
         {904,933}
        };

        delegate void FretboardClickedEventHandler(Point p);
        event FretboardClickedEventHandler FretboardClicked; // Координаты x и y клика по грифу

        delegate void FretAndStringClickedEventHandler(int fret, int _string);
        event FretAndStringClickedEventHandler FretAndStringClicked; // Струна и лад по которым был осуществлен клик

        public delegate void NoteEventHandler(uint[] note);
        public event NoteEventHandler NotePlayed; // Извлечённая нота
        
        event RoutedEventHandler FirstStringClicked; // Удар по первой струне
        event RoutedEventHandler SecondStringClicked; // Удар по второй струне
        event RoutedEventHandler BothStringClicked; // Удар по обеим струнам
        event RoutedEventHandler FirstStringReseted; // Сбросить первую струну
        event RoutedEventHandler SecondStringReseted; // Сбросить вторую струну
        event RoutedEventHandler BothStringReseted; // Сбросить обе струны

        public Fretboard()
        {
            InitializeComponent();
            FirstString.LongClick += (sender, e) => { if (FirstStringReseted != null) FirstStringReseted(sender, e); };
            SecondString.LongClick += (sender, e) => { if (SecondStringReseted != null) SecondStringReseted(sender, e); };
            BothString.LongClick += (sender, e) => { if (BothStringReseted != null) BothStringReseted(sender, e); };
            this.FirstStringReseted += (sender, e) => { firstStringFret = 0; MoveStringDot(0, 0); };
            this.SecondStringReseted += (sender, e) => { secondStringFret = 0; MoveStringDot(0, 1); };
            this.BothStringReseted += (sender, e) => { firstStringFret = secondStringFret = 0; MoveStringDot(0, 0); MoveStringDot(0, 1); };
        }

        private void FirstString_Click(object sender, RoutedEventArgs e)
        {
            if (NotePlayed != null) NotePlayed(new uint[] { FIRST_STRING_OPEN_NOTE + firstStringFret });
            if (FirstStringClicked != null) FirstStringClicked(sender, e);
        }

        private void SecondString_Click(object sender, RoutedEventArgs e)
        {
            if (NotePlayed != null) NotePlayed(new uint[] { SECOND_STRING_OPEN_NOTE + secondStringFret });
            if (SecondStringClicked != null) SecondStringClicked(sender, e);
        }

        private void BothString_Click(object sender, RoutedEventArgs e)
        {
            if (NotePlayed != null) NotePlayed(new uint[] { FIRST_STRING_OPEN_NOTE + firstStringFret, SECOND_STRING_OPEN_NOTE + secondStringFret });
            if (BothStringClicked != null) BothStringClicked(sender, e);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(sender as Image);
            Tuple<int, int> result = GetFretAndStringFromCoordinates(p);
            if (result.Item1 != -1 && result.Item2 != -1) SetSelectedFretAndString((uint)result.Item1, (uint)result.Item2);
            if (FretboardClicked != null) FretboardClicked(p);
            if (FretAndStringClicked != null) FretAndStringClicked(result.Item1, result.Item2);
        }

        void SetSelectedFretAndString(uint fret,uint _string)
        {
            if (_string == 0) firstStringFret = fret; else secondStringFret = fret;
            MoveStringDot( fret, _string);
        }

        void MoveStringDot(uint fret, uint _string)
        {
            var dot = _string == 0 ? FirstStringDot : SecondStringDot;
            double marginLeft = frets[fret, 0] + (frets[fret, 1] - frets[fret, 0]) / 2 - dot.Width / 2;
            dot.Margin = new Thickness(marginLeft, 0, 0, 0);
            dot.Visibility = fret == 0 ? Visibility.Hidden:Visibility.Visible;
        }

        Tuple<int,int> GetFretAndStringFromCoordinates(Point p)
        {
            int fret = -1;
            int _string = -1;

            for (int i=0;i<frets.GetLength(0);i++)
                if (p.X > frets[i, 0] && p.X < frets[i, 1]) fret=i;

            for (int i = 0; i < strings.GetLength(0); i++)
                if (p.Y > strings[i, 0] && p.Y < strings[i, 1]) _string = i;

            return new Tuple<int, int>(fret,_string);
        }
    }
}
