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
using System.Collections.ObjectModel;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;

namespace Units
{
    /// <summary>
    /// Interaction logic for UnitUploadVerificationLookup.xaml
    /// </summary>
    /// 
    public partial class UnitUploadVerificationLookup : DialogBase
    {
        public ObservableCollection<Unit> Units { get; set; }
        public UnitVerificaiton UnitVerificaiton { get; set; }
        public List<Unit> FilteredUnits = new List<Unit>();
        public bool CanUpload { get { return Upload.IsEnabled; } }
        public FieldLayoutCollection FieldLayouts { get; set; }
        private bool FieldLayoutSet = false;

        public UnitUploadVerificationLookup(ObservableCollection<Unit> Units, UnitVerificaiton UnitVerificaiton, FieldLayoutCollection FieldLayouts)
        {
            InitializeComponent();
            this.Units = Units;
            this.UnitVerificaiton = UnitVerificaiton;
            this.FieldLayouts = FieldLayouts;
            INIT();
        }

        private void INIT()
        {
            BuildReport();
            LoadGrid();
            ClearAll();
        }

        private void ClearAll()
        {
            MultiOps.IsEnabled = false;
            Validation.IsEnabled = false;
            Exceptions.IsEnabled = false;

            MultiOps.Visibility = System.Windows.Visibility.Collapsed;
            Validation.Visibility = System.Windows.Visibility.Collapsed;
            Exceptions.Visibility = System.Windows.Visibility.Collapsed;

            clearMultiOps();
            clearExceptionTab();
            clearValidationTab();
        }

        private void LoadGrid()
        {
            int NonModelExceptionCount = 0;
            FilteredUnits = new List<Unit>();

            var xGrid = FilteredGrid.xGrid;
            xGrid.FieldLayoutSettings.SelectionTypeRecord = SelectionType.Single;
            xGrid.FieldLayoutSettings.MaxSelectedRecords = 1;
            xGrid.FieldLayoutSettings.AllowFieldMoving = Infragistics.Windows.DataPresenter.AllowFieldMoving.No;
            xGrid.FieldLayoutSettings.AllowDelete = false;
            xGrid.FieldLayoutSettings.AllowAddNew = false;

            var UnitsWithIssues = (from x in Units
                                   where x.HasCorrections == true
                                   select x).ToList();

            foreach (Unit Unit in UnitsWithIssues)
            {
                int x = (from i in Unit.UploadCorrections
                         where i.HasMultiplePossibleMatches == true ||
                         i.HasUnreconciledData == true ||
                         i.HasValidationError == true
                         select i).Count();

                if (x != 0)
                {
                    FilteredUnits.Add(Unit);
                }

                x = (from i in Unit.UploadCorrections
                     where !(i.HasUnreconciledData==true && (i.PropertyName=="model" || i.PropertyName=="brand"))
                     select i).Count();

                NonModelExceptionCount += x;
            }

            SetActiveTab();

            if (NonModelExceptionCount == 0) 
            {
                Upload.IsEnabled = true;
                Upload.IsSelected = true;
            }

            xGrid.DataItems.Clear();

            if (!FieldLayoutSet)
            {
                FieldLayoutSet = true;
                xGrid.FieldLayouts.Clear();

                foreach (FieldLayout fieldLayout in this.FieldLayouts)
                {
                    FieldLayout newFieldLayout = new FieldLayout();

                    foreach (Field field in fieldLayout.Fields)
                    {
                        Field newField = new Field(field.Name, field.DataType, field.Label);
                        newField.Settings.EditAsType = field.Settings.EditAsType;
                        newFieldLayout.Fields.Add(newField);
                    }

                    xGrid.FieldLayouts.Add(newFieldLayout);
                }
            }
            
            xGrid.DataSource = FilteredUnits;
        }

        private void SetActiveTab()
        {
            if (Validation.IsEnabled) { Validation.IsSelected = true; }
            if (Exceptions.IsEnabled) { Exceptions.IsSelected = true; }
            if (MultiOps.IsEnabled) { MultiOps.IsSelected = true; }
        }

