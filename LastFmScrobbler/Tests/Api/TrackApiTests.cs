// ReSharper disable InconsistentNaming
using System;
using System.Xml;
using System.Xml.XPath;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpfm.LastFmScrobbler.Tests.Api
{
    [TestClass]
    public class TrackApiTests
    {
        const string NowPlayingXml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><lfm status=\"ok\"><nowplaying><track corrected=\"0\">Spying Glass</track><artist corrected=\"0\">Massive Attack</artist><album corrected=\"0\">Protection</album><albumArtist corrected=\"0\">Massive Attack (Album Artist)</albumArtist><ignoredMessage code=\"0\"></ignoredMessage></nowplaying></lfm>";

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_MockResponse_TrackNameSet()
        {
            // Arrange
            var document = new XmlDocument();
            document.LoadXml(NowPlayingXml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.AreEqual("Spying Glass", response.Track.TrackName);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_MockResponse_ArtistNameSet()
        {
            // Arrange
            var document = new XmlDocument();
            document.LoadXml(NowPlayingXml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.AreEqual("Massive Attack", response.Track.ArtistName);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_MockResponse_AlbumNameSet()
        {
            // Arrange
            var document = new XmlDocument();
            document.LoadXml(NowPlayingXml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.AreEqual("Protection", response.Track.AlbumName);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_MockResponse_AlbumArtistSet()
        {
            // Arrange
            var document = new XmlDocument();
            document.LoadXml(NowPlayingXml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.AreEqual("Massive Attack (Album Artist)", response.Track.AlbumArtist);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_CorrectedTrackName_TrackNameCorrectedIsTrue()
        {
            // Arrange
            const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><lfm status=\"ok\"><nowplaying><track corrected=\"1\">Spying Glass</track><artist corrected=\"0\">Massive Attack</artist><album corrected=\"0\">Protection</album><albumArtist corrected=\"0\">Massive Attack</albumArtist><ignoredMessage code=\"0\"></ignoredMessage></nowplaying></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.IsTrue(((CorrectedTrack) response.Track).TrackNameCorrected);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_CorrectedTrackName_AlbumNameCorrectedIsTrue()
        {
            // Arrange
            const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><lfm status=\"ok\"><nowplaying><track corrected=\"0\">Spying Glass</track><artist corrected=\"0\">Massive Attack</artist><album corrected=\"1\">Protection</album><albumArtist corrected=\"0\">Massive Attack</albumArtist><ignoredMessage code=\"0\"></ignoredMessage></nowplaying></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.IsTrue(((CorrectedTrack)response.Track).AlbumNameCorrected);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_CorrectedTrackName_ArtistNameCorrectedIsTrue()
        {
            // Arrange
            const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><lfm status=\"ok\"><nowplaying><track corrected=\"0\">Spying Glass</track><artist corrected=\"1\">Massive Attack</artist><album corrected=\"0\">Protection</album><albumArtist corrected=\"0\">Massive Attack</albumArtist><ignoredMessage code=\"0\"></ignoredMessage></nowplaying></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.IsTrue(((CorrectedTrack)response.Track).ArtistNameCorrected);
        }

        [TestMethod]
        public void GetNowPlayingResponseFromNavigator_CorrectedTrackName_AlbumArtistCorrectedIsTrue()
        {
            // Arrange
            const string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><lfm status=\"ok\"><nowplaying><track corrected=\"0\">Spying Glass</track><artist corrected=\"0\">Massive Attack</artist><album corrected=\"0\">Protection</album><albumArtist corrected=\"1\">Massive Attack</albumArtist><ignoredMessage code=\"0\"></ignoredMessage></nowplaying></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            var api = new TrackApiUnderTest();

            // Act
            var response = api.GetNowPlayingResponseFromNavigatorUnderTest(mockedNavigator);

            // Assert
            Assert.IsTrue(((CorrectedTrack)response.Track).AlbumArtistCorrected);
        }
    }

    internal class TrackApiUnderTest : TrackApi
    {
        public NowPlayingResponse GetNowPlayingResponseFromNavigatorUnderTest(XPathNavigator navigator)
        {
            return GetNowPlayingResponseFromNavigator(navigator);
        }
    }
}
