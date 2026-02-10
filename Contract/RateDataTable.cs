using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public class RateDataTable : ContractDataTable
    {
        //private const string StartDateFieldName = "start_date";
        //private const string EndDateFieldName = "end_date";
        //private const string StatusFlagFieldName = "status_flag";

        //public RateDataTable(ContractBusObject busObject, string tableName)
        //    : base(busObject, tableName)
        //{
        //}

        //public Infragistics.Windows.DataPresenter.Record SelectedRecord { get; internal set; }

        //public override string Validate()
        //{
        //    string message = "";

        //    //if (DataTable.Rows.ToList().Count(dr => (short)dr[StatusFlagFieldName] == 1) == 0)
        //    //{
        //    //    message += "At least one rate must be active." + "\n";
        //    //}
        //    //else if (DataTable.Rows.ToList().Where(dr => (short)dr[StatusFlagFieldName] == 1)
        //    //    .Count(dr => (DateTime)dr[StartDateFieldName] >= DateTime.Now.Date &&
        //    //                 (DateTime)dr[EndDateFieldName] <= DateTime.Now.Date) == 0)
        //    //{
        //    //    message += "No active rate " + "\n";
        //    //}

        //    return message;
        //}

        //protected override void CreateColumns()
        //{
        //    CustomColumnTypes = new Dictionary<string, Type>() { { "rate_id", typeof(ReadOnlyContractDataColumn) } };
        //    base.CreateColumns();
        //}
    }
}
