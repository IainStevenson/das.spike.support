using System;
using NUnit.Framework;

namespace Spike.Support.Shared.UnitTests
{
    [TestFixture]
    public abstract class WhenTesting<T> where T : class
    {
        [SetUp]
        public void Setup()
        {
            Given();
            When();
        }

        protected T Unit;

        protected virtual void Given()
        {
            Unit = Activator.CreateInstance<T>();
        }

        protected virtual void When()
        {
        }
    }
}