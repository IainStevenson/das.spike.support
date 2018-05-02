using System;
using NUnit.Framework;

namespace Spike.Support.Shared.UnitTests
{
    [TestFixture]
    public abstract class WhenTesting<T> where T: class
    {
        protected T Unit;
        [SetUp]public void Setup() {
            Given();
            When();
        }

        protected virtual void Given()
        {
            Unit = Activator.CreateInstance<T>();
        }
        protected virtual void When() { }

    }
}
