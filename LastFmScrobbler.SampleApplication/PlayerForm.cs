using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using AxWMPLib;
using Lpfm.LastFmScrobbler;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.Win32;
using TagLib;
using WMPLib;
using Timer = System.Timers.Timer;

namespace LastFmScrobbler.SampleApplication
{
    public partial class PlayerForm : Form
    {
        private const string LpfmRegistryNameSpace = "HKEY_CURRENT_USER\\Software\\LastFmScrobbler.SampleApplication";

        //TODO: Go to http://www.last.fm/api/account and apply for an API account. Then paste the key and secret into these constants
        private const string ApiKey = "";
        private const string ApiSecret = "";

        private readonly Timer _controllerTimer;
        private readonly AsyncScrobbler _scrobbler;

        public PlayerForm()
        {
            try
            {
                InitializeComponent();

                // Set up the Scrobbler
                if (string.IsNullOrEmpty(ApiKey) || string.IsNullOrEmpty(ApiSecret))
                {
                    throw new InvalidOperationException(
                        "ApiKey and ApiSecret have not been set. Go to http://www.last.fm/api/account and apply for an API account. Then paste the key and secret into the constants on PlayerForm.cs");
                }

                string sessionKey = GetSessionKey();

                // instantiate the async scrobbler
                _scrobbler = new AsyncScrobbler(ApiKey, ApiSecret, sessionKey);

                // create a thread safe Timer that processess any outstanding scrobbles every 2 seconds
                _controllerTimer = new Timer(2000) {AutoReset = true};
                _controllerTimer.Elapsed += ControllerTimer_Elapsed;
                _controllerTimer.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private Track CurrentTrack { get; set; }

        private void ControllerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _scrobbler.Process();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

        private static string GetSessionKey()
        {
            const string sessionKeyRegistryKeyName = "LastFmSessionKey";

            // try get the session key from the registry
            string sessionKey = GetRegistrySetting(sessionKeyRegistryKeyName, null);

            if (string.IsNullOrEmpty(sessionKey))
            {
                // instantiate a new scrobbler
                var scrobbler = new Scrobbler(ApiKey, ApiSecret);

                //NOTE: This is demo code. You would not normally do this in a production application
                while (string.IsNullOrEmpty(sessionKey))
                {
                    // Try get session key from Last.fm
                    try
                    {
                        sessionKey = scrobbler.GetSession();

                        // successfully got a key. Save it to the registry for next time
                        SetRegistrySetting(sessionKeyRegistryKeyName, sessionKey);
                    }
                    catch (LastFmApiException exception)
                    {
                        // get session key from Last.fm failed
                        MessageBox.Show(exception.Message);

                        // get a url to authenticate this application
                        string url = scrobbler.GetAuthorisationUri();

                        // open the URL in the default browser
                        Process.Start(url);

                        // Block this application while the user authenticates
                        MessageBox.Show("Click OK when Application authenticated");
                    }
                }
            }

            return sessionKey;
        }

        private void FileMenuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                var result = FileOpenDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var wmpMedia = (IWMPMedia3)WindowsMediaPlayer.newMedia(FileOpenDialog.FileName);
                    WindowsMediaPlayer.currentMedia = wmpMedia;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void WindowsMediaPlayer_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                switch (WindowsMediaPlayer.playState)
                {
                    case WMPPlayState.wmppsPlaying:
                        // Convert the media player media info to a Track. Store the track in a property so that is can be scrobbled 
                        //  when media ended. At that time WindowsMediaPlayer.currentMedia will be null. This is not fool-proof and
                        //  is demo code only
                        CurrentTrack = WmpMediaToTrack(WindowsMediaPlayer.currentMedia);
                        CurrentTrack.WhenStartedPlaying = DateTime.Now;

                        // we are using the Async scrobbler here so that we don't block the form while the scrobble request is being sent
                        //  to the Last.fm web service. The request will be sent when the Process() method is called by the _controllerTimer event
                        _scrobbler.NowPlaying(CurrentTrack);
                        break;

                    case WMPPlayState.wmppsMediaEnded:
                        // Scrobble the track that just finished
                        _scrobbler.Scrobble(CurrentTrack);
                        break;
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private static Track WmpMediaToTrack(IWMPMedia media)
        {
            // Get meta data from media as track using TagLib sharp
            File fileTag = File.Create(media.sourceURL);

            var track = new Track
                            {
                                TrackName = fileTag.Tag.Title,
                                AlbumName = fileTag.Tag.Album,
                                ArtistName = fileTag.Tag.JoinedPerformers,
                                TrackNumber = (int) fileTag.Tag.Track,
                                Duration = new TimeSpan(0, 0, 0, (int) media.duration)
                            };
            return track;
        }

        public static string GetRegistrySetting(string valueName, string defaultValue)
        {
            if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
            valueName = valueName.Trim();

            object regValue = Registry.GetValue(LpfmRegistryNameSpace, valueName, defaultValue);

            if (regValue == null)
            {
                // Key does not exist
                return defaultValue;
            }
            else
            {
                return regValue.ToString();
            }
        }

        public static void SetRegistrySetting(string valueName, string value)
        {
            if (string.IsNullOrEmpty(valueName)) throw new ArgumentException("valueName cannot be empty or null", "valueName");
            valueName = valueName.Trim();

            Registry.SetValue(LpfmRegistryNameSpace, valueName, value);
        }
    }
}