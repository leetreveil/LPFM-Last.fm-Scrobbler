// ReSharper disable InconsistentNaming
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lpfm.LastFmScrobbler.Api;

namespace Lpfm.LastFmScrobbler.Tests.Api
{
    [TestClass]
    public class AuthApiTests
    {
        [TestMethod]
        public void GetSessionFromNavigator_TestXml_SessionUsernameIsSet()
        {
            // Arrange
            var api = new AuthApiUnderTest();
            var xml = new XmlDocument();
            xml.LoadXml("<lfm status=\"ok\"><session><name>MyLastFMUsername</name><key>d580d57f32848f5dcf574d1ce18d78b2</key><subscriber>0</subscriber></session></lfm>");

            // Act
            var session = api.GetSessionFromNavigatorUnderTest(xml.CreateNavigator());

            // Assert
            Assert.AreEqual("MyLastFMUsername", session.Username);
        }
        [TestMethod]
        public void GetSessionFromNavigator_TestXml_SessionKeyIsSet()
        {
            // Arrange
            var api = new AuthApiUnderTest();
            var xml = new XmlDocument();
            xml.LoadXml("<lfm status=\"ok\"><session><name>MyLastFMUsername</name><key>d580d57f32848f5dcf574d1ce18d78b2</key><subscriber>0</subscriber></session></lfm>");

            // Act
            var session = api.GetSessionFromNavigatorUnderTest(xml.CreateNavigator());

            // Assert
            Assert.AreEqual("d580d57f32848f5dcf574d1ce18d78b2", session.Key);
        }

        [TestMethod]
        public void GetSessionFromNavigator_TestXml_SessionIsSubscriberIsSet()
        {
            // Arrange
            var api = new AuthApiUnderTest();
            var xml = new XmlDocument();
            xml.LoadXml("<lfm status=\"ok\"><session><name>MyLastFMUsername</name><key>d580d57f32848f5dcf574d1ce18d78b2</key><subscriber>0</subscriber></session></lfm>");

            // Act
            var session = api.GetSessionFromNavigatorUnderTest(xml.CreateNavigator());

            // Assert
            Assert.AreEqual(false, session.IsSubscriber);
        }

        [TestMethod]
        public void GetSessionFromNavigator_TestXmlSessionEquals1_SessionIsSubscriberIsSet()
        {
            // Arrange
            var api = new AuthApiUnderTest();
            var xml = new XmlDocument();
            xml.LoadXml("<lfm status=\"ok\"><session><name>MyLastFMUsername</name><key>d580d57f32848f5dcf574d1ce18d78b2</key><subscriber>1</subscriber></session></lfm>");

            // Act
            var session = api.GetSessionFromNavigatorUnderTest(xml.CreateNavigator());

            // Assert
            Assert.AreEqual(true, session.IsSubscriber);
        }
    }

    internal class AuthApiUnderTest : AuthApi
    {
        public Session GetSessionFromNavigatorUnderTest(XPathNavigator navigator)
        {
            return GetSessionFromNavigator(navigator);
        }
    }
}
