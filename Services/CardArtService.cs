using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CCGGame.Services
{
    /// <summary>
    /// Servicio para obtener imágenes temáticas de cartas (locales).
    /// Devuelve rutas internas en Resources/Images para evitar dependencias externas.
    /// </summary>
    public class CardArtService : ICardArtService
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();

        public CardArtService() { }

        /// <summary>
        /// Obtiene una ruta de imagen para un tipo de carta. Cachea la respuesta.
        /// </summary>
        public async Task<string> GetImageUrlForTypeAsync(string cardType)
        {
            if (string.IsNullOrWhiteSpace(cardType))
                cardType = "default";

            if (_cache.TryGetValue(cardType, out var cached))
                return cached;

            // Rutas locales (Resources/Images). Fallback al bot si no hay mapeo.
            var normalized = cardType.ToLowerInvariant();
            // Usamos un único recurso local para asegurar disponibilidad.
            var url = "dotnet_bot.png";

            _cache[cardType] = url;
            return await Task.FromResult(url);
        }
    }
}

