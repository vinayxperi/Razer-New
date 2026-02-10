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
using RazerInterface;
using Infragistics.Windows.Editors;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Razer.CustomInvoice
{
    /// <summary>
    /// Interaction logic for ProformaTrackingQueue.xaml
    /// </summary>
    public partial class ProformaTrackingQueue : ScreenBase, IScreen, IPreBindable
    {
      
        //private bool IsPreBound = false;
        public void Init(cBaseBusObject businessObject)
        {
            _WindowCaption = "Proforma Tracking Queue";
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.MainTableName = "ProformaTrackingQueue";
            this.DoNotSetDataContext = false;

            // set the businessObject
            this.CurrentBusObj = businessObject;

            ProformaTrackingQueueGrid.MainTableName = "main";
            ProformaTrackingQueueGrid.xGrid.FieldLayoutSettings.AllowDelete = false;
            ProformaTrackingQueueGrid.xGrid.FieldSettings.AllowEdit = false;

            ProformaTrackingQueueGrid.SetGridSelectionBehavior(true, false);

            ProformaTrackingQueueGrid.FieldLayoutResourceString = "ProformaTrackingQueueGrid";
            ProformaTrackingQueueGrid.ConfigFileName = "ProformaTrackingQueue";
            ProformaTrackingQueueGrid.ContextMenuAddIsVisible = false;
            ProformaTrackingQueueGrid.ContextMenuRemoveIsVisible = false;

          
            ProformaTrackingQueueGrid.ContextMenuGenericDelegate2 = ChangeStatusDelegate;
            ProformaTrackingQueueGrid.ContextMenuGenericDisplayName2 = "Change Status";
            ProformaTrackingQueueGrid.ContextMenuGenericIsVisible2 = true;

           
            ProformaTrackingQueueGrid.WindowZoomDelegate = GridDoubleClickDelegate;
            if (SecurityContext == AccessLevel.NoAccess || SecurityContext == AccessLevel.ViewOnly)
            {
                ProformaTrackingQueueGrid.ContextMenuGenericIsVisible1 = false;
                ProformaTrackingQueueGrid.ContextMenuGenericIsVisible2 = false;
                ProformaTrackingQueueGrid.ContextMenuGenericIsVisible3 = false;
            }





            GridCollection.Add(ProformaTrackingQueueGrid);

            this.Load();

            //cboStatus.SelectedValue = -1;

            //FilterInvoiceEmailQueueGrid();
        }

        public void GridDoubleClickDelegate()
        {
            //call custom Invoice folder
            //cGlobals.ReturnParms[1] = ("CustomInvoiceZoom");
            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            ProformaTrackingQueueGrid.ReturnSelectedData("invoice_number");
            cGlobals.ReturnParms.Add("CustomInvoiceZoom");
            RoutedEventArgs args = new RoutedEventArgs(EventAggregator.GeneratedClickEvent);
            args.Source = ProformaTrackingQueueGrid.xGrid;
            EventAggregator.GeneratedClickHandler(this, args);


        } 

        private void ChangeStatusDelegate()
        {
            string Description;
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows.Count != 0)
            {
                foreach (Record record in ProformaTrackingQueueGrid.xGrid.SelectedItems.Records)
                {
                    DataRow row = ((record as DataRecord).DataItem as DataRowView).Row;
                    Description = row["invoice_number"].ToString();
                    if (Convert.ToInt32(row["proforma_closed_flag"]) == 0)
                    {
                       
                        MessageBoxResult result = Messages.ShowYesNo("Are you sure you want to close this Proforma Invoice? " + Description.Trim() + "?", MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            row["proforma_closed_flag"] = "1";
                            Save();
                            Messages.ShowInformation("Status changed to Closed on Proforma Invoice "  + Description.Trim());
                        }
                    }                  
                
                }

                this.Load();
            }            

        }

       


        //private void FilterInvoiceEmailQueueGrid()
        //{
        //    ProformaTrackingQueueGrid.ClearFilter();

        //    if (cboStatus.SelectedValue.ToString() != "-1")
        //    {
        //        ProformaTrackingQueueGrid.FilterGrid("status_flag", cboStatus.SelectedValue.ToString());
        //    }


        //}

        private string _WindowCaption;
        public string WindowCaption
        {
            get { return _WindowCaption; }
        }
        

        public ProformaTrackingQueue()
        {
            InitializeComponent();
        }


        public void PreBind()
        {
        }

      



      
    }
}
