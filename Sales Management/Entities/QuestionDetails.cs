using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    /// <summary>
    /// Entity representing mapping with the  JSON data returned from the webservice. All the Question Details like node, title
    /// description can be stored in this entity, once the JSON response is received from webservice and converted back
    /// </summary>
    public class QuestionDetails
    {
        /**
        * primary key node of the case study
        */
        public String caseStudyNode {get; set;}

        /**
         * case study title
         */
        public String caseStudyTitle { get; set; }

        /**
         * name of the organization related to case study
         */
        public String caseStudyOrganization { get; set; }

        /**
         * case study full description
         */
        public String caseStudyDescription { get; set; }

    }
}
