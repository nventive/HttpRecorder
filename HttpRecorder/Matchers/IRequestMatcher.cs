using System.Net.Http;

namespace HttpRecorder.Matchers
{
    /// <summary>
    /// The <see cref="IRequestMatcher"/> is responsible from matching incoming <see cref="HttpRequestMessage"/>
    /// from existing <see cref="Interaction"/>.
    /// </summary>
    public interface IRequestMatcher
    {
        /// <summary>
        /// Matches <paramref name="request"/> in the <paramref name="interaction"/>.
        /// </summary>
        /// <param name="request">The incoming <see cref="HttpRequestMessage"/> to match.</param>
        /// <param name="interaction">The <see cref="Interaction"/>.</param>
        /// <returns>The matched <see cref="InteractionMessage"/>, or null if not found.</returns>
        InteractionMessage Match(HttpRequestMessage request, Interaction interaction);
    }
}
