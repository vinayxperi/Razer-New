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

    /// Interaction logic for CustomerCreditComment.xaml

    public partial class CustomerCreditCMComment : ScreenBase
    {

        private static readonly string mainTableName = "creditComment";
        public cBaseBusObject CreditComment = new cBaseBusObject();
        public cBaseBusObject CustomerBusObj = new cBaseBusObject();
        string DocumentID = "";
        int SeqID;
        Boolean NewCMFlag = false;




        public string WindowCaption { get { return string.Empty; } }


        public CustomerCreditCMComment(string documentID, int seqID, cBaseBusObject _CustomerBusObj)
            : base()
        {
            
                //set obj
                this.CurrentBusObj = CreditComment;
                //name obj
                this.CurrentBusObj.BusObjectName = "creditComment";
                //save Customer Business Object to reload Aging detail
                CustomerBusObj = _CustomerBusObj;
                DocumentID = documentID;
                SeqID = seqID;

                InitializeComponent();

                // Perform initializations for this object
                Init();
            
        }

       

        public void Init()
        {


            this.CanExecuteSaveCommand = true;
            txtdocumentID.Text = DocumentID;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            this.CurrentBusObj.Parms.AddParm("@documentID", DocumentID);

            this.CurrentBusObj.Parms.AddParm("@seqID", SeqID);
            this.Load();

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            {
                txtComment.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["comment"].ToString();

            }
            else
                this.New();

            txtComment.CntrlFocus();


        }

        public override void New()
        {
            base.New();
            txtComment.CntrlFocus();
        }


        /// Handler for double click on grid to return the app selected to run


        // Save the changes
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if (txtComment.Text == "")
            {
                Messages.ShowError("A comment must be entered.");
                return;
            }
            if (this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"].ToString() == "")
            {
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["document_id"] = DocumentID.ToString();
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["seq_code"] = SeqID.ToString();
                this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["seq_id"] = 1;

            }

               
            this.Save();

        }
        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
                Messages.ShowInformation("Save Successful");
               CustomerBusObj.LoadTable("aging_detail");
               CustomerBusObj.LoadTable("proforma");
                CloseScreen();
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }

        private void CloseScreen()
        {
            
            System.Windows.Window CustomerParent = UIHelper.FindVisualParent<System.Windows.Window>(this);

            this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                CustomerParent.Close();
            }
        }
    }
}

      

        
 
