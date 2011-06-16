// ReSharper disable InconsistentNaming
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpfm.LastFmScrobbler.Tests.Integration
{
    [TestClass]
    public class ScrobblerTests : IntegrationTests
    {
        [TestMethod, TestCategory("Integration")]

        public void GetAuthorisationUri_TestApiKey_ReturnsWellFormedUri()
        {
            // Arrange
            var scrobbler = new Scrobbler(ApiKey, ApiSecret);
            string expectedResult = string.Format(Scrobbler.RequestAuthorisationUriPattern, ApiKey, "");

            // Act
            string uri = scrobbler.GetAuthorisationUri();

            // Assert
            Assert.IsTrue(uri.StartsWith(expectedResult));
        }

        [TestMethod, TestCategory("Integration"), ExpectedException(typeof(LastFmApiException))]
        public void GetSession_UnAuthorisedToken_ThrowsException()
        {
            // Arrange
            var scrobbler = new Scrobbler(ApiKey, ApiSecret);

            // Act
            string session = scrobbler.GetSession();

            // Assert
            Assert.IsNotNull(session);
        }

        [TestMethod, TestCategory("Integration"), ExpectedException(typeof(LastFmApiException))]
        public void GetSession_AuthorisedToken_ReturnsSessionKey()
        {
            // Arrange
            var scrobbler = new Scrobbler(ApiKey, ApiSecret);

            // break here and authorise via a browser
            string uri = scrobbler.GetAuthorisationUri();

            // Act
            string session = scrobbler.GetSession();

            // Assert
            Assert.IsNotNull(session);
        }

        [TestMethod, TestCategory("Integration")]
        public void NowPlaying_SessionKey_NoException()
        {
            // Arrange
            var scrobbler = new Scrobbler(ApiKey, ApiSecret, SessionKey);
            var track = new Track
            {
                TrackName = "Spying Glass",
                AlbumArtist = "Massive Attack",
                ArtistName = "Massive Attack",
                AlbumName = "Protection",
                Duration = new TimeSpan(0, 0, 5, 21),
                TrackNumber = 5,
            };

            // Act
            var response = scrobbler.NowPlaying(track);
        }

        [TestMethod, TestCategory("Integration")]
        public void Scrobble_TestTrack_NoException()
        {
            // Arrange
            var scrobbler = new Scrobbler(ApiKey, ApiSecret, SessionKey);
            var track = new Track
            {
                TrackName = "Spying Glass",
                AlbumArtist = "Massive Attack",
                ArtistName = "Massive Attack",
                AlbumName = "Protection",
                Duration = new TimeSpan(0, 0, 5, 21),
                TrackNumber = 5,
            };

            track.WhenStartedPlaying = DateTime.Now.Subtract(track.Duration);

            // Act
            var response = scrobbler.Scrobble(track);

        }
    }
}
