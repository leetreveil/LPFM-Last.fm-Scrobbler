// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using System.Xml;
using Lpfm.LastFmScrobbler.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpfm.LastFmScrobbler.Tests.Api
{
    [TestClass]
    public class ApiHelperTests
    {
        private const string ApiKey = "64e33f8feab8756ed8e3a97db96c0ce6";
        private const string ApiSecret = "98a03328296293b1a0688ed5e3b247ee";
        private const string Token = "70358894d8504f129f574ce0e754870c";

        [TestMethod]
        public void CheckLastFmStatus_StatusOk_NoException()
        {
            // Arrange
            const string xml = "<lfm status=\"ok\"><token>1acf8bef4ac65bbd124fe8d4ddd9ab32</token></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            // Act
            ApiHelper.CheckLastFmStatus(mockedNavigator);

            // Assert - OK
        }

        [TestMethod]
        [ExpectedException(typeof (LastFmApiException))]
        public void CheckLastFmStatus_StatusFailed_ThrowsException()
        {
            // Arrange
            const string xml = "<lfm status=\"failed\"><error code=\"10\">Invalid API Key</error></lfm>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            var mockedNavigator = document.CreateNavigator();

            // Act
            ApiHelper.CheckLastFmStatus(mockedNavigator);

            // Assert
            Assert.Fail("Should have thrown exception");
        }

        [TestMethod]
        public void GetStringOfOrderedParamsForHashing_TestValues_ReturnsOrderedString()
        {
            // Arrange
            var values = new Dictionary<string, string>
                             {
                                 {"A", "B"},
                                 {"1", "2"},
                                 {"C", "D"}
                             };

            const string expectedResult = "12ABCD";

            // Act
            string result = ApiHelper.GetStringOfOrderedParamsForHashing(values);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void GetApiSignature_TestValues_Returns32ByteHash()
        {
            // Arrange
            var values = new Dictionary<string, string>
                             {
                                 {ApiHelper.ApiKeyParamName, ApiKey},
                                 {ApiHelper.MethodParamName, AuthApi.GetSessionMethodName},
                                 {"token", Token}
                             };

            // Act
            string result = ApiHelper.GetApiSignature(values, ApiSecret);

            // Assert
            Assert.IsTrue(result.Length == 32);
        }

        [TestMethod]
        public void Hash_FixedString_ReturnsSameValueTwice()
        {
            // Arrange
            string result1 = ApiHelper.Hash(Token);

            // Act
            string result2 = ApiHelper.Hash(Token);

            // Assert
            Assert.AreEqual(result1, result2);
        }
    }
}