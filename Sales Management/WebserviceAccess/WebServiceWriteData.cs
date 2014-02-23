using Newtonsoft.Json;
using RestSharp;
using Sales_Management.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.WebserviceAccess
{
    class WebServiceWriteData
    {
        private const String ACCESS_QUESTION_OPTIONS_URL = "http://tomcat7-shrikanthavale.rhcloud.com/SalesManagementWebservice/rest/salesmanagement/writeoptions";

        private const String ACCESS_QUESTION_URL = "http://tomcat7-shrikanthavale.rhcloud.com/SalesManagementWebservice/rest/salesmanagement/writequestion";


        public static async Task<String> SaveQuestionDetails(QuestionDetails questionDetails)
        {
            String jsonResponse = "";

            using (var httpClient = new HttpClient())
            {

                string postBody = JsonConvert.SerializeObject(questionDetails);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("SalesManagementQuestion", postBody)

                });
                
                HttpResponseMessage response = await httpClient.PostAsync(ACCESS_QUESTION_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    //save successfull, then reset data 
                    WebServiceReadData.questionDetails = null;
                    
                }

                return jsonResponse;
            }

        }


        public static async Task<String> SaveQuestionOptionDetails(List<QuestionOptionDetails> questionOptionDetailsNew)
        {
            String jsonResponse = "";

            using (var httpClient = new HttpClient())
            {

                string postBody = JsonConvert.SerializeObject(questionOptionDetailsNew);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("SalesManagementOptionList", postBody)

                });

                HttpResponseMessage response = await httpClient.PostAsync(ACCESS_QUESTION_OPTIONS_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    jsonResponse = await response.Content.ReadAsStringAsync();

                    //save successfull, then reset data 
                    WebServiceReadData.questionOptionDetails = null;

                }

                return jsonResponse;
            }
        }
    }
}
