using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    /// <summary>
    /// Administration Question Group containing , the list of entites. Related to population long list in pivot element
    /// </summary>
    class AdministrationQuestionGroup
    {
        /// <summary>
        /// List of questions containing the node and node title
        /// </summary>
        public List<AdministrationQuestion> ListQuestions { get; set; }

        public AdministrationQuestionGroup()
        {
            ListQuestions = new List<AdministrationQuestion>();

        }

    }
}
