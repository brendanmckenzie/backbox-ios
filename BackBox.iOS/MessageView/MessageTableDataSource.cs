using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Linq;
using System.Drawing;

namespace BackBox.iOS.MessageView
{
    public class MessageTableDataSource : UITableViewSource
    {
        #region Private Members

        List<Message> _messages;
        internal const string CellIdentifier = "MessageTableCell";

        #endregion

        #region Public Constructors

        public MessageTableDataSource()
        {
            _messages = new List<Message>();
        }

        #endregion

        #region Pubic Methods

        #region Overrides

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) as MessageCell;

            if (cell == null)
            {
                cell = new MessageCell();
            }

            cell.UpdateCell(_messages[indexPath.Row]);

            return cell;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return _messages.Count();
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var content = _messages[indexPath.Row].ToString();
            var text = new NSString(content);
            var size = text.StringSize(MessageCell.DefaultFont, new SizeF(320, 5000));

            return size.Height + 20;
        }

        #endregion

        public void AddMessage(Message Message)
        {
            _messages.Add(Message);
        }

        #endregion
    }
}