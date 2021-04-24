using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiseOfTheUndeaf.Core.Logging;

namespace RiseOfTheUndeaf.Tests.Core.Logging
{
    [TestClass]
    public class LogContextProviderTests
    {
        [TestMethod]
        public void AppliesContextCorrectly()
        {
            Dictionary<string, object> expectedContext;

            using (var scope1 = LogContextProvider.CreateScope(new() { { "P1", 1 }, { "P2", "w" } }))
            {
                expectedContext = new() { { "P1", 1 }, { "P2", "w" } };
                CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);

                using (var scope2 = LogContextProvider.CreateScope(new() { { "P1", 2 }, { "P3", "x" } }))
                {
                    expectedContext = new() { { "P1", 2 }, { "P2", "w" }, { "P3", "x" } };
                    CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);
                }

                expectedContext = new() { { "P1", 1 }, { "P2", "w" } };
                CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);
            }

            expectedContext = new() { };
            CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);
        }

        [TestMethod]
        public async Task WithAsync_ParentContextIsPreserved_ChildContextIsSeparate()
        {
            using (var scope1 = LogContextProvider.CreateScope(new() { { "P1", 1 }, { "P2", "w" } }))
            {
                await Task.WhenAll(
                    new Func<Task>(async () =>
                    {
                        using (var scope2 = LogContextProvider.CreateScope(new() { { "P3", "x" } }))
                        {
                            await Task.Delay(10);
                            var expectedContext = new Dictionary<string, object>() { { "P1", 1 }, { "P2", "w" }, { "P3", "x" } };
                            CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);
                        }
                    })(),
                    new Func<Task>(async () =>
                    {
                        using (var scope2 = LogContextProvider.CreateScope(new() { { "P3", "y" } }))
                        {
                            await Task.Delay(10);
                            var expectedContext = new Dictionary<string, object>() { { "P1", 1 }, { "P2", "w" }, { "P3", "y" } };
                            CollectionAssert.AreEquivalent(expectedContext, LogContextProvider.CurrentContext);
                        }
                    })());
            }
        }
    }
}