        private void BuildReport()
        {
            StringBuilder reportSB = new StringBuilder("Exceptions Report:");
            List<string> sAutoCorrection = new List<string>();
            List<string> sUnreconciledData = new List<string>();
            List<string> sMultiplePossibleMatches = new List<string>();
            List<string> sValidationErrors = new List<string>();

            List<UploadCorrection> AutoCorrectedValues = new List<UploadCorrection>();
            List<UploadCorrection> UnreconciledData = new List<UploadCorrection>();
            List<UploadCorrection> MultiplePossibleMatches = new List<UploadCorrection>();
            List<UploadCorrection> ValidationErrors = new List<UploadCorrection>();

            //find Autocorrected values
            foreach (Unit Unit in Units)
            {
                if (Unit.HasCorrections)
                {
                    AutoCorrectedValues.AddRange((from x in Unit.UploadCorrections where x.AutoCorrected == true select x).ToList());
                    UnreconciledData.AddRange((from x in Unit.UploadCorrections where x.HasUnreconciledData == true select x).ToList());
                    MultiplePossibleMatches.AddRange((from x in Unit.UploadCorrections where x.HasMultiplePossibleMatches == true select x).ToList());
                    ValidationErrors.AddRange((from x in Unit.UploadCorrections where x.HasValidationError == true select x).ToList());
                }
            }

            if (AutoCorrectedValues.Count != 0)
            {
                reportSB.AppendLine("");
                reportSB.AppendLine("");
                reportSB.AppendLine("Auto Corrected Values:");
                string act = "Property Name: {0}  Original Value: {1}  Corrected Value: {2}";

                foreach (UploadCorrection acv in AutoCorrectedValues)
                {
                    sAutoCorrection.Add(string.Format(act, acv.PropertyName, acv.OriginalValue, acv.CorrectedValue));
                }

                foreach (string item in (from x in sAutoCorrection select x).Distinct())
                {
                    reportSB.AppendLine(item);
                }
            }

            if (UnreconciledData.Count != 0)
            {
                reportSB.AppendLine("");
                reportSB.AppendLine("");
                reportSB.AppendLine("Exceptions (Only Models and Brands will be created upon upload if not corrected):");
                string urt = "Property Name: {0}  Value: {1}";


                foreach (UploadCorrection urd in UnreconciledData)
                {
                    sUnreconciledData.Add(string.Format(urt, urd.PropertyName, urd.OriginalValue));
                }

                foreach (string item in (from x in sUnreconciledData select x).Distinct())
                {
                    reportSB.AppendLine(item);
                }
            }

            if (MultiplePossibleMatches.Count != 0)
            {
                reportSB.AppendLine("");
                reportSB.AppendLine("");
                reportSB.AppendLine("Multiple Matches:");
                string mmt = "Property Name: {0}  Value: {1}  Possible Value: {2}";

                foreach (UploadCorrection mm in MultiplePossibleMatches)
                {
                    sMultiplePossibleMatches.Add(string.Format(mmt, mm.PropertyName, mm.OriginalValue, mm.CorrectedValue));
                }

                foreach (string item in (from x in sMultiplePossibleMatches select x).Distinct())
                {
                    reportSB.AppendLine(item);
                }
            }

            if (ValidationErrors.Count != 0)
            {
                reportSB.AppendLine("");
                reportSB.AppendLine("");
                reportSB.AppendLine("Validation Errors:");
                string vet = "Property Name: {0}  Value: {1}  Validation Issue: {2}";

                foreach (UploadCorrection ve in ValidationErrors)
                {
                    sValidationErrors.Add(string.Format(vet, ve.PropertyName, ve.OriginalValue, ve.CorrectedValue));
                }

                foreach (string item in (from x in sValidationErrors select x).Distinct())
                {
                    reportSB.AppendLine(item);
                }
            }

            txtReport.Text = reportSB.ToString();
        }

