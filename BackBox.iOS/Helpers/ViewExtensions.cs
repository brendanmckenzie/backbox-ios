using System;
using MonoTouch.UIKit;

namespace BackBox.iOS
{
    public static class ViewExtensions
    {
        public static void ToggleKeyboard(this UIView view, bool shown)
        {
            var frame = view.Frame;

            UIView.BeginAnimations(string.Empty, IntPtr.Zero);
            UIView.SetAnimationDuration(0.2);

            frame.Y += 215.0f * (shown ? -1.0f : 1.0f);

            view.Frame = frame;
            UIView.CommitAnimations();
        }
    }
}

