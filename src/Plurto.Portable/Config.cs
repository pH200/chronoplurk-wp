using Plurto.Core;

namespace Plurto
{
    public static class Config
    {
        /// <summary>
        /// <para>Gets or sets value indicating whether disabling HTTPS on every WebRequest made by Plurto.</para>
        /// <para>Default is false.</para>
        /// </summary>
        public static bool AlwaysDisableHttps
        {
            get { return _alwaysDisableHttps; }
            set { _alwaysDisableHttps = value; }
        }
        private static bool _alwaysDisableHttps = false;

        /// <summary>
        /// <para>Gets or sets value indicating whether limiting the synchrnous web requests made by Plurto.</para>
        /// <para>Default is true.</para>
        /// </summary>
        public static bool AlwaysThrowOnSynchronousWebRequest
        {
            get { return _alwaysThrowOnSynchronousWebRequest; }
            set { _alwaysThrowOnSynchronousWebRequest = value; }
        }
        private static bool _alwaysThrowOnSynchronousWebRequest = true;

        #region No Effect on Silverlight or WP7

        /// <summary>
        /// <para>Gets or sets value indicating whether to disable proxy for WebRequest.</para>
        /// <para>Has no effect on Silverlight or WP7.</para>
        /// <para>Default is false.</para>
        /// </summary>
        public static bool DisableProxy
        {
            get { return _disableProxy; }
            set { _disableProxy = value; }
        }
#if DEBUG
        private static bool _disableProxy = true;
#else
        private static bool _disableProxy = false;
#endif

        /// <summary>
        /// <para>Gets or sets whether to enable timeout for WebRequest.</para>
        /// <para>Has no effect on Silverlight or WP7.</para>
        /// <para>Default is false.</para>
        /// </summary>
        public static bool EnableTimeoutForWebRequest
        {
            get { return _enableTimeoutForWebRequest; }
            set { _enableTimeoutForWebRequest = value; }
        }
        private static bool _enableTimeoutForWebRequest = false;

        /// <summary>
        /// <para>Gets or sets timeout value in milliseconds for WebRequest.</para>
        /// <para>Has no effect on Silverlight or WP7.</para>
        /// <para>Default is 10000.</para>
        /// </summary>
        public static int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        private static int _timeout = 10000;

        /// <summary>
        /// <para>Gets or sets value that indicates whether disabling 100-continue behavior on every WebRequest made by Plurto.</para>
        /// <para>Has no effect on Silverlight or WP7.</para>
        /// <para>Default is false.</para>
        /// </summary>
        public static bool DisableExpect100Continue
        {
            get { return _disableExpect100Continue; }
            set { _disableExpect100Continue = value; }
        }
        private static bool _disableExpect100Continue = false;

#endregion

        #region Logging

        /// <summary>
        /// <para>Gets or sets logging verbosity for Plurto.</para>
        /// <para>Default is None.</para>
        /// </summary>
        public static LoggingLevel LoggingLevel
        {
            get { return _loggingLevel; }
            set { _loggingLevel = value; }
        }
        private static LoggingLevel _loggingLevel = LoggingLevel.None;

        /// <summary>
        /// <para>Gets or sets auto linebreak column for Plurto loggings.</para>
        /// <para>Default is 0.</para>
        /// </summary>
        public static int LoggingLinebreak
        {
            get { return _loggingLinebreak; }
            set { _loggingLinebreak = value; }
        }
        private static int _loggingLinebreak = 0;

#endregion

    }
}
