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
using System.Windows.Input;



namespace Razer
{

    /// Interaction logic for AdminMFA.xaml

    public partial class AdminMFA : ScreenBase
    {

        //private static readonly string mainTableName = "creditComment";
        //private static readonly string mainTableName = "creditComment";
        //public cBaseBusObject CreditComment = new cBaseBusObject();
        public cBaseBusObject CustomerBusObj = new cBaseBusObject();
        //string DocumentID = "";
        string Password = "";
        //int SeqID;
        Boolean NewCMFlag = false;

        public string WindowCaption { get { return string.Empty; } }


        public AdminMFA(string adminPassword)
            : base()
        {
            
                //set obj
                //this.CurrentBusObj = CreditComment;
                //name obj
                //this.CurrentBusObj.BusObjectName = "creditComment";
                //save Customer Business Object to reload Aging detail
                //CustomerBusObj = _CustomerBusObj;
                Password = adminPassword;
                //SeqID = seqID;

                InitializeComponent();

                // Perform initializations for this object
                Init();
            
        }

       

        public void Init()
        {


            this.CanExecuteSaveCommand = true;
            //txtdocumentID.Text = DocumentID;
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Unknown;

            this.MainTableName = "main";
            //this.CurrentBusObj.Parms.AddParm("@documentID", DocumentID);

            //this.CurrentBusObj.Parms.AddParm("@seqID", SeqID);
            //this.Load();

            System.Windows.Input.Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
            //if (CurrentBusObj.HasObjectData && CurrentBusObj.ObjectData.Tables["main"] != null && CurrentBusObj.ObjectData.Tables["main"].Rows.Count > 0)
            //{
            //    txtComment.Text = this.CurrentBusObj.ObjectData.Tables["main"].Rows[0]["comment"].ToString();

            //}
            //else
            //    this.New();

            //txtdocumentID.CntrlFocus();
            txtdocumentID.Focus();


        }

        public override void New()
        {
            base.New();
            //txtdocumentID.CntrlFocus();
            txtdocumentID.Focus();
        }


        /// Handler for double click on grid to return the app selected to run

        //private void txtdocumentID_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    //if (e.Key == Key.Space)
        //    //{
        //    txtdocumentID.Text = new string('*', txtdocumentID.Text.Length);
        //    //}
        //}
        // Save the changes
        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //cGlobals.SecurityDT.Rows[0]["role_id"] = 'N';
            //if (txtdocumentID.Text == "" || txtdocumentID.Text == null)
            //{
            //    Messages.ShowError("Please Enter Password to continue");
            //    return;
            //}
            //if (txtdocumentID.Text == Password)
            //{
            //    cGlobals.SecurityDT.Rows[0]["passwordOK"] = 'Y';
            //    CloseScreen();
            //}
            //else
            //{
            //    Messages.ShowError("Password not valid");
            //    return;
            //}   

            if (txtdocumentID.Password == "" || txtdocumentID.Password == null)
            {
                Messages.ShowError("Please Enter Password to continue");
                return;
            }
            if (txtdocumentID.Password == Password)
            {
                cGlobals.SecurityDT.Rows[0]["passwordOK"] = 'Y';
                CloseScreen();
            }
            else
            {
                Messages.ShowError("Password not valid");
                txtdocumentID.Password = "";
                txtdocumentID.Focus();
                return;
            }

        }

        private void txtdocumentID_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            //cGlobals.SecurityDT.Rows[0]["role_id"] = 'N';
            if (txtdocumentID.Password == "" || txtdocumentID.Password == null)
            {
                Messages.ShowError("Please Enter Password to continue");
                return;
            }
            if (txtdocumentID.Password == Password)
            {
                cGlobals.SecurityDT.Rows[0]["passwordOK"] = 'Y';
                CloseScreen();
            }
            else
            {
                Messages.ShowError("Password not valid");
                return;
            }

        }

        public override void Save()
        {
            base.Save();
            if (SaveSuccessful)
            {
               //Messages.ShowInformation("Save Successful");
               //CustomerBusObj.LoadTable("aging_detail");
               //CustomerBusObj.LoadTable("proforma");
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

            //this.CurrentBusObj.ObjectData.AcceptChanges();
            StopCloseIfCancelCloseOnSaveConfirmationTrue = false;

            if (!ScreenBaseIsClosing)
            {
                CustomerParent.Close();
            }
        }
    }
}

      

        
 
