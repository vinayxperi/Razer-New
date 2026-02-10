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
using Microsoft.Win32;
using System.Data;


namespace Razer.TableMaintenance
{
    /// <summary>
    /// Interaction logic for GLChart.xaml
    /// </summary>
    public partial class GLChart : RazerBase.ScreenBase, IScreen
    {
        private static readonly string fieldLayoutResource = "GLChartMaintenance";
        private static readonly string mainTableName = "gl_chart";



        public GLChart()
        {
            InitializeComponent();
        }

        public void Init(cBaseBusObject businessObject)
        {
            FieldLayoutSettings layouts = new FieldLayoutSettings();
            layouts.HighlightAlternateRecords = true;
            //Adds the insert row at the top
            layouts.AllowAddNew = true;
            layouts.AddNewRecordLocation = AddNewRecordLocation.OnTop;
            

            this.CurrentBusObj = businessObject;


            this.MainTableName = mainTableName;
            idgGLChart.xGrid.FieldLayoutSettings = layouts;
            idgGLChart.FieldLayoutResourceString = fieldLayoutResource;
            idgGLChart.MainTableName = mainTableName;
            idgGLChart.xGrid.FieldSettings.AllowEdit = true;


            


            this.Load(businessObject);
            
            idgGLChart.LoadGrid(businessObject, mainTableName);

            //Vinay's changes
            //Set up lost focus event handler for gl_region!
            var gl_chart_FieldLayout = idgGLChart.xGrid.FieldLayouts["gl_chart"];
            var gl_region_field = gl_chart_FieldLayout.Fields.FirstOrDefault(f => f.Name == "gl_region");
            if (gl_region_field != null)
            {
            
                ValueConstraint valueConstraint = new ValueConstraint();
                valueConstraint.MaxLength = 4;
            
                // Field found, proceed with your logic
                gl_region_field.Settings.EditorStyle = new Style(typeof(XamTextEditor));
                gl_region_field.Settings.EditorStyle.Setters.Add(new Setter(XamTextEditor.ValueConstraintProperty, valueConstraint));
                gl_region_field.Settings.EditorStyle.Setters.Add(new EventSetter(UIElement.LostFocusEvent, new RoutedEventHandler(GLRegion_LostFocus)));
            
            }
           
            //Set uplost focus event handler for gl_product
            var gl_product_field = gl_chart_FieldLayout.Fields.FirstOrDefault(f => f.Name == "gl_product");
            if (gl_product_field != null)
            {

                ValueConstraint valueConstraint = new ValueConstraint();
                valueConstraint.MaxLength = 4;

                // Field found, proceed with your logic
                gl_product_field.Settings.EditorStyle = new Style(typeof(XamTextEditor));
                gl_product_field.Settings.EditorStyle.Setters.Add(new Setter(XamTextEditor.ValueConstraintProperty, valueConstraint));
                gl_product_field.Settings.EditorStyle.Setters.Add(new EventSetter(UIElement.LostFocusEvent, new RoutedEventHandler(GLProduct_LostFocus)));

            } 
        }


        //Vinay's changes
        private void GLRegion_LostFocus(object sender, RoutedEventArgs e)
        {
            var editor = sender as XamTextEditor;

            if (editor != null && editor.DisplayText != "")
            {

                cBaseBusObject region = new cBaseBusObject("GLRegion");
                region.Parms.ClearParms();
                region.Parms.AddParm("@gl_region", editor.DisplayText);

                region.LoadTable("gl_region");

                if (region.HasObjectData)
                {
                    if (region.ObjectData.Tables["gl_region"].Rows.Count > 0)
                    {
                        DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                        if (record != null)
                            record.SetCellValue(record.FieldLayout.Fields["gl_region_desc"], region.ObjectData.Tables["gl_region"].Rows[0]["description"].ToString());
                    }
                    else
                    {
                        DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                        if (record != null)
                            record.SetCellValue(record.FieldLayout.Fields["gl_region_desc"], "");
                    }
                }
            }
            else
            {
                DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                if (record != null)
                    record.SetCellValue(record.FieldLayout.Fields["gl_region_desc"], "");
            }
        }

