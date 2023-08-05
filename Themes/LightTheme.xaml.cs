using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
         * - Make sure to read through all of this documents commented sections, for either removal or notes that something needs to be done.
         * - I would like to eventually integrate the horizontal scrolling and vertical scrolling into a single class, but for now I have everything working and I might
         *      still make adjustments to thinks separately so for now it just makes things simpler, but please think about this for the future.
         * - 
         * - 
         * 
         */


        public void scrollerLoaded(object sender, RoutedEventArgs e)
        {
            ScrollAnimationBehavior.scrollerLoaded(sender, e);
            TiltWheelHorizontalScroller.scrollerLoaded(sender, e);
        }

        private void listboxLoaded(object sender, RoutedEventArgs e)
        {
            ScrollAnimationBehavior.listboxLoaded(sender, e);
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






    // I haven't personally looked at this much in detail, but I definitely want to take a closer look at some point.

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

            scrollViewer.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, null);
            DoubleAnimation horizontalAnimation = new DoubleAnimation();
            horizontalAnimation.From = scrollViewer.HorizontalOffset;
            horizontalAnimation.To = scrollViewer.HorizontalOffset + delta;
            horizontalAnimation.Duration = new Duration( ScrollAnimationBehavior.GetTimeDuration(scrollViewer));
            scrollViewer.BeginAnimation(ScrollAnimationBehavior.HorizontalOffsetProperty, horizontalAnimation);
        }

        private static int HIWORD(IntPtr ptr) => (short)((((int)ptr.ToInt64()) >> 16) & 0xFFFF);

    }








    /**
     * Notes about this class:
     * - I figured out most of this code at this point. basically the reason the listbox is breaking is because since these methods are shared, things break. 
     *      Ex. the math behind the scrollview is that if you scroll once, lets say that it scrolls from pixel 0 to pixel 120. this works fine for basic scrollviews.
     *      The problem when it attempts the same math with listbox is that the From value is actually the index of the listbox, and the To value is also the index, which
     *      you think would be good, but the problem is that it is using the same offset as the scrollview, which means its trying to go from item 0 to item 120, which 
     *      obviously is too much. In time I could try coming back to this and fix the listbox, but even if I do I don't know if it would work because I still haven't been
     *      able to see the animation play properly so I don't know if its worth it. That and I couldn't turn off virtualization so the smooth scrolling would be kind of 
     *      useless to begin with. It would kind of require a complete rewrite if I wanted to get this working properly. I was also planning on seeing if I could change 
     *      the base scrollview stuff as well to 1. make my own and 2. make the code simplier to understand, as currnetly it just feels like a web of methods.
     *      
     * - Note: All the horizontal methods, properties, etc were added in by me. tl;dr dont add in the event stuffs for listbox as it just doesn't work properly. Fix 
     *      at a later date please and thanks. Also might want to try cleaning up the code at some point.
     *      
     *      
     */

    // Everything here is code for smooth scrolling.
    public static class ScrollAnimationBehavior
    {
        public static double intendedLocation = 0;
        private static ScrollViewer _listBoxScroller = new ScrollViewer();

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

        // Currently hardcoded as there doesn't really need to have differences in scrolling speed. Might be changed once I come back and tighten up animations.
        public static DependencyProperty TimeDurationProperty = DependencyProperty.RegisterAttached
            ("TimeDuration", typeof(TimeSpan), typeof(ScrollAnimationBehavior), new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 100))); // days, hours, minutes, seconds, milliseconds.

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
            ("PointsToScroll", typeof(double), typeof(ScrollAnimationBehavior), new PropertyMetadata(0.0));

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
            scrollViewer.BeginAnimation(VerticalOffsetProperty, null); // From what I see, it makes the animation less janky like its stopping the previous animation to start a new one with a new speed etc.
            DoubleAnimation verticalAnimation = new DoubleAnimation();
            verticalAnimation.From = scrollViewer.VerticalOffset;
            verticalAnimation.To = ToValue;
            verticalAnimation.Duration = new Duration(GetTimeDuration(scrollViewer));
            scrollViewer.BeginAnimation(VerticalOffsetProperty, verticalAnimation);

            Debug.WriteLine("From: " + verticalAnimation.From + " To: " + verticalAnimation.To);
        }

        #endregion



        // ScrollViewer only shit

        #region scrollerLoaded Event Handler
        // Used to be private. Had to be made public as I had to circumvent the IsEnabled variables as there was no way to create that in the xaml. (Well there
        //      probaby was but this was a lot simpler)
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
            scroller.PreviewMouseWheel += new MouseWheelEventHandler(ScrollViewerPreviewMouseWheel);
            scroller.PreviewKeyDown += new KeyEventHandler(ScrollViewerPreviewKeyDown);
            scroller.PreviewMouseLeftButtonUp += Scroller_PreviewMouseLeftButtonUp;
        }

        #endregion

        #region ScrollViewerPreviewMouseWheel Event Handler
        // THIS IS THE METHOD YOU WILL NEED TO GRAB FROM IF YOU WANTED TO GET SMOOTH HORIZONTAL MOVEMENT.
        private static void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = (double)e.Delta;
            ScrollViewer scroller = (ScrollViewer)sender;
            double newVOffset = intendedLocation - (mouseWheelChange * 1); // 2 can be changed to be higher (faster) or lower (slower)
            //We got hit by the mouse again. jump to the offset.
            scroller.ScrollToVerticalOffset(intendedLocation);

            if (newVOffset < 0)
            {
                newVOffset = 0;
            }
            if (newVOffset > scroller.ScrollableHeight)
            {
                newVOffset = scroller.ScrollableHeight;
            }

            AnimateScroll(scroller, newVOffset);
            intendedLocation = newVOffset;
            e.Handled = true;
        }

        #endregion

        #region ScrollViewerPreviewKeyDown Handler

        private static void ScrollViewerPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ScrollViewer scroller = (ScrollViewer)sender;

            Key keyPressed = e.Key;
            double newVerticalPos = GetVerticalOffset(scroller);
            bool isKeyHandled = false;

            if (keyPressed == Key.Down)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + GetPointsToScroll(scroller)), Orientation.Vertical);
                intendedLocation = newVerticalPos;
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageDown)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos + scroller.ViewportHeight), Orientation.Vertical);
                intendedLocation = newVerticalPos;
                isKeyHandled = true;
            }
            else if (keyPressed == Key.Up)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - GetPointsToScroll(scroller)), Orientation.Vertical);
                intendedLocation = newVerticalPos;
                isKeyHandled = true;
            }
            else if (keyPressed == Key.PageUp)
            {
                newVerticalPos = NormalizeScrollPos(scroller, (newVerticalPos - scroller.ViewportHeight), Orientation.Vertical);
                intendedLocation = newVerticalPos;
                isKeyHandled = true;
            }

            if (newVerticalPos != GetVerticalOffset(scroller))
            {
                intendedLocation = newVerticalPos;
                AnimateScroll(scroller, newVerticalPos);
            }

            e.Handled = isKeyHandled;
        }

        #endregion

        private static void Scroller_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            intendedLocation = ((ScrollViewer)sender).VerticalOffset;
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
