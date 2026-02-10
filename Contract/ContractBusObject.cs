using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RazerBase;

namespace Contract
{
    public class ContractBusObject
    {

        public ContractBusObject(cBaseBusObject busObject)
        {
            BusObject = busObject;
            CreateDataTables();
        }

        #region Properties

        public cBaseBusObject BusObject { get; private set; }

        internal List<ContractDataTable> DataTables { get; set; }

        public bool IsValid
        {
            get
            {
                return Validate();
            }
        }

        public string ValidationMessage { get; private set; }

        #endregion

        #region Methods

        private bool Validate()
        {
            ValidationMessage = string.Join("\n", DataTables.Select(dt => dt.Validate()).Where(s => !String.IsNullOrEmpty(s)).ToArray());

            return string.IsNullOrWhiteSpace(ValidationMessage);
        }

        private void CreateDataTables()
        {
            DataTables = new List<ContractDataTable>();

            if (BusObject.ObjectData != null)
            {
                foreach (DataTable dataTable in BusObject.ObjectData.Tables)
                {
                    DataTables.Add(ContractDataTableFactory.Create(this, dataTable.TableName));
                }
            }
        }

        #endregion

    }
}
