using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

namespace RazerWS
{
    public class NationalAdsAdjustment
    {
        #region Properties
        public string invoice_number { get; set; }
        public string ErrorMessage { get; set; }
        private static readonly string adjustment_reason = string.Empty;
        public string is_adjustment_number { get; set; }      
        public string gs_user_id { get; set; }
        private string is_natl_ads_invoice_nbr { get; set; }
        public string is_co { get; set; }
        public string gs_product { get; set; }
        public string is_geography { get; set; }
        public string is_bal_center { get; set; }
        public string gl_product { get; set; }  //was is_future_acct1 is now gl_product
        public string is_def_acct { get; set; }
        public string is_interdivision { get; set; }
        public string is_ar_acct { get; set; }
        public string is_rev_acct { get; set; }
        public string is_center { get; set; }
        public string is_vat_nbr { get; set; }
        public string is_emea_ind { get; set; }
        public string ad_comm_acct { get; set; } //was is is_free_acct now ad_comm_acct


        public string[] is_rev_co { get; set; }
        public string[] is_rev_prod_acct { get; set; }
        public string[] is_rev_center { get; set; }
        public string[] is_rev_free_acct { get; set; }
        public string[] is_rev_geography { get; set; }
        public string[] is_rev_interdivision { get; set; }
        public string[] rev_gl_product { get; set; }
        public string ls_comm_flag { get; set; } //was is_rev_future_acct1 now rev_gl_product
        public string ls_vat_flag { get; set; } //was is_rev_future_acct1 now rev_gl_product


        public bool ib_comm_set_to_zero { get; set; }
        public bool ib_calc_commission { get; set; }
        public bool ib_vat_set_to_zero { get; set; }
        public bool ib_add_vat { get; set; }
        public bool ib_reverse_adjustment { get; set; }
        public bool ib_tab_update_ok { get; set; }
        public bool ib_adjustment_created { get; set; }

        public decimal comm_amt { get; set; }
        public decimal net_amt { get; set; }
        public decimal vat_tax { get; set; }
        public decimal total_amt { get; set; }
        public decimal id_prev_comm { get; set; }
        public decimal gd_tot_adjusted { get; set; }
        public decimal ld_comm_rate = 0.00m;

        public int status_flag { get; set; }

    
        public DataTable ids_products_allocated { get; set; }
        public DataTable NationalAdsDetails { get; set; }

        public cDataRazer.cLASERBaseTable myData { get; set; }
        #endregion

        #region Member Variables
        int K, L, iAdjustmentClass, li_calc_comm, li_comm_percent;

        bool lb_comm_chg, lb_vat_chg, lb_comm_increase, lb_vat_flag, lb_vat_increase;

        long?  ll_xrate, ll_rows, 
            iLength, ll_length, ll_class, lNextInvNum, ll_products_used,
             ll_alloc1_pct, ll_alloc2_pct, ll_adj_acct_id, ll_seq_code;

        //decimal {4}
        decimal ld_orig = 0, ld_new = 0, ld_amount, ld_comm_amount = 0, ld_vat_amount = 0, ld_orig_comm,
            ld_new_comm, ld_orig_vat, ld_new_vat, ld_adj_amount = 0,
            ld_alloc_amount, ld_tot_amount, ld_prod1_tot, ld_comm_detail_amount, ld_vat_detail_amount,
            ld_prod2_tot, ld_prod3_tot, ld_inv_tot, ld_original_amount, ld_orig_net ;

        //decimal {2};
        decimal ld_alloc1_tot, ld_alloc2_tot, ld_prod1_pct, ld_prod2_pct,
            ld_prod1_tot_amt, ld_prod2_tot_amt, ld_prod0_tot_amt,
            ld_diff, ld_comm_total, ld_orig_total,ld_amt_chgs;

        //decimal
        decimal ld_comm_percent;

        DateTime ldt_today, ldt_gl_period,ldt_service_period;
       

        //dwItemStatus ldw_status

        string ls_invoice, ls_racct,
            ls_document, sPrefix, ls_company, ls_currency, ls_orig_comm, ls_orig_vat, ls_new_vat, ls_new_comm,
            ls_err_text, sAdjustmentReason, ls_rev_co,
            ls_rev_prod_acct, ls_rev_center, ls_rev_free_acct, ls_alloc_co,
            ls_alloc_acct, ls_alloc_center, ls_alloc_free_acct, ls_comm_co,
            ls_comm_free_acct, ls_comm_center, ls_rev_geography,
            ls_rev_interdivision, ls_rev_future_acct1,
            ls_default_geography, ls_geography_value, ls_geography, ls_comm_geography,
            ls_comm_interdivision, ls_comm_future_acct1, 
            ls_alloc_interdivision, ls_alloc_future_acct1,
            ls_order_type, ls_default_region, ls_product, ls_dept,
            ls_bal_center, ls_future_acct1;
        #endregion

        public NationalAdsAdjustment(DataSet NationalAdsAdjustmentDS, cDataRazer.cLASERBaseTable myData)
        {
            this.myData = myData;

            DataRow dtNationalAdsAdjustmentParms = NationalAdsAdjustmentDS.Tables["NationalAdsAdjustmentParms"].Rows[0];
 
            this.invoice_number = dtNationalAdsAdjustmentParms["invoice_number"].ToString();
            this.sAdjustmentReason = dtNationalAdsAdjustmentParms["adjustment_reason"].ToString();
            //this.is_reversal_type = dtNationalAdsAdjustmentParms["reversal_type"].ToString();
            this.gs_user_id = dtNationalAdsAdjustmentParms["user_name"].ToString();
            //this.is_natl_ads_invoice_nbr = dtNationalAdsAdjustmentParms["natl_ads_invoice_nbr"].ToString();
            this.is_natl_ads_invoice_nbr = this.invoice_number;

            this.ib_comm_set_to_zero = Convert.ToBoolean(dtNationalAdsAdjustmentParms["comm_set_to_zero"]);
            this.ib_calc_commission = Convert.ToBoolean(dtNationalAdsAdjustmentParms["calc_commission"]);
            this.ib_reverse_adjustment = Convert.ToBoolean(dtNationalAdsAdjustmentParms["reverse_adjustment"]);
            this.ib_vat_set_to_zero = Convert.ToBoolean(dtNationalAdsAdjustmentParms["vat_set_to_zero"]);
            this.ib_add_vat = Convert.ToBoolean(dtNationalAdsAdjustmentParms["add_vat"]);

            this.comm_amt = Convert.ToDecimal(dtNationalAdsAdjustmentParms["comm_amt"]);
            this.id_prev_comm = this.comm_amt;
            this.net_amt = Convert.ToDecimal(dtNationalAdsAdjustmentParms["net_amt"]);
            ld_orig_net = this.net_amt;

            this.total_amt = Convert.ToDecimal(dtNationalAdsAdjustmentParms["total_amt"]);
            //this.id_prev_comm = Convert.ToDecimal(dtNationalAdsAdjustmentParms["prev_comm"]);
            this.gd_tot_adjusted = Convert.ToDecimal(dtNationalAdsAdjustmentParms["total_adjusted"]);

            this.vat_tax = Convert.ToDecimal(dtNationalAdsAdjustmentParms["vat_tax"]);

            ld_orig_vat = this.vat_tax;
            

            //this.iadjustmentClass = Convert.ToInt32(dtNationalAdsAdjustmentParms["adjustmentClass"]);

            this.NationalAdsDetails = NationalAdsAdjustmentDS.Tables["NationalAdsDetails"];

            DataTable localDT = GetInfoByInvoiceNumber();
            DateTime broadcast_month = new DateTime();
            if (localDT != null)
            {
                foreach (DataRow row in localDT.Rows)
                {
                    is_co = row["gl_co"].ToString();
                    gs_product = row["product_code"].ToString();
                    is_geography = row["geography"].ToString();
                    is_bal_center = row["bal_gl_center"].ToString();
                    gl_product = row["gl_product"].ToString();
                    is_def_acct = row["mp_def_acct"].ToString();
                    is_interdivision = row["interdivision"].ToString();
                    is_ar_acct = row["ar_acct"].ToString();
                    is_rev_acct = row["revenue_acct"].ToString();
                    is_center = row["gl_center"].ToString();
                    ad_comm_acct = row["ad_comm_acct"].ToString();
                    broadcast_month = Convert.ToDateTime(row["broadcast_month"]);
                    ld_orig = Convert.ToDecimal(row["total_amt"]);
                    status_flag = Convert.ToInt16(row["status_flag"]);
                    ld_comm_percent = Convert.ToDecimal(row["comm_percent"]);
                    ld_comm_percent = ld_comm_percent / 100;
                    is_vat_nbr = row["vat_nbr"].ToString();
                    is_emea_ind = row["emea_ind"].ToString();
                }
            }

            if (myData.SqlSpPopDt("usp_sel_natl_ads_products_used"))
            {
                int maxIndex = myData.GetDataTable.Rows.Count;
                is_rev_co = new string[maxIndex];
                is_rev_prod_acct = new string[maxIndex];
                is_rev_center = new string[maxIndex];
                is_rev_free_acct = new string[maxIndex];
                is_rev_geography = new string[maxIndex];
                is_rev_interdivision = new string[maxIndex];
                rev_gl_product = new string[maxIndex];

                int iCount = 0;
                foreach (DataRow row in myData.GetDataTable.Rows)
                {
                    is_rev_co[iCount] = row["gl_co"].ToString();
                    is_rev_prod_acct[iCount] = row["product_code"].ToString();
                    is_rev_center[iCount] = row["gl_center"].ToString();
                    is_rev_free_acct[iCount] = row["revenue_acct"].ToString();
                    is_rev_geography[iCount] = row["geography"].ToString();
                    is_rev_interdivision[iCount] = row["interdivision"].ToString();
                    rev_gl_product[iCount] = row["gl_product"].ToString();
                    iCount++;
                }
            }

            ids_products_allocated = GetNationalAdsAllocation(broadcast_month);

            ldt_today = DateTime.Parse(DateTime.Now.ToShortDateString());
            ld_amt_chgs = 0;
            ld_amt_chgs = this.gd_tot_adjusted;
            ld_new = total_amt;
            /* clb 08/2003 - pull agency_comm to determine if they get commission*/
            /* from now on, commission will be calculated automatically on adjustments */
            /* at 15% - user can override, but the commission must be either $0 or 15% */
        }

