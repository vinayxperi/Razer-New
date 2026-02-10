using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RazerWS
{
    public class PreWarmCache : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            ////Load WebService
            var x = new RazerWS.RazerService();
            var dtParms = new cTransaction.cDsParms();

            dtParms.add("SA0099", "@receivable_account");
            dtParms.add("A", "@product_code");

            x.GetObject("RecvAcct", dtParms.GetDS(), new System.Data.DataSet());
        }
    }
}