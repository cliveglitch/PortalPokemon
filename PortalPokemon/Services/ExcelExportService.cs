using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using PortalPokemon.Models;

namespace PortalPokemon.Services
{
    public class ExcelExportService
    {
        private readonly HttpClient _httpClient;

        public ExcelExportService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Exporta una lista de pokemones a un archivo Excel y devuelve el contenido como un arreglo de bytes.
        /// </summary>
        /// <param name="pokemons">La lista de pokemones a exportar.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns>El contenido del archivo Excel como un arreglo de bytes.</returns>
        public async Task<byte[]> ExportAsync(IReadOnlyList<PokemonModel> pokemons, CancellationToken cancellationToken)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Pokemons");

                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1,2).Value = "Sprite";
                for (int i = 0; i < pokemons.Count; i++)
                {
                    var pokemon = pokemons[i];
                    worksheet.Cell(i + 2, 1).Value = pokemon.Name;
                    worksheet.Row(i + 2).Height = 75;

                    worksheet.Row(i + 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    var spriteUrl = pokemon.Sprites.FrontDefault;

                    if (!string.IsNullOrWhiteSpace(spriteUrl))
                    {
                        var imageBytes = await _httpClient.GetByteArrayAsync(
                            spriteUrl, cancellationToken);

                        using var imageStream = new MemoryStream(imageBytes);

                        worksheet.AddPicture(imageStream)
                            .MoveTo(worksheet.Cell(i + 2, 2))
                            .WithSize(96, 96);
                    }
                }

                worksheet.Column(1).Width = 16;
                worksheet.Column(2).Width = 16;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                return stream.ToArray();
            }
        }
    }
}
