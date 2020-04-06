using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Http;

namespace HttpRecorder.Context
{
    /// <summary>
    /// Sets a global context for the recording.
    /// </summary>
    public sealed class HttpRecorderContext : IDisposable
    {
        private static HttpRecorderContext _current;

        private static volatile ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRecorderContext"/> class.
        /// </summary>
        /// <param name="configurationFactory">Factory to allow customization per <see cref="HttpClient"/>.</param>
        /// <param name="testName">The <see cref="CallerMemberNameAttribute"/>.</param>
        /// <param name="filePath">The <see cref="CallerFilePathAttribute"/>.</param>
        /// <example>
        /// <![CDATA[
        /// // In service registration.
        /// services.AddRecorderContextSupport();
        ///
        /// // In the test case.
        /// using var context = new HttpRecorderContext();
        /// ]]>
        /// </example>
        public HttpRecorderContext(
            Func<IServiceProvider, HttpMessageHandlerBuilder, HttpRecorderConfiguration> configurationFactory = null,
            [CallerMemberName] string testName = "",
            [CallerFilePath] string filePath = "")
        {
            ConfigurationFactory = configurationFactory;
            TestName = testName;
            FilePath = filePath;
            _lock.EnterWriteLock();
            try
            {
                if (_current != null)
                {
                    throw new HttpRecorderException(
                        $"Cannot use multiple {nameof(HttpRecorderContext)} at the same time. Previous usage: {_current.FilePath}, current usage: {filePath}.");
                }

                _current = this;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets the current <see cref="HttpRecorderContext"/>.
        /// </summary>
        public static HttpRecorderContext Current
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _current;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the configuration factory.
        /// </summary>
        public Func<IServiceProvider, HttpMessageHandlerBuilder, HttpRecorderConfiguration> ConfigurationFactory { get; }

        /// <summary>
        /// Gets the TestName, which should be the <see cref="CallerMemberNameAttribute"/>.
        /// </summary>
        public string TestName { get; }

        /// <summary>
        /// Gets the Test file path, which should be the <see cref="CallerFilePathAttribute"/>.
        /// </summary>
        public string FilePath { get; }

        /// <inheritdoc/>
        [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Dispose pattern used for context here, not resource diposal.")]
        [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = "Dispose pattern used for context here, not resource diposal.")]
        public void Dispose()
        {
            _lock.EnterWriteLock();
            try
            {
                _current = null;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