        //Vinay's changes
        private void GLProduct_LostFocus(object sender, RoutedEventArgs e)
        {
            var editor = sender as XamTextEditor;

            if (editor != null && editor.DisplayText != "")
            {

                cBaseBusObject region = new cBaseBusObject("GLProduct");
                region.Parms.ClearParms();
                region.Parms.AddParm("@gl_product", editor.DisplayText);

                region.LoadTable("gl_product");

                if (region.HasObjectData)
                {
                    if (region.ObjectData.Tables["gl_product"].Rows.Count > 0)
                    {
                        //   MessageBox.Show(region.ObjectData.Tables["gl_region"].Rows[0]["description"].ToString());
                        DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                        if (record != null)
                            record.SetCellValue(record.FieldLayout.Fields["gl_product_desc"], region.ObjectData.Tables["gl_product"].Rows[0]["description"].ToString());
                    }
                    else
                    {
                        DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                        if (record != null)
                            record.SetCellValue(record.FieldLayout.Fields["gl_product_desc"], "");
                    }
                }
            }
            else
            {
                DataRecord record = idgGLChart.xGrid.ActiveRecord as DataRecord;
                if (record != null)
                    record.SetCellValue(record.FieldLayout.Fields["gl_product_desc"], "");
            }
        }

        
        public string WindowCaption
        {
            get { return string.Empty; }
        }

        public override void Save()
        {
            //Validate all mandatory fields.
            foreach (DataRow r in this.CurrentBusObj.ObjectData.Tables["gl_chart"].Rows)
            {

                if (r.RowState == DataRowState.Added)
                {
    
                    if (r["description"] == null || r["description"].ToString() == "")
                    {
                        MessageBox.Show("Description field needs to be populated before saving", "Description field is Blank");
                        return;
                    }
                    if (r["gl_co"] == null || r["gl_co"].ToString() == "")
                    {
                        MessageBox.Show("gl_co field needs to be populated before saving", "gl_co field is Blank");
                        return;
                    }
                    if (r["gl_center"] == null || r["gl_center"].ToString() == "")
                    {
                        MessageBox.Show("gl_center field needs to be populated before saving", "gl_center field is Blank");
                        return;
                    }
                    if (r["gl_account"] == null || r["gl_account"].ToString() == "")
                    {
                        MessageBox.Show("gl_account field needs to be populated before saving", "gl_account field is Blank");
                        return;
                    }
                    if (r["gl_product"] == null || r["gl_product"].ToString() == "")
                    {
                        MessageBox.Show("gl_product field needs to be populated before saving", "gl_product field is Blank");
                        return;
                    }
                    if (r["gl_region"] == null || r["gl_region"].ToString() == "")
                    {
                        MessageBox.Show("gl_region field needs to be populated before saving", "gl_region field is Blank");
                        return;
                    }
                    if (r["gl_intercompany"] == null || r["gl_intercompany"].ToString() == "")
                    {
                        MessageBox.Show("gl_intercompany field needs to be populated before saving", "gl_intercompany field is Blank");
                        return;
                    }
                    if (r["gl_region_desc"] == null || r["gl_region_desc"].ToString() == "")
                    {
                        MessageBox.Show("gl_region_desc field needs to be populated before saving", "gl_region_desc field is Blank");
                        return;
                    }
                    if (r["gl_product_desc"] == null || r["gl_product_desc"].ToString() == "")
                    {
                        MessageBox.Show("gl_product_desc field needs to be populated before saving", "gl_product_desc field is Blank");
                        return;
                    }

                }
            }
            base.Save();
            if (SaveSuccessful)
            {
               
                Messages.ShowInformation("Save Successful");
            }
            else
            {
                Messages.ShowInformation("Save Failed");
            }
        }
    }
}
