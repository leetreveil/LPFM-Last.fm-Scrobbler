// ReSharper disable InconsistentNaming
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lpfm.LastFmScrobbler.Tests
{
    [TestClass]
    public class ScrobblerTests
    {
        [TestMethod]
        public void GetAuthenticationToken_ValidToken_DoesNotCallApiGetToken()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("testApiKey", "testApiSecret");
            scrobbler.AuthenticationTokenUnderTest = new AuthenticationToken("testToken1");

            // Act
            scrobbler.GetAuthenticationTokenUnderTest();

            // Assert
            scrobbler.AuthApiMock.Verify(api => api.GetToken(It.IsAny<Authentication>()), Times.Never());

        }

        [TestMethod]
        public void GetAuthenticationToken_NullToken_CallsApiGetToken()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("testApiKey", "testApiSecret");
            scrobbler.AuthenticationTokenUnderTest = null;

            // Act
            scrobbler.GetAuthenticationTokenUnderTest();

            // Assert
            scrobbler.AuthApiMock.Verify(api => api.GetToken(It.IsAny<Authentication>()), Times.Once());

        }

        [TestMethod]
        public void GetAuthenticationToken_InvalidToken_CallsApiGetToken()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("testApiKey", "testApiSecret");
            scrobbler.AuthenticationTokenUnderTest = new AuthenticationTokenUnderTest("testToken1", DateTime.Now.AddMinutes(-AuthenticationToken.ValidForMinutes * 2));

            // Act
            scrobbler.GetAuthenticationTokenUnderTest();

            // Assert
            scrobbler.AuthApiMock.Verify(api => api.GetToken(It.IsAny<Authentication>()), Times.Once());

        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Scrobble_DurationTooShort_ThrowsException()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("test", "test");
            
            // Act
            scrobbler.Scrobble(new Track {Duration = new TimeSpan(Scrobbler.MinimumScrobbleTrackLengthInSeconds/2), WhenStartedPlaying = DateTime.Now.AddMinutes(-4)});
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Scrobble_NotPlayedForLongEnough_ThrowsException()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("test", "test");
            
            // Act
            scrobbler.Scrobble(new Track {Duration = new TimeSpan(Scrobbler.MinimumScrobbleTrackLengthInSeconds*2), WhenStartedPlaying = DateTime.Now});
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Scrobble_TrackDurationAndWhenStartedOk_CallsApiScrobble()
        {
            // Arrange
            var scrobbler = new ScrobblerUnderTest("test", "test");
            
            // Act
            scrobbler.Scrobble(new Track {Duration = new TimeSpan(Scrobbler.MinimumScrobbleTrackLengthInSeconds*2), WhenStartedPlaying = DateTime.Now.AddMinutes(-4)});

            // Assert
            scrobbler.TrackApiMock.Verify(a=>a.Scrobble(It.IsAny<Track>(), It.IsAny<Authentication>()));
        }

    }

    internal class ScrobblerUnderTest : Scrobbler
    {
        public ScrobblerUnderTest(string apiKey, string apiSecret) : this(apiKey, apiSecret, null)
        {
        }

        public ScrobblerUnderTest(string apiKey, string apiSecret, string sessionKey) : base(apiKey, apiSecret, sessionKey)
        {
            AuthApiMock.Setup(api => api.GetToken(It.IsAny<Authentication>())).Returns(new AuthenticationToken("testToken2"));
            AuthApi = AuthApiMock.Object;
            TrackApi = TrackApiMock.Object;
        }

        public Mock<IAuthApi> AuthApiMock = new Mock<IAuthApi>();
        public Mock<ITrackApi> TrackApiMock = new Mock<ITrackApi>();
            
        public AuthenticationToken AuthenticationTokenUnderTest { set { AuthenticationToken = value; } }

        public void GetAuthenticationTokenUnderTest()
        {
            GetAuthenticationToken();
        }
    }
}
