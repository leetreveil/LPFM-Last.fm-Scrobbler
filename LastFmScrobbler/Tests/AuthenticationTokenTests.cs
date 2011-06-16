// ReSharper disable InconsistentNaming
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpfm.LastFmScrobbler.Tests
{
    [TestClass]
    public class AuthenticationTokenTests
    {
        [TestMethod]
        public void IsValid_CreatedDateTooOld_ReturnsFalse()
        {
            // Arrange
            var token = new AuthenticationTokenUnderTest("abc", DateTime.Now.AddMinutes(-AuthenticationToken.ValidForMinutes*2));

            // Act
            bool result = token.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_CreatedDateNotTooOld_ReturnsTrue()
        {
            // Arrange
            var token = new AuthenticationTokenUnderTest("abc", DateTime.Now.AddMinutes(-AuthenticationToken.ValidForMinutes / 2));

            // Act
            bool result = token.IsValid();

            // Assert
            Assert.IsTrue(result);

        }
    }

    internal class AuthenticationTokenUnderTest : AuthenticationToken
    {
        public AuthenticationTokenUnderTest(string value, DateTime created) : base(value)
        {
            Created = created;
        }
    }
}
