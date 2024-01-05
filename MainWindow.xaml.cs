using Beat_Saber_Song_Downloader.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Beat_Saber_Song_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// 
        /// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// 
        /// CURRENT Section Stuff in my search:
        /// Dates: 3/1/2023 - 4/1/2023
        /// Tags: Anime, Dance, drum-and-bass, instrumental, j-pop, j-rock, swing, video-game-soundtrack, vocaloid
        /// Page: 0
        /// 
        /// When doing older searches of same params, it's from 9/1/22 to 10/1/22 (this is when going backwards)
        /// 
        /// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// 
        /// 
        /// 
        /// Additions:
        /// - (OPTIONAL) If I like a song but I don't like the mapping, I can add a thing were I can hyperlink the names, and if you click the name it could ask
        ///         if you want to search for songs under said name.
        /// - (OPTIONAL) make tags slightly smaller in both size and maybe font?  Also color tags that are part of the search list.
        /// - (OPTIONAL) Add in a query text box where I can search for specific songs. Would need to find a way to make sure that settings dont save if 
        ///         I'm just doing a quick search. You can find the link to the api in the API_Call method
        /// (Related) The 'q' variable in the api seems to be searching for anything text related. users name, description, song name, everything
        /// 
        /// - (SUPER OPTIONAL BUT COOL IDEA) When downloading a song, there can be a item to toggle or maybe a third button (button on the duplicate and complete border) that
        ///         mentions that if you click it, you can will automatically do a search of the song name (only by name, no other parameters). I would 
        ///         prefer for this to take place in another window so that it doesn't fuck with my other stuff, and can be easily closed afterwards. This
        ///         allows user to try the same song from different mappers if they genuinely enjoyed the preview and know they want to play it.
        /// 
        /// - (OPTIONAL) Create a universal volume slider that is given the media in question once the button is pressed. Until then, deafult loudness
        /// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed?view=netframeworkdesktop-4.8
        /// 
        /// - (Optional) There are the traditional tags like rock, hiphop, etc, but then there are tags like speed and tech. I would like for these to be in their own colored category
        ///         and added in appropriately. I also want to revamp the current tag system so that it 1: Actually includes all tages regardless of my tastes, and 2: Makes that tag distinction.
        ///         Use https://beatsaver.com/ filter stuffs to get a better idea of the tages that are usable.
        /// - (Optional) There are mods like movie player and shit like that which might have their own tags or way to check if they have them. If they do, I want to implement those as well just 
        ///         to keep the information as accurate as possible.
        /// - (Back-End Design) I'm thinking that to show I can databind, I might make a class for the Song Control for it to databind too.
        /// - (Front-End Design) Tighten up SongControl visuals. I want to add more info regarding available difficulties and basic information regarding them so user can make informed decision before downloading. 
        ///         Probably other stuff I'll add as well but can't recall atm. Also if I do this, it means that the images will need to be smaller, so  add in a way to hover the image to enlarge 
        ///         itself in the UI to give the presentation a better look.
        /// - (Super Optional) when changing pages, have the scrollviewer focused so I can use arrows immediately and not have to click on the control again.
        /// - (Optional but nice) Once you come across a return of not 20 things, the code should see that, and keep next click perpetually disabled, as well as now checking to stop the page number
        ///         from going any higher.
        /// 
        /// 
        /// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// 
        /// 
        /// Bugs:
        /// - it is possible to save bad values to the download settings (like a bad format that is never checked). I will need a way to fix this I think. 
        ///         Maybe like a "last good date" or something like that with all those values.
        /// - I hard coded locations on my own computer for this program. Please make it so it's possible to choose them initially so the program can save them, as well as somewhere in the 
        ///         program to be able to update them later.
        /// - Labeling as bug - Current way I deal with starting and stopping the audio on a SongControl is pretty scuffed and does rarely break from my knowledge. I should really figure out a 
        ///         better way to deal with this, especially if I eventually add a volume slider to the mix.
        /// - App is freezing when downloading a song, which means I don't put it to another thread which honestly I should be doing. I believe I did it in the past, however I remember it being
        ///         a complete mess of code for it to work, so I reverted it. I should revisit at some point and see if I can't do it better.
        /// - Currently I have to manually move the page back to 0 before searching, Honestly if I click the search button, it should immediately set it to 0 I think, or have a better way of changing
        ///         pages like how most websites do it these days (with like the arrows and numbers and shit.) Only problem is that there is no good way of figuring out how many pages there are. I guess
        ///         I could do multithreading and do steps of like 5 and just figure out where the fuck the last page is and go from there, but that would make the initial search pretty long and not sure
        ///         if I am willing to do that.
        /// - Fix the look of the scrollbar. the thumb does't have borders but the scroll buttons and the track does for some reason. Might want to fix.
        /// 
        /// 
        /// Notes:
        /// - (Front-End Deisgn) I also saved the webpage for the control that I'm using for the scrollviewer in Chrome Bookmarks. I plan on editing it quite a bit so if I break something and need the 
        ///         original code again I can just go back to that webpage.
        /// 
        /// 
        /// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// 
        /// 
        /// Git-hub Notes to include:
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>

        private int previousPageNumber;
        private static String currentApiUrl;

        public List<String> idList = new List<String>();

        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            SongControl.mainWin = this;

            String jsonString = File.ReadAllText(Directory.GetCurrentDirectory() + "/DownloadSettings.json");
            DownloadSettings settings = JsonSerializer.Deserialize<DownloadSettings>(jsonString);

            fromPicker.SelectedDate = settings.From;
            toPicker.SelectedDate = settings.To;
            chromaBox.Text = settings.Chroma;
            noodleBox.Text = settings.Noodle;
            curatedBox.Text = settings.Curated;
            verifiedBox.Text = settings.Verified;
            pageBox.Text = settings.Page;
            previousPageNumber = int.Parse(settings.Page);


            foreach (ToggleButton tbutton in tagPanel.Children)
            {
                foreach (String tag in settings.Tags)
                {
                    if (tbutton.Content.ToString() == tag)
                    {
                        tbutton.IsChecked = true;
                        break;
                    }
                }
            }

            // original. commented out for new filepath.
            //List<String> subDir = Directory.GetDirectories("D:/Steam/steamapps/common/Beat Saber/Beat Saber_Data/CustomLevels/").Select(System.IO.Path.GetFileName).ToList();
            List<String> subDir = Directory.GetDirectories("C:/Users/Richi/Desktop/BSManager/BSInstances/1.28.0/Beat Saber_Data/CustomLevels/").Select(System.IO.Path.GetFileName).ToList();

            foreach (String fileName in subDir)
            {
                String idFile = fileName.Substring(0, fileName.IndexOf(" "));

                idList.Add(idFile);
            }


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Api_Start(int.Parse(pageBox.Text));
        }


        private void Window_Closing(object sender, EventArgs e)
        {
            // this needs to change becaues of the whole zipfile shit. I think I should keep it like this for now just for safety reasons

            if (Directory.Exists("C:/Users/Richi/Desktop/Beat Saber New Songs"))
            {
                foreach (String filePath in Directory.GetFiles("C:/Users/Richi/Desktop/Beat Saber New Songs")) // Deletes all files in Directory before directory is deleted.
                    File.Delete(filePath);

                Directory.Delete("C:/Users/Richi/Desktop/Beat Saber New Songs");
            }

            DownloadSettings settings = new DownloadSettings();

            settings.From = fromPicker.SelectedDate;
            settings.To = toPicker.SelectedDate;
            settings.Chroma = chromaBox.Text;
            settings.Noodle = noodleBox.Text;
            settings.Curated = curatedBox.Text;
            settings.Verified = verifiedBox.Text;
            settings.Page = pageBox.Text;

            foreach (ToggleButton tbutton in tagPanel.Children)
            {
                if (tbutton.IsChecked == true)
                    settings.Tags.Add((String)tbutton.Content);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            String jsonString = JsonSerializer.Serialize(settings, options);

            File.WriteAllText(Directory.GetCurrentDirectory() + "/DownloadSettings.json", jsonString);
        }


        // async got added automatically from calling an await thing
        private async void Api_Start(int pageNumber)
        {
            // I never check here if they are null or not, or just straight invalid. Need to test this and fix accordingly
            DateTime fromDate = (DateTime)fromPicker.SelectedDate;
            DateTime toDate = (DateTime)toPicker.SelectedDate;

            String chroma = chromaBox.Text;
            String noodle = noodleBox.Text;
            String curated = curatedBox.Text;
            String verified = verifiedBox.Text;

            // code to get a list of tags

            List<String> tagList = new List<String>();

            foreach (ToggleButton tbutton in tagPanel.Children)
            {
                if (tbutton.IsChecked == true) //selected
                    tagList.Add(tbutton.Content.ToString());
            }


            // Start of URL creation

            String url = "https://api.beatsaver.com/search/text/" + pageNumber + "?sortOrder=Latest";

            url += "&from=" + fromDate.Year + "-" + String.Format("{0:00}", fromDate.Month) + "-" + String.Format("{0:00}", fromDate.Day);
            url += "&to=" + toDate.Year + "-" + String.Format("{0:00}", toDate.Month) + "-" + String.Format("{0:00}", toDate.Day);

            if (chroma == "true")
                url += "&chroma=true";
            else if (chroma == "false")
                url += "&chroma=false";

            if (noodle == "true")
                url += "&noodle=true";
            else if (noodle == "false")
                url += "&noodle=false";

            if (curated == "true")
                url += "&curated=true";
            else if (curated == "false")
                url += "&curated=false";

            if (verified == "true")
                url += "&verified=true";
            else if (verified == "false")
                url += "&verified=false";

            String tagURL = "&tags=";

            foreach (String tag in tagList)
                tagURL += tag + "|";

            if (tagURL != "&tags=")
                url += tagURL.Substring(0, tagURL.Length - 1);

            //Debug.WriteLine(url); // this is just here so I can look at the url directly if I ever needed to for some reason. Will stay commented out until it is needed again.

            if (currentApiUrl != null && currentApiUrl.Equals(url))
            {
                Debug.WriteLine("SAME URL SO NOTHING CHANGED\nSAME URL SO NOTHING CHANGED\nSAME URL SO NOTHING CHANGED\nSAME URL SO NOTHING CHANGED\nSAME URL SO NOTHING CHANGED\n");

                // if this is true, this should either give some error on screen, or just do nothing, not sure just yet. should prob say something at least in the Text Output
            }
            else
            {
                //Debug.WriteLine("nope not same");

                searchButton.IsEnabled = false;
                previousButton.IsEnabled = false;
                nextButton.IsEnabled = false;

                foreach (SongControl control in songPanel.Children)
                    control.FadeOut();

                await Task.Delay(550); // delay added so that the fadeout animation can complete before they are removed from the songPanel

                ScrollAnimationBehavior.SetIntendedLocation(scrollView, 0);
                scrollView.ScrollToTop();
                scrollView.ScrollToLeftEnd(); // technically not needed as I'm going to be fixing the horizontal stuff eventually but I guess until then this can stay.
                songPanel.Children.Clear(); // This could technically be called extremely late (like right about to add the new ones late).
                SongControl.oldSongControl = null; // might be removed in the future not sure. Don't think I can remove this. as I believe this stops the song from playing after page change.
                SongControl.ImageLoadedControlCount = 0;

                //GC.Collect(); // If at any point this shit breaks on me again, uncomment this
                

                currentApiUrl = url;

                Thread thread = new Thread(() => { Api_Call(url); });
                thread.Start();
            }
        }


        // https://api.beatsaver.com/docs/index.html?url=./swagger.json
        // Link so I can reference this back at any point.
        // version of this method with threading commented out just so i can juse to program.
        // in this version the from and todate values will be assumed they are not null.
        private async void Api_Call(String url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(req);
            String jsonString = response.Content.ReadAsStringAsync().Result;
            var jsonDoc = JsonDocument.Parse(jsonString);
            var docs = jsonDoc.RootElement.GetProperty("docs");

            bool didRun = false;

            foreach (var doc in docs.EnumerateArray()) // This loop creates SongControls
            {
                didRun = true;

                String name = doc.GetProperty("name").GetString();
                String uploader = doc.GetProperty("uploader").GetProperty("name").GetString();
                int duration = doc.GetProperty("metadata").GetProperty("duration").GetInt32();
                int upvotes = doc.GetProperty("stats").GetProperty("upvotes").GetInt32();
                int downvotes = doc.GetProperty("stats").GetProperty("downvotes").GetInt32();
                DateTime uploaded = doc.GetProperty("uploaded").GetDateTime();

                String download = "";
                String cover = "";
                String preview = "";

                foreach (var version in doc.GetProperty("versions").EnumerateArray())
                {
                    download = version.GetProperty("downloadURL").GetString();
                    cover = version.GetProperty("coverURL").GetString();
                    preview = version.GetProperty("previewURL").GetString();
                }

                List<String> tags = new List<String>();

                try
                {
                    foreach (var tag in doc.GetProperty("tags").EnumerateArray())
                        tags.Add(tag.GetString());
                }
                catch (KeyNotFoundException e1)
                {
                    // just does nothing rn
                }

                String id = doc.GetProperty("id").GetString();
                String songName = doc.GetProperty("metadata").GetProperty("songName").GetString();
                String levelAuthorName = doc.GetProperty("metadata").GetProperty("levelAuthorName").GetString();

                bool isChroma = false, isNoodle = false;


                // loops through each difficulty to check if it uses chroma or noodle
                foreach (var difficulty in doc.GetProperty("versions")[0].GetProperty("diffs").EnumerateArray())
                {
                    if (difficulty.GetProperty("chroma").GetBoolean() == true)
                        isChroma = true;

                    if (difficulty.GetProperty("ne").GetBoolean() == true)
                        isNoodle = true;
                }

                bool verifiedMapper, isCurated;

                try { verifiedMapper = doc.GetProperty("uploader").GetProperty("verifiedMapper").GetBoolean(); }
                catch (KeyNotFoundException e2) { verifiedMapper = false; }

                try { isCurated = doc.GetProperty("curator").GetProperty("curator").GetBoolean(); }
                catch (KeyNotFoundException e3) { isCurated = false; }



                // Remember that Invoke runs things synchronously where as BeginInvoke runs things async. Also I believe the await on the thing here can be removed once its changed back to just Invoke.
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SongControl song = new SongControl(name, uploader, duration, upvotes, downvotes, uploaded, download, cover, preview, tags, id, songName, levelAuthorName, verifiedMapper, isCurated, isChroma, isNoodle);
                    songPanel.Children.Add(song);
                }));
            }

            if (!didRun)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    noresultGrid.Visibility = Visibility.Visible;
                }));
            }

        }
        



        private void Api_Click(object sender, RoutedEventArgs e)
        {
            Api_Start(int.Parse(pageBox.Text));
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            int currentPageNumber = int.Parse(pageBox.Text) - 1;

            if (currentPageNumber >= 0)
            {
                Api_Start(currentPageNumber);
                pageBox.Text = currentPageNumber.ToString();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Api_Start(int.Parse(pageBox.Text) + 1);
            pageBox.Text = (int.Parse(pageBox.Text) + 1).ToString();
        }


        private void No_Result_Click(object sender, RoutedEventArgs e)
        {
            noresultGrid.Visibility = Visibility.Collapsed;
        }


        // currnetly commented out the code as I don't want to accidentally download 20 songs :)
        private void Download_All_Click(object sender, RoutedEventArgs e)
        {
            //foreach (SongControl song in songPanel.Children)
            //    song.Download_Song();
        }


        private void Reset_Tags_Click(object sender, RoutedEventArgs e)
        {
            // original. this method is being highjacked just so I can test something.

            foreach (ToggleButton tbutton in tagPanel.Children)
                tbutton.IsChecked = false;



        }

        private void pageBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }


        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private void pageBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

    }



}
