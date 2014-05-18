using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BackBox.iOS.MessageView;

namespace BackBox.iOS
{
    public partial class MessageViewController : UIViewController
    {
        #region Private Members

        UITableView _tableView;
        MessageTableDataSource _dataSource;
        UITextField _inputText;

        #endregion

        #region Public Constructors

        public MessageViewController(IntPtr handle) : base(handle)
        {
        }

        #endregion

        #region Public Methods

        #region Overrides

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CreateControls();

            AppDelegate.Instance.MessageReceived += (Message message) =>
            {
                _dataSource.AddMessage(message);

                _tableView.ReloadData();
            };
        }

        #endregion

        #endregion

        #region Private Methods

        void CreateControls()
        {
            _dataSource = new MessageTableDataSource();

            _tableView = new UITableView()
            {
                Source = _dataSource,
                AllowsSelection = false,
                Frame = new RectangleF(0, 20, 320, 540),
                SeparatorStyle = UITableViewCellSeparatorStyle.None
            };

            _inputText = new UITextField()
            {
                Frame = new RectangleF(5, 540, 310, 30),
                BorderStyle = UITextBorderStyle.None,
                Placeholder = "What would you like to say?",
                Font = MessageCell.DefaultFont
            };

            Add(_tableView);
            Add(_inputText);

            _inputText.EditingDidBegin += (object sender, EventArgs e) => { View.ToggleKeyboard(true); };
            _inputText.EditingDidEnd += (object sender, EventArgs e) => { View.ToggleKeyboard(false); };

            _inputText.ShouldReturn += field =>
            {
                if (!string.IsNullOrEmpty(field.Text))
                {
                    AppDelegate.Instance.SendMessage(field.Text);

                    field.Text = string.Empty;
                }

                field.ResignFirstResponder();
                return false;
            };

        }

        #endregion
    }
}