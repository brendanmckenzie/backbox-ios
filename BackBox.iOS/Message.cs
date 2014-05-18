using System;

namespace BackBox.iOS
{
    public class Message
    {
        #region Public Properties

        public Guid Id { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            //return string.Format("[Message: Id={0}, User={1}, Timestamp={2}, Content={3}, Lat={4}, Lng={5}]", Id, User, Timestamp, Content, Lat, Lng);
            return string.Format("[{0:hh:mm}] <{1}> {2}", Timestamp, User, Content);
        }

        #endregion
    }
}