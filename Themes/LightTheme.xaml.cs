using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Beat_Saber_Song_Downloader.Themes
{
    /// <summary>
    /// Interaction logic for LightTheme.xaml
    /// </summary>
    public partial class LightTheme
    {
        /**
         * Note for this entire document:
         * 
         * Note: scroll view animation thingy scrolls down too fast when items are loading in. Not sure this is fixable but please look into it. also when tryin to 
         *         free spin scroll it can sometimes hang up and like lag behind or something. Not really sure what is happening so I want to see if its fixable.
         * Note: Never tested whether 2 or more scrollviews and or bars would fuck up the smooth scrolling thingy.
         * Note: Arrow keys left and right dont have smooth scrolling when trying to go horizontal. up and down don't even do anything. Please fix.
         * 
         * 
         * 
         * - Make sure to read through all of this documents commented sections, for either removal or notes that something needs to be done.
         * - I would like to eventually integrate the horizontal scrolling and vertical scrolling into a single class, but for now I have everything working and I might
         *      still make adjustments to things separately so for now it just makes things simpler, but please think about this for the future.
         * - 
         * - So I am starting to believe that there is like an acceleration and decceleration of the scroll to most other applications, instead of being instant like how this currently works. I think this is
         *      why I believe the scroll animation still looks a bit funky. I need to see if there is an algorithm out there for this. Think of it like each time I scroll it can 
         *      scroll faster but its like blocks of increased intensity and then just stops at the end, where I want more of like a gradient speed on both start and end movement.
         * 
         * Test for the note above: Record scrolling through obs or something so we can see if there is some form of accel/decceleration stuffs at work for things like chrome.
         * 
         * Test at some point: test for 2 things of horizontal scrolling to see if they conflict or not. (haven't yet still need to do this.)
         * 
         * note to look up later: can you add time to an animation while its currently running? (most likely a no. Will need a better way to make things look better.)
         * 
         * 
         * 
         * Bug/oversight: So horizontal scrolling has its own code stolen from the main code and found a weird bug with it. I should integrate normalized scrolling
         *                  or whatever the vertical version implements so values can't be larger than the max or smaller than the minimum(I implemented a bandaid fix for now). 
         *                  ScrollViewerPreviewMouseWheel is the class that stops the vertical scrolling from having the same issues. I will probably want to
         *                  merge the class soon. What i have currently put in place is a quick check for the horizontal movement for tilt and added functionality
         *                  to the Normalized position shit.
         *          
         * 
         * Note: I have at least either 3 or 4 spots that check for the min and mix of the scroll before doing an action, and setting values to 0 or max height/width if a value
         *          is too big/small. I should really just use that normalize scroll position method and use it for everyone, as I believe it can be used pretty universally.
         * 
         * Note: when I eventually merge these classes together, I want to make sure that scrolling both with the arrow keys and the thumb at the same time doesn't cause any weird
         *          jittering, I should probably have some form of system to check if another action is already happening.
         * Note: when you grab the thumb and start moving it, if you start scrolling in chrome, the thumb will disconnect from the mousedown and will only take input from the scrollwheel.
         * 
         * Bug: when pressing down and scrolling down at the same time, it gets a speed boost. Please disallow one or the other when both are happening.
         * 
         * When bug testing, make sure to go back and forth between tilt scrolling, scrolling, and arrow scrolling. Need to make sure that values aren't going above maximum
         *      or below minimum. Make sure there is no jerkiness and no weirdness. This note is mainly when I merge the tilt mouse wheel and smooth scrolling code together.
         */


        public void scrollerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollAnimationBehavior.scrollerLoaded(sender, e);
            TiltWheelHorizontalScroller.scrollerLoaded(sender, e);
        }

        private void listboxLoaded(object sender, RoutedEventArgs e)
        {
            //ScrollAnimationBehavior.listboxLoaded(sender, e); // original
        }





        private void CloseWindow_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { CloseWind(Window.GetWindow((FrameworkElement)e.Source)); }
                catch { }
        }
        private void AutoMinimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { MaximizeRestore(Window.GetWindow((FrameworkElement)e.Source)); }
                catch { }
        }
        private void Minimize_Event(object sender, RoutedEventArgs e)
        {
            if (e.Source != null)
                try { MinimizeWind(Window.GetWindow((FrameworkElement)e.Source)); }
                catch { }
        }

        public void CloseWind(Window window) => window.Close();
        public void MaximizeRestore(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
        }
        public void MinimizeWind(Window window) => window.WindowState = WindowState.Minimized;



        /**
         * NGL not entirely sure what this did, but I believe it was necessary when changing between
         *      light and dark modes for some reason. I'm going to comment it out for now, but will 
         *      leave it here in the event it is needed in the future.
         * Reference My Original Anime Organizer code to see how it worked.
         * 
         * iirc it basically just reloaded everything to make sure the datagrid colors updated properly
         */
        /**
        private void DataGridColumnHeader_Loaded(object sender, RoutedEventArgs e)
        {
            new DataGridScrollHelper().DataGridColumnHeader_Loaded(sender);
        }
         */

    }





    public static class TiltWheelHorizontalScroller
    {
        static HashSet<int> controls = new HashSet<int>();

        public static void scrollerLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject d = (DependencyObject)sender;
            Control control = sender as Control;

            if (control != null && controls.Add(control.GetHashCode()))
            {
                control.MouseEnter += (sender, e) =>
                {
                    var scrollViewer = d.FindChildOfType<ScrollViewer>();

                    if (scrollViewer != null)
                    {
                        new TiltWheelMouseScrollHelper(scrollViewer, d);
                    }
                };
            }
        }





        // This method should really be in either a class of its own or just in the main one, as both the tilt class and the smooth scrolling class use this and its weird.
        //      This is because the listbox shit uses it but that doesn't even work atm, so for now it can stay as is
        /// <summary>
        /// Finds first child of provided type. If child not found, null is returned
        /// </summary>
        /// <typeparam name="T">Type of chiled to be found</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T FindChildOfType<T>(this DependencyObject originalSource) where T : DependencyObject
        {
            T ret = originalSource as T;
            DependencyObject child = null;

            if (originalSource != null && ret == null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(originalSource); i++)
                {
                    child = VisualTreeHelper.GetChild(originalSource, i);

                    if (child != null)
                    {
                        if (child is T)
                        {
                            ret = child as T;
                            break;
                        }
                        else
                        {
                            ret = child.FindChildOfType<T>();
                            if (ret != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        }

    }

    

    class TiltWheelMouseScrollHelper
    {
        /// <summary>
        /// multiplier of how far to scroll horizontally. Change as desired.
        /// </summary>
        private const int scrollFactor = 48; // original was 3
        private const int WM_MOUSEHWEEL = 0x20e;
        ScrollViewer scrollViewer;
        HwndSource hwndSource;
        HwndSourceHook hook;
        static HashSet<int> scrollViewers = new HashSet<int>();

        public TiltWheelMouseScrollHelper(ScrollViewer scrollViewer, DependencyObject d)
        {
            this.scrollViewer = scrollViewer;
            hwndSource = PresentationSource.FromDependencyObject(d) as HwndSource;
            hook = WindowProc;
            hwndSource?.AddHook(hook);
            if (scrollViewers.Add(scrollViewer.GetHashCode()))
            {
                scrollViewer.MouseLeave += (sender, e) =>
                {
                    hwndSource.RemoveHook(hook);
                };
            }
        }

        IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWEEL:
                    Scroll(wParam);
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }



        private void Scroll(IntPtr wParam)
        {
            /**
            // original version of this code. New code is to allow integrating with smooth scrolling.
            int delta = (HIWORD(wParam) > 0 ? 1 : -1) * scrollFactor;
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + delta);
             */
            
            int delta = (HIWORD(wParam) > 0 ? 1 : -1) * scrollFactor;

            // Slightly modified version of the ScrollAnimationBehavior class for the animation stuffs. I figured it didn't need to have anything special for horizontal scrolling
            //      (since I doubt a lot of people use it, and honestly this is pretty smooth to begin with anyway so I think it's fine.), so its just has a fairly stiff animation
            //      but honestly I'm ok with that, might update it to a accel/deccel if I ever implement that.
            scrollViewer.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, null);
            DoubleAnimation horizontalAnimation = new DoubleAnimation();
            horizontalAnimation.From = scrollViewer.HorizontalOffset;

            double toValue = scrollViewer.HorizontalOffset + delta;

            // This is the fix that made sure that the tilt wheel wasn't making negative or overly large values. This makes it so Arrow key interaction is also fine.
            
            if (toValue < 0.0)
                toValue = 0;
            else if (toValue > scrollViewer.ScrollableWidth)
                toValue = scrollViewer.ScrollableWidth;
            

            horizontalAnimation.To = toValue;
            horizontalAnimation.Duration = new Duration(ScrollAnimationBehavior.GetTimeDuration(scrollViewer));
            scrollViewer.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, horizontalAnimation);

            Debug.WriteLine("From: " + horizontalAnimation.From + "    To: " + horizontalAnimation.To + "    ");

        }

        private static int HIWORD(IntPtr ptr) => (short)((((int)ptr.ToInt64()) >> 16) & 0xFFFF);

    }








    /**
     * https://stackoverflow.com/questions/20731402/animated-smooth-scrolling-on-scrollviewer
     * this is where this code comes from.
     * 
     * 
     * Notes about this class:
     * - I figured out most of this code at this point. basically the reason the listbox is breaking is because since these methods are shared, things break. 
     *      Ex. the math behind the scrollview is that if you scroll once, lets say that it scrolls from pixel 0 to pixel 120. this works fine for basic scrollviews.
     *      The problem when it attempts the same math with listbox is that the From value is actually the index of the listbox, and the To value is also the index, which
     *      you think would be good, but the problem is that it is using the same offset as the scrollview, which means its trying to go from item 0 to item 120, which 
     *      obviously is too much. In time I could try coming back to this and fix the listbox (like figuring out how to virtualize it cause that wasn't working for some reason), 
     *      but even if I do, I don't know if it would work because I still haven't been able to see the animation play properly so I don't know if its worth it. That and I 
     *      couldn't turn off virtualization so the smooth scrolling would be kind of useless to begin with. It would kind of require a complete rewrite if I wanted to get 
     *      this working properly.
     *      
     * - Note: All the horizontal methods, properties, etc were added in by me. tl;dr dont add in the event stuffs for listbox as it just doesn't work properly. Fix 
     *      at a later date please and thanks. Also might want to try cleaning up the code at some point.
     *      
     *      
     */

    // Everything here is code for smooth scrolling.
    public static class ScrollAnimationBehavior
    {
        private static ScrollViewer _listBoxScroller = new ScrollViewer();


        #region IntendedLocation Property

        public static DependencyProperty IntendedLocationProperty = DependencyProperty.RegisterAttached
            ("IntendedLocation", typeof(double), typeof(ScrollAnimationBehavior), new PropertyMetadata(0.0));

        public static void SetIntendedLocation(FrameworkElement target, double value)
        {
            target.SetValue(IntendedLocationProperty, value);
        }

        public static double GetIntendedLocation(FrameworkElement target)
        {
            return (double)target.GetValue(IntendedLocationProperty);
        }

        #endregion

        
        // Quickly abandoning this set and get properties cause they caused way too many headaches. I might get rid of this property too if I can find a way to use an already existing one.
        #region VerticalOffset Property

        public static DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached
            ("VerticalOffset", typeof(double), typeof(ScrollAnimationBehavior), new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        
        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }

        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }
        

        #endregion
        


        #region HorizontalOffset Property

        public static DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached
            ("HorizontalOffset", typeof(double), typeof(ScrollAnimationBehavior), new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

        public static void SetHorizontalOffset(FrameworkElement target, double value)
        {
            target.SetValue(HorizontalOffsetProperty, value);
        }

        public static double GetHorizontalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(HorizontalOffsetProperty);
        }

        #endregion

        #region TimeDuration Property

        public static DependencyProperty TimeDurationProperty = DependencyProperty.RegisterAttached
            ("TimeDuration", typeof(TimeSpan), typeof(ScrollAnimationBehavior), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 150))); // original was 150 | days, hours, minutes, seconds, milliseconds.

        public static void SetTimeDuration(FrameworkElement target, TimeSpan value)
        {
            target.SetValue(TimeDurationProperty, value);
        }

        public static TimeSpan GetTimeDuration(FrameworkElement target)
        {
            return (TimeSpan)target.GetValue(TimeDurationProperty);
        }

        #endregion

        #region PointsToScroll Property

        public static DependencyProperty PointsToScrollProperty = DependencyProperty.RegisterAttached
            ("PointsToScroll", typeof(double), typeof(ScrollAnimationBehavior), new PropertyMetadata(35.0)); // originally 0.0. I want this value to be the same as the normal scroll for now so I'm changing it to 60

        public static void SetPointsToScroll(FrameworkElement target, double value)
        {
            target.SetValue(PointsToScrollProperty, value);
        }

        public static double GetPointsToScroll(FrameworkElement target)
        {
            return (double)target.GetValue(PointsToScrollProperty);
        }

        #endregion

        #region AnimateScroll Helper

        private static void AnimateScroll(ScrollViewer scrollViewer, double ToValue)
        {
            scrollViewer.BeginAnimation(VerticalOffsetProperty, null); // This runs to make animation less jittery. Probably can't be removed. This stops jitters because it kills the current running animation, then the code below creates a new one and then starts there, allowing for a smooth viewing experience.
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));
            scrollViewer.BeginAnimation(VerticalOffsetProperty, verticalAnimation);

            //Debug.WriteLine("From: " + verticalAnimation.From + " To: " + verticalAnimation.To);
        }


        // this one is for testing the keydown and keyup events. may become permanent depending on how I can implement this. prob wont, but I may remove the original animatescroll
        //      method in favor of this one because then I can implement my smooth scrolling arrow key stuffs.
        private static void AnimateScrollTesting(ScrollViewer scrollViewer, double ToValue, Duration duration, Orientation orientation)
        {
            // has the orientation value, but currently isn't used until I merge stuff together :)

            scrollViewer.BeginAnimation(VerticalOffsetProperty, null); // This runs to make animation less jittery. Probably can't be removed. This stops jitters because it kills the current running animation, then the code below creates a new one and then starts there, allowing for a smooth viewing experience.
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = duration;
            scrollViewer.BeginAnimation(VerticalOffsetProperty, verticalAnimation);

            //Debug.WriteLine(" From AnimateScrollTesting | From: " + verticalAnimation.From + " To: " + verticalAnimation.To);
        }




        #endregion

















        // ScrollViewer only shit

        #region scrollerLoaded Event Handler

        public static void scrollerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scroller = sender as ScrollViewer;
            SetEventHandlersForScrollViewer(scroller);
        }

        #endregion

        #region SetEventHandlersForScrollViewer Helper
        // this isn't actually only scrollviewer. The listview thing also used this method.
        private static void SetEventHandlersForScrollViewer(ScrollViewer scroller)
        {
            scroller.PreviewMouseWheel += ScrollViewerPreviewMouseWheel;
            scroller.PreviewKeyDown += ScrollViewerPreviewKeyDown;
            scroller.PreviewMouseLeftButtonUp += Scroller_PreviewMouseLeftButtonUp;
            scroller.PreviewKeyUp += ScrollViewerPreviewKeyUp; // I added this

        }

        #endregion














        #region ScrollViewerPreviewMouseWheel Event Handler
        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;
            double intendedLocation = GetIntendedLocation(scroller);
            double mouseWheelChange = (double)e.Delta;
            double newVOffset = intendedLocation - (mouseWheelChange * 1); // Original value was 2 (pretty fast, current value is 0.5, more manageable). Can be changed to be higher (faster) or lower (slower)
            //We got hit by the mouse again. jump to the offset.
            scroller.ScrollToVerticalOffset(intendedLocation); // explanation for this is wack, but I do believe that having this commented out makes the visuals feel a bit more sluggish but its hard to tell. Keep it uncommented for now.

            if (newVOffset < 0)
            {
                newVOffset = 0;
            }
            if (newVOffset > scroller.ScrollableHeight)
            {
                newVOffset = scroller.ScrollableHeight;
            }

            AnimateScroll(scroller, newVOffset);
            SetIntendedLocation(scroller, newVOffset);
            e.Handled = true;
        }

        #endregion

        #region ScrollViewerPreviewKeyDown Handler

        private static int num = 0;



        // 2945 pixelheight for my code for the height of pixels for the content in the ScrollViewer, and it runs at 4 seconds currently when you hold the button down.
        // this means I want a velocity of 736.25 pixels a second for that smooth speed. DONT DELETE AS I MAY TWEAK THE VELOCITY VALUES IN THE FUTURE.


        // START HERE WHEN YOU START CODING AGAIN:
        //- implement the check of if the key is a repeat, and if it is, if its the first repeat, and start the animation as normal, otherwise keep going.
        // - clean up code in keydown event and make it horizontal compatible
        //

        // I noticed that scrolling with mouse wheel makes it per 60 points, where as here per key is 50 points. I may want to find a way to normalize this somehow.
        private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;
            Key keyPressed = e.Key;

            double newVerticalPos = scroller.VerticalOffset; // original. Just straight wouldn't work, and this method was only used here. So not sure how the original worked.
            double newHorizontalPos = scroller.HorizontalOffset; // added myself

            bool isKeyHandled = false;

            //Debug.WriteLine("");
            //Debug.WriteLine("Key: " + keyPressed.ToString() + "\t\tOGVerticalPos: " + newVerticalPos + "\t\tNewVertPos: " + (newVerticalPos + GetPointsToScroll(scroller)));


            ///////////////////////////////////////////////////////////////////////////////////////////
            // this is all testing code


            if (!e.IsRepeat)
            {
                
                if (keyPressed == Key.Down)
                {
                    //newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
                    newVerticalPos = scroller.ScrollableHeight;
                    SetIntendedLocation(scroller, newVerticalPos);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.Up)
                {
                    //newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
                    newVerticalPos = 0;
                    SetIntendedLocation(scroller, newVerticalPos);
                    isKeyHandled = true;
                }



                //Debug.WriteLine("Num: " + num + "\tIsRepeat: " + e.IsRepeat);
                //num++;
                //Debug.Write("\nnewVerticalPos != GetVerticalOffset(scroller) = " + (newVerticalPos != GetVerticalOffset(scroller)) + "\n" + newVerticalPos + " != " + GetVerticalOffset(scroller));


                if (newVerticalPos != scroller.VerticalOffset)
                {
                    //velocity = 736.25 pixels a second
                    // velocity = distance / time 

                    // time * velocity = distance
                    // time = distance / velocity (final equation)


                    double distance; // distance to cover

                    // shouldn't be doing this check here honestly, there are like 2 opportunities before this place to check for up or down key, please fix at earliest convenience.
                    // need to incorporate page up and down as well in this section, but not doing that rn and will clean everything up later.
                    if (keyPressed == Key.Down)
                    {
                        distance = scroller.ScrollableHeight - scroller.VerticalOffset;
                    }
                    else //if (keyPressed == Key.Up)
                    {
                        distance = scroller.VerticalOffset;
                    }


                    double velocity = 736.25;
                    double time = distance / velocity;

                    //Debug.WriteLine("Time: " + time + "\t\t" + distance + " / " + velocity + "\t\tScrollableHeight: " + scroller.ScrollableHeight);

                    int seconds = (int) time;
                    double remainder = time - (double)seconds;
                    int milliseconds = (int)(remainder * 1000);

                    SetIntendedLocation(scroller, newVerticalPos);
                    AnimateScrollTesting(scroller, newVerticalPos, new TimeSpan(0, 0, 0, seconds, milliseconds), Orientation.Vertical); // days, hours, minutes, seconds, milliseconds | Note: The duration here is hard coded when it shouldn't be. Duration needs to dynamically change based on distance from max or min so the speed is the same each time.

                }
                else if (newHorizontalPos != GetHorizontalOffset(scroller))
                {
                    // this section hasn't been touched at all. just a reminder.


                    SetIntendedLocation(scroller, newHorizontalPos); // original
                                                                     //AnimateScroll(scroller, newHorizontalPos); // original (cant use this as it is only for vertical scrolling at the moment.)

                    // forced code duplication that will be fixed when I merge code together.
                    scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, null);
                    DoubleAnimation horizontalAnimation = new DoubleAnimation();
                    horizontalAnimation.From = scroller.HorizontalOffset;
                    horizontalAnimation.To = newHorizontalPos;
                    horizontalAnimation.Duration = new Duration(ScrollAnimationBehavior.GetTimeDuration(scroller));
                    scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, horizontalAnimation);

                    //Debug.WriteLine("From: " + horizontalAnimation.From + " To: " + horizontalAnimation.To);
                }

            }
            else if (keyPressed == Key.Up || keyPressed == Key.Down || keyPressed == Key.Left || keyPressed == Key.Right || keyPressed == Key.PageUp || keyPressed == Key.PageDown)
            {
                isKeyHandled = true;
            }



            






            ///////////////////////////////////////////////////////////////////////////////////////////

            /**
            ScrollViewer scroller = (ScrollViewer)sender;
            Key keyPressed = e.Key;

            double newVerticalPos = GetVerticalOffset(scroller); // original. Just straight wouldn't work, and this method was only used here. So not sure how the original worked.
            //double newVerticalPos = GetIntendedLocation(scroller);
            double newHorizontalPos = GetHorizontalOffset(scroller); // added myself

            bool isKeyHandled = false;

            //Debug.WriteLine("");
            //Debug.Write("Key: " + keyPressed.ToString() + "\t\tOGHorizontalPos: " + newHorizontalPos + "\t\tNewHorPos: " + (newHorizontalPos + GetPointsToScroll(scroller)));
            Debug.Write("Key: " + keyPressed.ToString() + "\t\tOGVerticalPos: " + newVerticalPos + "\t\tNewVertPos: " + (newVerticalPos + GetPointsToScroll(scroller)));

            
            if (keyPressed == Key.Down)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
                SetIntendedLocation(scroller, newVerticalPos);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageDown)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
                SetIntendedLocation(scroller, newVerticalPos);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.Up)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
                SetIntendedLocation(scroller, newVerticalPos);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageUp)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
                SetIntendedLocation(scroller, newVerticalPos);
                isKeyHandled = true;
            }

            else if (keyPressed == Key.Left)
            {
                newHorizontalPos = NormalizeScrollPos(scroller, (newHorizontalPos - GetPointsToScroll(scroller)), Orientation.Horizontal);
                SetIntendedLocation(scroller, newHorizontalPos);
                isKeyHandled = true;
            }
            else if (keyPressed == Key.Right)
            {
                newHorizontalPos = NormalizeScrollPos(scroller, (newHorizontalPos + GetPointsToScroll(scroller)), Orientation.Horizontal);
                SetIntendedLocation(scroller, newHorizontalPos);
                isKeyHandled = true;
            }
            


            Debug.WriteLine("Num: " + num);
            num++;

            //Debug.Write("\nnewVerticalPos != GetVerticalOffset(scroller) = " + (newVerticalPos != GetVerticalOffset(scroller)) + "\n" + newVerticalPos + " != " + GetVerticalOffset(scroller));


            if (newVerticalPos != GetVerticalOffset(scroller))
            {
                SetIntendedLocation(scroller, newVerticalPos);
                AnimateScroll(scroller, newVerticalPos); // original
            }
            else if (newHorizontalPos != GetHorizontalOffset(scroller))
            {
                SetIntendedLocation(scroller, newHorizontalPos); // original
                //AnimateScroll(scroller, newHorizontalPos); // original (cant use this as it is only for vertical scrolling at the moment.)

                // forced code duplication that will be fixed when I merge code together.
                scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, null);
                DoubleAnimation horizontalAnimation = new DoubleAnimation();
                horizontalAnimation.From = scroller.HorizontalOffset;
                horizontalAnimation.To = newHorizontalPos;
                horizontalAnimation.Duration = new Duration(ScrollAnimationBehavior.GetTimeDuration(scroller));
                scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, horizontalAnimation);

                Debug.WriteLine("From: " + horizontalAnimation.From + " To: " + horizontalAnimation.To);
            }
             */


            e.Handled = isKeyHandled;
        }




        // mostly a testing method for now, but might be necessary in the future not sure yet.
        private static void ScrollViewerPreviewKeyUp(object sender, KeyEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;
            Key keyPressed = e.Key;

            double newVerticalPos = scroller.VerticalOffset; // original. Just straight wouldn't work, and this method was only used here. So not sure how the original worked.
            double newHorizontalPos = scroller.HorizontalOffset; // added myself

            bool isKeyHandled = false;


            // for now I'm going to assume that pageup and pagedown will just with the vertical stuffs. please test this at some point.

            if (keyPressed == Key.Up || keyPressed == Key.Down || keyPressed == Key.PageUp || keyPressed == Key.PageDown)
            {
                SetIntendedLocation(scroller, newVerticalPos);
                isKeyHandled = true;

                //Debug.Write("\nnewVerticalPos != scroller.VerticalOffset = " + (newVerticalPos != scroller.VerticalOffset) + "\n" + newVerticalPos + " != " + scroller.VerticalOffset);

                AnimateScrollTesting(scroller, newVerticalPos, new TimeSpan(0, 0, 0, 0, 1), Orientation.Vertical);

            }
            else if (keyPressed == Key.Left || keyPressed == Key.Right)
            {
                SetIntendedLocation(scroller, newHorizontalPos);
                isKeyHandled = true;


                // commenting out for how since I know that horizontal wont work here just yet, but I'm prepping for it.

                //AnimateScrollTesting(scroller, newHorizontalPos, new TimeSpan(0, 0, 0, 0, 1), Orientation.Horizontal);
                /**
                // forced code duplication that will be fixed when I merge code together.
                scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, null);
                DoubleAnimation horizontalAnimation = new DoubleAnimation();
                horizontalAnimation.From = scroller.HorizontalOffset;
                horizontalAnimation.To = newHorizontalPos;
                horizontalAnimation.Duration = new Duration(ScrollAnimationBehavior.GetTimeDuration(scroller));
                scroller.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, horizontalAnimation);
                 */

            }

            e.Handled = isKeyHandled;
        }




        #endregion

        private static void Scroller_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            


            ScrollViewer scroller = (ScrollViewer)sender;

            SetVerticalOffset(scroller, scroller.VerticalOffset);



            Debug.Write("From PreviewMouseLeftButtonUp | Vertical Offset: " + scroller.VerticalOffset + "\t\tFrom SetVerticalOffset Value: " + GetVerticalOffset(scroller));

            SetIntendedLocation(scroller, scroller.VerticalOffset);

            Debug.WriteLine("\tintended location: " + GetIntendedLocation(scroller));
        }








        #region NormalizeScrollPos Helper

        private static double NormalizeScrollPos(ScrollViewer scroll, double scrollChange, Orientation o)
        {
            double returnValue = scrollChange;

            
            if (scrollChange < 0)
            {
                returnValue = 0;
            }
            

            if (o == Orientation.Vertical && scrollChange > scroll.ScrollableHeight)
            {
                returnValue = scroll.ScrollableHeight;
            }
            else if (o == Orientation.Horizontal && scrollChange > scroll.ScrollableWidth)
            {
                returnValue = scroll.ScrollableWidth;
            }

            return returnValue;
        }

        #endregion






        #region OnVerticalOffset Changed

        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        #endregion

        #region OnHorizontalOffset Changed

        private static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }

        #endregion












        // ListBox stuffs.

        #region listboxLoaded Event Handler
        // Used to be private. Had to be made public as I had to circumvent the IsEnabled variables as there was no way to create that in the xaml. (Well there
        //      probaby was but this was a lot simpler)
        public static void listboxLoaded(object sender, RoutedEventArgs e)
        {
            ListBox listbox = sender as ListBox;

            // NOTE: THIS FIND CHILD TYPE METHOD NEEDS TO GO TO ITS OWN CLASS OR SOMETHING. IT'S SUPER WRONG TO JUST GRAB THIS FROM ANOTHER CLASS THAT'S COMPLETELY UNRELATED.
            _listBoxScroller = TiltWheelHorizontalScroller.FindChildOfType<ScrollViewer>(listbox);

            Debug.WriteLine(_listBoxScroller + "\n\n\n\n\n\n\n\n\n EEEEEEEEEEEEEEEEEEEEEEEE");

            SetEventHandlersForScrollViewer(_listBoxScroller);
            SetTimeDuration(_listBoxScroller, new TimeSpan(0, 0, 0, 2, 0)); // originally the last value was 200 and everything else was 0
            SetPointsToScroll(_listBoxScroller, 16.0); // oringally 16.0 // I notice literally no difference if this method is commented out or not.

            listbox.SelectionChanged += new SelectionChangedEventHandler(ListBoxSelectionChanged);
            listbox.Loaded += new RoutedEventHandler(ListBoxLoaded);
            listbox.LayoutUpdated += new EventHandler(ListBoxLayoutUpdated);
        }


        #endregion

        #region ListBox Event Handlers

        private static void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private static void ListBoxLayoutUpdated(object sender, EventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        #endregion

        #region UpdateScrollPosition Helper

        private static void UpdateScrollPosition(object sender)
        {
            ListBox listbox = sender as ListBox;

            if (listbox != null)
            {
                double scrollTo = 0;

                for (int i = 0; i < (listbox.SelectedIndex); i++)
                {
                    ListBoxItem tempItem = listbox.ItemContainerGenerator.ContainerFromItem(listbox.Items[i]) as ListBoxItem;

                    if (tempItem != null)
                    {
                        scrollTo += tempItem.ActualHeight;
                    }
                }


                Debug.WriteLine(scrollTo);

                AnimateScroll(_listBoxScroller, scrollTo);
            }
        }

        #endregion
















    }
}
