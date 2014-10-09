using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavis;

namespace ExpenseApprovalAppLogic.Links
{
    public class BaseLink : Link
    {
        public bool KeepInHistory { get; set; }
    }
}
