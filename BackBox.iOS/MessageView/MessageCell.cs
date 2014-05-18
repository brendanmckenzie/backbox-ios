using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;

namespace BackBox.iOS.MessageView
{
    public class MessageCell : UITableViewCell
    {
        #region Private Members

        UILabel _displayText;
        Message _message;

        #endregion

        #region Public Members

        public static readonly UIFont DefaultFont = UIFont.FromName("Helvetica", 13f);

        #endregion

        #region Public Constructors

        public MessageCell(IntPtr p) : base(p)
        {
            CreateControls();
        }

        public MessageCell() : base(UITableViewCellStyle.Default, MessageTableDataSource.CellIdentifier)
        {
            CreateControls();
        }

        #endregion

        #region Public Methods

        public void UpdateCell(Message message)
        {
            _message = message;

            _displayText.Text = message.ToString();
        }

        #region Overrides

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var text = new NSString (_message.ToString());
            var maxSize = new SizeF (300, 5000);
            var currentSize = text.StringSize(DefaultFont, maxSize);

            _displayText.Frame = new RectangleF(10, 10, 300, currentSize.Height);
        }

        #endregion

        #endregion

        #region Private Methods

        void CreateControls()
        {
            _displayText = new UILabel()
            {
                Font = DefaultFont,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            ContentView.Add(_displayText);
        }

        #endregion
    }
}