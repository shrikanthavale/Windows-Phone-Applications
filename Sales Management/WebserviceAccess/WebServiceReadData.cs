using Newtonsoft.Json;
using RestSharp;
using Sales_Management.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.WebserviceAccess
{

    /// <summary>
    /// Web service methods accessing the appropriate URL and fetching the data accordingly
    /// </summary>
    public class WebServiceReadData
    {

        /// <summary>
        /// Web service URL for accessing question options
        /// </summary>
        private const String ACCESS_QUESTION_OPTIONS_URL = "http://tomcat7-shrikanthavale.rhcloud.com/SalesManagementWebservice/rest/salesmanagement/options";

        /// <summary>
        /// Web service URL for accessing question
        /// </summary>
        private const String ACCESS_QUESTION_URL = "http://tomcat7-shrikanthavale.rhcloud.com/SalesManagementWebservice/rest/salesmanagement/questions";

        /// <summary>
        /// list for storing all the question details
        /// </summary>
        public static List<QuestionDetails> questionDetails = null;

        /// <summary>
        /// list for storing all the question options details
        /// </summary>
        public static List<QuestionOptionDetails> questionOptionDetails = null;

        /// <summary>
        /// This method gets all the 16 Questions details, like node, title, organization, description etc
        /// </summary>
        /// <returns></returns>
        public static async Task<List<QuestionDetails>> GetQuestionDetails()
        {
            // http client
            using (var httpClient = new HttpClient())
            {
                // http client does the caching and sometimes returns the data without actually calling the webservice.
                // this results in returning of stale data, even after new data is saved.
                // to avoid this we have to make sure that http client thinks each requests as new , so we append new generated string
                // everytim we do a call
                String uniqueString = "?r=" + Guid.NewGuid().ToString();

                // get the response
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(ACCESS_QUESTION_URL + uniqueString, UriKind.RelativeOrAbsolute));

                if (response.IsSuccessStatusCode)
                {
                    // json response
                    String jsonResponse = await response.Content.ReadAsStringAsync();

                    // convert to the entity
                    questionDetails = JsonConvert.DeserializeObject<List<QuestionDetails>>(jsonResponse);

                }

                // return the details
                return questionDetails;
            }
            
        }

        public static async Task<List<QuestionOptionDetails>> GetQuestionOptionDetails()
        {
            using (var httpClient = new HttpClient())
            {
                String uniqueString = "?r=" + Guid.NewGuid().ToString();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(ACCESS_QUESTION_OPTIONS_URL + uniqueString, UriKind.RelativeOrAbsolute));

                if (response.IsSuccessStatusCode)
                {
                    String jsonResponse = await response.Content.ReadAsStringAsync();

                    questionOptionDetails = JsonConvert.DeserializeObject<List<QuestionOptionDetails>>(jsonResponse);

                }

                return questionOptionDetails;
            }

        }

        public static async Task<Dictionary<String, int>> GetMaximumAmountNodeMap()
        {
            Dictionary<String, int> mapNodeMaxAmount = new Dictionary<string, int>();
 
            if (questionOptionDetails == null)
            {
                await GetQuestionOptionDetails();
            }

            foreach (QuestionOptionDetails tempQuestionOptions in questionOptionDetails)
            {
                if (mapNodeMaxAmount.ContainsKey(tempQuestionOptions.caseStudyNode))
                {
                    if (tempQuestionOptions.questionOptionMoneyAssoicated > mapNodeMaxAmount[tempQuestionOptions.caseStudyNode])
                    {
                        mapNodeMaxAmount.Remove(tempQuestionOptions.caseStudyNode);
                        mapNodeMaxAmount.Add(tempQuestionOptions.caseStudyNode, tempQuestionOptions.questionOptionMoneyAssoicated);
                    }

                }
                else
                {
                    mapNodeMaxAmount.Add(tempQuestionOptions.caseStudyNode, tempQuestionOptions.questionOptionMoneyAssoicated);
                }
            }

            return mapNodeMaxAmount;

        }

        public static async Task<QuestionDetails> GetSingleQuestionDetails(String nodeVisited)
        {
            if (questionDetails == null)
            {
                await GetQuestionDetails();
            }

            foreach(QuestionDetails tempQuestionDetails in questionDetails){
                if(tempQuestionDetails.caseStudyNode.Equals(nodeVisited)){
                    return tempQuestionDetails;
                }
            }

            return null;

        }


        public static async Task<List<QuestionOptionDetails>> GetQuestionOptionDetails(String nodeVisited)
        {
            List<QuestionOptionDetails> questionOptions = new List<QuestionOptionDetails>();

            if (questionOptionDetails == null)
            {
                await GetQuestionOptionDetails();
            }

            foreach (QuestionOptionDetails tempQuestionOptions in questionOptionDetails)
            {
                if (nodeVisited.ToLower().Equals(tempQuestionOptions.caseStudyNode.ToLower()))
                {
                    questionOptions.Add(tempQuestionOptions);
                }
            }

            return questionOptions;

        }

    }
}
