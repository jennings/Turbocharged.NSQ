using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turbocharged.NSQ
{
    public interface IBackoffStrategy
    {
        IBackoffLimiter Create();
    }

    public interface IBackoffLimiter
    {
        bool ShouldReconnect(out TimeSpan delay);
    }

    /// <summary>
    /// Delays a constant amount of time between reconnections.
    /// </summary>
    public class FixedDelayBackoffStrategy : IBackoffStrategy
    {
        readonly TimeSpan _delay;
        public FixedDelayBackoffStrategy(TimeSpan delay)
        {
            _delay = delay;
        }

        public IBackoffLimiter Create()
        {
            return new FixedDelayBackoffLimiter(_delay);
        }

        class FixedDelayBackoffLimiter : IBackoffLimiter
        {
            TimeSpan _delay;
            public FixedDelayBackoffLimiter(TimeSpan delay)
            {
                _delay = delay;
            }

            public bool ShouldReconnect(out TimeSpan delay)
            {
                delay = _delay;
                return true;
            }
        }
    }

    /// <summary>
    /// Delays reconnection with an expontential back-off. For example,
    /// repeated attempts will delay 1 second, 2 seconds, 4 seconds, etc.
    /// </summary>
    public class ExponentialBackoffStrategy : IBackoffStrategy
    {
        readonly TimeSpan _initialDelay;
        readonly TimeSpan _maxDelay;
        public ExponentialBackoffStrategy(TimeSpan initialDelay, TimeSpan maxDelay)
        {
            _initialDelay = initialDelay;
            _maxDelay = maxDelay;
        }
        public IBackoffLimiter Create()
        {
            return new ExponentialBackoffLimiter(_initialDelay, _maxDelay);
        }

        class ExponentialBackoffLimiter : IBackoffLimiter
        {
            TimeSpan _currentDelay;
            readonly TimeSpan _maxDelay;
            public ExponentialBackoffLimiter(TimeSpan initialDelay, TimeSpan maxDelay)
            {
                _currentDelay = initialDelay;
                _maxDelay = maxDelay;
            }

            public bool ShouldReconnect(out TimeSpan delay)
            {
                delay = _currentDelay;
                var nextDelay = _currentDelay.Add(_currentDelay);
                _currentDelay = nextDelay < _maxDelay ? nextDelay : _maxDelay;
                return true;
            }
        }
    }

    /// <summary>
    /// Never retries reconnecting. A connection with this back-off strategy
    /// will simply die when disconnected.
    /// </summary>
    public class NoRetryBackoffStrategy : IBackoffStrategy
    {
        public IBackoffLimiter Create()
        {
            return new NoRetryBackoffLimiter();
        }

        class NoRetryBackoffLimiter : IBackoffLimiter
        {
            public bool ShouldReconnect(out TimeSpan delay)
            {
                delay = TimeSpan.Zero;
                return false;
            }
        }
    }
}
