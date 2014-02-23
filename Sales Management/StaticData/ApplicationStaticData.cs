using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sales_Management.StaticData
{
    class ApplicationStaticData
    {
        public static String currentNodeVisit = "";

        public static Button focusedButton = null;

        public static List<Button> listQuestionNonQuestionNode = null;

        public static bool restorePreviousGrid = false;

        public static List<Button> listVisitedNodes = new List<Button>();

        public static int timeUsed = 0;

        public static int moneyEarned = 0;

        public static bool questionAnswered = false;
    }
}
