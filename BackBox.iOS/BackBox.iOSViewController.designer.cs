// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace BackBox.iOS
{
	[Register ("BackBox_iOSViewController")]
	partial class BackBox_iOSViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextView displayText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (displayText != null) {
				displayText.Dispose ();
				displayText = null;
			}
		}
	}
}
