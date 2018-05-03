using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Spike.Support.Shared.Models;

namespace Spike.Support.Shared.UnitTests
{
    public class WhenTestingNavItem : WhenTesting<NavItem>
    {
        private Dictionary<string, NavItem> _result;
        private Dictionary<string, NavItem> _badResult;

        protected override void Given()
        {
            base.Given();
            Unit.Key = "1";
            Unit.Text = "1";
            Unit.NavigateUrl = "/controller/method";
            Unit.NavItems.Add(new NavItem
            {
                Key = "1.1",
                Text = "1.1",
                NavigateUrl = "controller/method/{entityId}",
                NavItems = new List<NavItem>
                {
                    new NavItem
                    {
                        Key = "1.1.1",
                        Text = "1.1.1",
                        NavigateUrl = "controller/method/{entityId}/items/{itemId}/header"
                    },
                    new NavItem
                    {
                        Key = "1.1.2",
                        Text = "1.1.2",
                        NavigateUrl = "controller/method/{entityId}/items/{itemId}/body"
                    }
                }
            });
            Unit.NavItems.Add(new NavItem
            {
                Key = "1.2",
                Text = "1.2",
                NavigateUrl = "controller/method/{entityId}/items/{itemId}"
            });
        }

        protected override void When()
        {
            base.When();
            _result = NavItem.TransformNavItems(
                new Dictionary<string, NavItem> {{Unit.Key, Unit}},
                new Uri("http://localhost:44347/api", UriKind.RelativeOrAbsolute),
                new Dictionary<string, string> {{"entityId", "1"}, {"itemId", "2"}}
            );
            _badResult = NavItem.TransformNavItems(
                new Dictionary<string, NavItem> {{Unit.Key, Unit}},
                new Uri("http://localhost:44347/api", UriKind.RelativeOrAbsolute),
                new Dictionary<string, string> {{"entityId", "1"}}
            );
        }

        [Test]
        public void ItShouldContainNestedNavItems()
        {
            CollectionAssert.IsNotEmpty(Unit.NavItems);
        }


        [Test]
        public void ItShouldContainUnTransformedNavigationUrls()
        {
            var final = JsonConvert.SerializeObject(_badResult);
            Assert.IsTrue(final.Contains("/{"));
        }

        [Test]
        public void ItShouldHaveTheCorrectNumberOfNavItems()
        {
            Assert.AreEqual(5, new List<NavItem> {Unit}.Map(x => true, item => item.NavItems).Count());
        }

        [Test]
        public void ItShouldHaveTransformedAllOfTheNavigationUrls()
        {
            var final = JsonConvert.SerializeObject(_result);
            Assert.IsFalse(final.Contains("/{"));
        }

        [Test]
        public void ItShouldNotNbuNull()
        {
            Assert.IsNotNull(Unit);
        }

        [Test]
        public void ItShouldNotTransformTheOriginalItems()
        {
            var original = JsonConvert.SerializeObject(new List<NavItem> {Unit});
            var final = JsonConvert.SerializeObject(_result.Values);
            Assert.AreNotEqual(original, final);
        }

        [Test]
        public void ItShouldTransformNestedNavItems()
        {
            Assert.AreEqual(1, _result.Count);
        }
    }
}