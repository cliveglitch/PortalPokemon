using System.Text.Json.Serialization;

namespace PortalPokemon.Models
{
    public class PokemonFormTypeModel
    {
        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("type")]
        public PokemonTypeModel Type { get; set; } = new PokemonTypeModel();
    }
}
