using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonSpeciesModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; } = string.Empty;
        [JsonPropertyName("order")]
        public int? Order { get; set; }
        [JsonPropertyName("gender_rate")]
        public int? GenderRate { get; set; }
        [JsonPropertyName("capture_rate")]
        public int? CaptureRate { get; set; }
        [JsonPropertyName("base_happiness")]
        public int? BaseHappiness { get; set; }
        [JsonPropertyName("is_baby")]
        public bool IsBaby { get; set; }
        [JsonPropertyName("is_legendary")]
        public bool IsLegendary { get; set; }
        [JsonPropertyName("is_mythical")]
        public bool IsMythical { get; set; }
        [JsonPropertyName("hatch_counter")]
        public int? HatchCounter { get; set; }

        [JsonPropertyName("varieties")]
        public List<PokemonSpeciesVarietiesModel> Varieties { get; set; } = new List<PokemonSpeciesVarietiesModel>();

    }
}
