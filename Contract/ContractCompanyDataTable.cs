using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Contract
{
    public class ContractCompanyDataTable : ContractDataTable
    {
        //private const string CompanyIdFieldName = "company_id";
        //private const string PercentageFieldName = "percentage";
        //private const string PrimaryCompanyFlagFieldName = "primary_company_flag";

        //public ContractCompanyDataTable(ContractBusObject busObject, string tableName) : base(busObject, tableName) { }

        //public override string Validate()
        //{
        //    string message = "";

        //    if (DataTable.Rows.ToList().Count(dr => (short)dr[PrimaryCompanyFlagFieldName] == 1) != 1)
        //    {
        //        message += "One and only one company must be flagged as primary." + "\n";
        //    }

        //    if (DataTable.Rows.ToList().Sum(dr => (double)dr[PercentageFieldName]) != 100)
        //    {
        //        message += "The percentages of all rates must equal 100." + "\n";
        //    }

        //    if (DataTable.Rows.ToList().Select(dr => (string)dr[CompanyIdFieldName]).Distinct().Count() != DataTable.Rows.Count)
        //    {
        //        message += "No company can be entered more that once." + "\n";
        //    }

        //    return message.TrimEnd();
        //}
    }
}
