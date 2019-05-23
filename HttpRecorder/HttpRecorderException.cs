using System;
using System.Runtime.Serialization;

namespace HttpRecorder
{
    /// <summary>
    /// Represents errors that occurs related to the <see cref="HttpRecorderDelegatingHandler" /> execution,
    /// or any of its sub-components.
    /// </summary>
    [Serializable]
    public class HttpRecorderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRecorderException" /> class.
        /// </summary>
        public HttpRecorderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRecorderException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpRecorderException(string message)
          : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRecorderException" /> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The <see cref="Exception" /> that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public HttpRecorderException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRecorderException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected HttpRecorderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
