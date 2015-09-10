﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NextLevelSeven.Core;
using NextLevelSeven.Native;
using NextLevelSeven.Routing;

namespace NextLevelSeven.Test.Routing
{
    [TestClass]
    public class ConditionalMethodRouterTests
    {
        [TestMethod]
        public void ConditionalMethodRouter_ReceivesMessages()
        {
            var queried = false;
            var message = Message.Create(ExampleMessages.Standard);
            var router = new ConditionalMethodRouter(m => { queried = true; return true; }, m => {});
            Assert.IsFalse(queried, "Test initialized incorrectly.");
            message.RouteTo(router);
            Assert.IsTrue(queried, "Router was not queried.");
        }

        [TestMethod]
        public void ConditionalMethodRouter_PassesMessagesThrough()
        {
            var routed = false;
            var message = Message.Create(ExampleMessages.Standard);
            var router = new ConditionalMethodRouter(m => true, m => routed = true);
            Assert.IsFalse(routed, "Test initialized incorrectly.");
            message.RouteTo(router);
            Assert.IsTrue(routed, "Router did not reroute.");
        }

        [TestMethod]
        public void ConditionalMethodRouter_PassesCorrectData()
        {
            INativeMessage routedData = null;
            var message = Message.Create(ExampleMessages.Standard);
            var router = new ConditionalMethodRouter(m => true, m => { routedData = m; });
            Assert.IsNull(routedData, "Test initialized incorrectly.");
            message.RouteTo(router);
            Assert.IsNotNull(routedData);
            Assert.AreEqual(message.ToString(), routedData.ToString());
        }

        [TestMethod]
        public void ConditionalMethodRouter_ReturnsSuccessIfNoTarget()
        {
            var message = Message.Create(ExampleMessages.Standard);
            var listener = new ConditionalMethodRouter(m => true, null);
            Assert.IsTrue(message.RouteTo(listener), "Listener must return True when there is no target router.");
        }
    }
}