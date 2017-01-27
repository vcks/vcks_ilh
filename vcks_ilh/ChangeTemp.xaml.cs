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
    public partial class ChangeTemp : Window
    {
        public delegate void TempChangedEventHandler(uint temp);
        public event TempChangedEventHandler TempChanged;
        public ChangeTemp(uint temp)
        {
            InitializeComponent();
            TextBlockTemp.Text = temp.ToString();
        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            uint temp = (uint)int.Parse(TextBlockTemp.Text);
            if (TempChanged != null) TempChanged(temp);
            this.Close();
        }

        private void ButtonTempUp_Click(object sender, RoutedEventArgs e)
        {
            int val = int.Parse(TextBlockTemp.Text);
            if (val < 240) TextBlockTemp.Text = (val + 5).ToString();
        }

        private void ButtonTempDown_Click(object sender, RoutedEventArgs e)
        {
            int val = int.Parse(TextBlockTemp.Text);
            if (val > 60) TextBlockTemp.Text = (val - 5).ToString();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
