using Sales_Management.WebserviceAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    /// <summary>
    /// Represents a model for Pivot element LongLister
    /// </summary>
    class AdministrationQuestionModel
    {

        /// <summary>
        /// Stores different QuestionGroups containing Question entities
        /// </summary>
        public AdministrationQuestionGroup QuestionGroup { get; set; }

        /// <summary>
        /// Load Data method, which calls the webservice and gets the data
        /// </summary>
        /// <returns>Returns the model </returns>
        public async Task<AdministrationQuestionModel> LoadData()
        {
            this.QuestionGroup = new AdministrationQuestionGroup();

            // web service call 
            List<QuestionDetails> questionDetails = await WebServiceReadData.GetQuestionDetails();

            // construct the complete model as list of data to be displayed
            foreach (QuestionDetails temp in questionDetails)
            {
                AdministrationQuestion administrationQuestion = new AdministrationQuestion();
                administrationQuestion.caseStudyNode = temp.caseStudyNode;
                administrationQuestion.caseStudyTitle = temp.caseStudyTitle;
                this.QuestionGroup.ListQuestions.Add(administrationQuestion);
            }

            return this;
        }
    }
}