        private DataTable GetNationalAdsAllocation(DateTime broadcastMonth)
        {
            DataTable retDT = null;
            myData.Add_SP_Parm(broadcastMonth, "@broadcast_month");
            if (myData.SqlSpPopDt("usp_sel_natl_ads_allocation"))
            {
                retDT = myData.GetDataTable.Copy();
            }
            return retDT;
        }

        private DataTable GetInfoByInvoiceNumber()
        {
            DataTable retDT = null;

            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("SELECT     hdr.product_code, prodItem.geography, prodItem.gl_center, prodItem.gl_product, prodItem.interdivision, prodItem.revenue_acct, products.gl_co, ");
            sbSQL.AppendLine("                      products.bal_gl_center, products.ar_acct, co.mp_def_acct, co.ad_comm_acct, hdr.broadcast_month, hdr.total_amt, hdr.status_flag, recv_account.comm_percent * 100 as comm_percent, ltrim(rtrim(recv_account.vat_nbr)) as vat_nbr, hdr.emea_ind");
            sbSQL.AppendLine("FROM         natl_ads_inv_hdr AS hdr INNER JOIN");
            sbSQL.AppendLine("                      product_item AS prodItem ON hdr.product_code = prodItem.item_code INNER JOIN");
            sbSQL.AppendLine("                      products ON products.product_code = prodItem.product_code INNER JOIN");
            sbSQL.AppendLine("                      recv_account ON hdr.receivable_account = recv_account.receivable_account INNER JOIN");                                                    
            sbSQL.AppendLine("                      company AS co ON co.company_code = products.gl_co");
            sbSQL.AppendLine("WHERE     (hdr.invoice_number = '{0}')");

            string sSQL = string.Format(sbSQL.ToString(), this.invoice_number);

            if (myData.SqlStringPopDt(sSQL, false))
            {
                retDT = myData.GetDataTable.Copy();
            }

            return retDT;
        }

