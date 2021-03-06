﻿using System;
using FluentAssertions;
using Polly;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Fault
{
    [Collection(Constants.AmbientContextDependentTestCollection)]
    public class InjectFaultTResultSpecs : IDisposable
    {
        public InjectFaultTResultSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
        }

        public void Dispose()
        {
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Basic Overload, Exception, Context Free
        [Fact]
        public void InjectFaultContext_Free_Enabled_Should_not_execute_user_delegate()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.6, () => true);
            policy.Invoking(x => x.Execute(action))
                .ShouldThrowExactly<Exception>().WithMessage(exceptionMessage);

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultContext_Free_Enabled_Should_execute_user_delegate()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.3, () => true);
            policy.Invoking(x => x.Execute(action))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Basic Overload, Exception, With Context
        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(
                new Exception(),
                0.6,
                (ctx) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrowExactly<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_execute_user_delegate()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(
                new Exception(),
                0.4,
                (ctx) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute(action, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_execute_user_delegate_with_enabled_lambda_disabled()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = false;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(
                new Exception(),
                0.6,
                (ctx) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute(action, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Overload, All based on context
        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate_default_context()
        {
            Boolean executed = false;
            Context context = new Context();
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            Func<Context, Exception> fault = (ctx) => new Exception();
            Func<Context, double> injectionRate = (ctx) => 0.6;
            Func<Context, bool> enabled = (ctx) => true;
            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrow<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate_full_context()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            Func<Context, Exception> fault = (ctx) =>
            {
                if (ctx["Message"] != null)
                {
                    return new InvalidOperationException(ctx["Message"].ToString());
                }

                return new Exception();
            };

            Func<Context, double> injectionRate = (ctx) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, bool> enabled = (ctx) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrowExactly<InvalidOperationException>();

            executed.Should().BeFalse();
        }
        #endregion

        #region TResult Based Monkey Policies
        [Fact]
        public void InjectFaultContext_Free_Should_Return_Fault()
        {
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.6, () => true);
            ResultPrimitive response = policy.Execute(action);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultContext_Free_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.4, () => true);
            ResultPrimitive response = policy.Execute(action);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_Enabled_Should_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, bool> enabled = (ctx) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.6, enabled);
            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Enabled_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = false;
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, bool> enabled = (ctx) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, 0.6, enabled);
            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_InjectionRate_Should_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context();
            context["InjectionRate"] = 0.6;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            Func<Context, ResultPrimitive> fault = (ctx) =>
            {
                return ResultPrimitive.Fault;
            };

            Func<Context, Double> injectionRate = (ctx) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, bool> enabled = (ctx) =>
            {
                return true;
            };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, injectionRate, enabled);
            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_InjectionRate_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context();
            context["InjectionRate"] = 0.4;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            Func<Context, ResultPrimitive> fault = (ctx) =>
            {
                return ResultPrimitive.Fault;
            };

            Func<Context, Double> injectionRate = (ctx) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, bool> enabled = (ctx) =>
            {
                return true;
            };

            var policy = MonkeyPolicy.InjectFault<ResultPrimitive>(fault, injectionRate, enabled);
            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }
        #endregion
    }
}
