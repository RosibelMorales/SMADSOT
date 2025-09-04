using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Smadot.Models.Entities.Generic.Response;
using Smadot.Models.Entities.GoogleReCaptchaService;
using Smadot.Models.Entities.GoogleReCaptchaService.Request;
using Smadot.Models.Entities.GoogleReCaptchaService.Response;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.ProxyWebAPI.Interfaces;

namespace Smadot.Web.Helper.Operaciones.GoogleCaptcha
{
    public class VerifyTokenReCaptchaHelper
    {
        private readonly IOptionsMonitor<GoogleReCaptchaConfig> config;

        public VerifyTokenReCaptchaHelper(IOptionsMonitor<GoogleReCaptchaConfig> config)
        {
            this.config = config;
        }
        public async Task<ResponseViewModel> VerificarToken(string token, string? remoteip)
        {
            try
            {
                var googleReCaptcha = new Dictionary<string, object>
                                        {
                                            { "secret", config.CurrentValue.SecretKey },
                                            { "response", token },
                                            { "remoteip", remoteip },
                                        };
                var queryString = ConvertToQueryString(googleReCaptcha);
                using (var client = new HttpClient())
                {
                    var url = string.Format("https://www.google.com/recaptcha/api/siteverify?{0}", queryString);
                    var httpResult = await client.GetAsync(url);

                    if (httpResult.StatusCode != HttpStatusCode.OK)
                    {
                        return await Task.FromResult(new ResponseViewModel(isSucces: false, "El ReCaptcha es inválido."));
                    }
                    var responseString = await httpResult.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(responseString);
                    return await Task.FromResult(new ResponseViewModel(isSucces: response.Success && response.Score > 0.66, "Por favor, rellene el ReCaptcha para comprobar su identidad."));
                }

            }
            catch (Exception ex)
            {
                return new ResponseViewModel(false,"El ReCaptcha es inválido. Por favor, compruebe su identidad.");
            }
        }

        public static string ConvertToQueryString(Dictionary<string, object> obj)
        {
            var keyValuePairs = new List<string>();
            var urlEncoder = UrlEncoder.Default;

            foreach (var pair in obj)
            {
                var key = urlEncoder.Encode(pair.Key);
                var value = urlEncoder.Encode(pair.Value.ToString());
                keyValuePairs.Add($"{key}={value}");
            }

            return string.Join("&", keyValuePairs);
        }
    }
}