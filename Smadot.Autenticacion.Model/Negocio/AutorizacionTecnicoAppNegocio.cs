using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Smadot.Utilities.Modelos;
using Microsoft.Extensions.Configuration;
using Smadot.Models.DataBase;
using Smadot.Utilities.BlobStorage;
using Smadot.Utilities.GestionTokens;
using Microsoft.EntityFrameworkCore;

namespace Smadot.Autenticacion.Model.Negocio
{
    public class AutorizacionTecnicoAppNegocio : IAutorizacionTecnicoAppNegocio
    {
        private SmadotDbContext _context;
        private readonly IUserResolver _userResolver;
        private readonly BlobStorage _blobStorage;
        private readonly IConfiguration _configuration;
        public AutorizacionTecnicoAppNegocio(SmadotDbContext context, IUserResolver userResolver, IConfiguration configuration)
        {
            _context = context;
            _userResolver = userResolver;
            _blobStorage = new BlobStorage(configuration["ConnectionBlobStrings:BlobString"], configuration["ConnectionBlobStrings:Container"]);
            _configuration = configuration;
        }

        public async Task<ResponseGeneric<AutorizacionTecnicoAppResponse>> Autorizar(AutorizacionTecnicoAppRequest req)
        {
            try
            {
                var personal = await _context.vPersonalAutorizacions.FirstOrDefaultAsync(x => x.NumeroTrabajador == req.NumEmpleado );//&& x.IdVerificentro == req.IdVerificentro
                if (personal == null)
                {
                    return new ResponseGeneric<AutorizacionTecnicoAppResponse>
                    {
                        Response = null,
                        mensaje = "No se encontró un empleado activo con ese número.",
                        Status = ResponseStatus.Failed,
                    };
                }
                // var userFoto = personal.FirstOrDefault(/*x => x.IdNormaAcreditacion != null*/);
                // //if (userFoto == null)
                // //{
                // //    return new ResponseGeneric<AutorizacionTecnicoAppResponse>
                // //    {
                // //        Response = null,
                // //        mensaje = "El empleado no tiene una acreditación registrada.",
                // //        Status = ResponseStatus.Failed,
                // //    };
                // //}
                var verificentro = _context.vVerificentros.FirstOrDefault(x => x.Id == personal.IdVerificentro );
                string sourceImageFileName = personal.UrlFoto;
                //Obtiene la foto del técnico
                var url = await _blobStorage.UploadFileAsync(req.Foto, "AutorizacionTecnicoApp/" + personal.IdVerificentro + "/" + req.NumEmpleado + "/" + DateTime.Now.ToString("dd-MM-yyyy") + "/foto_" + DateTime.Now.ToString("HHmmss") + ".png");
                if (string.IsNullOrEmpty(url))
                {
                    return new ResponseGeneric<AutorizacionTecnicoAppResponse>
                    {
                        Response = null,
                        mensaje = "Ocurrió un error al cargar la foto.",
                        Status = ResponseStatus.Failed,
                    };
                }
                //
                var detectFaceClass = new DetectFace();
                List<string> targetImageFileNames = new List<string>
                    {
                        url
                    };

                IList<Guid?> targetFaceIds = new List<Guid?>();

                //Ya sabemos que sí hay un rostro, hace uso de Face Api para saber que tiene un rostro 
                foreach (var targetImageFileName in targetImageFileNames)
                {
                    // Detecta rostros a partir de la URL de la imagen de destino.
                    var faces = await detectFaceClass.DetectFaceRecognize($"{detectFaceClass.base_url}{targetImageFileName}");
                    // Agregar el faceId detectado a la lista de GUIDs.
                    if (faces.Count > 0 && faces[0].FaceId.HasValue)
                        targetFaceIds.Add(faces[0].FaceId.Value);
                }

                if(targetFaceIds.Count == 0 ) {
                    return new ResponseGeneric<AutorizacionTecnicoAppResponse>
                    {
                        Response = null,
                        mensaje = "No se detectó un rostro en la foto.",
                        Status = ResponseStatus.Failed,
                    };
                }

                // Detect faces from source image url.
                IList<DetectedFace> detectedFaces = await detectFaceClass.DetectFaceRecognize($"{detectFaceClass.base_url}{sourceImageFileName}");

                IFaceClient client = detectFaceClass.Authenticate(detectFaceClass.ENDPOINT, detectFaceClass.SUBSCRIPTION_KEY);
                IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds);
                //IList<SimilarFace> similarResults = await client.Face.FindSimilarAsync(detectedFaces[0].FaceId.Value, null, null, targetFaceIds, null, FindSimilarMatchMode.MatchFace);
                var result = new AutorizacionTecnicoAppResponse();
                if (similarResults.Count > 0 && similarResults[0].Confidence > .70)
                {
                    result = new AutorizacionTecnicoAppResponse
                    {
                        IdUser = personal.Id,
                        Nombre = personal.Nombre,
                        IdVerificentro = personal.IdVerificentro,
                        Puesto = personal.NombrePuesto,
                        Verificentro = verificentro.Nombre,
                        UrlFotoEvidencia = url,
                        Token = _configuration["SmadotAPISettings:APIKey"]
                    };
                }
                else
                {
                    return new ResponseGeneric<AutorizacionTecnicoAppResponse>
                    {
                        Response = null,
                        mensaje = "La similitud no cumple con el porcentaje de requerido.",
                        Status = ResponseStatus.Failed,
                    };
                }
                return new ResponseGeneric<AutorizacionTecnicoAppResponse>(result);
            }
            catch (Exception ex)
            {
                return new ResponseGeneric<AutorizacionTecnicoAppResponse>(ex){mensaje="Ocurrió un error al procesar la solicitud."};
            }
        }
    }

    public interface IAutorizacionTecnicoAppNegocio
    {
        public Task<ResponseGeneric<AutorizacionTecnicoAppResponse>> Autorizar(AutorizacionTecnicoAppRequest req);
    }

    public class DetectFace
    {
        public string base_url = "";
        //public string SUBSCRIPTION_KEY = "8dac83404a88445bb29199f2eeb58d9a";
        //public string ENDPOINT = "https://southcentralus.api.cognitive.microsoft.com";
        //const string SUBSCRIPTION_KEY = "0f6b196b43374171aead09fa5623d09f";
        //const string ENDPOINT = "https://ms-face-api-zion.cognitiveservices.azure.com/";

        public string SUBSCRIPTION_KEY = "0f6b196b43374171aead09fa5623d09f";
        public string ENDPOINT = " https://ms-face-api-zion.cognitiveservices.azure.com/";
        public async Task<List<DetectedFace>> DetectFaceRecognize(string url)
        {
            const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;

            // Authenticate.
            IFaceClient client = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

            // Detect faces from image URL. Since only recognizing, use the recognition model 1.
            // We use detection model 3 because we are not retrieving attributes.
            IList<DetectedFace> detectedFaces = await client.Face.DetectWithUrlAsync(url, returnFaceId: true, recognitionModel: RECOGNITION_MODEL4, detectionModel: DetectionModel.Detection03, returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.QualityForRecognition });
            List<DetectedFace> sufficientQualityFaces = new List<DetectedFace>();
            foreach (DetectedFace detectedFace in detectedFaces)
            {
                var faceQualityForRecognition = detectedFace.FaceAttributes.QualityForRecognition;
                if (faceQualityForRecognition.HasValue && (faceQualityForRecognition.Value >= QualityForRecognition.Medium))
                {
                    sufficientQualityFaces.Add(detectedFace);
                }
            }

            return sufficientQualityFaces.ToList();
        }

        public IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }
    }

    public class AutorizacionTecnicoAppResponse
    {
        public long IdUser { get; set; }
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public long IdVerificentro { get; set; }
        public string Verificentro { get; set; }
        public string UrlFotoEvidencia { get; set; }
        public string Token { get; set; }
    }
    public class AutorizacionTecnicoAppRequest
    {
        public long IdVerificentro { get; set; }
        public string NumEmpleado { get; set; }
        public byte[] Foto { get; set; }
    }
}
