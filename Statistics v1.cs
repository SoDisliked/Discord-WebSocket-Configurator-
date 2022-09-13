using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DislikedWebSocketConfigurator
{
    /// <summary>
    /// DislikedDiscordWebSocket BotStatisticsDisplay.
    /// </summary>
    public class Statistics
    {
        #region Public-Members

        /// <summary>
        /// The time at which the client or server bot has started
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
        }

        ///<summary>
        /// The amount of time between the user and bot server in order to be completed
        /// </summary>
        public TimeSpan UpTime
        {
            get
            {
                return DateTime.Now.ToUniversalTime() - _StartTime;
            }
        }

        ///<summary>
        /// The number of bytes currently received.
        /// </summary>
        public long ReceivedBytes
        {
            get
            {
                return _ReceivedBytes;
            }
        }

        ///<summary>
        /// The number of messages received from console host.
        /// </summary>
        public long ReceivedMessages
        {
            get
            {
                return _ReceivedMessages;
            }
        }

        ///<summary>
        /// Average message received and size must be in bytes.
        /// </summary>
        public long ReceivedMessageSizeAverage
        {
            get
            {
                if (_ReceivedBytes > 0 && _ReceivedMessages > 0)
                {
                    return (int)(_ReceivedBytes / ReceivedMessages);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// The number of bytes sent 
        /// </summary>
        public long SentBytes
        {
            get
            {
                return _SentBytes;
            }
        }

        /// <summary>
        /// The number of messages sent.
        /// </summary>
        public long SentMessages
        {
            get
            {
                return _SentMessages;
            }
        }

        ///<summary>
        /// Average sent message size in bytes following interraction with host console.
        /// </summary>
        public long SentMessagesSizeAverage
        {
            get
            {
                if (_SentBytes > 0 && _SentBytes > 0)
                {
                    return (int)(_SentBytes / _SentMessages);
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Private-Members 

        private DateTime _StartTime = DateTime.Now.ToUniversalTime();
        private long _ReceivedBytes = 0;
        private long _ReceivedMessages = 0;
        private long _SentBytes = 0;
        private long _SentMessages = 0;

        #endregion

        #region Constructors-And-Factories

        ///<summary>
        /// Initialize the statistics of the bot's property
        /// </summary>
        public Statistics()
        {

        }

        #endregion

        #region Public-Methods

        /// <summary>
        ///  Return current displayed object to normal
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ret =
                "--- Statistics ---" + Environment.NewLine +
                "    Started     : " + _StartTime.ToString() + Environment.NewLine +
                "    Uptime      : " + UpTime.ToString() + Environment.NewLine +
                "    Received    : " + Environment.NewLine +
                "       Bytes    : " + ReceivedBytes + Environment.NewLine +
                "       Messages : " + ReceivedMessages + Environment.NewLine +
                "       Average  : " + ReceivedMessageSizeAverage + " bytes" + Environment.NewLine +
                "    Sent        : " + Environment.NewLine +
                "       Bytes    : " + SentBytes + Environment.NewLine +
                "       Messages : " + SentMessages + Environment.NewLine +
                "       Average  : " + SentMessageSizeAverage + " bytes" + Environment.NewLine;
            return ret;
        }

        /// <summary>
        /// Reset statistics other than StartTime and UpTime/
        /// </summary>
        public void Reset()
        {
            _ReceivedBytes = 0;
            _ReceivedMessages = 0;
            _SentBytes = 0;
            _SentMessages = 0;
        }

        #endregion

        #region Internal-Methods

        internal void IncrementReceivedMessages()
        {
            _ReceivedMessages = Interlocked.Increment(ref _ReceivedMessages);
        }

        internal void IncrementSentMessages()
        {
            _SentMessages = Interlocked.Increment(ref _SentMessages);
        }

        internal void AddReceivedBytes(long bytes)
        {
            _ReceivedBytes = Interlocked.Add(ref _ReceivedBytes, bytes);
        }

        internal void AddSentBytes(long bytes)
        {
            _SentBytes = Interlocked.Add(ref _SentBytes, bytes);
        }

        #endregion

        #region Private-Methods

        #endregion 
    }
}