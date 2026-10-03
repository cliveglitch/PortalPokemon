using System.Net;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PortalPokemon.Configuration;
using PortalPokemon.Models;

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
            message.Subject = "Pokémon Report";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = GeneratePokemonReport(pokemons);
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
