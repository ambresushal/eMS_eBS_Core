using System;
using Owin;
using System.Web.Http;
using System.Net;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.Threading.Tasks;
using tmg.equinox.services.api.Models;

namespace tmg.equinox.services.api.test
{
    [TestClass]
    public abstract class BaseTest
    {
        static string baseAddress = "http://localhost:33649/";
        protected Token token;
        IDisposable app = WebApp.Start<Startup>(url: baseAddress);

        public Const Constant { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "superuser"},
                   {"password", "Summer@2018#"},
               };

            token = Post<Token>("api/v1/Token", form);
            
            Initialise();

        }
        public virtual void Initialise()
        {

        }
        public string Get(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var authorizedResponse = client.GetAsync(string.Format("{0}{1}", baseAddress , url)).Result;
                string res = authorizedResponse.Content.ReadAsStringAsync().Result;
                return res;
            }
        }
        public Response<T> Get<T>(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var response = client.GetAsync(string.Format("{0}{1}", baseAddress, url)).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                
                var deserialized = JsonConvert.DeserializeObject<Response<T>>(result);
                return deserialized;
            }
        }

        public ResponsePagedResult<T> GetPageResult<T>(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var response = client.GetAsync(string.Format("{0}{1}", baseAddress, url)).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                var deserialized = JsonConvert.DeserializeObject<ResponsePagedResult<T>>(result);
                return deserialized;
            }
        }

        public Response<T> Post<T>(string url, Object  body) 
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var response = client.PostAsJsonAsync(string.Format("{0}{1}", baseAddress, url), body).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                var deserialized = JsonConvert.DeserializeObject< Response<T>>(result);
                return deserialized;
            }
        }
        public Response<T> Put<T>(string url, Object body)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var response = client.PutAsJsonAsync(string.Format("{0}{1}", baseAddress, url), body).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                var deserialized = JsonConvert.DeserializeObject<Response<T>>(result);
                return deserialized;
            }
        }
        public Response<T> Delete<T>(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var response = client.DeleteAsync(string.Format("{0}{1}", baseAddress, url)).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                var deserialized = JsonConvert.DeserializeObject<Response<T>>(result);
                return deserialized;
            }
        }
        public T Post<T>(string url, Dictionary<string, string> form)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                var response = client.PostAsync(string.Format("{0}{1}", baseAddress, url), new FormUrlEncodedContent(form)).Result;
                return response.Content.ReadAsAsync<T>(new[] { new JsonMediaTypeFormatter() }).Result;
            }
        }
        [TestCleanup]
        public void Cleanup()
        {
            app.Dispose();
        }

    }

    public class Response<T>
    {
        public string Status { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
        public Response(string status,T result, string message)
        {
            Status = status;
            Result = result;
            Message = message;
        }
    }

    public class ResponsePagedResult<T>
    {
        public string Status { get; set; }
        public PagedResult<T> Result { get; set; }
        public string Message { get; set; }
        public ResponsePagedResult(string status, PagedResult<T> result, string message)
        {
            Status = status;
            Result = result;
            Message = message;
        }
    }

    public class ResponseData<T>
    {
        public int MyProperty { get; set; }
    }

    public class Const
    {
        public static string Failure = "Failure";
        public static string Success = "Success";

    }
}
