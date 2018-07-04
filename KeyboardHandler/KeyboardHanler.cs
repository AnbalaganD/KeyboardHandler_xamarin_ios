using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace KeyboardHandler
{
    public class KeyboardHanler
    {
        static KeyboardHanler instance;
        public static KeyboardHanler Instance => instance ?? (instance = new KeyboardHanler());

        UIView activeTextInputView;
        bool isKeyboardPresent;
        CGRect? viewOriginalFrameCache;
        nfloat keyboardYposition = 0.0f;

        KeyboardHanler() { }

        public void EnableKeyboardHandling()
        {
            UITextField.Notifications.ObserveTextDidBeginEditing(OnInputFieldBeginEditing);
            UITextView.Notifications.ObserveTextDidBeginEditing(OnInputFieldBeginEditing);
            UITextField.Notifications.ObserveTextDidEndEditing(OnInputFieldEndEditing);
            UITextView.Notifications.ObserveTextDidEndEditing(OnInputFieldEndEditing);
            UITextView.Notifications.ObserveTextDidChange(OnEditingChanged);
            UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
            UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
            UIKeyboard.Notifications.ObserveDidShow(OnKeyboardShow);
        }

        void OnEditingChanged(object sender, NSNotificationEventArgs e)
        {
            if (isKeyboardPresent)
            {
                activeTextInputView = (UIView)e.Notification.Object;
                OnKeyboardShow(null, null);
            }
        }

        void OnInputFieldEndEditing(object sender, NSNotificationEventArgs e)
        {
            activeTextInputView = null;
        }

        void OnInputFieldBeginEditing(object sender, NSNotificationEventArgs e)
        {
            activeTextInputView = (UIView)e.Notification.Object;
        }

        void OnKeyboardShow(object sender, UIKeyboardEventArgs e)
        {
            isKeyboardPresent = true;
            if (e != null)
            {
                keyboardYposition = e.FrameEnd.GetMinY();
            }
            var keyboardMinY = e == null ? keyboardYposition : e.FrameEnd.GetMinY();
            if (activeTextInputView != null)
            {
                var inputViewOrigin = activeTextInputView.ConvertPointToView(new CGPoint(0, 0), null);
                if (activeTextInputView is UITextView)
                {
                    var textView = activeTextInputView as UITextView;
                    if (textView.ScrollEnabled)
                    {
                        inputViewOrigin.Y += textView.ContentOffset.Y;
                    }
                }
                if (keyboardMinY < inputViewOrigin.Y + activeTextInputView.Frame.Height)
                {
                    var rootView = GetTopViewController(UIApplication.SharedApplication.KeyWindow.RootViewController).View;
                    var movingDistance = (keyboardMinY - inputViewOrigin.Y) - activeTextInputView.Frame.Height + rootView.Frame.GetMinY();

                    if (movingDistance > keyboardMinY)
                        movingDistance = -keyboardMinY;

                    if (!viewOriginalFrameCache.HasValue)
                    {
                        viewOriginalFrameCache = rootView.Frame;
                    }
                    rootView.Frame = new CGRect(new CGPoint(rootView.Frame.X, movingDistance), rootView.Frame.Size);
                }
            }
        }

        void OnKeyboardHide(object sender, UIKeyboardEventArgs e)
        {
            isKeyboardPresent = false;
            if (viewOriginalFrameCache.HasValue)
            {
                var rootView = GetTopViewController(UIApplication.SharedApplication.KeyWindow.RootViewController).View;
                rootView.Frame = viewOriginalFrameCache.Value;
                rootView.LayoutIfNeeded();
            }
            viewOriginalFrameCache = null;
        }

        UIViewController GetTopViewController(UIViewController controller)
        {
            if (controller == null)
                return null;

            if (controller is UINavigationController)
            {
                var navigationController = controller as UINavigationController;
                return GetTopViewController(navigationController.VisibleViewController);
            }

            if (controller is UITabBarController)
            {
                var tabBarControll = controller as UITabBarController;
                return GetTopViewController(tabBarControll.SelectedViewController);
            }

            if (controller.PresentedViewController != null)
                return GetTopViewController(controller.PresentedViewController);

            return controller;
        }
    }
}