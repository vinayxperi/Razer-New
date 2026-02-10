using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RazerWS;

namespace Razer.AppCode
{
    class RazerServiceDelegate : IRazerService
    {
        private RazerService.RazerServiceClient razerSrvc = new RazerService.RazerServiceClient();

        public RazerServiceDelegate()
        {
            var env = System.Configuration.ConfigurationManager.AppSettings["env"].ToString();
            StringBuilder builder = new StringBuilder("svcURL");
            builder.Append(env);
            var svcURL = System.Configuration.ConfigurationManager.AppSettings[builder.ToString()].ToString();
            razerSrvc.Endpoint.Address = new System.ServiceModel.EndpointAddress(svcURL);
        }

        public System.Data.DataSet GetObject(string BusinessObjectName, System.Data.DataSet Parms, System.Data.DataSet SecurityRole)
        {
            return razerSrvc.GetObject(BusinessObjectName, Parms, SecurityRole);
        }

        public System.Data.DataSet GetTableObject(string BusinessObjectName, System.Data.DataSet Parms, string TableName, System.Data.DataSet SecurityRole)
        {
            return razerSrvc.GetTableObject(BusinessObjectName, Parms, TableName, SecurityRole);
        }

        public System.Data.DataSet SaveObject(string BusinessObjectName, System.Data.DataSet ObjectData, System.Data.DataSet SecurityRole)
        {
            return razerSrvc.SaveObject(BusinessObjectName, ObjectData, SecurityRole);
        }

        public System.Data.DataSet SaveTableObject(string BusinessObjectName, System.Data.DataSet ObjectData, string TableName, System.Data.DataSet SecurityRole)
        {
            return razerSrvc.SaveTableObject(BusinessObjectName, ObjectData, TableName, SecurityRole);
        }

        public System.Data.DataSet NewObject(string BusinessObjectName, System.Data.DataSet Parms, System.Data.DataSet SecurityRole)
        {
            return razerSrvc.NewObject(BusinessObjectName, Parms, SecurityRole);
        }

        public string GetDatabaseServerName()
        {
            return razerSrvc.GetDatabaseServerName();
        }
    }
}
