using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cDataRazer;
using System.Data;

namespace RazerWS
{
    public class cBilling
    {

       // private DataTable dtTemp; //Temporary data table for holding intermediate results
        private DataSet dsWork = new DataSet(); //Work Dataset 
        private cDataRazer.cLASERDB SQLCA = new cDataRazer.cLASERDB();

        private cDataRazer.cLASERBaseTable msTable;
        public cDataRazer.cLASERBaseTable sTable
        {
            get { return msTable; }
            set { msTable = value; }
        }

         public cBilling(cLASERDB sqlCA)
        {
            //Establish the database connection variable
            SQLCA = sqlCA;

            //Create the base table used for queries
            sTable = new cLASERBaseTable(ref SQLCA, "Table");

        }

         /// <summary>
         /// Service to update billing location table to include all billing data for the selected group
         /// If no specific batch or company is provided, then the billing location is updated for all contracts
         /// If both company and batch is provided, then update runs for the batch only.
         /// </summary>
         /// <param name="batchID">The ID of the batch to update the locations for / 0=Not for a batch</param>
         /// <param name="companyCode">The 2 character company code of the company to run for - Empty string means it is not for a specific company</param>
         /// <returns>empty string means the job ran successfully - A value in the string represents any error message received from running the service</returns>
         public string UpdateBillingLocations(int batchID, string companyCode)
         {
             //Add the stored procedure parameters
             sTable.Add_SP_Parm(batchID, "@batch_id");
             sTable.Add_SP_Parm(companyCode, "@company_code");

             //Execute the SP
             sTable.NonQuerrySqlSp("usp_batch_prebill_upd_billing_location");
                          
             return sTable.dberror ;

         }


         public bool ReprintNationAdInvoice(string invoiceNumber, bool reprintRevised, bool saveRevision, string printer)
         {
             string revised = reprintRevised == true ? "Y" : "N";
             string saved = saveRevision == true ? "Y" : "N";

             sTable.Add_SP_Parm(revised, "@revised_flag");
             sTable.Add_SP_Parm(saved, "@saveflag");
             sTable.Add_SP_Parm(invoiceNumber, "@invoice_number");
             sTable.Add_SP_Parm(printer, "@printer");

             if (sTable.NonQuerrySqlSp("usp_natl_ads_reprint"))
             {
                 return true;
             }

             return false;
         }


         public bool UpdateCustomInvoiceDetail(DataSet detailData)
         {
             return false;
         }


         public bool ReprintCustomInvoice(string invoiceNumber)
         {
             
             sTable.Add_SP_Parm(invoiceNumber, "@invoice_number");

             if (sTable.NonQuerrySqlSp("usp_ins_man_inv_reprint"))
             {
                 return true;
             }

             return false;
         }

    }
}