        public string CreateAdjustment()
        {
            #region Unposted Invoice Code
            // Invoice is either posted or not posted. 2 Sets of code based on the next IF to deal with each type of invoice.
            // If invoice is not posted, simply update the hdr and detail tables with any changes, yea right... "Simply".. nothing simple about this ha!

            if (status_flag == 0) //unposted
            {
                ld_orig_comm = id_prev_comm;
                // If set commission to zero checked, need to just zero out the commission on the hdr table and update the net amount.
                if (ib_comm_set_to_zero == true)
                {
                    comm_amt = 0;
                    net_amt = ld_new + ld_orig_vat;                   
                    
                }

                if (ib_vat_set_to_zero == true)
                {
                    //Neet to reduce the net_amt by the vat_tax amount.
                    net_amt = ld_orig_net - vat_tax;

                    //then reset vat_tax to zero;
                    vat_tax = 0;


                }

                if (ib_add_vat == true)
                {
                    //Need to calculate vat tax

                    vat_tax = (System.Math.Round(ld_new * .15m, 2, MidpointRounding.AwayFromZero));

                    net_amt = net_amt + vat_tax;         

                    


                }

                // If Do not recalc is NOT checked and the origin commission is not zero, we need to recalc the commission and vat.

                if (ib_calc_commission == false && ib_comm_set_to_zero == false && ib_vat_set_to_zero == false && ib_add_vat == false )
                {

                    if (ld_orig_comm != 0)
                    {
                        /*calculate using commission percentage from recv_account if there is one, otherwise use 15 percent */
                        if (ld_comm_percent == 0)
                        {
                            ld_comm_amount = System.Math.Round(ld_new * .15m, 2,MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            ld_comm_amount = System.Math.Round(ld_new * ld_comm_percent, 2,MidpointRounding.AwayFromZero);
                        }

                        comm_amt = ld_comm_amount;
                        net_amt = ld_new - comm_amt;
                    }
                    else
                    {
                        net_amt = ld_new;
                    }

                    if (vat_tax > 0 )
                    {
                        vat_tax = (System.Math.Round(ld_new * .15m, 2, MidpointRounding.AwayFromZero));
                        net_amt = net_amt + vat_tax;

                    }

                }       

              
              
                  
               


                
                //That is updating the totals from the header that are at the bottom of the detail tab
                //and it also calls the stored procedure to make the updates to the national ads header row.
                myData.Add_SP_Parm(invoice_number, "@invoice_number");
                myData.Add_SP_Parm(total_amt, "@total_amt");
                myData.Add_SP_Parm(comm_amt, "@comm_amt");
                myData.Add_SP_Parm(net_amt, "@net_amt");
                myData.Add_SP_Parm(this.gs_user_id, "@user_id");
                myData.Add_SP_Parm("None", "@document_id");
                myData.Add_SP_Parm(vat_tax, "@vat_tax");



                if (!myData.NonQuerrySqlSp("usp_upd_natl_ads_inv_hdr"))
                {
                    ThrowErrorMessage("Error updating National Ads header amounts in stored procedure usp_upd_natl_ads_inv_hdr - {0}", myData.dberror);
                    return string.Empty;
                }
                else
                {// okay to update the detail records now if any changes were made to the detail row.


                    foreach (DataRow row in NationalAdsDetails.Rows)
                    {
                        bool IsRowModified = (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added);

                        if (IsRowModified)
                        {
                            foreach (DataColumn col in NationalAdsDetails.Columns)
                            {
                                if (row[col, DataRowVersion.Original].ToString() != row[col, DataRowVersion.Current].ToString())
                                {
                                    myData.Add_SP_Parm(row["invoice_number"], "@invoice_number");
                                    myData.Add_SP_Parm(row["seq_code"], "@seq_code");
                                    myData.Add_SP_Parm(row["air_date"], "@air_date");
                                    myData.Add_SP_Parm(row["agency_copy_code"], "@agency_copy_code");
                                    myData.Add_SP_Parm(row["ad_length"], "@ad_length");
                                    myData.Add_SP_Parm(row["ad_amt"], "@ad_amt");
                                    myData.Add_SP_Parm(row["ad_product"], "@ad_product");
                                    myData.Add_SP_Parm(row["record_source"], "@record_source");
                                    myData.Add_SP_Parm(col.ColumnName, "@column_changed");
                                    myData.Add_SP_Parm(row[col, DataRowVersion.Original].ToString(), "@original_value");
                                    myData.Add_SP_Parm(row[col, DataRowVersion.Current].ToString(), "@new_value");
                                    myData.Add_SP_Parm(this.gs_user_id, "@user_id");
                                    myData.Add_SP_Parm("None","@document_id");
                                    if (!myData.NonQuerrySqlSp("usp_upd_natl_ads_detail"))
                                    {
                                        ThrowErrorMessage("Error updating National Ads Detail in stored procedure usp_upd_natl_ads_detail - {0}", myData.dberror);
                                        return string.Empty;
                                    }

                                }
                            }
                        }
                       

                    }
                    is_adjustment_number = "Updated";
                    return is_adjustment_number;
                }


            }

            #endregion

            // This set of code deals with POSTED invoices that may need an adjustment depending on what they changed... 
            /*retrieve commission percentage from recv_account */

            #region Adjustment housekeeping

            #region Get Commssion rate
            string sSQL = string.Format("select a.agency_comm, isnull(a.comm_percent,0) * 100 as comm_percent, total_amt, order_type from recv_account a, natl_ads_inv_hdr b where a.receivable_account = b.receivable_account and b.invoice_number = '{0}'", invoice_number);
            if (myData.SqlStringPopDt(sSQL, true))
            {
                foreach (DataRow row in myData.GetDataTable.Rows)
                {
                    li_calc_comm = Convert.ToInt32(row["agency_comm"]);
                    ld_comm_percent = Convert.ToDecimal(row["comm_percent"]);
                    ld_comm_percent = ld_comm_percent/100;
                    ld_orig_total = Convert.ToDecimal(row["total_amt"]);
                    ls_order_type = row["order_type"].ToString();
                }
            }
            else
            {
                ThrowErrorMessage("ERROR pulling receivable account from recv_account - {0}", myData.dberror);
                return string.Empty;
            }

            #endregion
            iAdjustmentClass = 15;  //Standard National ads adjustment
            //sAdjustmentReason = "ADS ADJUSTMENT";            

            /* pull the accounting period */
            sSQL = string.Format("SELECT date_type.date_value FROM date_type WHERE date_type.date_type = 'ACCTPERIOD'");
            if (myData.SqlStringPopDt(sSQL, true))
            {
                foreach (DataRow row in myData.GetDataTable.Rows)
                {
                    ldt_gl_period = Convert.ToDateTime(row["date_value"]);
                }
            }
            else
            {
                ls_err_text = myData.dberror;
                ThrowErrorMessage("Error selecting from date_type!\r\n{0}", ls_err_text);
                return string.Empty;
            }

            ///* Determine the document id for the new adjustment */
            //lNextInvNum = null;
            //iLength = null;
            //myData.Add_SP_Parm("ADJUSTMENT", "@inv_type");
            //if (myData.SqlSpPopDt("usp_next_invoice_number", true))
            //{
            //    if (myData.GetDataTable != null && myData.GetDataTable.Rows.Count > 0)
            //    {
            //        is_adjustment_number = myData.GetDataTable.Rows[0]["document_id"].ToString();
            //        ls_document = is_adjustment_number;
            //    }
            //}

            #endregion
            //myData.BeginTransaction();


            #region Commission calculation

            //Commission flag will indicate as follows: N - no commission to recalc or zero; R - recalc; Z - zero out
            ls_comm_flag = "N";

            /* check to see if commission has changed.  If so, include in the adjustment */
            if (ib_comm_set_to_zero) //checkbox was checked to zero out total commission
            {
                ld_orig_comm = id_prev_comm;
                ld_new_comm = 0;
                ld_comm_amount = id_prev_comm * -1;
                
                if (id_prev_comm == ld_new_comm) //if original commission was 0, then no need to zero out - screen logic should prevent this condition from ever being true...
                {
                    lb_comm_chg = false;
                    ib_comm_set_to_zero = false;
                    ls_comm_flag = "N";
                    net_amt = ld_new + ld_orig_vat;
                }
                else //will be making a commission change to zero out the amount.
                {
                    lb_comm_chg = true;
                    lb_comm_increase = false;
                    comm_amt = ld_comm_amount * -1;
                    //need to make net net amount equal to the original total amount plus original vat tax
                    net_amt = ld_new  + ld_orig_vat;
                    ls_comm_flag = "Z";
                }
             

                
            }
            else //checkbox was NOT checked to zero out total commission
            {
                //Now need to see if we need to recalculate commission on this invoice
                if ((ib_calc_commission == false) && (comm_amt != 0)) //need to recalc commission checkbox was NOT checked.
                {
                    if (li_calc_comm == 0 && ld_amt_chgs != 0) //if agency_comm = 0 or no changes, no need to recalc
                    {
                        ld_orig_comm = comm_amt;
                        ls_comm_flag = "R";

                        if (ld_orig > ld_new) //ld_orig is total_amt of invoice before adjustment
                        {
                            ld_amount = ld_orig - ld_new; //ld_amount now reflects the change
                            lb_comm_increase = false;
                        }
                        else
                        {
                            ld_amount = ld_new - ld_orig;
                            lb_comm_increase = true;
                        }

                        /*calculate using commission percentage from recv_account if there is one, otherwise use 15 percent */
                        if (ld_comm_percent == 0)
                        {
                                                
                            
                             ld_comm_amount = System.Math.Round(ld_amount * .15m, 2,MidpointRounding.AwayFromZero);
                             ld_comm_rate = .15m;

                        }
                        else
                        {
                            ld_comm_amount = System.Math.Round(ld_amount * ld_comm_percent, 2,MidpointRounding.AwayFromZero);
                            ld_comm_rate = ld_comm_percent;
                        }


                        if (ld_orig_comm == 0) //new recalcualte the entire commission since the original commission was zero not just based on adjustment amount
                        {
                            ld_comm_amount = System.Math.Round(ld_new * ld_comm_rate, 2);
                        }
                        if (System.Math.Round(ld_amount, 2,MidpointRounding.AwayFromZero) != 0)
                        {
                            lb_comm_chg = true;

                            if (lb_comm_increase == true)
                            {
                                                           
                                //lb_comm_increase = true;
                                ld_new_comm = ld_orig_comm + ld_comm_amount;
                            }
                            else
                            {
                                //lb_comm_increase = false;
                                ld_new_comm = ld_orig_comm - ld_comm_amount;
                            }
                            
                        }
                        else
                        {
                            lb_comm_chg = false;
                            ld_new_comm = 0;
                            lb_comm_increase = false;
                            // RES Added because 0 $ adjustments were causing comm to be zeroed out
                            if (ld_amount == 0)
                            {
                                ld_new_comm = ld_orig_comm;
                            }
                        }
                       
                        comm_amt = ld_new_comm;
                        net_amt = ld_new - System.Math.Round(comm_amt, 2,MidpointRounding.AwayFromZero);
                    }
                    else //
                    {
                        lb_comm_chg = false;
                        net_amt = ld_new + ld_orig_comm;
                    }
                }
                else //checkbox was checked need to recalculate commission.
                {
                    lb_comm_chg = false;
                    //KSH - 8/28/12 this is putting TotalAmt in NetAmt
                    //net_amt = ld_new + ld_orig_comm; 
                    //add net amt to comm amt
                    net_amt += ld_orig_comm; 
                    ////////////////////////////////////////////////////////////////////////////////
                }
            }


            

            //if (lb_comm_chg) //Yes, there is going to be a commission change, let's log it in the table
            //{
            //    ls_invoice = invoice_number;
            //    long ll_seq = 0;

            //    ls_orig_comm = ld_orig_comm.ToString();
            //    ls_new_comm = ld_new_comm.ToString();
            //    /* insert a row to national ads adjustment table */
            //    sSQL = "INSERT INTO natl_ads_adjustment ( invoice_number, seq_code, column_changed, original_value, new_value, user_id, adj_date, adj_reason, document_id ) VALUES ( '{0}',  {1}, '{2}', {3}, {4}, '{5}', '{6}', '{7}',' {8}' )";
            //    sSQL = string.Format(sSQL, ls_invoice, ll_seq, "comm_amt", ls_orig_comm, ls_new_comm, gs_user_id, ldt_today, " ", ls_document);
            //    if (!myData.NonQuerrySql(sSQL))
            //    {
            //        ThrowErrorMessage("Error inserting commission into natl_ads_adjustment! {0}", myData.dberror);
            //        return string.Empty;
            //    }
            //}
            //else //no change noted no need to do anything exept reset the variable for the fun of it??? who knows...
            //{
            //    lb_comm_chg = false;
            //}


            #endregion

            #region VAT Housekeeping

            //VAT flag will indicate as follows: N - no vat to recalc or zero; R - recalc; Z - zero out; A - Add vat tax
            ls_vat_flag = "N";

            /* check to see if VAT has changed.  If so, include in the adjustment */
            if (ib_vat_set_to_zero) //checkbox was checked to zero out total commission
            {
                ld_orig_vat = vat_tax;
                ld_new_vat = 0;
                ld_vat_amount = vat_tax;

                if (ld_orig_vat == ld_new_vat) //if original vat was 0, then no need to zero out
                {
                    lb_vat_chg = false;
                    ib_vat_set_to_zero = false;
                    ls_vat_flag = "N";
                }
                else //will be making a change to zero out the amount.
                {
                    lb_vat_chg = true;
                    lb_vat_increase = false;
                    net_amt = net_amt - System.Math.Round(ld_vat_amount, 2,MidpointRounding.AwayFromZero);
                    ls_vat_flag = "Z";
                }
            }
            else //checkbox was NOT checked to zero out VAT tax
            {
                //Now need to see if we need to recalc VAT on this invoice

                //Does this invoice currently have a VAT amount, if so, yes, we need to Recalc if there is a  non zero adjustment

                if (vat_tax > 0 && gd_tot_adjusted != 0)
                {
                    ls_vat_flag = "R";
                    lb_vat_chg = true;
                  
                    if (ld_orig > ld_new) //ld_orig is total_amt of invoice before adjustment
                    {
                        lb_vat_increase = false;
                        ld_amount = ld_orig - ld_new; //ld_amount now reflects the change

                    }
                    else
                    {
                        lb_vat_increase = true;
                        ld_amount = ld_new - ld_orig; //ld_amount now reflects the change
                    }

                    ld_vat_amount = System.Math.Round(ld_amount * .15m, 2,MidpointRounding.AwayFromZero); //ld_vat_amount now has the VAT changed amount.
                    lb_vat_flag = true;

                    
                    if (lb_vat_increase)
                    {

                        ld_new_vat = vat_tax + ld_vat_amount;
                        net_amt = net_amt + System.Math.Round(ld_vat_amount, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        ld_new_vat = vat_tax - ld_vat_amount;
                        net_amt = net_amt - System.Math.Round(ld_vat_amount, 2, MidpointRounding.AwayFromZero);
                    }

                    

                }

                if (ib_add_vat) //Add vat tax to an invoice that does not have it already
                {
                    ls_vat_flag = "A";
                    lb_vat_chg = true;

                    lb_vat_increase = true;

                    ld_new_vat = System.Math.Round(ld_orig * .15m, 2, MidpointRounding.AwayFromZero); //ld_new_vat now has the new VAT amount.
                    lb_vat_flag = true;
                
                    net_amt = net_amt + System.Math.Round(ld_new_vat, 2, MidpointRounding.AwayFromZero); //Increase net amount
                  
                }

            }

            //if (lb_vat_chg) //Yes, there is going to be a vat change, let's log it in the table
            //{
            //    ls_invoice = invoice_number;
            //    long ll_seq = 0;

            //    ls_orig_vat = vat_tax.ToString();
            //    ls_new_vat = ld_new_vat.ToString();
            //    /* insert a row to national ads adjustment table */
            //    sSQL = "INSERT INTO natl_ads_adjustment ( invoice_number, seq_code, column_changed, original_value, new_value, user_id, adj_date, adj_reason, document_id ) VALUES ( '{0}',  {1}, '{2}', {3}, {4}, '{5}', '{6}', '{7}',' {8}' )";
            //    sSQL = string.Format(sSQL, ls_invoice, ll_seq, "vat_tax", ls_orig_vat, ls_new_vat, gs_user_id, ldt_today, " ", ls_document);
            //    if (!myData.NonQuerrySql(sSQL))
            //    {
            //        ThrowErrorMessage("Error inserting vat into natl_ads_adjustment! {0}", myData.dberror);
            //        return string.Empty;
            //    }
            //}
            //else //no change noted no need to do anything exept reset the variable for the fun of it??? who knows...
            //{
            //    lb_vat_chg = false;
            //}


            #endregion



            #region Begin Heavy Calculation Logic

            ld_adj_amount = gd_tot_adjusted;

            

            /* insert an adjustment to reflect amount changes to the posted invoice */
            if (ld_amt_chgs != 0 || lb_comm_chg || lb_vat_chg)
            {
                #region Calculate adjustment amounts
                if (ld_amt_chgs == 0 && ib_comm_set_to_zero) /* strictly zeroing out commission */
                {
                    ld_adj_amount = ld_comm_amount * -1;
                    ll_class = 0;
                }
                else
                {
                    if (ld_amt_chgs == 0 && (ib_vat_set_to_zero || ib_add_vat)) /* if zeroing or adding vat  */
                    {
                        if (ib_add_vat) //add new vat
                        {
                            ld_adj_amount = ld_new_vat;
                            ll_class = 0;
                        }
                        else //subtract old vat
                        {
                            ld_adj_amount = ld_vat_amount;
                            ll_class = 0;
                        }
                    }
                    else
                    {
                        if (ld_orig > ld_new) /* credit adjustment for the decreased amount */
                        {
                            ld_amount = ld_orig - ld_new;
                            ll_class = 0;
                        }
                        else /* debit adjustment for the increased amount */
                        {
                            ld_amount = ld_new - ld_orig;
                            ll_class = 1;
                        }
                        if (ld_comm_amount > ld_amount) //I don't think the commission would be larger than the new total amount
                        {
                            ld_adj_amount = ld_comm_amount - ld_amount;
                        }
                        else
                        {
                            /* clb - logic added 10/13/2003 for total amount to appear right */
                            if (ld_comm_amount < 0 && ld_amount > 0)
                            {
                                ld_adj_amount = ld_amount + ld_comm_amount;
                            }
                            else
                            {
                                ld_adj_amount = ld_amount - ld_comm_amount;
                            }
                        }
                        //Add VAT 
                        if (ld_vat_amount < 0 && ld_amount > 0)
                        {
                            ld_adj_amount = ld_adj_amount - ld_vat_amount;
                        }
                        else
                        {
                            ld_adj_amount = ld_adj_amount + ld_vat_amount;
                        }

                        gd_tot_adjusted = ld_adj_amount;
                    }
                }


                #endregion

                #region Account Code Housekeeping

                /* pull the invoice company, currency and receivable account */
                sSQL = string.Format("SELECT natl_ads_inv_hdr.receivable_account, natl_ads_inv_hdr.broadcast_month, natl_ads_inv_hdr.currency_code, natl_ads_inv_hdr.exchange_rate FROM natl_ads_inv_hdr WHERE  natl_ads_inv_hdr.invoice_number = '{0}'", is_natl_ads_invoice_nbr);
                if (myData.SqlStringPopDt(sSQL))
                {
                    foreach (DataRow row in myData.GetDataTable.Rows)
                    {
                        ls_racct = row["receivable_account"].ToString();
                        ldt_service_period = Convert.ToDateTime(row["broadcast_month"]);
                        ls_currency = row["currency_code"].ToString();
                        ll_xrate = Convert.ToInt64(row["exchange_rate"]);
                    }
                }
                else
                {
                    ThrowErrorMessage("Error selecting from natl_ads_inv_hdr!\r\n{0}", myData.dberror);
                    return string.Empty;
                }

                /* clb 08/2003 add the datastore with accounting info */

                if (myData.SqlSpPopDt("usp_sel_natl_ads_products_used"))
                {
                    ll_rows = Convert.ToInt64(myData.GetDataTable.Rows.Count);
                }

                if (ll_rows <= 0)
                {
                    ThrowErrorMessage("Error pulling from natl_ads_products_used table");
                    return string.Empty;
                }

                /* load the arrays with the information from the datastore */

                DataTable dtAccountingInfo = myData.GetDataTable;
                for (int ll_ds_ctr = 0; ll_ds_ctr < ll_rows; ll_ds_ctr++)
                {
                    DataRow row = dtAccountingInfo.Rows[ll_ds_ctr];
                    is_rev_co[ll_ds_ctr] = row["gl_co"].ToString();
                    is_rev_prod_acct[ll_ds_ctr] = row["revenue_acct"].ToString();
                    is_rev_center[ll_ds_ctr] = row["gl_center"].ToString();
                    //Team COA BLD TVG 6-6-2006
                    is_rev_geography[ll_ds_ctr] = row["geography"].ToString();
                    is_rev_interdivision[ll_ds_ctr] = row["interdivision"].ToString();
                    rev_gl_product[ll_ds_ctr] = row["gl_product"].ToString();
                    //END TEAM
                }

                if (myData.SqlStringPopDt("Select value from razer_ref where item_type = 'GEO'"))
                {
                    foreach (DataRow rowl_s_default_geography in myData.GetDataTable.Rows)
                    {
                        ls_default_geography = rowl_s_default_geography["value"].ToString();
                    }
                }

                //TVG2MSI RES 07/29/08
                if (myData.SqlStringPopDt("Select value from razer_ref where item_type = 'REGION'"))
                {
                    foreach (DataRow rowl_s_default_geography in myData.GetDataTable.Rows)
                    {
                        ls_default_region = rowl_s_default_geography["value"].ToString();
                    }
                }

                if (myData.SqlStringPopDt("Select value from razer_ref where item_type = 'PRODUCT'"))
                {
                    foreach (DataRow rowl_s_default_geography in myData.GetDataTable.Rows)
                    {
                        ls_product = rowl_s_default_geography["value"].ToString();
                    }
                }

                if (myData.SqlStringPopDt("Select value from razer_ref where item_type = 'DEPT'"))
                {
                    foreach (DataRow rowl_s_default_geography in myData.GetDataTable.Rows)
                    {
                        ls_dept = rowl_s_default_geography["value"].ToString();
                    }
                }

                /* go to the natl_ads_allocation table to find out the allocation pct */
                myData.Add_SP_Parm(ldt_service_period, "@broadcast_month");
                if (myData.SqlSpPopDt("usp_sel_natl_ads_allocation"))
                {
                    ll_rows = Convert.ToInt64(myData.GetDataTable.Rows.Count);
                }

                if (ll_rows <= 0)
                {
                    ThrowErrorMessage("No data retrieved for broadcast period in Allocation table");
                    return string.Empty;
                }

                ls_company = is_co;

                #endregion


                #region Insert Adjustment Header and Detail records
                /* insert the adjustment */
                //Modified 8/21/02 to pull in the adjustment class

                /* Determine the document id for the new adjustment */
                lNextInvNum = null;
                iLength = null;
                myData.Add_SP_Parm("ADJUSTMENT", "@inv_type");
                if (myData.SqlSpPopDt("usp_next_invoice_number", true))
                {
                    if (myData.GetDataTable != null && myData.GetDataTable.Rows.Count > 0)
                    {
                        is_adjustment_number = myData.GetDataTable.Rows[0]["document_id"].ToString();
                        ls_document = is_adjustment_number;
                    }
                }


                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append("INSERT into adjustment");
                sbSQL.AppendLine("(document_id, adjustment_type_id, adj_date, adj_reason, user_id, approved_by, approval_date, company_code,");
                sbSQL.AppendLine("currency_code, amount, status_flag, acct_period)");
                sbSQL.AppendLine("VALUES ('{0}', {1}, '{2}', '{3}', '{4}', ' ', '01/01/1900', '{5}', '{6}',");
                sbSQL.AppendLine("{7}, 0,'{8}')");

                sSQL = string.Format(sbSQL.ToString(), ls_document, iAdjustmentClass, ldt_today, sAdjustmentReason, gs_user_id, ls_company, ls_currency, ld_adj_amount, ldt_gl_period);

                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error inserting adjustment - {0}", myData.dberror);
                    return string.Empty;
                }

                /* insert to the adjustment_detail table  for the product header amount excluding commission*/
                //Need to add 1 line per detail product code that is being adjusted.
                //Need to use a temp table for adjustment_detail with an identity on the sequence code

                sbSQL = new StringBuilder("CREATE TABLE temp_adjustment_detail ( ");
                sbSQL.AppendLine("document_id varchar(22) null, line_id int IDENTITY(1,1) not null, detail_type char(8) null, product_code char(8) null, company_code char(2) null, receivable_account varchar(17) null,");
                sbSQL.AppendLine("amount money null, apply_to_doc varchar(22) null, apply_to_seq int null, currency_code char(3) null, exchange_rate float null, rebill_type int null)");

                sSQL = string.Format(sbSQL.ToString());
                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error creating temp table adjustment detail - {0}", myData.dberror);
                    return string.Empty;
                }

                //used to get product code and summarize the adustment amounts for use in the adjustment_detail table.

                sbSQL = new StringBuilder("CREATE TABLE temp_product_adjustment_detail ( ");
                sbSQL.AppendLine("product_code char(8) null, ");
                sbSQL.AppendLine("amount money null,)");

                sSQL = string.Format(sbSQL.ToString());
                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error creating temp table product adjustment detail - {0}", myData.dberror);
                    return string.Empty;
                }

                foreach (DataRow row in NationalAdsDetails.Rows)
                {
                    bool IsRowModified = (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added);
                    decimal ld_adj_amt;
                    decimal ld_orig_value;
                    decimal ld_curr_value;
                    string ls_product_code;



                    if (IsRowModified)
                    {
                        foreach (DataColumn col in NationalAdsDetails.Columns)
                        {
                            if (col.ColumnName.ToString() == "ad_amt")
                            {

                                if (row[col, DataRowVersion.Original].ToString() != row[col, DataRowVersion.Current].ToString())
                                {

                                    ld_orig_value = Convert.ToDecimal(row[col, DataRowVersion.Original].ToString());
                                    ld_curr_value = Convert.ToDecimal(row[col, DataRowVersion.Current].ToString());
                                    ls_product_code = row["product_code"].ToString();
                                    ld_adj_amt = ld_orig_value - ld_curr_value;

                                    sbSQL = new StringBuilder("INSERT into temp_product_adjustment_detail");
                                    sbSQL.AppendLine("(product_code, amount )");
                                    sbSQL.AppendLine("VALUES('{0}',   '{1}' )");

                                    sSQL = string.Format(sbSQL.ToString(), ls_product_code, ld_adj_amt);
                                    if (!myData.NonQuerrySql(sSQL))
                                    {
                                        ThrowErrorMessage("Error inserting product adjustment detail ", myData.dberror);
                                        return string.Empty;
                                    }
                                }
                            }

                        }

                    }
                }






                if (ld_amt_chgs != 0)
                {
                    if (ll_class == 0)
                    {
                        ld_amount = ld_amount * -1;
                    }

                    //Need to get the adjustment amounts summed by product code





                    sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                    sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                    sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                    sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                    sbSQL.AppendLine("sum(amount) * -1,  '" + is_natl_ads_invoice_nbr + "', 1, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                    sbSQL.AppendLine("from temp_product_adjustment_detail group by product_code");

                    sSQL = string.Format(sbSQL.ToString());
                    if (!myData.NonQuerrySql(sSQL))
                    {
                        ThrowErrorMessage("Error inserting adjustment detail - {0}", myData.dberror);
                        return string.Empty;
                    }
                }

                /* if commission changed insert to the adjustment_detail table */
                if (lb_comm_chg)
                {
                    //if (lb_comm_increase) No longer needed since we putting commission by product into detail table.
                    //{
                    //    ld_comm_detail_amount = ld_comm_amount * -1;
                    //}
                    //else
                    //{
                    //    ld_comm_detail_amount = ld_comm_amount;
                    //}

                    if (ib_comm_set_to_zero != true)
                    {

                        sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                        sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                        sbSQL.AppendLine("round((sum(amount) * " + ld_comm_rate.ToString() + "),2),  '" + is_natl_ads_invoice_nbr + "', -1, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                        sbSQL.AppendLine("from temp_product_adjustment_detail group by product_code");
                        //sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        //sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                        //sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        //sbSQL.AppendLine("VALUES('{0}',  'NINVOICE', '{1}', '{2}', '{3}',");
                        //sbSQL.AppendLine("{4}, '{5}', -1, '{6}', {7}, 0)");

                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error inserting commission adjustment detail ", myData.dberror);
                            return string.Empty;
                        }
                    }
                    else //Zero out commission - no product detail changed, need to get commission amounts from natl_ads_acct_detail table and sum 
                    {
                        //used to get product code and summarize the adjustment amounts for use in the adjustment_detail table.

                        sbSQL = new StringBuilder("CREATE TABLE temp_product_commission_adjustment_detail ( ");
                        sbSQL.AppendLine("seq_code int null, ");
                        sbSQL.AppendLine("product_code char(8) null, ");
                        sbSQL.AppendLine("amount money null)");

                        sSQL = string.Format(sbSQL.ToString());
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error creating temp table  product commission adjustment detail - {0}", myData.dberror);
                            return string.Empty;
                        }

                        //insert amounts from natl_ads_acct_detail table

                        sbSQL = new StringBuilder("INSERT into temp_product_commission_adjustment_detail");
                        sbSQL.AppendLine("(seq_code, product_code,  amount) ");
                        sbSQL.AppendLine("SELECT seq_code, product_code, amount from natl_ads_inv_acct_detail where invoice_number = '" + is_natl_ads_invoice_nbr + "' and ");
                        sbSQL.AppendLine("type_id = 1 and amount <> 0 ");
                    

                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error inserting temp product commission adj detail", myData.dberror);
                            return string.Empty;
                        }

                        //Set product code to the one used on the natl_ads_inv_detail record

                        sbSQL = new StringBuilder("UPDATE a SET a.product_code = b.product_code from temp_product_commission_adjustment_detail a, natl_ads_inv_detail b ");
                        sbSQL.AppendLine("WHERE b.invoice_number = '" + is_natl_ads_invoice_nbr + "' and a.seq_code = b.seq_code ");
                        
                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error updating product code on temp product commission adj detail", myData.dberror);
                            return string.Empty;
                        }


                        //insert summed into temp product adjustment detail

                        sbSQL = new StringBuilder("INSERT into temp_product_adjustment_detail");
                        sbSQL.AppendLine("(product_code,  amount) ");
                        sbSQL.AppendLine("SELECT product_code, sum(amount) from temp_product_commission_adjustment_detail group by product_code ");
                  


                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error inserting temp product adj detail", myData.dberror);
                            return string.Empty;
                        }
                        
                        
                        sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                        sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                        sbSQL.AppendLine("amount, '"+ is_natl_ads_invoice_nbr + "', -1, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                        sbSQL.AppendLine("from temp_product_adjustment_detail");
                        //sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        //sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                        //sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        //sbSQL.AppendLine("VALUES('{0}',  'NINVOICE', '{1}', '{2}', '{3}',");
                        //sbSQL.AppendLine("{4}, '{5}', -1, '{6}', {7}, 0)");

                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error inserting zerocommission into adjustment detail - {0}", myData.dberror);
                            return string.Empty;
                        }


