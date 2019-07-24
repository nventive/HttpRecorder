using System.Threading;
using System.Threading.Tasks;

namespace HttpRecorder.Anonymizers
{
    /// <summary>
    /// Allows the alteration of <see cref="Interaction"/> to remove confidential parameters before storage.
    /// </summary>
    public interface IInteractionAnonymizer
    {
        /// <summary>
        /// Returns a new anonimyzed <see cref="Interaction"/> from <paramref name="interaction"/>.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction"/> to anonymize.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A new anonymized <see cref="Interaction"/>.</returns>
        Task<Interaction> Anonymize(Interaction interaction, CancellationToken cancellationToken = default);
    }
}
