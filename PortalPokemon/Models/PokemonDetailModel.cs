namespace PortalPokemon.Models
{
    public class PokemonDetailModel
    {
        required public PokemonModel Pokemon { get; set; }
        required public PokemonSpeciesModel Species { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}
