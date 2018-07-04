using System;

using UIKit;

namespace KeyboardHandler
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            myTextView.Layer.BorderWidth = 1.0f;
            myTextView.Layer.BorderColor = UIColor.Black.CGColor;
            myTextView.Layer.CornerRadius = 5;
        }
    }
}