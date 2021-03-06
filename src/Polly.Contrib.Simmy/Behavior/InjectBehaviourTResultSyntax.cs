﻿using System;
using Polly.Contrib.Simmy.Behavior;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="behaviour">Behaviour Delegate to be executed without context</param>
        /// <param name="injectionRate">The injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free mode</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy<TResult> InjectBehaviour<TResult>(
            Action behaviour,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            void BehaviourLambda(Context _) => behaviour();
            double InjectionRateLambda(Context _) => injectionRate;
            bool EnabledLambda(Context _) => enabled();

            return InjectBehaviour<TResult>(BehaviourLambda, InjectionRateLambda, EnabledLambda);
        }

        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="behaviour">Behaviour Delegate to be executed</param>
        /// <param name="injectionRate">The injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free mode</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy<TResult> InjectBehaviour<TResult>(
            Action<Context> behaviour,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            double InjectionRateLambda(Context _) => injectionRate;
            bool EnabledLambda(Context _) => enabled();

            return InjectBehaviour<TResult>(behaviour, InjectionRateLambda, EnabledLambda);
        }

        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="behaviour">Behaviour Delegate to be executed</param>
        /// <param name="injectionRate">The injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy<TResult> InjectBehaviour<TResult>(
            Action<Context> behaviour,
            Double injectionRate,
            Func<Context, bool> enabled)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            double InjectionRateLambda(Context _) => injectionRate;
            return InjectBehaviour<TResult>(behaviour, InjectionRateLambda, enabled);
        }

        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="behaviour">Behaviour Delegate to be executed</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy<TResult> InjectBehaviour<TResult>(
            Action<Context> behaviour,
            Func<Context, Double> injectionRate,
            Func<Context, bool> enabled)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectBehaviourPolicy<TResult>(
                    behaviour,
                    injectionRate,
                    enabled);
        }
    }
}
