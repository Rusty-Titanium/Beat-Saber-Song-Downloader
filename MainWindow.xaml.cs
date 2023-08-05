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
        /// for the dates, the next one I should be doing is from 9/1/22 to 10/1/22 but changing to update myself on the newer shit first.
        /// 
        /// - apparently there is wpf integrated spash screen loading. my apps dont really need this cause they speed but look anyway.
        /// 
        /// 
        /// - Bug: if something is still somehow in the downloader folder after attempt at closing, the program crashes. Please fix. I already noted in here where it breaks.
        /// 
        /// - OPTIONAL: If I like a song but I don't like the mapping, I can add a thing were I can hyperlink the names, and if you click the name it could ask
        ///         if you want to search for songs under said name.
        /// 
        /// - OPTIONAL: make tags slightly smaller in both size and maybe font?  Also color tags that are part of the search list.
        /// 
        /// - OPTIONAL: Add in a query text box where I can search for specific songs. Would need to find a way to make sure that settings dont save if 
        ///         I'm just doing a quick search.
        /// 
        /// - OPTIONAL: When downloading a song, there can be a item to toggle or maybe a third button (buton on the duplicate and complete border) that
        ///         mentions that if you click it, you can will automatically do a search of the song name (only by name, no other parameters). I would 
        ///         prefer for this to take place in another window so that it doesn't fuck with my other stuff, and can be easily closed afterwards. This
        ///         allows user to try the same song from different mappers if they genuinely enjoyed the preview and know they want to play it.
        /// 
        /// - OPTIONAL: Create a universal volume slider that is given the media in question once the button is pressed. Until then, deafult loudness
        /// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-control-a-mediaelement-play-pause-stop-volume-and-speed?view=netframeworkdesktop-4.8
        /// 
        /// the 'q' variable in the api seems to be searching for anything text related. users name, description, song name, everything
        /// 
        /// 
        /// 
        /// ALSO MASSIVE NOTE: I THINK I STARTED THIS AROUND 3 MONTHS AGO? SO LIKE WHEN CHECKING OUT SONGS AGAIN FRO NEWER SHIT CHECK FEBRUARY 2023 AND NEWER.
        /// 
        /// Note / bug: it is possible to save bad values to the download settings. I will need a way to fix this I think. maybe like a "last good date" or something like that with
        ///                     all those values.
        ///                     
        /// Note: weirdness with tags. There are the traditional tags like rock and shit, but then there are tags like speed and tech. These need to be in their own colored category
        ///         and added in appropriately. I also want to revamp the current tag system so that it 1. actually includes all tages regardless of my tastes, and 2. makes that tag distinction.
        /// 
        /// Note: I want to be able to look up names of these songs, in the event I found a good song but a shit map of it, I want to be able to search up if there are other
        ///         maps that mapped that song so I can possibly have a better time.
        /// 
        /// 
        /// 
        /// Note: If I want to look up songs by name in the future, I may want to add functionality so that you can turn off the saving mode so it wont save to the settings file
        ///         and be able to click the button to quickly get to where you left off. This could also be an excuse to do some form of databinding in the SongControl class. Maybe
        ///         When doing an action like this, it opens a new window so as to not disrupt the main page.
        /// 
        /// 
        /// 
        /// bug: its possible to be too quick when changing screens and songs from last page are added to the new one. Disable search buttons until
        ///         the songs are all added in (while I'll need to figure out how to do that)
        /// 
        /// 
        /// visual bug: when you scroll all the way down, there isn't any spacing on the last item, making it look awkward. find a way to get spacing to that last item.
        /// 
        /// possible inconsistency: So the way the song control is currently programmed is to dynamically make itself taller if it needs too. Though good, I think there are
        ///         definitely a couple ways to break how that ui looks if that stretching does happen. I think I should look into fixing the ui up a bit more if I wanted to deal with that.
        /// 
        /// but/fix later: I hard coded locations on my own computer for this program. Please make it so it's possible to choose them initially so the program can save them.
        /// 
        /// 
        /// 
        /// note for design: I might at some point try to tighten up the visuals of the Song Visual, aka attempting to make it less tall while keeping the same amount of 
        ///         info on it. I might be adding in more info soon so I have to hold off this idea. Also if I do this, it means that the images will need to be smaller, so
        ///         maybe I'll add in a way to click on the image to enlarge itself in the UI to give the preson a better look.
        /// 
        /// 
        /// 
        /// note to test at some point: it's possible for the information stuffs next to the button to maybe change width and I don't really want that if I can help it because it 
        ///         would make the visuals look weird, even if they are edge cases.
        /// 
        /// 
        /// Note: when checking the tags, I need to change the code so that it isn't going off of the color of the backgrounds, because thats going to fuck with alot of design decisions.
        /// 
        /// 
        /// add-on: I'm thinking that to show I can databind, I might make a class for the Song Control for it to databind too.
        /// 
        /// note: would like to do more testing around clipinbounds value as it would be nice to figure out how it can be used, how it works and so on.
        /// 
        /// note: combobox has the slightest of gradient colors on it. I edited it to be slightly darker to better match the button background color. I may remove this in the future
        ///         or just leave in to give me an example of how the gradient stuff works. at a note there are no gradient stuffs in the theme file.
        ///         
        /// note: would like to test out one more type of style for the tag shit. Might just leave for now until I come back to change design again, as I don't want to 
        ///         be making tom uch work for myself if I dont need too.
        /// 
        /// note: fix the look of the scrollbar. the thumb does't have borders but the scroll buttons and the track does for some reason. Might want to fix.
        /// Note: doesn't have to be now but should be at some point. the border looks weird when both scrollbars are visible (mainly the horizontal one). 
        ///         It's pretty jarring. I either need to see if its fixable or not. This note may need to be moved to the theme file itself in the event I don't handle it here.
        ///         A problem that could arise is that the only way to fix it is to completely redesign the corners to allow nice looking rounded corners. Not sure how to go about it yet.
        /// 
        /// Note: Implemented smoother scrolling, but it can seem both too fast and too slow at the same time. I want to work on making it feel as smooth as possible so 
        ///         at some point please mess with the parameters at some point thanks.
        /// Note: I also saved the webpage for the control that I'm using for the scrollviewer. I plan on editing it quite a bit so if I break something and need the 
        ///         original code again I can just go back to that webpage.
        /// 
        /// Note: I should make note that the listbox methods for the smoother scrolling does not work on the listview control. I may want to find a way to just inject the 
        ///         smooth scroll into everything if I can figure out how to program that properly.
        /// 
        /// Note: scroll view animation thingy scrolls down too fast when items are loading in. Not sure this is fixable but please look into it. also when tryin to 
        ///         free spin scroll it can sometimes hang up and like lag behind or something. Not really sure what is happening so I want to see if its fixable.
        /// 
        /// Note: on the listbox stuffs, it seems if I attempt to scroll fast enough when its all the way at the top it can just teleport to the bottom. maybe I need
        ///         some way of having a max speed on these things?
        /// 
        /// Note: Try to remember to not remove all the version of loading in the SongControls stuffs. I want to add a timer in here and just see how much time I save or lose
        ///         depending on the method used.
        /// 
        /// Note: Tried messing with the cliptobounds stuffs with the songControl again, but it seems like the answer is a bit more complicated than just a simple fix, so I'm not doing it.
        /// 
        /// 
        /// Note: WHEN YOU WORK ON THE LOADING OF THE SONG CONTROLS, I WANT TO ALSO CHECK FOR PERFORMANCE DIFFERNCES.
        /// 
        /// 
        /// Note: when changing pages, lets assume the scrollbar was anywhere but the top (i.e. the bottom). My current code resets the scrollbar to the top when it needs
        ///         to load more children, but the value from the smooth scrolling thing does not get reset. So when you scroll next time, it forces the current thumb location
        ///         to be at the bottom again, causing a very jarring glitch.
        /// 
        /// 
        /// Note: Never tested whether 2 or more scrollviews and or bars would fuck up the smooth scrolling thingy.
        /// 
        /// 
        /// 
        /// Note: Arrow keys left and right dont have smooth scrolling when trying to go horizontal. up and down don't even do anything. Please fix.
        /// 
        /// </summary>

        // tags that I will probably use the most
        // "anime|dance|Drum and Bass|electronic|instrumental|j-pop|jazz|swing|video game|vocaloid"

        //private Brush mainBorderColor, mainBackgroundColor;
        //private Brush selectedBorderColor = Brushes.Black, selectedBackgroundColor = Brushes.Green;
        public List<String> idList = new List<String>();

        private static readonly HttpClient client = new HttpClient();


        public MainWindow()
        {
            InitializeComponent();

            String jsonString = File.ReadAllText(Directory.GetCurrentDirectory() + "/DownloadSettings.json");
            DownloadSettings settings = JsonSerializer.Deserialize<DownloadSettings>(jsonString);

            fromPicker.SelectedDate = settings.From;
            toPicker.SelectedDate = settings.To;
            chromaBox.Text = settings.Chroma;
            noodleBox.Text = settings.Noodle;
            curatedBox.Text = settings.Curated;
            verifiedBox.Text = settings.Verified;
            pageBox.Text = settings.Page;



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

            if (Directory.Exists("C:/Users/Richi/Desktop/Beat Saber New Songs")) // THIS IS WHERE THE PROGRAM CRASHES IF THERE IS A FILE IN THE DOWNLOADER FILE.
                Directory.Delete("C:/Users/Richi/Desktop/Beat Saber New Songs");

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
            ScrollAnimationBehavior.intendedLocation = 0; // remember to note this in the bug fix.
            
            foreach (SongControl control in songPanel.Children)
                control.FadeOut();

            await Task.Delay(500); // delay added so that the fadeout animation can complete before they are removed from the songPanel


            scrollView.ScrollToTop();
            scrollView.ScrollToLeftEnd(); // technically not needed as I'm going to be fixing the horizontal stuff eventually but I guess until then this can stay.
            songPanel.Children.Clear(); // This could technically be called extremely late (like right about to add the new ones late).
            SongControl.oldSongControl = null; // might be removed in the future not sure.


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

            Thread thread = new Thread(() => { Api_Call(pageNumber, fromDate, toDate, chroma, noodle, curated, verified, tagList); });
            thread.Start();
        }


        // version of this method with threading commented out just so i can juse to program.
        // in this version the from and todate values will be assumed they are not null.
        private async void Api_Call(int pageNumber, DateTime fromDate, DateTime toDate, String chroma, String noodle, String curated, String verified, List<String> tagList)
        {
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

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(req);
            String jsonString = response.Content.ReadAsStringAsync().Result;
            var jsonDoc = JsonDocument.Parse(jsonString);
            var docs = jsonDoc.RootElement.GetProperty("docs");

            bool didRun = false;


            

            foreach (var doc in docs.EnumerateArray())
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


                bool verifiedMapper;
                bool isCurated;

                try { verifiedMapper = doc.GetProperty("uploader").GetProperty("verifiedMapper").GetBoolean(); }
                catch (KeyNotFoundException e2) { verifiedMapper = false; }

                try { isCurated = doc.GetProperty("curator").GetProperty("curator").GetBoolean(); }
                catch (KeyNotFoundException e3) { isCurated = false; }
                
                
                // The await operator here is what allows the controls to load in one at a time smoothly instead of all at once.


                

                // remember that Invoke runs things synchronously where as BeginInvoke runs things async.
                // also I believe the await on the thing here can be removed once its changed back to just Invoke.
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SongControl song = new SongControl(name, uploader, duration, upvotes, downvotes, uploaded, download, cover, preview, tags, id, songName, levelAuthorName, verifiedMapper, isCurated, isChroma, isNoodle);
                    songPanel.Children.Add(song);
                }));

                //await Task.Delay(70); // This pause is here to mimic to speed at which the controls are removed.
                //await Task.Delay(70);



                /**
                // So this is working the way I think. It is done asynchronously, so its loading everything after the next one, this includes grabbing the image from the 
                //      internet each time, meaning more time is being taken for each image download.
                this.Dispatcher.Invoke(DispatcherPriority.Background, new Action( () =>
                {
                    SongControl song = new SongControl(name, uploader, duration, upvotes, downvotes, uploaded, download, cover, preview, tags, id, songName, levelAuthorName, verifiedMapper, isCurated, isChroma, isNoodle);
                    songPanel.Children.Add(song);
                }));

                await Task.Delay(70); // this pauses execution of code, however doesn't make it inactive, meaning the app is still usable during this state.
                // it seems like this task delay method has the unintended consequence of allowing the ui to update as it gets loaded, which is great.
                 */
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
            int pageNumber = int.Parse(pageBox.Text) - 1;

            if (pageNumber < 0)
            {
                Api_Start(0);
                pageBox.Text = "0";
            }
            else
            {
                Api_Start(pageNumber);
                pageBox.Text = (int.Parse(pageBox.Text) - 1).ToString();
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
