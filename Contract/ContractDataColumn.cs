using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public class ContractDataColumn
    {
        public ContractDataColumn(ContractDataTable dataTable, DataColumn dataColumn)
        {
            DataColumn = dataColumn;
        }

        protected DataColumn DataColumn { get; set; }
    }
}
