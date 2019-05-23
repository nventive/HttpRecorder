using System;

namespace HttpRecorder
{
    /// <summary>
    /// Information about <see cref="InteractionMessage"/> timings.
    /// </summary>
    public class InteractionMessageTimings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionMessageTimings"/> class.
        /// </summary>
        /// <param name="startedDateTime">The date and time stamp of the request start.</param>
        /// <param name="time">Total elapsed time of the request.</param>
        public InteractionMessageTimings(DateTimeOffset startedDateTime, TimeSpan time)
        {
            StartedDateTime = startedDateTime;
            Time = time;
        }

        /// <summary>
        /// Gets the date and time stamp of the request start.
        /// </summary>
        public DateTimeOffset StartedDateTime { get; }

        /// <summary>
        /// Gets the total elapsed time of the request.
        /// </summary>
        public TimeSpan Time { get; }
    }
}
