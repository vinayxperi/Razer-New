using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public class RuleDataTable : ContractReadOnlyDataTable
    {
        //private const string RateIdFieldName = "rate_id";

        //public RuleDataTable(ContractBusObject busObject, string tableName)
        //    : base(busObject, tableName)
        //{
        //}

        //protected override void DataTable_RowChanged(object sender, System.Data.DataRowChangeEventArgs e)
        //{
        //    base.DataTable_RowChanged(sender, e);

        //    if (e.Action == DataRowAction.Add)
        //    {
        //        if (e.Row[RateIdFieldName] != null)
        //        {
        //            var dataTable = ContractBusObject.DataTables.Where(dt => dt.GetType() == typeof(RateDataTable)).FirstOrDefault();
        //            RateDataTable rateDataTable = dataTable as RateDataTable;

        //            if (rateDataTable.SelectedRecord.Index >= 0)
        //            {
        //                e.Row[RateIdFieldName] = DataTable.DataSet.Tables[ContractDataTableFactory.RateTableName].Rows[rateDataTable.SelectedRecord.Index][RateIdFieldName];
        //            }
        //        }
        //    }
        //}
    }
}
