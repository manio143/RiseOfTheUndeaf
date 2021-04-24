using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace RiseOfTheUndeaf.Core.Logging
{
    /// <summary>
    /// Provider of context (additional data) for structured logging.
    /// </summary>
    public static class LogContextProvider
    {
        private static readonly AsyncLocal<ImmutableStack<Scope>> LocalScopeStack = new AsyncLocal<ImmutableStack<Scope>>();
        private static AsyncLocal<Dictionary<string, object>> current = new AsyncLocal<Dictionary<string, object>>();

        private static ImmutableStack<Scope> ScopeStack
        {
            get
            {
                return LocalScopeStack.Value;
            }
            set
            {
                LocalScopeStack.Value = value;
            }
        }

        static LogContextProvider()
        {
            LocalScopeStack.Value = ImmutableStack<Scope>.Empty;
            current.Value = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the current context enrichment (key-value pairs) for logging purposes.
        /// </summary>
        public static Dictionary<string, object> CurrentContext => current.Value;

        /// <summary>
        /// Creates a nested scope applying <paramref name="context"/> onto the current context.
        /// </summary>
        /// <param name="context">New context of the scope.</param>
        /// <returns>Disposable object that ends the created scope removing context changes.</returns>
        public static IDisposable CreateScope(Dictionary<string, object> context)
        {
            var scope = new Scope { context = context };
            ScopeStack = ScopeStack.Push(scope);
            RebuildCurrent();
            return scope;
        }

        private static void RebuildCurrent()
        {
            var dict = new Dictionary<string, object>();
            foreach (var scope in ScopeStack.Reverse())
            {
                foreach(var (key, value) in scope.context)
                {
                    dict[key] = value;
                }
            }
            current.Value = dict;
        }

        private class Scope : IDisposable
        {
            internal Dictionary<string, object> context;

            public void Dispose()
            {
                if (ScopeStack.Peek() != this)
                {
                    throw new InvalidOperationException("Parent logging scope is being disposed before child scope.");
                }

                ScopeStack = ScopeStack.Pop();
                RebuildCurrent();
            }
        }
    }
}