namespace PortalPokemon.Models
{
    public class PaginatedPokemonSpeciesModel
    {
        public int Count { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public string? NameFilter { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public IReadOnlyList<PaginatedPokemonSpeciesResultModel> Species { get; set; } = new List<PaginatedPokemonSpeciesResultModel>();
    }
}
