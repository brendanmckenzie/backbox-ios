using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using System.Net;

namespace BackBox.iOS
{
    public partial class BackBox_iOSViewController : UIViewController
    {
        const string ApiBase = @"http://172.16.71.134/BackBox.Api/";
        readonly WebClientEx client = new WebClientEx(new CookieContainer()) { BaseAddress = ApiBase };

        double MyLat = 0.0, MyLng = 0.0;

        void Log(string message)
        {
            displayText.Text += DateTime.Now.ToString("HH:mm  ") + message + Environment.NewLine;
        }

        public BackBox_iOSViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            Log("Hello there.");

            Log("Connecting to the service...");

            var id = client.DownloadString("connect");

            Log("Connected.");
            Log("My Id: " + id);

            Log("Need to keep track of your location.");

            var locMgr = new CLLocationManager();
            if (CLLocationManager.LocationServicesEnabled)
            {
                Log("Tracking...");

                locMgr.StartMonitoringSignificantLocationChanges();

                locMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    MyLat = e.Locations[0].Coordinate.Latitude;
                    MyLng = e.Locations[0].Coordinate.Longitude;

                    Log(string.Format("Your location is now: ({0}, {1}). Telling the service.", MyLat, MyLng));

                    var ret = client.DownloadString(string.Format("set-bounds/{0}/{1}/10", MyLat, MyLng));

                    Log("Response from the server: " + ret);
                };
            }
            else
            {
                Log("I can't keep track of you.  Enable me in your phones settings.");
            }

            const int KeyboardHeight = 170;

            inputText.EditingDidBegin += (object sender, EventArgs e) => 
            {
                View.Bounds = new RectangleF(new PointF(View.Bounds.X, View.Bounds.Y + KeyboardHeight), View.Bounds.Size);
            };

            inputText.EditingDidEnd += (object sender, EventArgs e) => 
            {
                View.Bounds = new RectangleF(new PointF(View.Bounds.X, View.Bounds.Y - KeyboardHeight), View.Bounds.Size);

                Log("Editing done...");
                Log(inputText.Text);

                client.DownloadString("send?message=" + inputText.Text);

                inputText.Text = string.Empty;
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion
    }

    public class WebClientEx : WebClient
    {
        public WebClientEx(CookieContainer container)
        {
            this.container = container;
        }

        private readonly CookieContainer container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            if (request != null)
            {
                request.CookieContainer = container;
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null)
            {
                CookieCollection cookies = response.Cookies;
                container.Add(cookies);
            }
        }
    }

}

