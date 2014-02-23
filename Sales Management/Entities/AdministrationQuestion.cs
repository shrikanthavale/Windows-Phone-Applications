using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    /// <summary>
    /// This entity represent the data model for the pivot item long list selector
    /// </summary>
    class AdministrationQuestion
    {

        /// <summary>
        /// This attribute for case study
        /// </summary>
        public String caseStudyNode { get; set; }

        /// <summary>
        /// this attribute for title of the case study
        /// </summary>
        public String caseStudyTitle { get; set; }

        public AdministrationQuestion()
        {

        }

        /// <summary>
        /// Initializing the Entity with default values
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="title">Title</param>
        public AdministrationQuestion(String node, String title)
        {
            caseStudyNode = node;
            caseStudyTitle = title;
        }
    
    }
}
