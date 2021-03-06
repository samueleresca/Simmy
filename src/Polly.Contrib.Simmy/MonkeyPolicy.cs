﻿using System;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Contains common functionality for policies which intentionally disrupt sync executions - which monkey around with calls.
    /// </summary>
    public abstract partial class MonkeyPolicy : Policy, IMonkeyPolicy
    {
        internal Func<Context, Double> InjectionRate { get; }

        internal Func<Context, bool> Enabled { get; }

        internal MonkeyPolicy(Func<Context, Double> injectionRate, Func<Context, bool> enabled)
        {
            InjectionRate = injectionRate ?? throw new ArgumentNullException(nameof(injectionRate));
            Enabled = enabled ?? throw new ArgumentNullException(nameof(enabled));
        }
    }

    /// <summary>
    /// Contains common functionality for policies which intentionally disrupt sync executions returning TResult - which monkey around with calls.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public abstract class MonkeyPolicy<TResult> : Policy<TResult>, IMonkeyPolicy<TResult>
    {
        internal Func<Context, Double> InjectionRate { get; }

        internal Func<Context, bool> Enabled { get; }

        internal MonkeyPolicy(Func<Context, Double> injectionRate, Func<Context, bool> enabled) 
        {
            InjectionRate = injectionRate ?? throw new ArgumentNullException(nameof(injectionRate));
            Enabled = enabled ?? throw new ArgumentNullException(nameof(enabled));
        }
    }
}
