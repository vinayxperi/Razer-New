using System.Windows.Controls;
using System.Windows.Input;
using Infragistics.Windows.DockManager;
using RazerBase;

namespace RecvAcctFolder
{
    /// <summary>
    /// Interaction logic for ucRecvAcctScreen.xaml
    /// </summary>
    public partial class ucRecvAcctScreen : ScreenBase
    {
        public ucRecvAcctScreen(cBaseBusObject businessObject)
        {
            //cCommandLibrary.Save. .CanExecute(true, this);
            // This call is required by the designer.
            InitializeComponent();
            //done in xaml
            //this.ScreenBaseParentObjName = "ucRecvAcctScreen";
            // Set the MainTableName (may not be needed)
            
            this.MainTableName = "RecvAcct";

            // set the businessObject
            this.CurrentBusObj = businessObject;
            
            // add the Tabs
            TabCollection.Add(uHistory);
            TabCollection.Add(uGeneral);
            TabCollection.Add(uAging);
            TabCollection.Add(uSystem);

            // if there are parameters than we need to load the data
            if (this.CurrentBusObj.Parms.ParmList.Rows.Count > 0)
            {
                // load the data
                this.Load();
            }

            // Set the Header
            SetHeaderName();
        }

        private void RecvAcct_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ucRecvAcctLookup f = new ucRecvAcctLookup();
            TextBox t = (TextBox)sender;

            //cGlobals.ReturnParms.Clear();
            this.CurrentBusObj.Parms.ClearParms();
            switch (((TextBox)sender).Name)
            {
                case "tRecvAcct":

                    // add the parms
                    this.CurrentBusObj.Parms.AddParm("@receivable_account", tRecvAcct.Text);
                    this.CurrentBusObj.Parms.AddParm("@product_code", "a");
                    
                    // required
                    break;

                default:

                    this.CurrentBusObj.Parms.AddParm("@receivable_account", tRecvAcct.Text);
                    this.CurrentBusObj.Parms.AddParm("@product_code", "a");
                    
                    // required
                    break;
            }


            // gets the users response
            f.ShowDialog();

            // Check if a value is returned
            if (cGlobals.ReturnParms.Count > 0)
            {
                // now we can load the account
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@receivable_account", cGlobals.ReturnParms[0].ToString());
                this.CurrentBusObj.Parms.AddParm("@product_code", "a");
                
                // Call load 
                this.Load();
                
                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }

            // Set the HeaderName
            SetHeaderName();
        }


        private void SetHeaderName()
        {
            ContentPane p = (ContentPane)this.Parent;
            if (p == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(tAcctName.Text))
            {
                p.Header = tAcctName.Text;
            }
            else
            {
                p.Header = "Recevable Account";
            }
        }
    }
}
