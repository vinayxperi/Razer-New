



using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using RazerInterface;
using Infragistics.Windows.DockManager;
using System;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.Editors;
using System.Data;




namespace Units
{

 
    /// <summary>
    /// This class represents a 'UnitsFilter' object.
    /// </summary>
    public partial class UnitsFilter : ScreenBase, IScreen
    {


        public string WindowCaption { get; set; }
        public string filterID { get; set; }
        private bool IsScreenInserting { get; set; }

        private int _filterid { get; set; }
        public int _seqid { get; set; }
        public int _md_id { get; set; }
        public string _md { get; set; }
        public string _valueid { get; set; }
        public string _value { get; set; }
        public int _construtorid { get; set; }
        public string _constructor { get; set; }
        public int _operatorid { get; set; }
        public string _operator { get; set; }
        public int _subfilterid { get; set; }
        public bool _hasvars { get; set; }

        private bool _issaved = false;
        private bool _isnew = false;


        /// <summary>
        /// Create a new instance of a 'UnitsFilter' object and call the ScreenBase's constructor.
        /// </summary>
        public UnitsFilter()
            : base()
        {
            // This call is required by the designer.
            InitializeComponent();
        }





        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            IsScreenInserting = false;
            businessObject.Parms.AddParm("@filter_id", -1);
            businessObject.Parms.AddParm("@show_inactive", 0);
           
            this.MainTableName = "unit_filter";

            this.CurrentBusObj = businessObject;
            FieldLayoutSettings f = new FieldLayoutSettings();
            gFilterGrid.MainTableName = "unit_filter_detail";
            gFilterGrid.ConfigFileName = "filtergrid1";
            gFilterGrid.xGrid.FieldLayoutSettings = f;
            gFilterGrid.ContextMenuAddDelegate = ContactGridAddDelegate;
            gFilterGrid.WindowZoomDelegate = GridDoubleClickDelegate;
            gFilterGrid.ContextMenuRemoveDelegate = UnitGridRemoveDelegate;
            gFilterGrid.SetGridSelectionBehavior(false, true);
            gFilterGrid.FieldLayoutResourceString = "filtergrid";
            gFilterGrid.xGrid.FieldSettings.AllowEdit = true;
            this.Load(businessObject);
        }


        public override void New()
        {
            IsScreenInserting = true;
            this.tbFilterID.IsEnabled = false;
            this.tbFilterName.Text = "New";
            _isnew = true;
            CurrentBusObj.ObjectData.Tables["unit_filter_detail"].Clear();
            gFilterGrid.LoadGrid(CurrentBusObj, "unit_filter_detail");
            base.New();
        }