                        sbSQL = new StringBuilder("DROP TABLE temp_product_commission_adjustment_detail ");



                        sSQL = string.Format(sbSQL.ToString());
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error dropping table temp product commissions adjustment detail", myData.dberror);
                            return string.Empty;
                        }




                    }
                }

                if (lb_vat_chg)
                {
                    //if (lb_vat_increase)
                    //{
                    //    ld_vat_detail_amount = ld_vat_amount;
                    //}
                    //else
                    //{
                    //    ld_vat_detail_amount = ld_vat_amount * -1;
                    //}

                    if (ib_vat_set_to_zero != true && ib_add_vat != true) //Vat is changing because the detail was adjusted
                    {

                        sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                        sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                        sbSQL.AppendLine("round((sum(amount) * .15 " +  "),2) * -1,  '" + is_natl_ads_invoice_nbr + "', -2, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                        sbSQL.AppendLine("from temp_product_adjustment_detail group by product_code");
                        

                        sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        if (!myData.NonQuerrySql(sSQL))
                        {
                            ThrowErrorMessage("Error inserting vat adjustment detail ", myData.dberror);
                            return string.Empty;
                        }

                        //sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                        //sbSQL.AppendLine("(document_id, detail_type, product_code, company_code, receivable_account,");
                        //sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                        //sbSQL.AppendLine("VALUES('{0}',  'NINVOICE', '{1}', '{2}', '{3}',");
                        //sbSQL.AppendLine("{4}, '{5}', -2, '{6}', {7}, 0)");

                        //sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_vat_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                        //if (!myData.NonQuerrySql(sSQL))
                        //{
                        //    ThrowErrorMessage("Error inserting adjustment detail - {0}", myData.dberror);
                        //    return string.Empty;
                        //}


                    }
                    else //Zero out VAT or Add Vat- no product detail changed, need to get VAT amounts from natl_ads_acct_detail table and sum 
                    {



                        if (ib_vat_set_to_zero)
                        {

                            //used to get product code and summarize the adjustment amounts for use in the adjustment_detail table.

                            sbSQL = new StringBuilder("CREATE TABLE temp_product_vat_adjustment_detail ( ");
                            sbSQL.AppendLine("seq_code int null, ");
                            sbSQL.AppendLine("product_code char(8) null, ");
                            sbSQL.AppendLine("amount money null)");

                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error creating temp table  product vat adjustment detail - {0}", myData.dberror);
                                return string.Empty;
                            }

                            //insert amounts from natl_ads_acct_detail table

                            sbSQL = new StringBuilder("INSERT into temp_product_vat_adjustment_detail");
                            sbSQL.AppendLine("(seq_code, product_code,  amount) ");
                            sbSQL.AppendLine("SELECT seq_code, product_code, amount from natl_ads_inv_acct_detail where invoice_number = '" + is_natl_ads_invoice_nbr + "' and ");
                            sbSQL.AppendLine("type_id = 2 and amount <> 0 ");


                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting temp product vat adj detail", myData.dberror);
                                return string.Empty;
                            }

                            //Set product code to the one used on the natl_ads_inv_detail record

                            sbSQL = new StringBuilder("UPDATE a SET a.product_code = b.product_code from temp_product_vat_adjustment_detail a, natl_ads_inv_detail b ");
                            sbSQL.AppendLine("WHERE b.invoice_number = '" + is_natl_ads_invoice_nbr + "' and a.seq_code = b.seq_code ");

                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error updating product code on temp product vat adj detail", myData.dberror);
                                return string.Empty;
                            }


                            //insert summed into temp product adjustment detail

                            sbSQL = new StringBuilder("INSERT into temp_product_adjustment_detail");
                            sbSQL.AppendLine("(product_code,  amount) ");
                            sbSQL.AppendLine("SELECT product_code, sum(amount) from temp_product_vat_adjustment_detail group by product_code ");



                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting temp product adj detail for vat ", myData.dberror);
                                return string.Empty;
                            }


                            sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                            sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                            sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                            sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                            sbSQL.AppendLine("amount * -1, '" + is_natl_ads_invoice_nbr + "', -2, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                            sbSQL.AppendLine("from temp_product_adjustment_detail");
                            //sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                            //sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                            //sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                            //sbSQL.AppendLine("VALUES('{0}',  'NINVOICE', '{1}', '{2}', '{3}',");
                            //sbSQL.AppendLine("{4}, '{5}', -1, '{6}', {7}, 0)");

                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting zero vat into adjustment detail - {0}", myData.dberror);
                                return string.Empty;
                            }


                            sbSQL = new StringBuilder("DROP TABLE temp_product_vat_adjustment_detail ");



                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error dropping table temp product vat adjustment detail", myData.dberror);
                                return string.Empty;
                            }

                        }
                        else //Add Vat Tax
                        {
                            //Need to add records to natl_ads_inv_acct_detail table for new VAT tax.
                            //Will use existing product type rows and update amounts, type_id and account codes
                            //Will then use these amounts to create the adjustmnet detail records and then set amounts to zero as adjustment posting
                            //should update the natl_ads_inv_acct_detail records with the correct adjusted amounts.
                            //We are adding a new VAT tax so the amount will go from zero to the new amount.

                            //First get the VAT Tax account code to be used.
                            string vSQL;
                            string VAT;
                            VAT = "";
                     
                            vSQL = "select value from razer_ref where item_type = 'VAT_TAX'";
                            if (!myData.SqlStringPopDt(vSQL))
                            {
                                ThrowErrorMessage("Error getting VAT tax account from razer_ref ",myData.dberror);
                                return string.Empty;
                            }
                           

                            if (myData.GetDataTable != null && myData.GetDataTable.Rows.Count > 0)
                            {
                                 VAT = myData.GetDataTable.Rows[0]["value"].ToString();
                     
                            }

                            //used to get product code and summarize the adjustment amounts for use in the adjustment_detail table.

                            sbSQL = new StringBuilder("CREATE TABLE temp_natl_ads_inv_acct_detail ( ");
                            sbSQL.AppendLine("invoice_number varchar(22) null, ");
                            sbSQL.AppendLine("seq_code int null, ");
                            sbSQL.AppendLine("product_code char(8) null, ");
                            sbSQL.AppendLine("amount money null, ");
                            sbSQL.AppendLine("acct_code varchar(21) null, ");
                            sbSQL.AppendLine("def_acct_code varchar(21) null, ");
                            sbSQL.AppendLine("type_id int null, ");
                            sbSQL.AppendLine("total_vat money null, ");
                            sbSQL.AppendLine("vat money null, ");
                            sbSQL.AppendLine("total_invoice money null )");


                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error creating temp table  natl_ads_inv_acct_detail - {0}", myData.dberror);
                                return string.Empty;
                            }

                            
                            //Insert new natl_ads_inv_acct_deatil records with the new account code and  type id
                            sbSQL = new StringBuilder("INSERT temp_natl_ads_inv_acct_detail ");
                            sbSQL.AppendLine("SELECT invoice_number,seq_code,product_code,amount, substring(acct_code, 1,6) + '" + VAT + "' + '0000000000',  def_acct_code, 2, " + ld_new_vat + ", 0,  0 ");
                            sbSQL.AppendLine("from natl_ads_inv_acct_detail where invoice_number = '" + is_natl_ads_invoice_nbr + "' and ");
                            sbSQL.AppendLine("type_id = 0 and amount <> 0 ");

                                                   

                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting new vat records into temp natl_ads_inv_acct_detail - {0}", myData.dberror);
                                return string.Empty;
                            }

                            //Update new natl_ads_inv_acct_deatil records with the total invoice amount
                            sbSQL = new StringBuilder("UPDATE temp_natl_ads_inv_acct_detail ");
                            sbSQL.AppendLine("SET total_invoice = (select sum(amount) from temp_natl_ads_inv_acct_detail) ");



                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error updating vat amount on temp_natl_ads_inv_acct_detail - {0}", myData.dberror);
                                return string.Empty;
                            }

                            //Update new natl_ads_inv_acct_deatil records with the vat tax amount
                            sbSQL = new StringBuilder("UPDATE temp_natl_ads_inv_acct_detail ");
                            sbSQL.AppendLine("SET vat = round(amount * .15, 2) ");



                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error updating vat amount on temp_natl_ads_inv_acct_detail - {0}", myData.dberror);
                                return string.Empty;
                            }






                            //Check for rounding issues and make necessary adjustment of the last vat tax record.


                            //Update new natl_ads_inv_acct_deatil records with the vat tax amount
                            sbSQL = new StringBuilder("UPDATE temp_natl_ads_inv_acct_detail ");
                            sbSQL.AppendLine("SET vat = vat + (total_vat - (select SUM(vat) from temp_natl_ads_inv_acct_detail)) ");
                            sbSQL.AppendLine("where seq_code = (select MAX(seq_code) from temp_natl_ads_inv_acct_detail)");




                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error updating rounding on vat amount on temp_natl_ads_inv_acct_detail - {0}", myData.dberror);
                                return string.Empty;
                            }





                            sbSQL = new StringBuilder("CREATE TABLE temp_product_vat_adjustment_detail ( ");
                            sbSQL.AppendLine("seq_code int null, ");
                            sbSQL.AppendLine("product_code char(8) null, ");
                            sbSQL.AppendLine("amount money null)");

                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error creating temp table  product vat adjustment detail - {0}", myData.dberror);
                                return string.Empty;
                            }

                            //insert amounts from natl_ads_acct_detail table

                            sbSQL = new StringBuilder("INSERT into temp_product_vat_adjustment_detail");
                            sbSQL.AppendLine("(seq_code, product_code,  amount) ");
                            sbSQL.AppendLine("SELECT seq_code, product_code, vat from temp_natl_ads_inv_acct_detail where invoice_number = '" + is_natl_ads_invoice_nbr + "' and ");
                            sbSQL.AppendLine("type_id = 2 and vat <> 0 ");


                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting temp product vat adj detail", myData.dberror);
                                return string.Empty;
                            }

                            //Set product code to the one used on the natl_ads_inv_detail record

                            sbSQL = new StringBuilder("UPDATE a SET a.product_code = b.product_code from temp_product_vat_adjustment_detail a, natl_ads_inv_detail b ");
                            sbSQL.AppendLine("WHERE b.invoice_number = '" + is_natl_ads_invoice_nbr + "' and a.seq_code = b.seq_code ");

                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error updating product code on temp product vat adj detail", myData.dberror);
                                return string.Empty;
                            }


                            //insert summed into temp product adjustment detail

                            sbSQL = new StringBuilder("INSERT into temp_product_adjustment_detail");
                            sbSQL.AppendLine("(product_code,  amount) ");
                            sbSQL.AppendLine("SELECT product_code, sum(amount) from temp_product_vat_adjustment_detail group by product_code ");



                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting temp product adj detail for vat ", myData.dberror);
                                return string.Empty;
                            }


                            sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                            sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                            sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                            sbSQL.AppendLine("SELECT '" + ls_document + "', 'NINVOICE', product_code, '" + ls_company + "', '" + ls_racct + "', ");
                            sbSQL.AppendLine("amount , '" + is_natl_ads_invoice_nbr + "', -2, '" + ls_currency + "', " + Convert.ToString(ll_xrate) + ",0 ");
                            sbSQL.AppendLine("from temp_product_adjustment_detail");
                            //sbSQL = new StringBuilder("INSERT into temp_adjustment_detail");
                            //sbSQL.AppendLine("(document_id,  detail_type, product_code, company_code, receivable_account,");
                            //sbSQL.AppendLine("amount, apply_to_doc, apply_to_seq, currency_code, exchange_rate, rebill_type)");
                            //sbSQL.AppendLine("VALUES('{0}',  'NINVOICE', '{1}', '{2}', '{3}',");
                            //sbSQL.AppendLine("{4}, '{5}', -1, '{6}', {7}, 0)");

                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error inserting zero vat into adjustment detail - {0}", myData.dberror);
                                return string.Empty;
                            }


                            sbSQL = new StringBuilder("DROP TABLE temp_product_vat_adjustment_detail ");



                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error dropping table temp product vat adjustment detail", myData.dberror);
                                return string.Empty;
                            }


                       

                            //Insert VAT rcrods with 0 amount into natl_ads_inv_acct_deatil  
                            sbSQL = new StringBuilder("INSERT natl_ads_inv_acct_detail ");
                            sbSQL.AppendLine("SELECT invoice_number, seq_code, product_code, vat,  acct_code, def_acct_code, type_id ");
                            sbSQL.AppendLine("FROM temp_natl_ads_inv_acct_detail ");



                            sSQL = string.Format(sbSQL.ToString(), ls_document, gs_product, ls_company, ls_racct, ld_comm_detail_amount, is_natl_ads_invoice_nbr, ls_currency, ll_xrate);
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error insert natl_ads_inv_acct_detail from vat temp_natl_ads- {0}", myData.dberror);
                                return string.Empty;
                            }


                            sbSQL = new StringBuilder("DROP TABLE temp_natl_ads_inv_acct_detail ");

                            
                            sSQL = string.Format(sbSQL.ToString());
                            if (!myData.NonQuerrySql(sSQL))
                            {
                                ThrowErrorMessage("Error dropping table temp_natl_ads_inv_acct_detail", myData.dberror);
                                return string.Empty;
                            }                   







                        }


                    }






                   
                }


                //Insert from temp table into adjustment_detail table

                sbSQL = new StringBuilder("INSERT into adjustment_detail ");
                sbSQL.AppendLine("select * from temp_adjustment_detail ");


                sSQL = string.Format(sbSQL.ToString());
                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error inserting into adjustment detail", myData.dberror);
                    return string.Empty;
                }


                sbSQL = new StringBuilder("DROP TABLE temp_adjustment_detail ");



                sSQL = string.Format(sbSQL.ToString());
                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error dropping table temp adjustment detail", myData.dberror);
                    return string.Empty;
                }




                sbSQL = new StringBuilder("DROP TABLE temp_product_adjustment_detail ");



                sSQL = string.Format(sbSQL.ToString());
                if (!myData.NonQuerrySql(sSQL))
                {
                    ThrowErrorMessage("Error dropping table temp product adjustment detail", myData.dberror);
                    return string.Empty;
                }





                #endregion

                if (ld_amt_chgs != 0) //this means there was a dollar amount change on the invoice
                {
                    /* insert the ar to the adjustment_acct_u table */

                    ls_geography = is_geography;
                    ls_bal_center = is_bal_center;
                    ls_future_acct1 = gl_product;

                    int iOut = 0;
                    if (is_ar_acct.Length >= 1 && int.TryParse(is_ar_acct.Substring(0, 1), out iOut) && iOut < 4)// replicates PB script "if left(is_ar_acct,1) < '4' then"
                    {
                        //TVG2MSI RES 07/29/08
                        ls_geography_value = ls_default_region;
                        ls_bal_center = ls_dept;
                        ls_future_acct1 = ls_product;
                    }
                    else
                    {
                        ls_geography_value = ls_geography;
                    }

                    //072612 tas modified for new table layout
                    //insert for each detail row that has a change
                    //need to loop through modified detail rows and insert a row for each modified amount detail row and each product
                    //can have more than 1 product per detail row so need to join to natl_ads_inv_acct_detail to get products, accounts and 
                    //and calculcate % that can be used to allocate the amount of the adjustment among the acct detail lines by product                    

                    foreach (DataRow NationalAdsModifiedRow in NationalAdsDetails.GetChanges(DataRowState.Modified).Rows)
                    {
                        int ll_seq = NationalAdsDetails.Rows.IndexOf(NationalAdsModifiedRow);
                        ll_seq_code = Convert.ToInt64(NationalAdsModifiedRow["seq_code", DataRowVersion.Current]);
                        ld_original_amount = Convert.ToDecimal(NationalAdsModifiedRow["ad_amt", DataRowVersion.Original]);
                        ld_amount = Convert.ToDecimal(NationalAdsModifiedRow["ad_amt", DataRowVersion.Current]);




                        if (ld_original_amount != ld_amount) //means there was an amount change on this detail row and need to insert otherwise, bypass
                        {
                            //get amount of change
                            ld_amount = (Convert.ToDecimal(NationalAdsModifiedRow["ad_amt", DataRowVersion.Current]) - Convert.ToDecimal(NationalAdsModifiedRow["ad_amt", DataRowVersion.Original]));

                            if (ls_comm_flag == "Z") //Don't want to zero commission here so... set it to a crazy character like 'C' for crazy..
                            {
                                ls_comm_flag = "C";
                            }


                            //That is inserting the adj acct and adj acct detail for each detail change we are looping through.
                            myData.Add_SP_Parm(is_natl_ads_invoice_nbr, "@apply_to_doc");
                            myData.Add_SP_Parm(ll_seq_code, "@apply_to_seq");
                            myData.Add_SP_Parm(ls_document, "@adj_document_id");
                            myData.Add_SP_Parm(ld_amount, "@amount_adjusted");
                            myData.Add_SP_Parm(ls_company, "@company_code");
                            myData.Add_SP_Parm(ls_bal_center, "@gl_center");
                            myData.Add_SP_Parm(is_ar_acct, "@gl_acct");
                            myData.Add_SP_Parm(ls_geography_value, "@geography");
                            myData.Add_SP_Parm(is_interdivision, "@interdivision");
                            myData.Add_SP_Parm(ls_future_acct1, "@gl_product");
                            myData.Add_SP_Parm(ls_comm_flag, "@comm_flag");
                            myData.Add_SP_Parm(ld_comm_rate, "@comm_rate");
                            myData.Add_SP_Parm("R", "@vat_flag");  //Changed to R from N to recalc at detail product level 092512 tas


                            if (!myData.NonQuerrySqlSp("usp_ins_adj_natl_ads_credit_debit_acct"))
                            {
                                ThrowErrorMessage("Error inserting National Ads Detail in stored procedure usp_ins_adj_natl_ads_credit_debit_acct", myData.dberror);
                                return string.Empty;
                            }

                        }


                        if (ls_comm_flag == "C") //Don't want to zero commission here so... BUT we need to change the 'C' back to a Z
                        {
                            ls_comm_flag = "Z";
                        }



                        #region Commission acct acct detail moved to usp_ins_adj_natl_ads_credit_debit_acct

                        //if (lb_comm_chg)
                        //{
                        //    if (lb_comm_increase)
                        //    {
                        //        ld_comm_detail_amount = ld_comm_amount * -1;
                        //    }
                        //    else
                        //    {
                        //        ld_comm_detail_amount = ld_comm_amount;
                        //    }

                        //    //Need to grab a new adj_acct_id from previous insert for next insert
                        //    if (myData.SqlStringPopDt("select max(adj_acct_id) + 1 from adjustment_acct"))
                        //    {
                        //        foreach (DataRow row in myData.GetDataTable.Rows)
                        //        {
                        //            ll_adj_acct_id = Convert.ToInt64(row["value"].ToString());
                        //        }
                        //    }

                        //    sbSQL = new StringBuilder("INSERT into adjustment_acct");
                        //    sbSQL.AppendLine("(adj_acct_id,document_id, line_id, description,");
                        //    sbSQL.AppendLine("trx_line_id, amount_change, rebill_type)");
                        //    sbSQL.AppendLine("VALUES({0}, '{1}', 2, 'ADS ADJUSTMENT',");
                        //    sbSQL.AppendLine("0, '{2}', 0)");

                        //    sSQL = string.Format(sbSQL.ToString(), ll_adj_acct_id, ls_document, ld_comm_detail_amount);

                        //    if (!myData.NonQuerrySql(sSQL))
                        //    {
                        //        ThrowErrorMessage("Error inserting adjustment accounting - {0}", myData.dberror);
                        //        return string.Empty;
                        //    }


                        //    //TEAM COA BLD TVG 6-6-2006
                        //    //TVG2MSI CLB Changed is_future_acct1 to ls_future_acct1
                        //    sbSQL = new StringBuilder("INSERT into adjustment_acct_detail");
                        //    sbSQL.AppendLine("(document_id, line_id, seq_id, gl_co, gl_acct, gl_center, description,");
                        //    sbSQL.AppendLine("inv_acct_detail, gl_period, service_period, amount_change, unit_change, new_units,");
                        //    sbSQL.AppendLine("geography, interdivision, gl_product)");
                        //    sbSQL.AppendLine("VALUES('{0}', 2, 1, '{1}', '{2}', '{3}', 'ADS ADJUSTMENT',");
                        //    sbSQL.AppendLine("0, '{4}', '{5}', {6}, 0, 0, '{7}', '{8}', '{9}')");

                        //    sSQL = string.Format(sbSQL.ToString(), ls_document, is_co, is_ar_acct, is_bal_center, ldt_gl_period, ldt_service_period, ld_comm_detail_amount,
                        //        ls_geography_value, is_interdivision, ls_future_acct1);

                        //    if (!myData.NonQuerrySql(sSQL))
                        //    {
                        //        ThrowErrorMessage("Error inserting adjustment accounting - {0}", myData.dberror);
                        //        return string.Empty;
                        //    }
                        //    ib_adjustment_created = true;


                        //}

                        #endregion


                    }/* end to For that inserts detail adjustments to the adjustment_acct_u table */



                }


                /* insert commission adjustment for zero out commission (if one occurred) to the adjustment_acct and adjustment_acct_detail table */
                if (lb_comm_chg)
                {
                    if (ib_comm_set_to_zero)
                    {
                        comm_amt = 0;
                        //net_amt = total_amt;      nat_amt getting set up above near line                 

                        ls_geography = is_geography;
                        ls_bal_center = is_bal_center;
                        ls_future_acct1 = gl_product;
                        //That is inserting the adj acct and adj acct detail for each detail change we are looping through.
                        myData.Add_SP_Parm(is_natl_ads_invoice_nbr, "@apply_to_doc");
                        myData.Add_SP_Parm(0, "@apply_to_seq");
                        myData.Add_SP_Parm(ls_document, "@adj_document_id");
                        myData.Add_SP_Parm(id_prev_comm, "@amount_adjusted"); //this will be used for the total commission to be adjusted
                        myData.Add_SP_Parm(ls_company, "@company_code");
                        myData.Add_SP_Parm(ls_bal_center, "@gl_center");
                        myData.Add_SP_Parm(is_ar_acct, "@gl_acct");
                        myData.Add_SP_Parm(ls_geography, "@geography");
                        myData.Add_SP_Parm(is_interdivision, "@interdivision");
                        myData.Add_SP_Parm(ls_future_acct1, "@gl_product");
                        myData.Add_SP_Parm(ls_comm_flag, "@comm_flag");
                        myData.Add_SP_Parm(ld_comm_rate, "@comm_rate");
                        myData.Add_SP_Parm("N", "@vat_flag");


                        if (!myData.NonQuerrySqlSp("usp_ins_adj_natl_ads_credit_debit_acct"))
                        {
                            ThrowErrorMessage("Error inserting National Ads Detail in stored procedure usp_ins_adj_natl_ads_credit_debit_acct", myData.dberror);
                            return string.Empty;
                        }
                        ib_adjustment_created = true;
                    }



                    ib_adjustment_created = true;
                }

                if (lb_vat_chg)
                {

                    ls_geography = is_geography;
                    ls_bal_center = is_bal_center;
                    ls_future_acct1 = gl_product;

                    if (ib_vat_set_to_zero)
                    {
                        vat_tax = 0;

                        //That is inserting the adj acct and adj acct detail for zeroing out vat
                        myData.Add_SP_Parm(is_natl_ads_invoice_nbr, "@apply_to_doc");
                        myData.Add_SP_Parm(0, "@apply_to_seq");
                        myData.Add_SP_Parm(ls_document, "@adj_document_id");
                        myData.Add_SP_Parm(ld_orig_vat, "@amount_adjusted"); //this will be used for the total vat to be adjusted
                        myData.Add_SP_Parm(ls_company, "@company_code");
                        myData.Add_SP_Parm(ls_bal_center, "@gl_center");
                        myData.Add_SP_Parm(is_ar_acct, "@gl_acct");
                        myData.Add_SP_Parm(ls_geography, "@geography");
                        myData.Add_SP_Parm(is_interdivision, "@interdivision");
                        myData.Add_SP_Parm(ls_future_acct1, "@gl_product");
                        myData.Add_SP_Parm("N", "@comm_flag");
                        myData.Add_SP_Parm(ld_comm_rate, "@comm_rate");
                        myData.Add_SP_Parm("Z", "@vat_flag");



                        if (!myData.NonQuerrySqlSp("usp_ins_adj_natl_ads_credit_debit_acct"))
                        {
                            ThrowErrorMessage("Error inserting National Ads Detail in stored procedure usp_ins_adj_natl_ads_credit_debit_acct", myData.dberror);
                            return string.Empty;
                        }
                    }
                    //tas 092512 commented out because vat recalc is now at product level moved to previous section along with product and commission logic for recalc.
                    else //Just need to use the VAT adjusted amount and send in the R
                        //Added Logic for adding VAT tax only
                    {
                        //That is inserting the adj acct and adj acct detail 
                        vat_tax = ld_new_vat;

                        if (ib_add_vat)
                        {
                            ld_vat_amount = vat_tax;


                            myData.Add_SP_Parm(is_natl_ads_invoice_nbr, "@apply_to_doc");
                            myData.Add_SP_Parm(0, "@apply_to_seq");
                            myData.Add_SP_Parm(ls_document, "@adj_document_id");
                            myData.Add_SP_Parm(ld_vat_amount, "@amount_adjusted"); //this will be the amount to be adjusted
                            myData.Add_SP_Parm(ls_company, "@company_code");
                            myData.Add_SP_Parm(ls_bal_center, "@gl_center");
                            myData.Add_SP_Parm(is_ar_acct, "@gl_acct");
                            myData.Add_SP_Parm(ls_geography, "@geography");
                            myData.Add_SP_Parm(is_interdivision, "@interdivision");
                            myData.Add_SP_Parm(ls_future_acct1, "@gl_product");
                            myData.Add_SP_Parm("N", "@comm_flag");
                            myData.Add_SP_Parm(ld_comm_rate, "@comm_rate");
                            myData.Add_SP_Parm("A", "@vat_flag");


                            if (!myData.NonQuerrySqlSp("usp_ins_adj_natl_ads_credit_debit_acct"))
                            {
                                ThrowErrorMessage("Error inserting National Ads Detail in stored procedure usp_ins_adj_natl_ads_credit_debit_acct", myData.dberror);
                                return string.Empty;
                            }
                        
                        
                        
                        
                        }







                    }
                    //    if (lb_vat_increase)
                    //    {
                    //        ld_vat_amount = (ld_vat_amount * -1);
                    //    }

                    //    myData.Add_SP_Parm(is_natl_ads_invoice_nbr, "@apply_to_doc");
                    //    myData.Add_SP_Parm(0, "@apply_to_seq");
                    //    myData.Add_SP_Parm(ls_document, "@adj_document_id");
                    //    myData.Add_SP_Parm(ld_vat_amount, "@amount_adjusted"); //this will be the amount to be adjusted
                    //    myData.Add_SP_Parm(ls_company, "@company_code");
                    //    myData.Add_SP_Parm(ls_bal_center, "@gl_center");
                    //    myData.Add_SP_Parm(is_ar_acct, "@gl_acct");
                    //    myData.Add_SP_Parm(ls_geography, "@geography");
                    //    myData.Add_SP_Parm(is_interdivision, "@interdivision");
                    //    myData.Add_SP_Parm(ls_future_acct1, "@gl_product");
                    //    myData.Add_SP_Parm("N", "@comm_flag");
                    //    myData.Add_SP_Parm(ld_comm_rate, "@comm_rate");
                    //    myData.Add_SP_Parm(ls_vat_flag, "@vat_flag");


                    //    if (!myData.NonQuerrySqlSp("usp_ins_adj_natl_ads_credit_debit_acct"))
                    //    {
                    //        ThrowErrorMessage("Error inserting National Ads Detail in stored procedure usp_ins_adj_natl_ads_credit_debit_acct", myData.dberror);
                    //        return string.Empty;
                    //    }
                    //}

                    ib_adjustment_created = true;
                }





                ib_adjustment_created = true;

                //That is updating the totals from the header that are at the bottom of the detail tab and it also calls the stored procedure to make the updates to the national ads header row.
                //That is updating the totals from the header that are at the bottom of the detail tab and it also calls the stored procedure to make the updates to the national ads header row.

                total_amt = ld_new;


                myData.Add_SP_Parm(invoice_number, "@invoice_number");
                myData.Add_SP_Parm(total_amt, "@total_amt");
                myData.Add_SP_Parm(comm_amt, "@comm_amt");
                myData.Add_SP_Parm(net_amt, "@net_amt");
                myData.Add_SP_Parm(gs_user_id, "@user_id");
                myData.Add_SP_Parm(is_adjustment_number, "@document_id");
                myData.Add_SP_Parm(vat_tax, "@vat_tax");


                if (!myData.NonQuerrySqlSp("usp_upd_natl_ads_inv_hdr"))
                {
                    ThrowErrorMessage("Error updating National Ads Detail in stored procedure usp_upd_natl_ads_inv_hdr - {0}", myData.dberror);
                    return string.Empty;
                }


                foreach (DataRow row1 in NationalAdsDetails.Rows)
                {
                    bool IsRowModified1 = (row1.RowState == DataRowState.Modified || row1.RowState == DataRowState.Added);

                    if (IsRowModified1)
                    {
                        foreach (DataColumn col in NationalAdsDetails.Columns)
                        {
                            if (row1[col, DataRowVersion.Original].ToString() != row1[col, DataRowVersion.Current].ToString())
                            {
                                myData.Add_SP_Parm(row1["invoice_number"], "@invoice_number");
                                myData.Add_SP_Parm(row1["seq_code"], "@seq_code");
                                myData.Add_SP_Parm(row1["air_date"], "@air_date");
                                myData.Add_SP_Parm(row1["agency_copy_code"], "@agency_copy_code");
                                myData.Add_SP_Parm(row1["ad_length"], "@ad_length");
                                myData.Add_SP_Parm(row1["ad_amt"], "@ad_amt");
                                myData.Add_SP_Parm(row1["ad_product"], "@ad_product");
                                myData.Add_SP_Parm(row1["record_source"], "@record_source");
                                myData.Add_SP_Parm(col.ColumnName, "@column_changed");
                                myData.Add_SP_Parm(row1[col, DataRowVersion.Original].ToString(), "@original_value");
                                myData.Add_SP_Parm(row1[col, DataRowVersion.Current].ToString(), "@new_value");
                                myData.Add_SP_Parm(this.gs_user_id, "@user_id");
                                myData.Add_SP_Parm(is_adjustment_number, "@document_id");
                                if (!myData.NonQuerrySqlSp("usp_upd_natl_ads_detail"))
                                {
                                    ThrowErrorMessage("Error updating National Ads Detail in stored procedure usp_upd_natl_ads_detail - {0}", myData.dberror);
                                    return string.Empty;
                                }
                            }
                        }

                    }



                }


            }

            else //No amount related changes, must have been agency code modifications.
            {

                foreach (DataRow row2 in NationalAdsDetails.Rows)
                {
                    bool IsRowModified2 = (row2.RowState == DataRowState.Modified || row2.RowState == DataRowState.Added);
                    is_adjustment_number = "None";

                    if (IsRowModified2)
                    {
                        foreach (DataColumn col in NationalAdsDetails.Columns)
                        {
                            if (row2[col, DataRowVersion.Original].ToString() != row2[col, DataRowVersion.Current].ToString())
                            {
                                myData.Add_SP_Parm(row2["invoice_number"], "@invoice_number");
                                myData.Add_SP_Parm(row2["seq_code"], "@seq_code");
                                myData.Add_SP_Parm(row2["air_date"], "@air_date");
                                myData.Add_SP_Parm(row2["agency_copy_code"], "@agency_copy_code");
                                myData.Add_SP_Parm(row2["ad_length"], "@ad_length");
                                myData.Add_SP_Parm(row2["ad_amt"], "@ad_amt");
                                myData.Add_SP_Parm(row2["ad_product"], "@ad_product");
                                myData.Add_SP_Parm(row2["record_source"], "@record_source");
                                myData.Add_SP_Parm(col.ColumnName, "@column_changed");
                                myData.Add_SP_Parm(row2[col, DataRowVersion.Original].ToString(), "@original_value");
                                myData.Add_SP_Parm(row2[col, DataRowVersion.Current].ToString(), "@new_value");
                                myData.Add_SP_Parm(this.gs_user_id, "@user_id");
                                myData.Add_SP_Parm(is_adjustment_number, "@document_id");
                                if (!myData.NonQuerrySqlSp("usp_upd_natl_ads_detail"))
                                {
                                    ThrowErrorMessage("Error updating National Ads Detail in stored procedure usp_upd_natl_ads_detail - {0}", myData.dberror);
                                    return string.Empty;
                                }
                            }
                        }

                    }





                }

                is_adjustment_number = "Updated";


            }
            //myData.Commit();
            return is_adjustment_number; 
                   
           
        }

            #endregion





        private void ThrowErrorMessage(string error_message, string db_error = null)
        {
            ib_tab_update_ok = false;

            myData.Rollback();

            //Since Begin and Rollback are not working correctly, added this to delete adjustment records that may have been created:
            string sSQL;
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("DELETE adjustment ");
            sbSQL.AppendLine("WHERE document_id = " + ls_document);         

            sSQL = string.Format(sbSQL.ToString());

            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

             //Since Begin and Rollback are not working correctly, added this to delete adjustment records that may have been created:
            sbSQL.Clear();        
            sbSQL.Append("DELETE adjustment_detail ");
            sbSQL.AppendLine("WHERE document_id = " + ls_document);         

            sSQL = string.Format(sbSQL.ToString());

            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            //Since Begin and Rollback are not working correctly, added this to delete adjustment records that may have been created:
            sbSQL.Clear();
            sbSQL.Append("DELETE adjustment_acct_detail ");
            sbSQL.AppendLine("WHERE adj_acct_id in (select adj_accct_id from adjustment_acct where document_id = " + ls_document + ")");

            sSQL = string.Format(sbSQL.ToString());

            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            //Since Begin and Rollback are not working correctly, added this to delete adjustment records that may have been created:
            sbSQL.Clear();
            sbSQL.Append("DELETE adjustment_acct ");
            sbSQL.AppendLine("WHERE document_id = " + ls_document);

            sSQL = string.Format(sbSQL.ToString());

            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            
            //Since Begin and Rollback are not working correctly, added this to delete adjustment records that may have been created:
            sbSQL.Clear();
            sbSQL.Append("DELETE natl_ads_adjustment ");
            sbSQL.AppendLine("WHERE document_id = " + ls_document);
            sSQL = string.Format(sbSQL.ToString());

            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }


            sbSQL.Append("DROP TABLE temp_adjustment_detail ");
            sSQL = string.Format(sbSQL.ToString());
            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            sbSQL.Clear();
            sbSQL.Append("DROP TABLE temp_product_adjustment_detail ");
            sSQL = string.Format(sbSQL.ToString());
            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }


            sbSQL.Clear();
            sbSQL.Append("DROP TABLE temp_product_commission_adjustment_detail ");
            sSQL = string.Format(sbSQL.ToString());
            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            sbSQL.Clear();
            sbSQL.Append("DROP TABLE temp_product_vat_adjustment_detail ");
            sSQL = string.Format(sbSQL.ToString());
            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }

            sbSQL.Clear();
            sbSQL.Append("DROP TABLE temp_natl_ads_inv_acct_detail ");
            sSQL = string.Format(sbSQL.ToString());
            if (!myData.NonQuerrySql(sSQL))
            {
                sSQL = "";
            }
           



                       
            if (db_error == null)
                ErrorMessage = error_message;
            else
                ErrorMessage = string.Format(error_message, db_error);
        }
    }
}