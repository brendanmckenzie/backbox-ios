using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json;
using System.Threading;
using MonoTouch.CoreLocation;
using System.Threading.Tasks;

namespace BackBox.iOS
{
    public delegate void MessageReceived(Message message);

    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        #region Private Constants

        const string ApiBase = @"http://backbox-dev.azurewebsites.net/";

        #endregion

        #region Private Members

        WebClientEx _client;
        double _currentLat = 0.0, _currentLng = 0.0;
        CLLocationManager _locationManager;

        #endregion

        #region Public Properties

        public static AppDelegate Instance { get; private set; }

        public override UIWindow Window
        {
            get;
            set;
        }

        #endregion

        #region Public Events

        public event MessageReceived MessageReceived;

        #endregion

        #region Public Methods

        #region Overrides

        public override void FinishedLaunching(UIApplication application)
        {
            Instance = this;

            _client = new WebClientEx() { BaseAddress = ApiBase };

            Startup();
        }

        #endregion

        public void SendMessage(string message)
        {
            Log(string.Format("Sending message at ({1}, {2}): {0}", message, _currentLat, _currentLng));

            Task<object>.Run(() =>
            {
                _client.DownloadString(string.Format("send?message={0}&lat={1}&lng={2}", Uri.EscapeUriString(message), _currentLat, _currentLng));

                return null;
            });
        }

        #endregion

        #region Private Methods

        void Log(string message)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm  ") + message);
        }

        void Startup()
        {
            Log("Hello there.");

            Log("Connecting to the service...");

            var id = _client.DownloadString("connect");

            Log("Connected.");
            Log("My Id: " + id);

            StartTrackingLocation();

            new Timer((a) => { InvokeOnMainThread(() => { UpdateMessages(); }); }, null, 2000, 2000);
        }

        void UpdateMessages()
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<Message>>(_client.DownloadString("get-latest"));

            foreach (var message in data)
            {
                Log(string.Format("[{0:hh':'mm}] <{1}> {2}", message.Timestamp, message.User ?? "Anonymous", message.Content));

                if (MessageReceived != null)
                {
                    MessageReceived(message);
                }
            }
        }

        void StartTrackingLocation()
        {
            Log("Need to keep track of your location.");

            if (CLLocationManager.LocationServicesEnabled)
            {
                Log("Tracking...");

                _locationManager = new CLLocationManager();

                _locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    UpdateLocation(e.Locations[0].Coordinate.Latitude, e.Locations[0].Coordinate.Longitude);
                };

                _locationManager.DistanceFilter = 1000;
                _locationManager.StartUpdatingLocation();

                if (_locationManager.Location != null)
                {
                    UpdateLocation(_locationManager.Location.Coordinate.Latitude, _locationManager.Location.Coordinate.Longitude);
                }
            }
            else
            {
                Log("I can't keep track of you.  Enable me in your phones settings.");
            }
        }

        void UpdateLocation(double lat, double lng)
        {
            _currentLat = lat;
            _currentLng = lng;

            Log(string.Format("Your location is now: ({0}, {1}). Telling the service.", _currentLat, _currentLng));

            var ret = _client.DownloadString(string.Format("set-bounds/{0}/{1}/10", _currentLat, _currentLng));

            Log("Response from the server: " + ret);
        }

        #endregion
    }
}