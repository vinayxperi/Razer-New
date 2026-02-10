using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public class RuleDetailDataTable : ContractReadOnlyDataTable
    {
        public RuleDetailDataTable(ContractBusObject busObject, string tableName)
            : base(busObject, tableName)
        {
        }
    }
}
