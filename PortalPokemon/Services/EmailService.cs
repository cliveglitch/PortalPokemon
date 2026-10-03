using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PortalPokemon.Configuration;
using PortalPokemon.Models;
using System.Net;
using System.Text;

namespace PortalPokemon.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _smtpSettings = options.Value;
        }

        /// <summary>
        /// Envía un correo electrónico de lo que se ve actualmente en el grid principal.
        /// </summary>
        /// <param name="recipientEmail">La dirección de correo del destinatario.</param>
        /// <param name="pokemons">La lista de Pokémon para incluir en el informe.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns></returns>
        public Task SendPokemonAsync(string recipientEmail, IReadOnlyList<PokemonModel> pokemons, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = "Listado Pokemon";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GeneratePokemonReport(pokemons);
            message.Body = bodyBuilder.ToMessageBody();

            return SendEmailAsync(message, cancellationToken);
        }

        /// <summary>
        /// Envía un correo electrónico con los detalles de un Pokémon específico.
        /// </summary>
        /// <param name="recipientEmail">La dirección de correo del destinatario.</param>
        /// <param name="pokemon">El Pokémon para el cual enviar detalles.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns></returns>
        public Task SendDetailPokemonAsync(string recipientEmail, PokemonDetailModel detail, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = $"Pokemon: {detail.Pokemon.Name}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GeneratePokemonDetailReport(detail);
            message.Body = bodyBuilder.ToMessageBody();

            return SendEmailAsync(message, cancellationToken);
        }

        /// <summary>
        /// Genera un grid HTML de los Pokémon proporcionados.
        /// </summary>
        /// <param name="pokemons">La lista de Pokémon para incluir en el informe.</param>
        /// <returns>El HTML del grid.</returns>
        private string GeneratePokemonReport(IReadOnlyList<PokemonModel> pokemons)
        {
            var html = new StringBuilder("""
                <!DOCTYPE html>
                <html>
                <body style="margin:0; padding:16px; background:#f5f7fb;
                             color:#172033; font-family:Arial,sans-serif;">

                    <table role="presentation" align="center"
                           width="100%" cellspacing="12" cellpadding="16"
                           style="max-width:900px; table-layout:fixed;">
                """);

            foreach (var row in pokemons.Chunk(5))
            {
                html.Append("<tr>");

                foreach (var pokemon in row)
                {
                    var name = WebUtility.HtmlEncode(pokemon.Name ?? "");
                    var sprite = WebUtility.HtmlEncode(pokemon.Sprites.FrontDefault);

                    var image = string.IsNullOrWhiteSpace(sprite)
                        ? """
                      <div style="height:96px; line-height:96px;">Sin imagen</div>
                      """
                            : $"""
                      <img src="{sprite}" alt="{name}" width="96" height="96"
                           style="display:block; margin:0 auto; border:0;
                                  image-rendering:pixelated;" />
                      """;

                        html.Append($"""
                    <td width="20%" align="center" valign="top"
                        bgcolor="#ffffff"
                        style="border:1px solid #dce1e7; border-radius:12px;">
                        {image}

                        <p style="margin:16px 0 0; color:#2456a6;
                                  font-size:16px; font-weight:bold;">
                            {name}
                        </p>
                    </td>
                """);
                }

                for (var i = row.Length; i < 5; i++)
                {
                    html.Append("<td></td>");
                }

                html.Append("</tr>");
            }

            html.Append("</table></body></html>");
            return html.ToString();
        }

        /// <summary>
        /// Genera un informe detallado en HTML para un Pokémon específico.
        /// </summary>
        /// <param name="pokemon">El Pokémon para el cual generar el informe.</param>
        /// <returns>El HTML del informe detallado.</returns>
        private string GeneratePokemonDetailReport(PokemonDetailModel detail)
        {
            var name = WebUtility.HtmlEncode(detail.Pokemon.Name ?? "No disponible");
            var speciesName = WebUtility.HtmlEncode(detail.Species.Name ?? "No disponible");

            var height = detail.Pokemon.Height is int h ? $"{h / 10m:0.#} m" : "No disponible";
            var weight = detail.Pokemon.Weight is int w ? $"{w / 10m:0.#} kg" : "No disponible";
            var experience = detail.Pokemon.BaseExperience?.ToString() ?? "No disponible";

            var captureRate = detail.Species.CaptureRate?.ToString() ?? "No disponible";

            var genderRate = detail.Species.GenderRate switch
            {
                null => "No disponible",
                -1 => "Sin género",
                int rate => $"{rate * 12.5m:0.#}% hembra"
            };

            var hatchCounter = detail.Species.HatchCounter is int hc ? $"{hc} ciclos" : "No disponible";
            var isBaby = detail.Species.IsBaby ? "Sí" : "No";
            var isLegendary = detail.Species.IsLegendary ? "Sí" : "No";
            var isMythical = detail.Species.IsMythical ? "Sí" : "No";

            var image = string.IsNullOrWhiteSpace(detail.Pokemon.Sprites.FrontDefault)
                ? "<p>Sin imagen</p>"
                : $"""
                    <img src="{WebUtility.HtmlEncode(detail.Pokemon.Sprites.FrontDefault)}"
                         alt="{name}" width="160" height="160"
                         style="display:block; border:0; image-rendering:pixelated;" />
                    """;

            var types = string.Join(" ", detail.Pokemon.Types.OrderBy(t => t.Slot).Select(t =>
                $"""
                    <span style="display:inline-block; padding:4px 12px; margin-right:4px;
                                 border:1px solid #d5e1f5; border-radius:20px;
                                 background:#eaf0fb; color:#2456a6; font-size:13px;
                                 text-transform:capitalize;">
                        {WebUtility.HtmlEncode(t.Type.Name)}
                    </span>
                 """));

            return ($"""
                <!DOCTYPE html>
                <html lang="es">
                <body style="margin:0; padding:16px; background:#f5f7fb;
                             color:#172033; font-family:Arial,sans-serif;">

                    <table role="presentation" align="center" width="100%"
                           cellspacing="0" cellpadding="20"
                           style="max-width:680px; background:white;
                                  border:1px solid #dce1e7; border-radius:12px;">
                        <tr>
                            <td width="160" align="center" valign="middle">
                                {image}
                            </td>
                            <td valign="middle">
                                <h2 style="margin:0; color:#2456a6;
                                           text-transform:capitalize;">
                                    {name}
                                    <small style="color:#626b78; font-size:14px;">
                                        #{detail.Pokemon.Id:D3}
                                    </small>
                                </h2>

                                <p style="margin:12px 0;">{types}</p>

                                <table width="100%" cellspacing="0" cellpadding="4">
                                    <tr>
                                        <th scope="row" align="left">Altura</th>
                                        <td>{height}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Peso</th>
                                        <td>{weight}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Experiencia base</th>
                                        <td>{experience}</td>
                                    </tr>

                                    <tr>
                                        <td colspan="2" style="padding:16px 0;">
                                            <div style="border-top:1px solid #dce1e7;"></div>
                                        </td>
                                    </tr>

                                    <tr>
                                        <th scope="row" align="left">Especie</th>
                                        <td>{speciesName}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Tasa de captura</th>
                                        <td>{captureRate}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Tasa de género</th>
                                        <td>{genderRate}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Contador de eclosión</th>
                                        <td>{hatchCounter}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Bebé</th>
                                        <td>{isBaby}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Legendario</th>
                                        <td>{isLegendary}</td>
                                    </tr>
                                    <tr>
                                        <th scope="row" align="left">Mítico</th>
                                        <td>{isMythical}</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
             """);
        }

        /// <summary>
        /// Envía un correo electrónico utilizando la configuración SMTP proporcionada.
        /// </summary>
        /// <param name="message">El mensaje de correo electrónico a enviar.</param>
        /// <param name="cancellationToken">El token de cancelación.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task SendEmailAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.Security, cancellationToken);

            if (_smtpSettings.UseAuthentication)
            {
                if (string.IsNullOrWhiteSpace(_smtpSettings.Username) || string.IsNullOrEmpty(_smtpSettings.Password))
                {
                    throw new InvalidOperationException("SMTP authentication requires a username and password.");
                }

                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
