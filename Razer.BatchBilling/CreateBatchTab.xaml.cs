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

namespace Razer.BatchBilling
{
    /// <summary>
    /// Interaction logic for CreateBatchTab.xaml
    /// </summary>
    public partial class CreateBatchTab : ScreenBase , IScreen
    {
        private static readonly string mainTableName = "batch";
        private static readonly string searchTableName = "contracts";
        private static readonly string batchParameter = "@batch_id";
        private static readonly string companyParameter = "@company_id";
        private static readonly string entityParameter = "@bill_mso_id";
        private static readonly string productParameter = "@product_code";
        private static readonly string expiredParameter = "@include_expired_flag";
        private static readonly string searchObject = "BatchBillingContract";
        private string batchId;

        public string WindowCaption { get { return string.Empty; } }
    
        public CreateBatchTab()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;

            this.DoNotSetDataContext = false;

            this.CurrentBusObj = businessObject;
            this.MainTableName = mainTableName;
        }

        private void txtBatchId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBatchId.Text) && txtBatchId.Text != batchId)
            {
                //Clear the current parameters
                this.CurrentBusObj.Parms.ClearParms();
                //Add new parameters
                this.CurrentBusObj.Parms.AddParm(batchParameter, txtBatchId.Text);
          
                if (this.CurrentBusObj != null)
                {
                    this.Load();                    
                }
            }
        }

        private void txtBatchId_GotFocus(object sender, RoutedEventArgs e)
        {
            batchId = txtBatchId.Text;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            cBaseBusObject busObject = new cBaseBusObject(searchObject);

            int companyId = 0;
            int.TryParse(txtRoviCompany.Text, out companyId);            
            busObject.Parms.AddParm(companyParameter, companyId);

            int entityId = 0;
            int.TryParse(txtContractEntity.Text, out entityId);
            busObject.Parms.AddParm(entityParameter, entityId);

            busObject.Parms.AddParm(productParameter, txtProduct.Text);
            busObject.Parms.AddParm(expiredParameter, chkIncludeExpired.IsChecked);

            busObject.LoadData();

            idgQueryList.LoadGrid(busObject, searchTableName);
        }        
    }
}
