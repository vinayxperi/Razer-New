using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Contract
{
    public class ContractDataColumnFactory : ContractFactory
    {
        public static ContractDataColumn Create(ContractDataTable dataTable, DataColumn dataColumn)
        {
            Type columnType = dataTable.DefaultColumnType;

            if (dataTable.CustomColumnTypes.ContainsKey(dataColumn.ColumnName))
            {
                columnType = dataTable.CustomColumnTypes[dataColumn.ColumnName];
            }

            ConstructorInfo ci = columnType.GetConstructor(new Type[] { typeof(ContractDataTable), typeof(DataColumn) });

            switch (columnType.Name)
            {
                default:
                    var activator = GetActivator<ContractDataColumn>(ci);
                    return activator(new object[] { dataTable, dataColumn });
            }
        }
    }
}
