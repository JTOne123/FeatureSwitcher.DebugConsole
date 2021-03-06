﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FeatureSwitcher.DebugConsole.Controllers;
using FeatureSwitcher.DebugConsole.Controllers.ViewModels;
using FeatureSwitcher.DebugConsole.Tests.Features;
using Xunit;

namespace FeatureSwitcher.DebugConsole.Tests.Controllers
{
    public class DebugConsoleControllerTest
    {
        [Fact]
        public void DebugConsoleController_Disabled_Test()
        {

            // Disabled by default
            DebugConsoleController controller = new DebugConsoleController();
            SetContext(controller);
            var states = controller.GetStates() as JsonResult;

            Assert.False((states.Data as GetStateResult).Enabled);
            Assert.Null((states.Data as GetStateResult).States);
        }

        [Fact]
        public void DebugConsoleController_Enabled_Test()
        {

            Configuration.Features.Are.ConfiguredBy.Custom(
                Configuration.Features.OfType<TestFeature2>.Enabled);

            DebugConsoleController controller = new DebugConsoleController();
            SetContext(controller);
            controller.SetForced();

            var result = (controller.GetStates() as JsonResult).Data as GetStateResult;

            Assert.True(result.Enabled);
            Assert.NotNull(result.States);
            Assert.Equal(3, result.States.Length);
            Assert.Equal(false, result.States.First(state => state.FeatureName == typeof(TestFeature1).FullName).Enabled);
            Assert.Equal(true, result.States.First(state => state.FeatureName == typeof(TestFeature2).FullName).Enabled);
            Assert.Equal(false, result.States.First(state => state.FeatureName == typeof(TestFeature3).FullName).Enabled);
            Assert.Equal("TestFeature1", result.States.First(state => state.FeatureName == typeof(TestFeature1).FullName).ShortFeatureName);
            Assert.Equal("TestFeature2", result.States.First(state => state.FeatureName == typeof(TestFeature2).FullName).ShortFeatureName);
            Assert.Equal("TestFeature3", result.States.First(state => state.FeatureName == typeof(TestFeature3).FullName).ShortFeatureName);
        }

        private static void SetContext(Controller controller)
        {
            HttpRequest orgRequest = new HttpRequest(
                null,
                "http://q.queue-it.net/Inqueue.aspx",
                "");

            HttpContext context = new HttpContext(orgRequest, new HttpResponse(new StringWriter()));
            controller.ControllerContext = new ControllerContext(new HttpContextWrapper(context), new RouteData(), controller);
        }

    }
}
