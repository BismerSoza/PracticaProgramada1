using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UtilesService : IUtilesService
    {
        private readonly IConfiguration _config;

        public UtilesService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarToken(int idUsuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("idUsuario", idUsuario.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerarContrasena()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var chars = new char[10];

            for (int i = 0; i < 10; i++)
                chars[i] = caracteres[random.Next(caracteres.Length)];

            return new string(chars);
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new MimeKit.MimeMessage();
            var correo = _config["Correos:Correo"]!;
            var appPassword = _config["Correos:AppPassword"]!;

            if (string.IsNullOrEmpty(appPassword))
                return;

            mensaje.From.Add(new MimeKit.MailboxAddress(string.Empty, correo));
            mensaje.To.Add(MimeKit.MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;

            mensaje.Body = new MimeKit.TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = cuerpoHtml
            };

            using var cliente = new MailKit.Net.Smtp.SmtpClient();
            await cliente.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await cliente.AuthenticateAsync(correo, appPassword);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }
    }
}