        private void FilteredGrid_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ClearAll();
            var prop = e.PropertyName;
            switch (e.PropertyName)
            {
                case "ActiveRecord":
                    Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;

                    //multiops
                    int x = (from i in Unit.UploadCorrections where i.HasMultiplePossibleMatches == true select i).Count();
                    if (x == 0)
                    {
                        MultiOps.Visibility = System.Windows.Visibility.Collapsed;
                        MultiOps.IsEnabled = false;
                    }
                    else
                    {
                        MultiOps.Visibility = System.Windows.Visibility.Visible;
                        MultiOps.IsEnabled = true;
                        MultiOpsCombo.ItemsSource = (from i in Unit.UploadCorrections where i.HasMultiplePossibleMatches == true select i.PropertyName).Distinct().ToList();
                    }

                    //Validation
                    x = (from i in Unit.UploadCorrections where i.HasValidationError == true select i).Count();
                    if (x == 0)
                    {
                        Validation.Visibility = System.Windows.Visibility.Collapsed;
                        Validation.IsEnabled = false;
                    }
                    else
                    {
                        Validation.Visibility = System.Windows.Visibility.Visible;
                        Validation.IsEnabled = true;
                        ValidationPropertyCombo.ItemsSource = (from i in Unit.UploadCorrections where i.HasValidationError == true select i.PropertyName).Distinct().ToList();
                    }

                    //Exception
                    x = (from i in Unit.UploadCorrections where i.HasUnreconciledData == true select i).Count();
                    if (x == 0)
                    {
                        Exceptions.Visibility = System.Windows.Visibility.Collapsed;
                        Exceptions.IsEnabled = false;
                    }
                    else
                    {
                        Exceptions.Visibility = System.Windows.Visibility.Visible;
                        Exceptions.IsEnabled = true;
                        ExceptionPropertyCombo.ItemsSource = (from i in Unit.UploadCorrections where i.HasUnreconciledData == true select i.PropertyName).Distinct().ToList();
                    }

                    break;
                default:
                    break;
            }
            SetActiveTab();
        }