        public void ContactGridAddDelegate()
        {

            cGlobals.ReturnParms.Clear();
            
            if (CurrentBusObj.ObjectData.Tables["unit_filter_detail"].Rows.Count < 1)
            {
                this.Save();
                _filterid = Int32.Parse(this.CurrentBusObj.Parms.ParmList.Rows[0].ItemArray[1].ToString());
                cGlobals.ReturnParms.Add(1);
            }
            else
            {
                cGlobals.ReturnParms.Add(get_max_seq());
               // cGlobals.ReturnParms.Add(Int32.Parse(tbFilterID.Text));
            }
            UnitFiltersLook ufl = new UnitFiltersLook(this.CurrentBusObj);
            ufl.ShowDialog();
           
            if(cGlobals.ReturnParms.Count > 2)
            {
                
                _seqid = get_max_seq();
                _md_id = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                _operatorid = Int32.Parse(cGlobals.ReturnParms[3].ToString());
                _valueid = cGlobals.ReturnParms[4].ToString();
             
                _construtorid = Int32.Parse(cGlobals.ReturnParms[5].ToString());
                _md = cGlobals.ReturnParms[6].ToString();
                _operator = cGlobals.ReturnParms[7].ToString();
                _value = cGlobals.ReturnParms[8].ToString();
                _constructor = cGlobals.ReturnParms[9].ToString();
            
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
                CurrentBusObj.ObjectData.Tables["unit_filter_detail"].Rows.Add(
                    _filterid, _seqid, _md_id, _md, _operatorid, _operator, _valueid, _value,  
                    _construtorid, _constructor);
                gFilterGrid.LoadGrid(CurrentBusObj, "unit_filter_detail");
            }
            if (_issaved)
            {
                
                _issaved = false;
            }
    
            //    gRemittance.LoadGrid(CurrentBusObj, "lookup_remit");
        }
        public void UnitGridRemoveDelegate()
        {
            DataRecord r = gFilterGrid.ActiveRecord;
            if (r != null)
            {
                DataRow row = (r.DataItem as DataRowView).Row;

                if (row != null)
                {
                    row.Delete();
                }
            }
        }
        public void GridDoubleClickDelegate()
        {

         
            DataRecord r = default(DataRecord);
            //Below catches xamdatagrid bug where double click highlights a cell instead of selecting a row.  
            //If error condition is received when retrieving selected row then the row of the currently active cell is used.
            try
            {
                r = (Infragistics.Windows.DataPresenter.DataRecord)gFilterGrid.xGrid.SelectedItems.Records[0];
            }
            catch (Exception ex)
            {
                // for debugging only
                string err = ex.ToString();

                // Set the current record
                r = gFilterGrid.xGrid.ActiveCell.Record;
            }
            cGlobals.ReturnParms.Clear();
            cGlobals.ReturnParms.Add(r.Cells["filter_id"].Value);
            cGlobals.ReturnParms.Add(r.Cells["seq_id"].Value);
            cGlobals.ReturnParms.Add(r.Cells["unit_md_id"].Value);
            cGlobals.ReturnParms.Add(r.Cells["operator"].Value);
            cGlobals.ReturnParms.Add(r.Cells["value"].Value);
            cGlobals.ReturnParms.Add(r.Cells["construct"].Value);
            UnitFiltersLook fl = new UnitFiltersLook(CurrentBusObj);
            fl.ShowDialog();
            //cGlobals.ReturnParms.Add(r.Cells["name"].Value);
          //  this.Close();
            if (cGlobals.ReturnParms.Count > 0)
            {
                _filterid = Int32.Parse(cGlobals.ReturnParms[0].ToString());
                _seqid = Int32.Parse(cGlobals.ReturnParms[1].ToString());
                _md_id = Int32.Parse(cGlobals.ReturnParms[2].ToString());
                _operatorid = Int32.Parse(cGlobals.ReturnParms[3].ToString());
                _valueid = cGlobals.ReturnParms[4].ToString();
                _construtorid = Int32.Parse(cGlobals.ReturnParms[5].ToString());
                _md = cGlobals.ReturnParms[6].ToString();
                _operator = cGlobals.ReturnParms[7].ToString();
                _value = cGlobals.ReturnParms[8].ToString();
                _constructor = cGlobals.ReturnParms[9].ToString();
                r.Cells["unit_md_id"].Value = _md_id;
                r.Cells["operator"].Value = _operatorid;
                r.Cells["value"].Value = _valueid;
                r.Cells["construct"].Value = _construtorid;
                r.Cells["md_name"].Value = _md;
                r.Cells["operator_text"].Value = _operator;
                r.Cells["value_text"].Value = _value;
                r.Cells["construct_text"].Value = _constructor;
            }
        }
        private void tbFilterID_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
            UnitFilterLookup ufl = new UnitFilterLookup();
            ufl.ShowDialog();
            if (cGlobals.ReturnParms.Count > 0)
            {
                // now we can load the account
                this.CurrentBusObj.Parms.ClearParms();
                this.CurrentBusObj.Parms.AddParm("@filter_id", cGlobals.ReturnParms[0].ToString());
                this.CurrentBusObj.Parms.AddParm("@show_inactive", 0);
                tbFilterID.Text = cGlobals.ReturnParms[0].ToString();

                // Call load 
                this.Load();
                //HeaderName = "Contract-" + txtContractID.Text;
                fill_data();
                filterID = tbFilterID.Text;

                // Clear the parms
                cGlobals.ReturnParms.Clear();
            }
        }

        private void tbFilterID_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (tbFilterID.Text != null)
            {
                filterID = tbFilterID.Text.ToString();
            }
            else
            {
                filterID = "";
            }
        }
       

        private void tbFilterID_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            int iID = 0;

            //Check to verify a numeric value was entered
            if(!Int32.TryParse(tbFilterID.Text, out iID))
            {
                Messages.ShowWarning("You must enter a numeric value in the Filter ID field");
                e.Handled = true;
                tbFilterID.Focus();
                return;
            }
            if (!string.IsNullOrEmpty(tbFilterID.Text) && tbFilterID.Text != filterID)
            {              //Clear the current parameters
                if (!_isnew)
                {
                    this.CurrentBusObj.Parms.ClearParms();
                    //Add new parameters
                    this.CurrentBusObj.Parms.AddParm("@filter_id", tbFilterID.Text);
                    this.CurrentBusObj.Parms.AddParm("@show_inactive", "0");
                    this.Load();
                    
                }
            }
            if (chkForData()) SetHeaderName();
        }
        private bool chkForData()
        {
            if (this.CurrentBusObj.ObjectData.Tables[0].Rows.Count != 0)
            {
                fill_data();
                       return true;
            }
            else
            {
                Messages.ShowWarning("Unit Filter Not Found");
                return false;
            }
        }
        private void fill_data()
        {
            if (this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows.Count > 0)
            {
                WindowCaption = "Unit Filter -" + this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[1].ToString();


                tbFilterName.Text = this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[1].ToString();
                tbFilterDescription.Text = this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[2].ToString();
                gFilterGrid.LoadGrid(CurrentBusObj, "unit_filter_detail");
            }
        }
        private void SetHeaderName()
        {
            //Sets the header name when being called from another folder
            if (this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows.Count > 0)
            {
                if (tbFilterID.Text == null)
                {
                    WindowCaption = "Unit Filter - " + this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[1].ToString();
                    tbFilterID.Text = this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[0].ToString();


                }
                //Sets the header name from within same folder
                else
                {
                    ContentPane p = (ContentPane)this.Parent;
                    p.Header = "Unit Filter - " + this.CurrentBusObj.ObjectData.Tables["unit_filter"].Rows[0].ItemArray[1].ToString();
                }
            }
        }

 
        public override void Save()
        {
            try
            {
                base.Save();
                Messages.ShowInformation("Filter Saved Successfully.");
            }
            catch 
            {
            }  
            
        }
        private int get_max_seq()
        {
            int _seq = 0;
            int _lastseq = 0;
            foreach (DataRow item in CurrentBusObj.ObjectData.Tables["unit_filter_detail"].Rows)
            {
                if (item.RowState != DataRowState.Deleted)
                {
                    _seq = Int32.Parse(item["seq_id"].ToString());
                    if (_seq > _lastseq)
                        _lastseq = _seq;
                }
            }
            _lastseq++;
            return _lastseq;
        }
    }


}
