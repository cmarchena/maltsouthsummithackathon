using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ePassport.API.Globals
{
    namespace Utilities
    {
        /// <summary>
        /// Cliente Http
        /// </summary>
        public class ClientHttp
        {

            public string Action { get; set; }
            public string Content { get; set; }
            public JObject Response { get; set; }
            public HttpClient Client { get; set; }

            public ClientHttp(string uri)
            {
                Client = new HttpClient();
                Client.BaseAddress = new Uri(uri);
            }

            async public Task<HttpResponseMessage> Post()
            {
                try
                {
                    var response = await Client.PostAsync(Action, new StringContent(Content));
                    var data = await response.Content.ReadAsStringAsync();

                    Response = JObject.Parse(data);

                    return response;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e.InnerException);
                }
            }

            async public Task<HttpResponseMessage> Get()
            {
                try
                {
                    var response = await Client.GetAsync(Action);
                    var data = await response.Content.ReadAsStringAsync();

                    Response = JObject.Parse(data);

                    return response;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e.InnerException);
                }
            }

        }
    }

}