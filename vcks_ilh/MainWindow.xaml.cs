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

using System.IO;
using System.Printing;
using System.Threading;
using Microsoft.Win32;

namespace vcks_ilh
{
    public partial class MainWindow : Window
    {
        Player player;
        Song song;

        public MainWindow()
        {
            InitializeComponent();            
            fretboard.NotePlayed += Fretboard_NotePlayed;
            modePanel.RestClicked += ModePanel_RestClicked;
            modePanel.NextPageClicked += ()=> { return song != null ? sheetWindow.SwitchToNextPage(song): 0; };
            modePanel.PreviousPageClicked += () => { return song != null ? sheetWindow.SwitchToPreviousPage(song): 0; };
        }

        private void ModePanel_RestClicked(double duration)
        {
            if (song != null && sheetWindow.IsLastPageDisplayed) song.AddMusicalObject(new MusicalObject [] {new Rest(duration)});
        }

        private void Fretboard_NotePlayed(uint[] note)
        {
            if (song != null && sheetWindow.IsLastPageDisplayed)
            {
                MusicalObject[] mo;
                ModePanel.Response state = modePanel.GetState();
                mo = note.Length == 1 ? new MusicalObject[] { new Note(note[0], state.duration,state.hasDot) }: new MusicalObject[] { new Note(note[0], state.duration, state.hasDot), new Note(note[1], state.duration, state.hasDot) };
                player.PlaySingleMusicalObject (mo);
                song.AddMusicalObject(mo);
            }
        }


        private void gridForMoving_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            if(player!= null) player.Close();
            this.Close();
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
               

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            switch(menuItem.Header.ToString())
            {
                case "Создать":
                    Create create = new Create();
                    create.SongCreated += Create_SongCreated;  
                    create.ShowDialog();
                    break;
                case "Открыть":
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "VCKS files (*.vcks)|*.vcks";
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (openFileDialog.ShowDialog()==true)
                    {
                        Song s = Serializer.Deserialize(openFileDialog.FileName);
                        if (s != null)
                        {
                            Create_SongCreated(s);
                            sheetWindow.DrawEntireSong(s);
                        }
                    }
                    break;
                case "Сохранить":
                    if (song != null)
                    {
                        SaveFileDialog saveFileDialogVcks = new SaveFileDialog();
                        saveFileDialogVcks.Filter = "VCKS files (*.vcks)|*.vcks";
                        saveFileDialogVcks.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        if (saveFileDialogVcks.ShowDialog() == true) Serializer.Serialize(song, saveFileDialogVcks.FileName);
                    }
                    break;
                case "Экспорт":
                    if (song != null)
                    {
                        SaveFileDialog saveFileDialogPng = new SaveFileDialog();
                        saveFileDialogPng.Filter = "PNG files (*.png)|*.png";
                        saveFileDialogPng.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        if (saveFileDialogPng.ShowDialog() == true) sheetWindow.ExportToPNG(song, saveFileDialogPng.FileName);
                    }
                    break;
                case "Печать":
                    if (song != null)
                    {
                        PrintDialog printDialog = new PrintDialog();
                        if (printDialog.ShowDialog() == true) printDialog.PrintVisual(sheetWindow.GetSheetView(), "Происходит печать нотного листа");
                    }
                    break;
                case "Отменить":
                    if (song != null && sheetWindow.IsLastPageDisplayed) song.RemoveLastMusicalObject();
                    break;
                case "Играть":
                    if(song!=null && player!=null)
                        if(!Player.IsSongPlaying) player.PlayEntireSong(song.GetAllMusicalObjectsInLine());
                    break;
                case "Остановить":
                    if (song != null && player != null) player.StopPlayingSong();
                    break;
                case "Смена темпа":
                    if (song != null && player != null)
                    {
                        ChangeTemp changeTemp = new ChangeTemp(song.temp);
                        changeTemp.TempChanged += (t)=> { song.ChangeTemp(t); player = new Player(song); };
                        changeTemp.ShowDialog();
                    }
                        break;
                case "О программе":
                    About about = new About();
                    about.ShowDialog();
                    break;
            }
        }

        private void Create_SongCreated(Song song)
        {
            this.song = song;
            sheetWindow.CreateNewSheet(song);
            modePanel.Init(song.timeSignature);
            sheetWindow.NewPageCreated+= modePanel.SetPageNumber;
            sheetWindow.PageRemoved += modePanel.SetPageNumber;
            player = new Player(song);
            song.Changed += sheetWindow.DrawBar;
            song.BarRemoved += sheetWindow.RemoveLastBar;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
                if (song != null && sheetWindow.IsLastPageDisplayed)
                    song.RemoveLastMusicalObject();
        }
    }
}
