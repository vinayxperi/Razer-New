using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RazerBase;
using RazerBase.Interfaces;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using Infragistics.Windows.DataPresenter.Events;
using System.Data;
using RazerInterface;
using RazerWS;

namespace Razer.NationalAds
{
    /// <summary>
    /// Interaction logic for NationalAdsFolder.xaml
    /// </summary>
    public partial class NationalAdsFolder : ScreenBase, IScreen
    {
        private static readonly string captionName = "Advertisement";
        private static readonly string mainTableName = "general";
        private static readonly string invoiceParameter = "@invoice_number";
        private static readonly string documentIdParameter = "@document_id";

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        private string invoiceNumber;

        public NationalAdsFolder()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            this.DoNotSetDataContext = false;

            this.CurrentBusObj = businessObject;
            this.MainTableName = mainTableName;

            // add the Tab user controls that are of type screen base
            TabCollection.Add(General);
            TabCollection.Add(Details);
            TabCollection.Add(View);
            TabCollection.Add(Deferred);

            //Window is being opened by another, open with the data based on the Id in the parameter list.
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                invoiceNumber = this.CurrentBusObj.Parms.ParmList.Rows[0][1].ToString();

                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    this.CurrentBusObj.Parms.ClearParms();
                    CurrentBusObj.Parms.AddParm(invoiceParameter, invoiceNumber);
                    CurrentBusObj.Parms.AddParm(documentIdParameter, invoiceNumber);
                    this.Load();
  
                    DataTable sales = CurrentBusObj.ObjectData.Tables["sales"];
                    General.gSales.DataContext = sales;
                    General.gSales.xGrid.DataSource = sales.DefaultView;

                    General.invoiceNumber = this.invoiceNumber;
                    Details.invoiceNumber = this.invoiceNumber;
                    Details.adjCount = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adj_count"];
                    Details.txtTotalAdjustment.Text = "$0.00";
                    Details.orderType = CurrentBusObj.ObjectData.Tables["general"].Rows[0]["order_type"].ToString();
                    General.statusFlag = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"];
                    General.printedFlag = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["printed_flag"];
                    windowCaption = windowCaption + invoiceNumber.TrimEnd();
                    if (General.statusFlag == 0)
                    {
                        Details.chkReverseAdjustment.IsEnabled = false;
                    }
                }
            }
        }

        private void txtInvoiceNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInvoiceNumber.Text) && txtInvoiceNumber.Text != invoiceNumber)
            {
                LoadData(txtInvoiceNumber.Text);
            }
        }


        private void txtInvoiceNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            invoiceNumber = txtInvoiceNumber.Text;
        }


        private void txtInvoiceNumber_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = EventAggregator.GeneratedClickEvent;
            args.Source = txtInvoiceNumber;
            EventAggregator.GeneratedClickHandler(this, args);

            if (cGlobals.ReturnParms.Count > 0)
            {
                string id = cGlobals.ReturnParms[0].ToString();
                LoadData(id);
                cGlobals.ReturnParms.Clear();
            }
        }


        public void LoadData(string invoiceId)
        {
            
            this.CurrentBusObj.Parms.ClearParms();            
            this.CurrentBusObj.Parms.AddParm(invoiceParameter, invoiceId);
            this.CurrentBusObj.Parms.AddParm(documentIdParameter, invoiceId);

            StringBuilder buildCaption = new StringBuilder(captionName);
            buildCaption.Append(": ");
            buildCaption.Append(invoiceId);
            windowCaption = buildCaption.ToString();

            if (this.CurrentBusObj != null)
            {
                this.Load();
                if (CurrentBusObj.ObjectData.Tables[mainTableName].Rows.Count > 0)
                {
                    invoiceNumber = invoiceId;
                    General.invoiceNumber = this.invoiceNumber;
                    Details.invoiceNumber = this.invoiceNumber;
                    Details.adjCount = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["adj_count"];
                    Details.txtTotalAdjustment.Text = "$0.00";
                    Details.orderType = CurrentBusObj.ObjectData.Tables["general"].Rows[0]["order_type"].ToString();
                    General.statusFlag = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["status_flag"];
                    General.printedFlag = (int)CurrentBusObj.ObjectData.Tables["general"].Rows[0]["printed_flag"];

                    if (General.statusFlag == 0)
                    {
                        Details.chkReverseAdjustment.IsEnabled = false;
                    }


                    DataTable sales = CurrentBusObj.ObjectData.Tables["sales"];
                    General.gSales.DataContext = sales;
                    General.gSales.xGrid.DataSource = sales.DefaultView;


                }
                else
                {
                    Messages.ShowInformation("Invoice not found.");
                    Details.txtTotalAdjustment.Text = "$0.00";
                    Details.txtCommission.Text = "$0.00";
                    Details.txtNetAmount.Text = "$0.00";
                    Details.txtTotalAds.Text = "0";
                    Details.txtTotalAmount.Text = "$0.00";

                }
            }
        }


        public override void Save()
        {
            if (CurrentBusObj.HasObjectData)
            {

                Details.SaveAdjustment();
                LoadData(invoiceNumber);
            }
            else
            {
                return;
            }
        }
        public override void New()
        {
            
                return;
            
        }

        //KSH - 8/22/12 Close Logic
        public override void Close()
        {
            if (Details.CurrentBusObj != null)
            {
                if (Details.CurrentBusObj.ObjectData != null)
                {
                    //only check Detail tab datatable for changes
                    if (Details.CurrentBusObj.ObjectData.HasChanges())
                    {
                        ForceScreenDirty = true;
                        base.Close();
                    }
                }
            }
        }
    }
}
