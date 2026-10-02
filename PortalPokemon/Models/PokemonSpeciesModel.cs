using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonSpeciesModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
