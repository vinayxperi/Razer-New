using RazerInterface;
using RazerBase;
using RazerBase.Lookups;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Linq;
using Microsoft.VisualBasic;
using System.Collections.Generic;



namespace Customer
{

    /// Interaction logic for CustomerPrintedAddr.xaml

    public partial class CustomerPrintedAddr : ScreenBase
    {
        

        private static readonly string mainTableName = "customerAddrPrint";
        public cBaseBusObject CustomerBusObj = new cBaseBusObject();
        public cBaseBusObject CustPrintAddr = new cBaseBusObject();
        string cust_id;



        public string WindowCaption { get { return string.Empty; } }


        public CustomerPrintedAddr(string Cust_id, cBaseBusObject _CustomerBusObj)
            : base()
        {

            //set obj
            this.CurrentBusObj = CustPrintAddr;
            //name obj
            this.CurrentBusObj.BusObjectName = "customerAddrPrint";
            //save Customer Business Object to reload Aging detail
            CustomerBusObj = _CustomerBusObj;
            cust_id = Cust_id;


            InitializeComponent();

            // Perform initializations for this object
            Init();

        }



        public void Init()
        {



            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            this.CurrentBusObj.Parms.AddParm("@receivable_account", cust_id);


            this.Load();


            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                int linectr = 0;
                bool AdsCust = false;
                string cityStPostal = "";
                string city = "";
                string state = "";
                string country = "";
                string postalcd = "";
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["account_name"] != null)
                {
                    txtAddr1.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["account_name"].ToString();
                    linectr++;
                }
                else
                {
                    MessageBox.Show("No account name set up for this customer");
                    return;
                }
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["natl_ads_ind"].ToString() == "1")
                    AdsCust = true;

                if (AdsCust == false) //Ads do not have attention line
                {
                    if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["attn_line"].ToString().Trim() != "")
                    {
                        txtAddr2.Text = "ATTN: " + this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["attn_line"].ToString();
                        linectr++;
                    }
                }
                if ((this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_1"].ToString().Trim() != "") && 
                  (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_1"].ToString() != null))  
                {
                    if (linectr == 1)
                        txtAddr2.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_1"].ToString();
                    else
                        txtAddr3.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_1"].ToString();
                    linectr++;
                }
                if ((this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_2"].ToString() != null) &&
                   ( this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_2"].ToString().Trim() != ""))

                {  
                   
                    if (linectr == 1)
                    {
                        txtAddr2.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_2"].ToString();
                        linectr++;
                    }
                    else
                        if (linectr == 2)
                        {
                            txtAddr3.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_2"].ToString();
                            linectr++;
                        }
                        else
                        {
                            txtAddr4.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_2"].ToString();
                            linectr++;

                        }
              
                }
              
                if ((this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString().Trim() != "") && 
                 !string.IsNullOrEmpty(this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString()))


                {
                   
                    if (linectr == 1)
                        txtAddr2.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString();
                    else
                        if (linectr == 2)
                            txtAddr3.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString();
                        else
                            if (linectr == 3)
                                txtAddr4.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString();
                            else
                                txtAddr5.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["address_3"].ToString();
                    linectr++;
                }

                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["city"].ToString().Trim() != "")
                    city = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["city"].ToString();
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["state"].ToString().Trim() != "")
                    state = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["state"].ToString();
                if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["postal_code"].ToString() != null)
                    postalcd = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["postal_code"].ToString();
                cityStPostal = city.ToString() + " " + state.ToString() + " " + postalcd.ToString();
                if (cityStPostal != "")
                {
                    if (linectr == 1)
                        txtAddr2.Text = cityStPostal.ToString();
                    else
                        if (linectr == 2)
                            txtAddr3.Text = cityStPostal.ToString();
                        else
                            if (linectr == 3)
                                txtAddr4.Text = cityStPostal.ToString();
                            else
                                if (linectr == 4)
                                    txtAddr5.Text = cityStPostal.ToString();
                                else
                                    txtAddr6.Text = cityStPostal.ToString();
                    linectr++;
                }
                if ( (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country"].ToString().Trim() != "") && 
                  (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country"].ToString() != null))  
                {
                    country = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["country"].ToString();
                    if (country.Substring(0, 2) != "US")
                    {
                        if (linectr == 1)
                            txtAddr2.Text = country.ToString();
                        if (linectr == 2)
                            txtAddr3.Text = country.ToString();
                        else
                            if (linectr == 3)
                                txtAddr4.Text = country.ToString();
                            else
                                if (linectr == 4)
                                    txtAddr5.Text = country.ToString();
                                else
                                    if (linectr == 5)
                                        txtAddr6.Text = country.ToString();
                                    else
                                        txtAddr7.Text = country.ToString();
                    }


                }

                if ((txtAddr1.Text == "") || (txtAddr1.Text == null))
                    txtAddr1.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr1.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr2.Text == "") || (txtAddr2.Text == null))
                    txtAddr2.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr2.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr3.Text == "") || (txtAddr3.Text == null))
                    txtAddr3.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr3.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr4.Text == "")  || (txtAddr4.Text == null))
                    txtAddr4.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr4.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr5.Text == "")  || (txtAddr5.Text == null))
                    txtAddr5.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr5.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr6.Text == "") || (txtAddr6.Text == null))
                    txtAddr6.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr6.Visibility = System.Windows.Visibility.Visible;
                if ((txtAddr7.Text == "")  || (txtAddr7.Text == null))
                    txtAddr7.Visibility = System.Windows.Visibility.Hidden;
                else
                    txtAddr7.Visibility = System.Windows.Visibility.Visible;
            }
           
            else
                MessageBox.Show("No address data retrieved for this customer");



        }
    }
}

      


     

      



      

        
 
