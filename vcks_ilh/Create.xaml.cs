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
using System.Windows.Shapes;

namespace vcks_ilh
{
    public partial class Create : Window
    {
        public delegate void SongCreatedEventHandler(Song song);
        public event SongCreatedEventHandler SongCreated;

        public Create()
        {
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonTempUp_Click(object sender, RoutedEventArgs e)
        {
            int val = int.Parse(TextBlockTemp.Text);
            if (val < 240)
                TextBlockTemp.Text = (val + 5).ToString();
        }

        private void ButtonTempDown_Click(object sender, RoutedEventArgs e)
        {
            int val = int.Parse(TextBlockTemp.Text);
            if (val > 60)
                TextBlockTemp.Text = (val - 5).ToString();
        }

        private void ButtonSharpUp_Click(object sender, RoutedEventArgs e)
        {
            TextBlockFlats.Text = "0";

            int val = int.Parse(TextBlockSharps.Text);
            if (val < 7)
                TextBlockSharps.Text = (val + 1).ToString();
        }

        private void ButtonSharpDown_Click(object sender, RoutedEventArgs e)
        {
            TextBlockFlats.Text = "0";
            int val = int.Parse(TextBlockSharps.Text);
            if (val > 0)
                TextBlockSharps.Text = (val - 1).ToString();
        }

        private void ButtonFlatUp_Click(object sender, RoutedEventArgs e)
        {
            TextBlockSharps.Text = "0";
            int val = int.Parse(TextBlockFlats.Text);
            if (val < 7)
                TextBlockFlats.Text = (val + 1).ToString();
        }

        private void ButtonFlatDown_Click(object sender, RoutedEventArgs e)
        {
            TextBlockSharps.Text = "0";
            int val = int.Parse(TextBlockFlats.Text);
            if (val > 0)
                TextBlockFlats.Text = (val - 1).ToString();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            string title = TextBoxTitle.Text.Trim();
            title = title == string.Empty ? "Untitled" : title;
            string author = TextBoxAuthor.Text.Trim();
            author = author == string.Empty ? "Unknown" : author;
            uint temp = (uint)int.Parse(TextBlockTemp.Text);
            uint sharps = uint.Parse(TextBlockSharps.Text);
            uint flats = uint.Parse(TextBlockFlats.Text);
            double timeSignature = double.Parse(GridTimeSigns.Uid);

            Song song = new Song(title, author, timeSignature, temp, flats, sharps);
            if (SongCreated != null) SongCreated(song);
            this.Close();
        }

        private void TimeSignClicked(object sender, MouseButtonEventArgs e)
        {
            StackPanel sp = sender as StackPanel;

            GridTimeSigns.Uid = sp.Uid;

            foreach (StackPanel s in GridTimeSigns.Children)
                s.Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 1 };

            sp.Effect = null;
        }
       
    }
}
