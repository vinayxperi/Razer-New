using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Contract
{
    public class ContractDataTable
    {
        //private const string ContractIdFieldName = "contract_id";

        //public ContractDataTable(ContractBusObject busObject, string tableName)
        //{
        //    ContractBusObject = busObject;
        //    DataTable = busObject.BusObject.ObjectData.Tables[tableName];
        //    DataTable.RowChanged += new DataRowChangeEventHandler(DataTable_RowChanged);
        //    CreateColumns();
        //}

        //public virtual Type DefaultColumnType
        //{
        //    get
        //    {
        //        return typeof(ContractDataColumn);
        //    }
        //}

        //public Dictionary<string, Type> CustomColumnTypes { get; protected set; }

        //public List<ContractDataColumn> DataColumns { get; set; }

        //protected ContractBusObject ContractBusObject { get; set; }

        //protected DataTable DataTable { get; set; }

        //public virtual string Validate()
        //{
        //    return "";  
        //}

        //protected virtual void CreateColumns()
        //{
        //    if (CustomColumnTypes == null)
        //    {
        //        CustomColumnTypes = new Dictionary<string, Type>();
        //    }

        //    DataColumns = new List<ContractDataColumn>();
        //    DataTable.Columns.Cast<DataColumn>().ToList().ForEach(c => DataColumns.Add(ContractDataColumnFactory.Create(this, c)));
        //}

        //protected virtual void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        //{
        //    if (e.Action == DataRowAction.Add)
        //    {
        //        // Set defaults for rows and relationships
        //        // NOTE:  Any tables can be accessed through the DataTable.DataSet.Tables property
        //        if (e.Row[ContractIdFieldName] != null)
        //        {
        //            e.Row[ContractIdFieldName] = DataTable.DataSet.Tables[ContractDataTableFactory.GeneralCompanyTableName].Rows[0][ContractIdFieldName];
        //        }
        //    }
        //}
    }
}
