using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales_Management.Entities
{
    class TimeTravelledMoneyEarned
    {

        public int TotalTimePath { get; set; }

        public int TotalMoneyPath { get; set; }

        public List<int> ActualPath { get; set; }

    }
}
