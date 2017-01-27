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
    public partial class ModePanel : UserControl
    {
        double songTimeSignature;

        double duration;
        bool hasDot;

        public delegate void RestEventHandler(double duration);
        public event RestEventHandler RestClicked;

        public delegate uint PageEventHandler();
        public event PageEventHandler PreviousPageClicked;
        public event PageEventHandler NextPageClicked;

        public ModePanel()
        {
            InitializeComponent();
            duration = Music.Durations.QUARTER;
        }

        public Response GetState()
        {
            return new Response(duration,hasDot);
        }

        public class Response
        {
            public double duration;
            public bool hasDot;

            public Response(double duration,bool hasDot)
            {
                this.duration = duration;
                this.hasDot = hasDot;
            }
        }

        public void Init(double duration)
        {
            TextBlockPage.Text = 1.ToString();
            songTimeSignature = duration;
            Note_MouseLeftButtonUp(Note4, null);
            Note1.IsEnabled = Rest1.IsEnabled= songTimeSignature >= Music.Durations.WHOLE? true: false;
            if (hasDot) Dot_MouseLeftButtonUp(Dot, null);
        }

        public void SetPageNumber(uint n)
        {
            TextBlockPage.Text = n.ToString();
        }

        private void Note_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            Note1.Background = Note2.Background = Note4.Background = Note8.Background = null;
            Note1.Style = Note2.Style = Note4.Style = Note8.Style = (Style)this.FindResource("FlatButtonIndigo500");
            grid.Background = (SolidColorBrush)Application.Current.FindResource("Indigo700");

            switch (grid.Name)
            {
                case "Note1": duration = Music.Durations.WHOLE; break;
                case "Note2": duration = Music.Durations.HALF; break;
                case "Note4": duration = Music.Durations.QUARTER; break;
                case "Note8": duration = Music.Durations.EIGHT; break;
            }

            Dot.IsEnabled = duration * 1.5 > songTimeSignature ? false : true;
            if (hasDot) Dot_MouseLeftButtonUp(Dot, null);
        }

        private void Rest_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            double d=0;

            switch ((sender as Grid).Name)
            {
                case "Rest1": d = Music.Durations.WHOLE; break;
                case "Rest2": d = Music.Durations.HALF; break;
                case "Rest4": d = Music.Durations.QUARTER; break;
                case "Rest8": d = Music.Durations.EIGHT; break;
            }

            if (RestClicked != null) RestClicked(d);
        }

        private void Dot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            hasDot = !hasDot;
            Grid grid = sender as Grid;
            grid.Style = (Style)this.FindResource("FlatButtonIndigo500");
            grid.Background = (SolidColorBrush)Application.Current.FindResource(hasDot ? "Indigo700" : "Indigo500");
        }

        private void ButtonPageDown_Click(object sender, RoutedEventArgs e)
        {
            if (PreviousPageClicked != null)
                TextBlockPage.Text = PreviousPageClicked().ToString();
        }
        private void ButtonPageUp_Click(object sender, RoutedEventArgs e)
        {
            if (NextPageClicked != null)
                TextBlockPage.Text = NextPageClicked().ToString();
        }
    }
}