        private void MultiOpsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MultiOpsCombo.ItemsSource != null)
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                txtMultiOpsOrig.Text = (from i in Unit.UploadCorrections where i.HasMultiplePossibleMatches == true select i.OriginalValue).Distinct().Single();
                MultiOpsPotential.ItemsSource = (from i in Unit.UploadCorrections where i.HasMultiplePossibleMatches == true && i.PropertyName == MultiOpsCombo.SelectedValue.ToString() select i.CorrectedValue).ToList();

            }
        }

        private void clearMultiOps()
        {
            txtMultiOpsOrig.Text = "";
            MultiOpsPotential.ItemsSource = null;
            MultiOpsCombo.ItemsSource = null;
        }

        private void clearValidationTab()
        {
            txtValidationNewValue.Text = "";
            txtValidationOrig.Text = "";
            ValidationPropertyCombo.ItemsSource = null;
        }

        private void clearExceptionTab()
        {
            txtExceptionNewValue.Text = "";
            txtExceptionOrig.Text = "";
            ExceptionPropertyCombo.ItemsSource = null;
        }

        private void btnMultiOpsSave_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredGrid.ActiveRecord != null && FilteredGrid.ActiveRecord.DataItem != null && MultiOpsCombo.SelectedValue != null && MultiOpsPotential.SelectedValue != null)
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                var propertyName = MultiOpsCombo.SelectedValue.ToString();
                var x = (from i in Unit.UploadCorrections where i.HasMultiplePossibleMatches == true && i.PropertyName == propertyName select i).ToList();

                foreach (UploadCorrection item in x)
                {
                    Unit.UploadCorrections.Remove(item);
                }

                var upc = new UploadCorrection
                    {
                        PropertyName = propertyName,
                        ManuallyCorrected = true,
                        ManuallyCorrectedValue = MultiOpsPotential.SelectedItem.ToString()
                    };

                Unit.UploadCorrections.Add(upc);

                UnitVerificaiton.Verify(Unit, new List<string>() { propertyName }, false, upc);
                LoadGrid();
                ClearAll();
            }
        }

        private void btnExceptionSave_Click(object sender, RoutedEventArgs e)
        {
            if (ExceptionPropertyCombo.SelectedValue != null && !string.IsNullOrEmpty(txtExceptionNewValue.Text))
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                var propertyName = ExceptionPropertyCombo.SelectedValue.ToString();

                if ((bool)ExceptionApplyToAll.IsChecked)
                {
                    string original_value = (from i in Unit.UploadCorrections where i.HasUnreconciledData == true && i.PropertyName == propertyName select i.OriginalValue).Single();

                    foreach (Unit FilteredItem in FilteredUnits)
                    {
                        var x = (from i in FilteredItem.UploadCorrections where i.HasUnreconciledData == true && i.PropertyName == propertyName && i.OriginalValue == original_value select i).ToList();

                        if (x != null && x.Count() != 0)
                        {
                            ExceptionCorrection(propertyName, FilteredItem, x);
                        }
                    }
                }
                else
                {
                    var x = (from i in Unit.UploadCorrections where i.HasUnreconciledData == true && i.PropertyName == propertyName select i).ToList();

                    ExceptionCorrection(propertyName, Unit, x);
                }

                LoadGrid();
                ClearAll();
            }
        }

        private void ExceptionCorrection(string propertyName, Unit FilteredItem, List<UploadCorrection> x)
        {
            foreach (UploadCorrection item in x)
            {
                FilteredItem.UploadCorrections.Remove(item);
            }

            SetPropertyValue(FilteredItem, propertyName.Replace(" ", "_"), txtExceptionNewValue.Text);

            UnitVerificaiton.Verify(FilteredItem, new List<string>() { propertyName }, false);
        }

        private void SetPropertyValue(Unit Unit, string propertyName, string newValue)
        {
            Type type = Unit.GetType();

            if (!string.Equals(newValue.Trim(), Unit.GetPropertyValue(propertyName).ToString().Trim(), StringComparison.OrdinalIgnoreCase))
            {
                switch (Unit.GetPropertyValue(propertyName).GetType().Name.ToLower())
                {
                    case "string":
                        Unit.SetProertyValue<string>(propertyName, newValue, type);
                        break;
                    case "decimal":
                        Unit.SetProertyValue<decimal>(propertyName, newValue, type);
                        break;
                    case "datetime":
                        Unit.SetProertyValue<DateTime>(propertyName, newValue, type);
                        break;
                    case "bool":
                        Unit.SetProertyValue<bool>(propertyName, newValue, type);
                        break;
                    case "int32":
                        Unit.SetProertyValue<int>(propertyName, newValue, type);
                        break;
                    default:
                        break;
                }
            }
        }

        private void btnValidationSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationPropertyCombo.SelectedValue != null && !string.IsNullOrEmpty(txtValidationNewValue.Text))
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                var propertyName = ValidationPropertyCombo.SelectedValue.ToString();
                if ((bool)ValidationApplyToAll.IsChecked)
                {
                    string original_value = (from i in Unit.UploadCorrections where i.HasValidationError == true && i.PropertyName == propertyName select i.OriginalValue).Single();

                    foreach (Unit FilteredItem in FilteredUnits)
                    {
                        var x = (from i in FilteredItem.UploadCorrections where i.HasValidationError == true && i.PropertyName == propertyName && i.OriginalValue == original_value select i).ToList();

                        if (x != null && x.Count() != 0)
                        {
                            ValidationCorrection(FilteredItem, propertyName, x);
                        }
                    }
                }
                else
                {
                    var x = (from i in Unit.UploadCorrections where i.HasValidationError == true && i.PropertyName == propertyName select i).ToList();

                    ValidationCorrection(Unit, propertyName, x);
                }
                LoadGrid();
                ClearAll();
            }
        }

        private void ValidationCorrection(Unit Unit, string propertyName, List<UploadCorrection> x)
        {
            foreach (UploadCorrection item in x)
            {
                Unit.UploadCorrections.Remove(item);
            }

            SetPropertyValue(Unit, propertyName.Replace(" ", "_"), txtValidationNewValue.Text);

            string validationMessage = Unit.ValidateProperty(propertyName.Replace(" ", "_"));
            if (!string.IsNullOrEmpty(validationMessage))
            {
                Unit.UploadCorrections.Add(
                    new UploadCorrection
                    {
                        PropertyName = propertyName,
                        HasValidationError = true,
                        OriginalValue = Unit.GetPropertyValue(propertyName).ToString(),
                        CorrectedValue = validationMessage
                    });
            }

            UnitVerificaiton.Verify(Unit, new List<string>() { propertyName }, false);
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ExceptionPropertyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExceptionPropertyCombo.ItemsSource != null)
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                txtExceptionOrig.Text = (from i in Unit.UploadCorrections
                                         where i.HasUnreconciledData == true
                                         && i.PropertyName == ExceptionPropertyCombo.SelectedValue.ToString()
                                         select i.OriginalValue).Distinct().Single().Trim();
            }
        }

        private void ValidationPropertyCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValidationPropertyCombo.ItemsSource != null)
            {
                Unit Unit = FilteredGrid.ActiveRecord.DataItem as Unit;
                txtValidationOrig.Text = (from i in Unit.UploadCorrections
                                          where i.HasValidationError == true
                                          && i.PropertyName == ValidationPropertyCombo.SelectedValue.ToString()
                                          select i.OriginalValue).Distinct().Single().Trim();
            }
        }
    }
}
