using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices;
using tmg.equinox.repository;

namespace tmg.equinox.tests.unittest.services
{
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {
        }

        [TestMethod]
        public void TestMethod1()
        {
            IUIElementService uiElementService = new UIElementService(new UnitOfWork());

            TextBoxElementModel rowModel = uiElementService.GetTextBox(1, 1, 3);
            Assert.AreEqual(3, rowModel.UIElementID);
        }
    }
}
