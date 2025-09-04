using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf.qrcode;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Smadot.Utilities.CognitiveServices.Interfaces;
using Smadot.Utilities.Modelos;

namespace Smadot.Utilities.CognitiveServices.Classes
{
    public class ComputerVisionService : IComputerVisionService

    {
        private readonly IConfiguration _configuration;
        // Add your Computer Vision subscription key and endpoint
        private readonly string _subscriptionKey;
        private readonly string _endpoint;
        // <snippet_main_calls>
        private readonly ComputerVisionClient _client;
        private readonly ILogger<ComputerVisionService> _logger;
        public ComputerVisionService(IConfiguration configuration, ILogger<ComputerVisionService> logger = null)
        {
            _configuration = configuration;
            _subscriptionKey = configuration["ComputerVisionService:SubscriptionKey"] ?? string.Empty;
            _endpoint = configuration["ComputerVisionService:Endpoint"] ?? string.Empty;
            _client = Authenticate(_endpoint, _subscriptionKey);
            _logger = logger;
        }

        public async Task<ResponseGeneric<Line>> AnalyzeImage(byte[] imageFile)
        {
            var _ = new ResponseGeneric<Line>();

            try
            {
                MemoryStream streamImage = new MemoryStream(imageFile);
                //streamImage.Write(imageFile, 0, imageFile.Length);
                // Read text from URL
                var textHeaders = await _client.ReadInStreamAsync(streamImage);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;

                // <snippet_extract_response>
                // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                ReadOperationResult results;
                do
                {
                    results = await _client.GetReadResultAsync(Guid.Parse(operationId));
                }
                while ((results.Status == OperationStatusCodes.Running ||
                        results.Status == OperationStatusCodes.NotStarted));
                // </snippet_extract_response>

                // <snippet_extract_display>
                // Display the found text.
                streamImage.Dispose();
                var textUrlFileResults = results.AnalyzeResult.ReadResults;
                if (textUrlFileResults.Any())
                {
                    var page = textUrlFileResults.FirstOrDefault();
                    if (page == null)
                        return new ResponseGeneric<Line>("No sé encontró ningún texto reconocible en la imagen.") { mensaje = "No sé encontró ningún texto reconocible en la imagen." };
                    if (page.Lines == null)
                        return new ResponseGeneric<Line>("No sé encontró ningún texto reconocible en la imagen.") { mensaje = "No sé encontró ningún texto reconocible en la imagen." };
                    if (!page.Lines.Any())
                        return new ResponseGeneric<Line>("No sé encontró ningún texto reconocible en la imagen.") { mensaje = "No sé encontró ningún texto reconocible en la imagen." };
                    var textoLinea = page.Lines.FirstOrDefault(x => x.Text.Length >= 5 && x.Text.Length < 10);
                    if (textoLinea == null)
                        return new ResponseGeneric<Line>("No sé encontró ningún texto reconocible en la imagen.") { mensaje = "No sé encontró ningún texto reconocible en la imagen." };
                    _ = new ResponseGeneric<Line>(textoLinea) { Status = ResponseStatus.Success };

                }
                else
                {
                    _ = new ResponseGeneric<Line>(new Line(), false);
                }
            }
            catch (Exception e)
            {
                _ = new ResponseGeneric<Line>(e);
            }

            return _;
        }
        /*
       * AUTHENTICATE
       * Creates a Computer Vision client used by each example.
       */
        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
                { Endpoint = endpoint };
            return client;
        }
    }
}
