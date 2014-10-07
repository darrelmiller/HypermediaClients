using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HypermediaAppServer.ExpenseApp.Model
{
    public class DataService
    {

        public ExpenseRepository ExpenseRepository { get; private set; }

        public DataService()
        {
            ExpenseRepository = new ExpenseRepository(LoadJsonArray("expenses.json"));
 
        }
        private JArray LoadJsonArray(string jsonDataFile)
        {
            var resourceStream = this.GetType().Assembly.GetManifestResourceStream(this.GetType(), jsonDataFile);
            var jsonString = new StreamReader(resourceStream).ReadToEnd();
            return JArray.Parse(jsonString);
        }
    }

    public class Repository<T>
    {
        protected Dictionary<int, T> _Entities = new Dictionary<int, T>();


        public IEnumerable<T> GetAll()
        {
            return _Entities.Values;
        }
        public T Get(int id)
        {
            return _Entities[id];
        }
    }

    public class ExpenseRepository : Repository<Expense>
    {
        public ExpenseRepository(JArray expenses)
        {

            foreach (dynamic expense in expenses)
            {
                _Entities.Add((int)expense.id, new Expense()
                {
                    Id = expense.id,
                    Description = expense.description,
                    Amount = expense.amount,
                    Category = expense.category,
                    ReceiptFileName = expense.receiptFileName,
                    Status = expense.status
                });
            }

        }


    }
}
