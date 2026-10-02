using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("base_experience")]
        public int BaseExperience { get; set; }
        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        [JsonPropertyName("types")]
        public PokemonFormTypeModel[] Types { get; set; } = Array.Empty<PokemonFormTypeModel>();
        [JsonPropertyName("sprites")]
        public PokemonSpritesModel Sprites { get; set; } = new PokemonSpritesModel();
        [JsonPropertyName("species")]
        public PokemonSpeciesModel Species { get; set; } = new PokemonSpeciesModel();
    }
}
