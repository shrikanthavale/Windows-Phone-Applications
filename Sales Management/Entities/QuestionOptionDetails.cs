using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    /// <summary>
    /// Entity representing mapping with the  JSON data returned from the webservice. All the Question Option Details like node,
    /// description, money associated, evaluation of option can be stored in this entity, once the JSON response is
    /// received from webservice and converted back
    /// </summary>
    public class QuestionOptionDetails
    {
        /**
        * case study node - referential key
        */
        public String caseStudyNode { get; set; }

        /**
         * solutions to the question, four options
         */
        public int caseStudyOptionNumber { get; set; }

        /**
         * question Option description
         */
        public String questionOptionDescription { get; set; }

        /**
         * each option evaluation
         */
        public String questionOptionEvaluation { get; set; }

        /**
         * money associated with each amount
         */
        public int questionOptionMoneyAssoicated { get; set; }

    }
}
