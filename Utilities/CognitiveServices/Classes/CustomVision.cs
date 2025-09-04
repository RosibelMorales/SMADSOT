using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Smadot.Utilities.CognitiveServices.Interfaces;
using Smadot.Utilities.Modelos;

namespace Smadot.Utilities.CognitiveServices.Classes
{
    public class CustomVision : ICustomVision
    {
        private readonly IConfiguration _configuration;
        // Add your Computer Vision subscription key and endpoint
        private readonly string _subscriptionKey;
        private readonly string _endpoint;
        private readonly string _modelName;
        // <snippet_main_calls>
        private readonly CustomVisionPredictionClient _client;
        private readonly Guid _IdProjectCustomVision;
        private readonly ILogger<CustomVision> _logger;
        public CustomVision(IConfiguration configuration, ILogger<CustomVision> logger)
        {
            _configuration = configuration;
            _subscriptionKey = configuration["CustomVisionService:SubscriptionKey"] ?? string.Empty;
            _endpoint = configuration["CustomVisionService:Endpoint"] ?? string.Empty;
            _client = AuthenticatePrediction(_endpoint, _subscriptionKey);
            _IdProjectCustomVision = Guid.Parse(configuration["CustomVisionService:ProjectKey"] ?? string.Empty);
            _modelName = configuration["CustomVisionService:ModelName"] ?? string.Empty;
            _logger = logger;
        }
        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }
        public ResponseGeneric<byte[]> ProcessLicensePlate(byte[] licensePlate)
        {
            using (var stream = new MemoryStream(licensePlate))
            {
                try
                {
                    var result = _client.DetectImage(_IdProjectCustomVision, _modelName, stream);

                    // Loop over each prediction and write out the results
                    //foreach (var c in result.Predictions)
                    //{
                    //}
                    var base64f = Convert.ToBase64String(stream.ToArray());
                    var prediction = result.Predictions.OrderByDescending(x => x.Probability).FirstOrDefault();
                    if (prediction == null)
                    {
                        return new ResponseGeneric<byte[]>("No sé encontró ninguna placa en la imagen.");
                    }
                    //if (prediction.Probability < 0.8)
                    //{
                    //    return new ResponseGeneric<byte[]>("No sé encontró ninguna placa en la imagen.");
                    //}
                    var openStream = new MemoryStream(licensePlate);
                    var img = Image.Load(openStream);
                    var imgClone = img.Clone(
                        i => i
                            .Crop(new Rectangle(
                                  (int)((img.Width * prediction.BoundingBox.Left) * (.95)),
                                  (int)((img.Height * prediction.BoundingBox.Top) * (.95)),
                                  (int)((img.Width * prediction.BoundingBox.Width) * (1.20)),
                                  (int)((img.Height * prediction.BoundingBox.Height) * (1.20))
                                )));
                    //Graphics sourceImage = Graphics.FromImage(img);
                    //RectangleF destinationRect = new RectangleF(
                    //    (float)(img.Width * prediction.BoundingBox.Left),
                    //    (float)(img.Height * prediction.BoundingBox.Top),
                    //    (float)(img.Width * prediction.BoundingBox.Width),
                    //    (float)(img.Height * prediction.BoundingBox.Height)
                    //    );
                    //RectangleF sourceRect = new RectangleF(
                    //    0,
                    //    0,
                    //    (float)img.Width,
                    //    (float)img.Height
                    //    );

                    //sourceImage.DrawImage(img, sourceRect, destinationRect , GraphicsUnit.Pixel);
                    var ms = new MemoryStream();
                    imgClone.Save(ms, new JpegEncoder());
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    var arrayImgCrop = ms.ToArray();
                    ms.Dispose();
                    return new ResponseGeneric<byte[]>(arrayImgCrop);
                }
                catch (Exception e)
                {
                    return new ResponseGeneric<byte[]>("Hubo un error al intentar procesar la imagen a través de OCR.");
                }

            }
        }
    }
}
