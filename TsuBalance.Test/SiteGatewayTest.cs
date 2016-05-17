using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TsuBalance.Test
{
    [TestClass]
    public class SiteGatewayTest
    {
        [TestMethod]
        public void Authorize()
        {
            var gateway = new SiteGateway();
            var okResult = gateway.AuthorizeAsync("validLogin", "validPassword").Result;
            var badResult = gateway.AuthorizeAsync("asfas", "asfas").Result;

            Assert.IsTrue(okResult);
            Assert.IsFalse(badResult);
        }

        [TestMethod]
        public void GetBalance()
        {
            var gateway = new SiteGateway();
            var res =  gateway.AuthorizeAsync("validLogin", "validPassword").Result;
            var balance = gateway.GetBalanceAsync().Result;
        }
    }
}
