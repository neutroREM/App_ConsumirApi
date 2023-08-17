using appApiREST.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace appApiREST
{
    public class Program
    {
        static HttpClient cliente = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static void MostrarProducto(cls_Objeto oObjeto)
        {
            Console.WriteLine($"Name: {oObjeto.Name}\tPrecio " +
                $"{oObjeto.Price}\tCategoría: {oObjeto.Category}");
        }

        static async Task<Uri> CreateProductAsync(cls_Objeto oObjeto)
        {
            HttpResponseMessage response = await cliente.PostAsJsonAsync(
                "api/products", oObjeto);
            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        static async Task<cls_Objeto> GetProductAsync(string path)
        {
            var formatters = new List<MediaTypeFormatter>() {
                new JsonMediaTypeFormatter(),
                new XmlMediaTypeFormatter()
            };

            cls_Objeto oObjeto = null;
            HttpResponseMessage response = await cliente.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                //oObjeto = (cls_Objeto)await response.Content.ReadAsAsync<IEnumerable<cls_Objeto>>(formatters);
                oObjeto = await response.Content.ReadAsAsync<cls_Objeto>();
            }

            return oObjeto;
        }

        static async Task<cls_Objeto> UpdateProductAsync(cls_Objeto oObjeto)
        {
            HttpResponseMessage response = await cliente.PutAsJsonAsync(
                $"api/products/{oObjeto.Id}", oObjeto);

            response.EnsureSuccessStatusCode();

            oObjeto = await response.Content.ReadAsAsync<cls_Objeto>();
            return oObjeto;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await cliente.DeleteAsync(
                $"api/products/{id}");

            return response.StatusCode;
        }

        static async Task RunAsync()
        {
            //Actualizar puerto # en la siguiente línea.
            cliente.BaseAddress = new Uri("http://localhost:64195/");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                cls_Objeto oObjeto = new cls_Objeto
                {
                    Name = "Mouse",
                    Price = 400,
                    Category = "Gamer"
                };

                var url = await CreateProductAsync(oObjeto);
                await Console.Out.WriteLineAsync($"Created at {url}");

                //Get 
                oObjeto = await GetProductAsync(url.PathAndQuery);
                MostrarProducto(oObjeto);


                //Update
                await Console.Out.WriteLineAsync("Actualizando...");
                oObjeto.Price = 80;
                await UpdateProductAsync(oObjeto);


                //Delete
                var statusCode = await DeleteProductAsync(oObjeto.Id);
                await Console.Out.WriteLineAsync($"Eliminado (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);
            }
            Console.ReadLine();
        }

    }
}
