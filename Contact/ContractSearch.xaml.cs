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
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Contact
{
    /// <summary>
    /// Interaction logic for BCFSearch.xaml
    /// </summary>
    public partial class ContactSearch : ScreenBase, IScreen
    {
        private static readonly string contactSearchTable = "contact_lookup";
        private static readonly string contactSearchLayout = "ContactLookupGrid";

        private static readonly string contactTypeObject = "Contact";
        private static readonly string contactTypeTable = "contact_type_lookup";
        private static readonly string stateObject = "TFFolder";    //Used existing one.
        private static readonly string stateTable = "state";

        private static readonly string contactId = "@contact_id";
        private static readonly string firstName = "@first_name";
        private static readonly string lastName = "@last_name";
        private static readonly string comment = "@comment";
        private static readonly string city = "@city";
        private static readonly string state = "@state";
        private static readonly string contactType = "@description";
        private static readonly string inactiveFlag = "@ShowInactives";
        private static bool inactiveFlagChecked;

        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
            set { windowCaption = value; }
        }

        public ContactSearch()
        {
            InitializeComponent();
        }

        System.Windows.Window searchScreenWindow;

        public void Init(cBaseBusObject businessObject)
        {
            this.CurrentBusObj = businessObject;
            idgContactSearchGrid.WindowZoomDelegate = ReturnSelectedData;

            searchScreenWindow = new System.Windows.Window();
            searchScreenWindow.Title = "Contact Search Screen";
            //set rules screen as content of new window
            searchScreenWindow.Content = this;

            //Load Contact type dropdown
            cBaseBusObject contactTypeBusObj = new cBaseBusObject(contactTypeObject);
            contactTypeBusObj.LoadTable(contactTypeTable);

            DataTable source = contactTypeBusObj.ObjectData.Tables[contactTypeTable] as DataTable;
            txtContactType.ItemsSource = source.DefaultView;


            //Load state drop down
            cBaseBusObject stateTypeBusObj = new cBaseBusObject(stateObject);
            stateTypeBusObj.LoadTable(stateTable);

            DataTable sourceTable = stateTypeBusObj.ObjectData.Tables[stateTable] as DataTable;
            txtState.ItemsSource = sourceTable.DefaultView;

            //txtState.SelectedValue = "";

            //open new window with embedded user control
            searchScreenWindow.ShowDialog();
        }

        /// <summary>
        /// Handler for double click on grid or buttton click.contact_id
        /// </summary>
        public void ReturnSelectedData()
        {
            idgContactSearchGrid.ReturnSelectedData("contact_id");

            CloseWindow();
            this.Close();
        }

        private void CloseWindow()
        {
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.Close();
            }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ReturnSelectedData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            loadContactSearchGrid();

        }

        private void loadContactSearchGrid()
        {
            idgContactSearchGrid.xGrid.DataSource = null;

            CurrentBusObj = new cBaseBusObject(this.CurrentBusObj.BusObjectName);

            CurrentBusObj.Parms.ClearParms();

            CurrentBusObj.Parms.AddParm(contactId, txtContactId.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(firstName, txtFirstName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(lastName, txtLastName.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(comment, txtComment.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(city, txtCity.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(state, txtState.Text ?? string.Empty);
            CurrentBusObj.Parms.AddParm(contactType, txtContactType.Text ?? string.Empty);

            if (Convert.ToBoolean(chkInactive.IsChecked))
                CurrentBusObj.Parms.AddParm(inactiveFlag, 1);
            else
                CurrentBusObj.Parms.AddParm(inactiveFlag, 0);

            this.MainTableName = contactSearchTable;

            FieldLayoutSettings layouts = new FieldLayoutSettings();

            idgContactSearchGrid.WindowZoomDelegate = ReturnSelectedData;
            idgContactSearchGrid.xGrid.FieldLayoutSettings = layouts;
            idgContactSearchGrid.FieldLayoutResourceString = contactSearchLayout;
            idgContactSearchGrid.MainTableName = contactSearchTable;

            this.Load(CurrentBusObj);

            if (CurrentBusObj.HasObjectData)
            {
                idgContactSearchGrid.LoadGrid(CurrentBusObj, this.MainTableName);

                if (CurrentBusObj.ObjectData.Tables[this.MainTableName].Rows.Count > 0)
                    btnOk.IsEnabled = true;
                else
                    btnOk.IsEnabled = false;
            }
            else
            {
                btnOk.IsEnabled = false;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtContactId.Text = String.Empty;
            txtFirstName.Text = String.Empty;
            txtLastName.Text = String.Empty;
            txtComment.Text = String.Empty;
            txtCity.Text = String.Empty;
            txtState.Text = String.Empty;
            txtContactType.Text = String.Empty;

            chkInactive.IsChecked = false;
            //inactiveFlagChecked = false;

            idgContactSearchGrid.xGrid.DataSource = null;

            btnOk.IsEnabled = false;

        }

        private void chkInactive_Click(object sender, RoutedEventArgs e)
        {
            //if (chkInactive.IsChecked == true)
            //    inactiveFlagChecked = true;
            //else
            //    inactiveFlagChecked = false;
            loadContactSearchGrid();
        }
    }
}