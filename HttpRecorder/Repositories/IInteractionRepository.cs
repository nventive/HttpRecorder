using System.Threading;
using System.Threading.Tasks;

namespace HttpRecorder.Repositories
{
    /// <summary>
    /// Allow storage of <see cref="Interaction"/>.
    /// </summary>
    public interface IInteractionRepository
    {
        /// <summary>
        /// Determines whether the recorded interaction exists.
        /// </summary>
        /// <param name="interactionName">The interaction name.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>true if the interaction exists.</returns>
        Task<bool> ExistsAsync(string interactionName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the interaction.
        /// </summary>
        /// <param name="interactionName">The interaction name.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The loaded interaction.</returns>
        /// <exception cref="HttpRecorderException">If interactions cannot be loaded.</exception>
        Task<Interaction> LoadAsync(string interactionName, CancellationToken cancellationToken);

        /// <summary>
        /// Store the interaction.
        /// </summary>
        /// <param name="interaction">The <see cref="Interaction"/> to store.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The persisted <see cref="Interaction"/>.</returns>
        Task<Interaction> StoreAsync(Interaction interaction, CancellationToken cancellationToken);
    }
}
