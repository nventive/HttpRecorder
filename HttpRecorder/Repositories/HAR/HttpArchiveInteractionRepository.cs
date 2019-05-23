using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HttpRecorder.Repositories.HAR
{
    /// <summary>
    /// <see cref="IInteractionRepository"/> implementation that stores <see cref="Interaction"/>
    /// in files in the HTTP Archive format (https://en.wikipedia.org/wiki/.har / https://w3c.github.io/web-performance/specs/HAR/Overview.html).
    /// </summary>
    /// <remarks>
    /// The interactionName parameter is used as the file path.
    /// The .har extension will be added if no file extension is provided.
    /// </remarks>
    public class HttpArchiveInteractionRepository : IInteractionRepository
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <inheritdoc />
        public Task<bool> ExistsAsync(string interactionName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(File.Exists(GetFilePath(interactionName)));
        }

        /// <inheritdoc />
        public Task<Interaction> LoadAsync(string interactionName, CancellationToken cancellationToken = default)
        {
            try
            {
                var archive = JsonConvert.DeserializeObject<HttpArchive>(
                File.ReadAllText(GetFilePath(interactionName), Encoding.UTF8),
                _jsonSettings);

                return Task.FromResult(archive.ToInteraction(interactionName));
            }
            catch (Exception ex) when ((ex is IOException) || (ex is JsonException))
            {
                throw new HttpRecorderException($"Error while loading file {GetFilePath(interactionName)}: {ex.Message}", ex);
            }
        }

        /// <inheritdoc />
        public Task StoreAsync(Interaction interaction, CancellationToken cancellationToken = default)
        {
            try
            {
                var archive = new HttpArchive(interaction);
                var archiveDirectory = Path.GetDirectoryName(GetFilePath(interaction.Name));
                if (!string.IsNullOrWhiteSpace(archiveDirectory) && !Directory.Exists(archiveDirectory))
                {
                    Directory.CreateDirectory(archiveDirectory);
                }

                File.WriteAllText(GetFilePath(interaction.Name), JsonConvert.SerializeObject(archive, Formatting.Indented, _jsonSettings));

                return Task.CompletedTask;
            }
            catch (Exception ex) when ((ex is IOException) || (ex is JsonException))
            {
                throw new HttpRecorderException($"Error while writing file {GetFilePath(interaction.Name)}: {ex.Message}", ex);
            }
        }

        private string GetFilePath(string interactionName)
            => Path.HasExtension(interactionName)
                ? Path.GetFullPath(interactionName)
                : Path.GetFullPath($"{interactionName}.har");
    }
}
