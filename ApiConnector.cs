using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MaxGenesis
{
    public class ApiConnector
    {
        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        private readonly string _apiUrl;

        public ApiConnector(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<string> Generate3DFromImage(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException("Imagen no encontrada localmente.");

            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileBytes = File.ReadAllBytes(imagePath);
                    var fileContent = new ByteArrayContent(fileBytes);
                    content.Add(fileContent, "file", Path.GetFileName(imagePath));

                    var response = await _client.PostAsync(_apiUrl, content);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        string errorDetail = await response.Content.ReadAsStringAsync();
                        throw new Exception($"El servidor de IA respondió con error ({response.StatusCode}): {errorDetail}");
                    }

                    string tempObjPath = Path.Combine(Path.GetTempPath(), "MaxGenesis_Result.obj");
                    using (var file = File.Create(tempObjPath))
                    {
                        await response.Content.CopyToAsync(file);
                    }
                    return tempObjPath;
                }
            }
            catch (TaskCanceledException)
            {
                throw new Exception("La IA está tardando demasiado. Verifica tu servidor de Python.");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("No se pudo conectar con el servidor de IA. ¿Está encendido el script de Python?");
            }
        }
    }
}
