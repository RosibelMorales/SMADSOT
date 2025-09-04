using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Smadot.Utilities.Modelos.Interfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Smadot.Utilities.BlobStorage.BlobStorage;

namespace Smadot.Utilities.BlobStorage
{
    public class BlobStorage
    {
        private string _connectionString;
        private string _containerName;
        public string _url;

        public BlobStorage(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
            _url = "https://storageverificentros.blob.core.windows.net/" + _containerName;
        }

        public async Task<string> UploadFileAsync(byte[] file, string fileName, string base64 = "")
        {
            string extension = Path.GetExtension(fileName).ToLower();
            var contentType = "";
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".jpg":
                    contentType = "image/jpg";
                    break;
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".xlsx":                
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                default:
                    return "";
            }
            //fileName = Path.GetFileNameWithoutExtension(fileName);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(_containerName);

            //CloudBlockBlob blockBlob = container.GetBlockBlobReference(idUser.ToUpper() + "_" + rfcUser.ToUpper() + "_" + type.ToUpper() + ".pdf");
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            blockBlob.Properties.ContentType = contentType;
            if(!string.IsNullOrEmpty(base64))
                file = Convert.FromBase64String(base64);

            await blockBlob.UploadFromByteArrayAsync(file, 0, file.Length);

            string filepath = blockBlob.StorageUri.PrimaryUri.ToString();
            return filepath;
        }

        public async Task<ResponseViewModel> Remove(string url)
        {
            var result = new ResponseViewModel(true);
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
                url = url.Replace(container.StorageUri.PrimaryUri.ToString()+"/", "");
                var blockBlob = container.GetBlockBlobReference(url);
                var res = await blockBlob.DeleteIfExistsAsync();
                result.Result = res;
                
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.IsSuccessFully = false;
            }
            return result;
        }

        public async Task<ResponseViewModel> DownloadFileAsync(string url, bool base64 = false)
        {
            var result = new ResponseViewModel(true);
            try
            {
                var filename = url;
                if (String.IsNullOrEmpty(url))
                {
                    throw new Exception("URL no válida.");
                }

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
                var blockBlob = new CloudBlockBlob(new Uri(url), storageAccount.Credentials);

                await blockBlob.FetchAttributesAsync();
                var fileByteLength = blockBlob.Properties.Length;
                var myByteArray = new byte[fileByteLength];
                await blockBlob.DownloadToByteArrayAsync(myByteArray, 0);
                if (base64)
                    result.Result = Convert.ToBase64String(myByteArray);
                else
                    result.Result = myByteArray;
            }
            catch (Exception e)
            {
                result.Message = e.Message;
                result.IsSuccessFully = false;
            }
            return result;
        }
        public async Task<FileContentResult> DownloadFileStream(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("URL no válida.");
            }
            string fullUrl = $"{_url}{url}";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blockBlob = new CloudBlockBlob(new Uri(fullUrl), storageAccount.Credentials);

            await blockBlob.FetchAttributesAsync();
            var fileByteLength = blockBlob.Properties.Length;
            var myByteArray = new byte[fileByteLength];
            await blockBlob.DownloadToByteArrayAsync(myByteArray, 0);

            // Obtener el tipo de contenido del archivo
            string contentType = blockBlob.Properties.ContentType;

            // Devolver el archivo en un FileContentResult
            var fileContentResult = new FileContentResult(myByteArray, contentType)
            {
                FileDownloadName = Path.GetFileName(url) // Establecer el nombre de descarga
            };

            return fileContentResult;
        }

    }

}
