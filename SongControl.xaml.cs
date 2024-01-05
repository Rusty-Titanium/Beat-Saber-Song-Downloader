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
using System.Security.Policy;

namespace Beat_Saber_Song_Downloader
{
    /// <summary>
    /// Interaction logic for SongControl.xaml
    /// </summary>
    public partial class SongControl : UserControl
    {
        /**
         * there is still mega hardcoding in this program. Please update it so I can have some way of saving it elsewhere.
         * 
         * 
         */

        private String downloadURL, id, songName, levelAuthorName, mediaPlayerURI;
        private bool isChroma, isNoodle;
        public static SongControl oldSongControl;
        private MediaPlayer mediaPlayer;

        public static int ImageLoadedControlCount = 0;
        public static MainWindow mainWin; // This is set at the beginning of the application runtime.


        public SongControl()
        {
            InitializeComponent();
        }


        public SongControl(String name, String uploader, int duration, int upvotes, int downvotes, DateTime uploaded, String download, String cover, 
            String preview, List<String> tags, String id, String songName, String levelAuthorName, bool verified, bool isCurated, bool isChroma, bool isNoodle)
        {
            
            this.Visibility = Visibility.Collapsed; // This is so the control is initially invisible for the animation.

            InitializeComponent();

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
            if (mainWin.idList.Contains(this.id))
                duplicateBorder.Visibility = Visibility.Visible;

            BitmapImage bitmap = new BitmapImage(new Uri(cover));
            imageControl.ImageSource = bitmap;
            bitmap.DownloadCompleted += (s, e) =>
            {
                ImageLoadedControlCount++;

                //somehow only this if statement is true once when in the output its clearly telling me that its true multiple times, meaning it should be running a lot, but isn't
                if (ImageLoadedControlCount == mainWin.songPanel.Children.Count) // if true, all images have loaded in.
                {
                    int startTimeMilli = 0, interval = 70;

                    foreach (SongControl control in mainWin.songPanel.Children)
                    {
                        Storyboard story = new Storyboard();
                        Storyboard.SetTarget(story, control);
                        
                        ObjectAnimationUsingKeyFrames objAnimation = new ObjectAnimationUsingKeyFrames();
                        Storyboard.SetTargetProperty(objAnimation, new PropertyPath(VisibilityProperty));
                        objAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, startTimeMilli);

                        DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame(Visibility.Visible, TimeSpan.FromSeconds(0.0));
                        objAnimation.KeyFrames.Add(keyFrame);
                        story.Children.Add(objAnimation);

                        
                        DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, 1.0, TimeSpan.FromSeconds(0.5));
                        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(OpacityProperty));
                        doubleAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, startTimeMilli);
                        story.Children.Add(doubleAnimation);
                        
                        story.Begin();

                        startTimeMilli += interval;
                    }


                    mainWin.searchButton.IsEnabled = true;
                    mainWin.nextButton.IsEnabled = true;
                    mainWin.previousButton.IsEnabled = true;
                }

            }; // maybe look into creating error thingy if it can't pull an image. Not sure what this is about honestly.

            /**
            // This code is mainly used for testing.
            this.Visibility = Visibility.Visible; // Only used for testing.

            mainWin.searchButton.IsEnabled = true;
            mainWin.nextButton.IsEnabled = true;

            if (int.Parse(mainWin.pageBox.Text) != 0)
                mainWin.previousButton.IsEnabled = true;
             */
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
            
            mainWin.idList.Add(id);
        }

        public void FadeOut()
        {
            (FindResource("FadeOutAnimation") as Storyboard).Begin(this);
        }







    }
}
