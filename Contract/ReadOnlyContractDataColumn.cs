using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public class ReadOnlyContractDataColumn : ContractDataColumn
    {
        public ReadOnlyContractDataColumn(ContractDataTable dataTable, DataColumn dataColumn) : base(dataTable, dataColumn)
        {
            dataColumn.ReadOnly = true;
        }
    }
}
