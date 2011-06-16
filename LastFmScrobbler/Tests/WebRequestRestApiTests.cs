// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml.XPath;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Lpfm.LastFmScrobbler.Tests
{
    [TestClass]
    public class WebRequestRestApiTests
    {
        [TestMethod]
        public void SendGetRequest_MockRequest_MethodIsGet()
        {
            // Arrange
            const string method = "GET";
            const string uri = "http://foo.bar/";
            var queryItems = new Dictionary<string, string>();

            var api = new WebRequestRestApiUnderTest();

            // Act
            api.SendGetRequest(uri, queryItems);
            

            // Assert
            Assert.AreEqual(method, api.MockWebRequest.Object.Method);
        }

        [TestMethod]
        public void BuildStringOfItems_TestQueryItems_StringContainsItemsInExpectedFormat()
        {
            var queryItems = new Dictionary<string, string> {{"TestKey1", "TestValue1"}, {"TestKey2", "TestValue2"}};

            var api = new WebRequestRestApiUnderTest();

            // Act
            string queryString = api.BuildStringOfItemsUnderTest(queryItems);

            // Assert
            Assert.AreEqual(queryString, FormatQueryItemsAsQueryString(queryItems));
        }

        private static string FormatQueryItemsAsQueryString(Dictionary<string, string> queryItems)
        {
            var builder = new StringBuilder();

            foreach (KeyValuePair<string, string> keyValuePair in queryItems)
            {
                builder.AppendFormat(WebRequestRestApi.NameValuePairStringFormat, keyValuePair.Key, keyValuePair.Value);
                builder.Append(WebRequestRestApi.NameValuePairStringSeperator);
            }

            return builder.ToString(0, builder.Length - 1);
        }
    }

    internal class WebRequestRestApiUnderTest : WebRequestRestApi
    {
        public Mock<WebRequest> MockWebRequest = new Mock<WebRequest>();
        public Mock<WebResponse> MockWebResponse = new Mock<WebResponse>();
        public Mock<XPathNavigator> MockXPathNavigator = new Mock<XPathNavigator>();

        public string Uri { get; set; }

        protected override WebRequest CreateWebRequest(Uri uri)
        {
            return CreateWebRequest(uri.ToString());
        }

        protected override WebRequest CreateWebRequest(string uri)
        {
            Uri = uri;
            MockWebRequest.SetupProperty(wr => wr.Method);
            MockWebRequest.Setup(wr => wr.GetResponse()).Returns(MockWebResponse.Object);
            return MockWebRequest.Object;
        }

        protected internal override XPathNavigator GetResponseAsXml(WebRequest request)
        {
            return MockXPathNavigator.Object;
        }

        protected override XPathNavigator GetXpathDocumentFromResponse(WebResponse response)
        {
            return MockXPathNavigator.Object;
        }

        public string BuildStringOfItemsUnderTest(Dictionary<string, string> queryItems)
        {
            return BuildStringOfItems(queryItems);
        }
    }
}
