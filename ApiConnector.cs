using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MaxGenesis
{
    public class ApiConnector
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _apiUrl;

        public ApiConnector(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<string> Generate3DFromImage(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException("Imagen no encontrada.");

            using (var content = new MultipartFormDataContent())
            {
                var fileStream = File.OpenRead(imagePath);
                var streamContent = new StreamContent(fileStream);
                content.Add(streamContent, "file", Path.GetFileName(imagePath));

                var response = await _client.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                // Suponemos que el API devuelve el archivo OBJ directamente o una URL
                // Por simplicidad, guardaremos el resultado en un archivo temporal
                string tempObjPath = Path.Combine(Path.GetTempPath(), "MaxGenesis_Result.obj");
                using (var file = File.Create(tempObjPath))
                {
                    await response.Content.CopyToAsync(file);
                }
                return tempObjPath;
            }
        }
    }
}
