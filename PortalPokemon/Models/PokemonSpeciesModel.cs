using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonSpeciesModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("varieties")]
        public List<PokemonSpeciesVarietiesModel> Varieties { get; set; } = new List<PokemonSpeciesVarietiesModel>();

    }
}
