using System;
using System.Threading;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BackBox.iOS
{
    public partial class BackBox_iOSViewController : UIViewController
    {
        #region Public Constructurs

        public BackBox_iOSViewController(IntPtr handle) : base(handle)
        {
        }

        #endregion

        #region Private Constants

        const string ApiBase = @"http://172.16.71.130/BackBox.Api/";
        readonly WebClientEx client = new WebClientEx() { BaseAddress = ApiBase };

        #endregion

        #region Private Members

        double _currentLat = 0.0, _currentLng = 0.0;

        #endregion

        #region Public Methods

        void Log(string message)
        {
            displayText.Text += DateTime.Now.ToString("HH:mm  ") + message + Environment.NewLine;
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Startup();
        }

        #endregion

        #endregion

        #region Private Methods

        void Startup()
        {


            Log("Hello there.");

            Log("Connecting to the service...");

            var id = client.DownloadString("connect");

            Log("Connected.");
            Log("My Id: " + id);


            StartTrackingLocation();

            inputText.EditingDidBegin += (object sender, EventArgs e) => { ToggleKeyboard(true); };
            inputText.EditingDidEnd += (object sender, EventArgs e) => { ToggleKeyboard(false); };

            inputText.ShouldReturn += field =>
            {
                if (!string.IsNullOrEmpty(field.Text))
                {
                    SendMessage(field.Text);

                    field.Text = string.Empty;
                }

                field.ResignFirstResponder();
                return false;
            };

            new Timer((a) => { InvokeOnMainThread(() => { UpdateMessages(); }); }, null, 2000, 2000);
        }

        void UpdateMessages()
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<Message>>(client.DownloadString("get-latest"));

            foreach (var message in data)
            {
                Log(string.Format("[{0:hh':'mm}] <{1}> {2}", message.Timestamp, message.User ?? "Anonymous", message.Content));
            }
        }

        void ToggleKeyboard(bool shown)
        {
            var frame = View.Frame;

            UIView.BeginAnimations(string.Empty, IntPtr.Zero);
            UIView.SetAnimationDuration(0.2);

            frame.Y += 215.0f * (shown ? -1.0f : 1.0f);

            View.Frame = frame;
            UIView.CommitAnimations();
        }

        void SendMessage(string message)
        {
            Log(string.Format("Sending message at ({1}, {2}): {0}", message, _currentLat, _currentLng));

            client.DownloadString(string.Format("send?message={0}&lat={1}&lng={2}", Uri.EscapeUriString(message), _currentLat, _currentLng));
        }

        void StartTrackingLocation()
        {
            Log("Need to keep track of your location.");

            var locMgr = new CLLocationManager();
            if (CLLocationManager.LocationServicesEnabled)
            {
                Log("Tracking...");

                locMgr.StartMonitoringSignificantLocationChanges();

                locMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    _currentLat = e.Locations[0].Coordinate.Latitude;
                    _currentLng = e.Locations[0].Coordinate.Longitude;

                    Log(string.Format("Your location is now: ({0}, {1}). Telling the service.", _currentLat, _currentLng));

                    var ret = client.DownloadString(string.Format("set-bounds/{0}/{1}/10", _currentLat, _currentLng));

                    Log("Response from the server: " + ret);
                };
            }
            else
            {
                Log("I can't keep track of you.  Enable me in your phones settings.");
            }
        }

        #endregion


        public class Message
        {
            public Guid Id { get; set; }
            public string User { get; set; }
            public DateTime Timestamp { get; set; }
            public string Content { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}