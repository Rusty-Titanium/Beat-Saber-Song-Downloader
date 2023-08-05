using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

namespace Beat_Saber_Song_Downloader
{
    /// <summary>
    /// Interaction logic for SongControl.xaml
    /// </summary>
    public partial class SongControl : UserControl
    {


        private String downloadURL, id, songName, levelAuthorName, mediaPlayerURI;
        private bool isChroma, isNoodle;
        public static SongControl oldSongControl;
        private MediaPlayer mediaPlayer;



        public SongControl()
        {
            InitializeComponent();
        }


        public SongControl(String name, String uploader, int duration, int upvotes, int downvotes, DateTime uploaded, String download, String cover, 
            String preview, List<String> tags, String id, String songName, String levelAuthorName, bool verified, bool isCurated, bool isChroma, bool isNoodle)
        {
            
            //this.Visibility = Visibility.Hidden;

            InitializeComponent();

            BitmapImage bitmap = new BitmapImage(new Uri(cover));
            //bitmap.DownloadCompleted += (s, e) => { this.Visibility = Visibility.Visible; }; // maybe look into creating error thingy if it can't pull an image.
            imageControl.ImageSource = bitmap;

            nameBlock.Text = name;

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Volume = 0.2;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayerURI = preview;

            this.downloadURL = download;
            uploaderLabel.Content = uploader;
            durationLabel.Content = (duration / 60) + ":" + String.Format("{0:00}", duration % 60);
            upvoteLabel.Content = upvotes;
            downvoteLabel.Content = downvotes;
            publishedLabel.Content = uploaded.Year + "-" + uploaded.Month + "-" + uploaded.Day;

            foreach (String tag in tags)
            {
                Label label = new Label();
                label.Content = tag;
                label.FontSize = 14; // new
                label.VerticalContentAlignment = VerticalAlignment.Center; // new
                label.Padding = new Thickness(4); // new


                Border border = new Border();
                border.Background = Brushes.LightBlue;
                border.CornerRadius = new CornerRadius(16);
                border.Padding = new Thickness(4, 0, 4, 3);
                border.Margin = new Thickness(2);
                border.Child = label;

                tagWrapPanel.Children.Add(border);
            }

            this.id = id;
            this.songName = songName;
            this.levelAuthorName = levelAuthorName;
            this.isChroma = isChroma;
            this.isNoodle = isNoodle;

            if (isCurated)
                curatedPath.Visibility = Visibility.Visible;
            if (verified)
                verifiedPath.Visibility = Visibility.Visible;
            if (isChroma)
                chromaPath.Visibility = Visibility.Visible;
            if (isNoodle)
                noodlePath.Visibility = Visibility.Visible;

            //check id with the beginning strings 

            // does a check whether or not you have the song installed.

            foreach (String idFromList in ((MainWindow)Application.Current.MainWindow).idList)
            {
                if (this.id == idFromList)
                {
                    duplicateBorder.Visibility = Visibility.Visible;

                    break;
                }
            }
            

        }

        

        private void Preview_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;

            // original if (button.Content is System.Windows.Shapes.Path)
            if ((bool)button.IsChecked) // True if content of button is the play triangle button
            {
                if (oldSongControl != null && (bool)oldSongControl.playToggleButton.IsChecked)
                    oldSongControl.End_Media();

                button.IsChecked = true;

                if (mediaPlayer.Source == null)
                    mediaPlayer.Open(new Uri(mediaPlayerURI));

                mediaPlayer.Play();
                oldSongControl = this;
            }
            else // True means stop playing.
            {
                End_Media();
            }
        }
        

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            End_Media();
        }

        private void End_Media()
        {
            mediaPlayer.Stop();
            playToggleButton.IsChecked = false;
        }


        private void Download_Click(object sender, RoutedEventArgs e)
        {
            Download_Song();
        }


        public void Download_Song()
        {
            if (!Directory.Exists("C:/Users/Richi/Desktop/Beat Saber New Songs"))
                Directory.CreateDirectory("C:/Users/Richi/Desktop/Beat Saber New Songs");

            // file name = id (songName - levelAuthorName)
            String fileName = id + " (" + songName + " - " + levelAuthorName + ")";
            fileName = String.Join("", fileName.Split(System.IO.Path.GetInvalidFileNameChars()));

            String customSongPath = "C:/Users/Richi/Desktop/BSManager/BSInstances/1.28.0/Beat Saber_Data/CustomLevels/" + fileName;

            if (!Directory.Exists(customSongPath))
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(downloadURL, "C:/Users/Richi/Desktop/Beat Saber New Songs/" + fileName + ".zip");
                }

                String zipPath = "C:/Users/Richi/Desktop/Beat Saber New Songs/" + fileName + ".zip";

                ZipFile.ExtractToDirectory(zipPath, customSongPath);
                File.Delete(zipPath);

                downloadCompleteBorder.Visibility = Visibility.Visible;
            }
            else
            {
                duplicateBorder.Visibility = Visibility.Visible; // this is useless but I think its here in the event the button is somehow clickable, it still wont let you download it.
            }

            End_Media();

            // Add song ID to list
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            window.idList.Add(id);
        }

        public void FadeOut()
        {
            (FindResource("FadeOutAnimation") as Storyboard).Begin(this);
        }







    }
}
