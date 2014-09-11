using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HypermediaAppServer.ExpenseApp.Model
{
    public class Expense
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string ReceiptFileName { get; set; }
    }
}
