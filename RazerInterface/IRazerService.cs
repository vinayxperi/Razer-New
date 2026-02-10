using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;

namespace RazerWS
{
    [ServiceContract]
    public interface IRazerService
    {
        [OperationContract]
        DataSet GetObject(string BusinessObjectName, DataSet Parms, DataSet SecurityRole);

        [OperationContract]
        DataSet GetTableObject(string BusinessObjectName, DataSet Parms, string TableName, DataSet SecurityRole);

        [OperationContract]
        DataSet SaveObject(string BusinessObjectName, DataSet ObjectData, DataSet SecurityRole);

        [OperationContract]
        DataSet SaveTableObject(string BusinessObjectName, DataSet ObjectData, string TableName, DataSet SecurityRole);

        [OperationContract]
        DataSet NewObject(string BusinessObjectName, DataSet Parms, DataSet SecurityRole);

        [OperationContract]
        string GetDatabaseServerName();
    }
}
