using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FacesApiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imagePath = @"D:\Project\Microservice\FacesAndFaces\Faces\FacesApiTest\_DSC8427.JPG";
            var urlAddress = "http://localhost:6000/api/faces";
            ImageUtility imgUtility = new ImageUtility();
            var bytes = imgUtility.ConvertToBytes(imagePath);
            List<byte[]> faceList = null;
            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using (var httpClient =new HttpClient())
            {
                using (var response= await httpClient.PostAsync(urlAddress,byteContent))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    faceList = JsonConvert.DeserializeObject<List<byte[]>>(apiResponse);
                }
            }
            if (faceList.Count>0)
            {
                for (int i = 0; i < faceList.Count; i++)
                {
                    imgUtility.FromBytesToImage(faceList[i], "face" + i);
                }
            }
        }
    }
}
