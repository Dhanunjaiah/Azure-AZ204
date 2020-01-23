using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace _04_StorageTable
{
    public class SaleData:TableEntity
    {
        public SaleData(string id, string region)
        {
            this.RowKey = id;
            this.PartitionKey = region;
        }

        //Parameterless constructor for read operation
        public SaleData()
        {

        }

        public string Name { get; set; }

        public double Amount { get; set; }

        public string Email { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
