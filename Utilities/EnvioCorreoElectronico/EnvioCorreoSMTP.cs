using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.EnvioCorreoElectronico
{
	public class EnvioCorreoSMTP
	{
		string respuesta;
		private static readonly string templatesPath = Directory.GetCurrentDirectory() + "/Templates/";
		private readonly string NombreEmail = "";
		private readonly string FromEmail = "";
		/// <summary>
		/// Metodo envio de correo
		/// </summary>
		/// <param name="emailMessage"></param>
		/// <returns></returns>
		public string Send(String destinatario, String subject, String body, string emailto, string username, string emailpassword, string smtp,
			string? puerto, bool htmlEmail = true)
		{
			List<String> to = new List<String>();
			to.Add(destinatario);

			NetworkCredential credentials = new NetworkCredential(username, emailpassword);
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(emailto, credentials.UserName);
			if (to != null) foreach (String addr in to) mailMessage.To.Add(new MailAddress(addr, "Test"));
			mailMessage.Subject = subject;
			mailMessage.IsBodyHtml = htmlEmail;
			mailMessage.Body = body;
			mailMessage.Priority = MailPriority.Normal;

			SmtpClient client = new SmtpClient
			{
				Host = smtp,
				Port = (!string.IsNullOrEmpty(puerto) ? int.Parse(puerto) : 25),
				EnableSsl = false,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false,
				Credentials = credentials
			};
			try
			{
				client.Send(mailMessage);
				respuesta = "Correcto";
			}
			catch (Exception e)
			{
				respuesta = "Error, no se pudo envíar el correo";
			}
			return respuesta;
		}


		public bool Send(
			List<string> destinatario, String subject, String body, string emailto,
			string username, string emailpassword, string smtp,
			List<String>? cc, List<Attachment>? adjuntos,
			string? puerto, bool htmlEmail = true)
		{

			List<String> to = new List<String>();
			to.AddRange(destinatario);

			NetworkCredential credentials = new NetworkCredential(username, emailpassword);
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(emailto, credentials.UserName);
			if (to != null) foreach (String addr in to) mailMessage.To.Add(new MailAddress(addr, addr));
			mailMessage.Subject = subject;
			mailMessage.IsBodyHtml = htmlEmail;
			mailMessage.Body = body;
			mailMessage.Priority = MailPriority.Normal;

			if (cc != null)
			{
				if (cc.Count() > 0)
				{
					foreach (String addrcc in cc) mailMessage.CC.Add(new MailAddress(addrcc, addrcc));
				}
			}
			if (adjuntos != null)
			{
				if (adjuntos.Count > 0)
				{
					foreach (Attachment file in adjuntos) mailMessage.Attachments.Add(file);
				}
			}

			SmtpClient client = new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = (!string.IsNullOrEmpty(puerto) ? int.Parse(puerto) : 25),
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Credentials = credentials
			};
			try
			{
				client.Send(mailMessage);
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
			
		}

		#region Generacion de plantillas
		/// <summary>
		/// GenerarPlantillaBasica
		/// </summary>
		/// <param name="asunto"></param>
		/// <param name="encabezado"></param>
		/// <param name="contenido"></param>
		/// <returns></returns>
		public static string GenerarPlantillaBasica(string asunto, string encabezado, string contenido)
		{
			string plantilla = System.IO.File.ReadAllText(templatesPath + "notificacion-basica.html");

			plantilla = plantilla.Replace("ASUNTO", asunto);
			plantilla = plantilla.Replace("PREVIEW", contenido.Substring(0, 20) + "...");
			plantilla = plantilla.Replace("ENCABEZADO", encabezado);
			plantilla = plantilla.Replace("CONTENIDO", contenido);

			return plantilla;
		}
		/// <summary>
		/// GenerarPlantillaBoton
		/// </summary>
		/// <param name="asunto"></param>
		/// <param name="encabezado"></param>
		/// <param name="contenido"></param>
		/// <param name="textoBoton"></param>
		/// <param name="enlaceBoton"></param>
		/// <returns></returns>
		public static string GenerarPlantillaBoton(string asunto, string encabezado, string contenido, string textoBoton, string enlaceBoton)
		{
			string plantilla = System.IO.File.ReadAllText(templatesPath + "notificacion-boton.html");

			plantilla = plantilla.Replace("ASUNTO", asunto);
			plantilla = plantilla.Replace("PREVIEW", contenido.Substring(0, 20) + "...");
			plantilla = plantilla.Replace("ENCABEZADO", encabezado);
			plantilla = plantilla.Replace("CONTENIDO", contenido);
			plantilla = plantilla.Replace("TEXTO-BOTON", textoBoton);
			plantilla = plantilla.Replace("href=''", "href='" + enlaceBoton + "'");

			return plantilla;
		}

		public string BodyEmail(string Asunto,string contenido, string image, string? buttonLeido)
		{
			var html = "";
			html +=
				"<!DOCTYPE html>" +
				"<html lang=\"es\">" +
				"<head>" +
				"<meta charset=\"UTF-8\">" +
				"<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">" +
				"<title></title>" +
                "<style>table, td, div, h1, p {font-family: Helvetica, sans-serif;}a:link {text-decoration: none;}a:visited {text-decoration: none;}a:hover {text-decoration: none;}a:active {text-decoration: none;}.button {\r\n        background-color: black;\r\n        border: none;\r\n        color: white;\r\n        padding: 20px 34px;\r\n        text-align: center;\r\n        text-decoration: none;\r\n        display: inline-block;\r\n        font-size: 15px;\r\n        margin: 4px 2px;\r\n        cursor: pointer;\r\n      }</style>" +
				"</head>" +
				"<body style=\"margin:0;padding:0;background-color:#eaebeb\">" +
				"<div style=\"width:100%;background:eaebeb\">" +
				"<table role=\"presentation\" align=\"center\" style=\"width:80%;border-collapse:collapse;border:0;border-spacing:0;background:#ffffff;\">" +
				"<theader>" +
				"<tr>" +
				"<td>" +
				"<table style=\"width:100%;\">" +
				"<tr>" +
				"<td align=\"center\" style=\"width:20%\">" +
				"<img src='data:image/png;base64," + image + "' data-filename='Logo' alt=\"\" width=\"40%\" style=\"height:auto;display:block;\" />";
				html += "</td>" +
				"<td>Verificentros Puebla</td>" +
				"</tr>" +
				"</table>" +
				"</td>" +
				"</tr>" +
				"<tr>" +
				"<td>" +
				"<div style=\"background-color:#772582;height:2px;\">" +
				"</td>" +
				"</tr>" +
				"</theader>" +
				"<tbody style=\"background:#ffffff\">" +
				"<tr>" +
				"<td style=\"padding-left:10px;padding-right:10px;padding-top:10px;padding-bottom:10px\">";
			html += Asunto;

			html += "</td>" +
				"</tr>" +
				"<tr style=\"height:20px\">" +
				"</tr>" +
				"<tr>" +
				"<td style=\"padding-left:10px;padding-right:10px\">" +
                "<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" integrity=\"sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH\" crossorigin=\"anonymous\">\r\n<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js\" integrity=\"sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz\" crossorigin=\"anonymous\"></script>";

			html += contenido;

            if (!string.IsNullOrEmpty(buttonLeido))
            {
				html += buttonLeido;
                //html += "<br><br><button style='background-color: #4CAF50;border: none;color: white;padding: 15px 32px;text-align: center;text-decoration: none;display: inline-block;font-size: 16px;margin: 4px 2px;cursor: pointer;border-radius: 15px;'>Leído</button>";
            }

			html+="</td>" +
				"</tr>" +
				"</tbody>" +
				"<tfooter>" +
				"<tr>" +
				"<td>" +
				"<table style=\"width:100%\">" +
				"<tr>" +
				"<td style=\"padding-left:10px;padding-right:10px;with:20%;font-size:13px\">" +
				"<span style=\"color:#a1a5b7\">2022©</span> Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial - Gobierno de Puebla" +
				"</td>" +
				"</tr>" +
				"</table>" +
				"</td>" +
				"</tr>" +
				"</tfooter>" +
				"</table>" +
				"</div>" +
				"</div>" +
				"</body>" +
				"</html>";

			return html;
		}

		#endregion
	}
}
