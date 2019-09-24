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

namespace tmg.equinox.services.api.test
{
    [TestClass]
    public class UnitTest1
    {


        public void My1Tes1t()
        {

        }
        public void MyTest()
        {
            string baseAddress = "http://localhost:33658";


            using (WebApp.Start<tmg.equinox.services.api.Startup>(baseAddress))
            {

                var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
                //   var t = client.GetAsync("/api/v1/Accounts").Result;

                /*req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                   req.AddHeader("Authorization", @"Bearer Nn4tN4E0SwLuPLLGcp5kZt2xwcuhYOztJeudgs-vZRr4vbA6HxhI-r5nZ6-rCSTvMmScH3MzeDtXy0qTBiXeuP_sfFTwx4OJjSVh-e565uhjucYDcLj_nla269Gzpz49P0djSNNr5hIZodH5LHgUG1_KyvRiqlFYFpdbK4pAJ7o6iN2wu0bd4A5yYy4oCeAmxk3hw0YMOtAelnBdIhysqvkMD_DggxVrLlkpC12IhJ17WhDHABDdnhWPrOEhw1Rmldu33k0weFyHr9xDQ8Ci--XOebqwPPzkbGz3kFoM7lIdhzWluyzJ4655Ac8ZYivCHlvtZeLmg4MDfEZM60KZhDIYQjr0JvYUU-sQRlQxhlO3LTVD7b8RcuJES2pwvWXnMfCATHbnNGvQPc2VJPkQodlso7N49BV2vDFV8gDYgEpQJdR2nQi0zMzO-V8GEQEDDElwsZFRgwoViGsrkhB5AhjkG9V_1ym_84LVY4_vtPnPqXY6epeOEkd2S1BiI4aBI_By8tLAjyr_AFDp4XmDvb7B5Ni6mCwEsuxIUNCeSMv25U_GP_zLEtSZvbMUBLvzSFP98i4geIJfMYTuLvHbP2x52lFUZVAlbmtWg4cT1j1uQ4bSecDx6khvgwetz0Ru27VUbr8xVNt-Mu_o4Uqg_tm905D7tpSq2NutSkd3JIxqh197q6S1yOgRs0BlRxzBY-kYFVH44bt_lfJaeIrps");

                var result = req.GetAsync().Result;
                string responseContent = result.Content.ReadAsStringAsync().Result;*/
                //    var entity = JsonConvert.DeserializeObject<List<Orders>>(responseContent);

                var req = new HttpClient { BaseAddress = new Uri(baseAddress) };
                req.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                //   req.AddHeader("Accept", @"application/json");

                var input = new StringContent("grant_type=password&username=tmgsuperuser&password=Summer@2018#", System.Text.Encoding.ASCII);
                var response = req.PostAsync("api/v1/Token", input).Result;

                var body = response.Content.ReadAsStringAsync().Result;


            }

            // arrange
            using (var server = TestServer.Create<tmg.equinox.services.api.Startup>())
            {

                var req = server.CreateRequest("/api/v1/Token");
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                //   req.AddHeader("Accept", @"application/json");

                req.And(x => x.Content = new StringContent("grant_type=password&username=tmgsuperuser&password=Summer@2018#", System.Text.Encoding.ASCII));
                var response = req.PostAsync().Result;

                var body = response.Content.ReadAsStringAsync().Result;

                // Did the request produce a 200 OK response?
                // Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);

                // Retrieve the content of the response
                //  string responseBody = await response.Content.ReadAsStringAsync();
                // this uses a custom method for deserializing JSON to a dictionary of objects using JSON.NET
                Dictionary<string, object> responseData = new Dictionary<string, object>();//= deserializeToDictionary(responseBody);

                // Did the response come with an access token?
                Assert.IsTrue(responseData.ContainsKey("access_token"));

            }
        }

        [TestMethod]
        public void testNew()
        {
           string baseAddress = "http://localhost:33646/";

            // Start OWIN host     
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                
             //   var response = client.GetAsync(baseAddress + "api/v1/Accounts").Result;

             //   var authorizationHeader = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("rajeev:secretKey"));
             //   client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);

                var form = new Dictionary<string, string>
               {
                   {"grant_type", "password"},
                   {"username", "tmgsuperuser"},
                   {"password", "Summer@2018#"},
               };

                var tokenResponse = client.PostAsync(baseAddress + "api/v1/Token", new FormUrlEncodedContent(form)).Result;
                var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var authorizedResponse = client.GetAsync(baseAddress + "api/v1/Accounts").Result;
                string res =  authorizedResponse.Content.ReadAsStringAsync().Result;
                

            }
        /*
            using (var server = TestServer.Create<Startup>())
            {

                var req = server.CreateRequest("/api/v1/Token");
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                //   req.AddHeader("Accept", @"application/json");

                req.And(x => x.Content = new StringContent("grant_type=password&username=tmgsuperuser&password=Summer@2018#", System.Text.Encoding.ASCII));
                var response = req.PostAsync().Result;

                var token = response.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                

                req = server.CreateRequest("http://locahost/api/v1/Accounts");
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                req.AddHeader("Authorization", "Bearer " + token.AccessToken);

                server.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var authorizedResponse = server.HttpClient.GetAsync("api/v1/Accounts").Result;
                string res = authorizedResponse.Content.ReadAsStringAsync().Result;


               // var result = server.HttpClient.GetAsync("/api/v1/Accounts").Result;
                //string res = result.Content.ReadAsStringAsync().Result;

            }*/
        }

      
        /*
      [TestClass]
      public class TestClass
      {
          private class TestStartup : Startup
          {
              public override void Configuration(IAppBuilder app)
              {
                  // do your web api, IoC, etc setup here
                  var config = new HttpConfiguration();
                  config.MapHttpAttributeRoutes();
                  // ...etc
                  app.UseWebApi(config);

              }
          }


      }*/
    }
}
