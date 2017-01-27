using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace vcks_ilh
{
    public class VcksButton : Grid
    {
        DispatcherTimer timer;
        public event RoutedEventHandler LongClick;
        public event RoutedEventHandler Click;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 1, Opacity = 0 };

            timer = new DispatcherTimer();
            WaitFor(TimeSpan.FromMilliseconds(700), DispatcherPriority.SystemIdle);
            if (e.LeftButton == MouseButtonState.Pressed)
                if (LongClick != null) LongClick(this, e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 1, Opacity = 1 };
            if (timer != null)
                if (timer.IsEnabled) Click(this, e);
        }

        private void WaitFor(TimeSpan time, DispatcherPriority priority)
        {
            timer = new DispatcherTimer(priority);
            timer.Tick += new EventHandler(OnDispatched);
            timer.Interval = time;
            DispatcherFrame dispatcherFrame = new DispatcherFrame(false);
            timer.Tag = dispatcherFrame;
            timer.Start();
            Dispatcher.PushFrame(dispatcherFrame);
        }
        private void OnDispatched(object sender, EventArgs args)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Tick -= new EventHandler(OnDispatched);
            timer.Stop();
            DispatcherFrame frame = (DispatcherFrame)timer.Tag;
            frame.Continue = false;
        }

    }

}
