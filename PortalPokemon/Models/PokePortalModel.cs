namespace PortalPokemon.Models
{
    public class PokePortalModel
    {
        public PaginatedPokemonModel PokemonGrid { get; set; } = new PaginatedPokemonModel();
        public string? NameFilter { get; set; }
        public string? SpeciesFilter { get; set; }
        public IReadOnlyList<string> AvailableSpecies { get; set; } = Array.Empty<string>();
    }
}
