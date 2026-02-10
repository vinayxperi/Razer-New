using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.VisualBasic;
using System.Reflection;
using Infragistics.Windows.DataPresenter;

namespace RazerBase
{
    public static class AutoValidation
    {

        public static void ValidateDataContextChanged(this Control ctrl, object sender, DependencyPropertyChangedEventArgs e)
        {
            ctrl.AutoValidateControl();
        }

        public static void AutoValidateControl(this Control ctrl)
        {
            //var generic = new GenericAutoValidateControl(ctrl);

            switch (ctrl.GetType().FullName)
            {
                case "RazerBase.ucLabelTextBox":
                    AddControlValidation<ucLabelTextBox>(ctrl as ucLabelTextBox);
                    break;
                case "System.Windows.Controls.TextBox":
                    AddControlValidation<TextBox>(ctrl as TextBox);
                    break;
                case "RazerBase.ucLabelDateTimePicker":
                    AddControlValidation<ucLabelDateTimePicker>(ctrl as ucLabelDateTimePicker);
                    break;
                case "RazerBase.ucLabelDatePicker":
                    AddControlValidation<ucLabelDatePicker>(ctrl as ucLabelDatePicker);
                    break;
                case "RazerBase.ucLedgerTextBox":
                    AddControlValidation<ucLedgerTextBox>(ctrl as ucLedgerTextBox);
                    break;
                case "RazerBase.ucLabelMultilineTextBox":
                    AddControlValidation<ucLabelMultilineTextBox>(ctrl as ucLabelMultilineTextBox);
                    break;
                default:
                    break;
            }
        }


        private static string RemoveBrakets(string bindingPathName)
        {
            return bindingPathName.Replace("Cells[", "").Replace("].Value", "").ToString();
        }

        private static void AddControlValidation<T>(T ctrl)
        {
            //get the binding path name (datacolumn name)
            FieldInfo TextPropertyFieldInfoOfT = typeof(T).GetField("TextProperty");
            DependencyProperty dp = TextPropertyFieldInfoOfT.GetValue(null) as DependencyProperty;
            MethodInfo method = typeof(T).GetMethod("GetBindingExpression", new Type[] { typeof(DependencyProperty) });
            BindingExpression bindingExpression = (method.Invoke(ctrl, new object[] { dp }) as BindingExpression);

            if (bindingExpression == null) { return; }

            var bindingPathName = bindingExpression.ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            PropertyInfo DataContextPropertyOfT = typeof(T).GetProperty("DataContext", typeof(object));
            DataTable dt = (DataContextPropertyOfT.GetValue(ctrl, null) as DataTable);

            if (dt == null)
            {
                DataRecord dr = DataContextPropertyOfT.GetValue(ctrl, null) as DataRecord;
                if (dr != null)
                {
                    if (dr.DataItem.GetType() == typeof(DataRowView))
                    {
                        dt = ((DataRowView)dr.DataItem).Row.Table;
                    }
                }
            }

            //get the binding of the textbox
            var binding = bindingExpression.ParentBinding;

            //check for the column. Not all bindings are to DataColumns
            if (dt != null && dt.Columns[bindingPathName] != null)
            {
                //generate the bindings based on the datatable information
                CreateBindings(dt.Columns[bindingPathName], binding);
            }

            ////add custom validations registered to the control to the binding
            FieldInfo customRulesFieldInfo = typeof(T).GetField("customRules");
            var customRules = customRulesFieldInfo.GetValue(ctrl) as List<ValidationRule>;
            if (customRules != null)
            {
                foreach (var rule in customRules)
                {
                    binding.ValidationRules.Add(rule);
                }
            }
        }

        private static void AdducLabelMultilineTextBoxValidation(ucLabelMultilineTextBox ucLabelMultilineTextBox)
        {
            //get the binding path name (datacolumn name)
            var bindingPathName = ucLabelMultilineTextBox.GetBindingExpression(ucLabelMultilineTextBox.TextProperty).ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            var dt = ucLabelMultilineTextBox.DataContext as DataTable;
            if (dt == null)
            {
                dt = (ucLabelMultilineTextBox.DataContext as DataRow).Table;
            }

            //get the binding of the textbox
            var binding = ucLabelMultilineTextBox.GetBindingExpression(ucLabelMultilineTextBox.TextProperty).ParentBinding;

            //generate the bindings based on the datatable information
            CreateBindings(dt.Columns[bindingPathName], binding);

            //add custom validations registered to the control to the binding
            foreach (var rule in ucLabelMultilineTextBox.customRules)
            {
                binding.ValidationRules.Add(rule);
            }
        }

        private static void AdducLedgerTextBoxValidation(ucLedgerTextBox ucLedgerTextBox)
        {
            //get the binding path name (datacolumn name)
            var bindingPathName = ucLedgerTextBox.GetBindingExpression(ucLedgerTextBox.TextProperty).ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            var dt = ucLedgerTextBox.DataContext as DataTable;
            if (dt == null)
            {
                dt = (ucLedgerTextBox.DataContext as DataRow).Table;
            }

            //get the binding of the textbox
            var binding = ucLedgerTextBox.GetBindingExpression(ucLedgerTextBox.TextProperty).ParentBinding;

            //generate the bindings based on the datatable information
            CreateBindings(dt.Columns[bindingPathName], binding);

            //add custom validations registered to the control to the binding
            foreach (var rule in ucLedgerTextBox.customRules)
            {
                binding.ValidationRules.Add(rule);
            }
        }

        private static void AdducLabelDatePickerValidation(ucLabelDatePicker ucLabelDatePicker)
        {
            //get the binding path name (datacolumn name)
            var bindingPathName = ucLabelDatePicker.GetBindingExpression(ucLabelDatePicker.TextProperty).ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            var dt = ucLabelDatePicker.DataContext as DataTable;

            if (dt == null)
            {
                dt = (ucLabelDatePicker.DataContext as DataRow).Table;
            }
            //get the binding of the textbox
            var binding = ucLabelDatePicker.GetBindingExpression(ucLabelDatePicker.TextProperty).ParentBinding;

            //generate the bindings based on the datatable information
            CreateBindings(dt.Columns[bindingPathName], binding);

            //add custom validations registered to the control to the binding
            foreach (var rule in ucLabelDatePicker.customRules)
            {
                binding.ValidationRules.Add(rule);
            }

        }

        private static void AdducLabelTextBoxValidation(ucLabelTextBox ucLabelTextBox)
        {
            //get the binding path name (datacolumn name)
            var bindingPathName = ucLabelTextBox.GetBindingExpression(ucLabelTextBox.TextProperty).ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            var dt = ucLabelTextBox.DataContext as DataTable;

            if (dt == null)
            {
                dt = (ucLabelTextBox.DataContext as DataRow).Table;
            }
            //get the binding of the textbox
            var binding = ucLabelTextBox.GetBindingExpression(ucLabelTextBox.TextProperty).ParentBinding;

            //generate the bindings based on the datatable information
            CreateBindings(dt.Columns[bindingPathName], binding);

            //add custom validations registered to the control to the binding
            foreach (var rule in ucLabelTextBox.customRules)
            {
                binding.ValidationRules.Add(rule);
            }
        }

        private static void AddTextBoxtValidation(TextBox textBox)
        {
            //get the binding path name (datacolumn name)
            var bindingPathName = textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding.Path.Path;
            bindingPathName = RemoveBrakets(bindingPathName);

            //get the dataTable from the dataConext of the control
            var dt = textBox.DataContext as DataTable;
            if (dt == null)
            {
                dt = (textBox.DataContext as DataRow).Table;
            }

            //get the binding of the textbox
            var binding = textBox.GetBindingExpression(TextBox.TextProperty).ParentBinding;

            //generate the bindings based on the datatable information
            CreateBindings(dt.Columns[bindingPathName], binding);
        }

        private static void CreateBindings(DataColumn dataColumn, Binding binding)
        {
            switch (dataColumn.DataType.FullName)
            {
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Single":
                case "System.Double":
                    AddNumericBinding(binding);
                    break;
                case "System.String":
                    //get the max string length
                    var MaxStringLength = dataColumn.MaxLength;
                    AddMaxLengthBinding(binding, MaxStringLength);
                    break;
                case "System.DateTime":
                    AddDateTimeBinding(binding);
                    break;
                default:
                    break;
            }
        }

        private static void AddDateTimeBinding(Binding binding)
        {
            var newRule = new DateValidator();
            binding.ValidationRules.Add(newRule);
        }

        private static void AddNumericBinding(Binding binding)
        {
            var newRule = new NumericValidator();
            binding.ValidationRules.Add(newRule);
        }

        private static void AddMaxLengthBinding(Binding binding, int MaxStringLength)
        {
            var newRule = new MaxLengthValidator();
            newRule.MaxLength = MaxStringLength;
            binding.ValidationRules.Add(newRule);
        }
    }

    public class GenericAutoValidateControl
    {
        public GenericAutoValidateControl(Control ctrl)
        {
            //Type t = ctrl.GetType();

            //MethodInfo method = this.GetType().GetMethod("AddControlValidation", BindingFlags.NonPublic | BindingFlags.Static);
            //method.MakeGenericMethod(new Type[] { t });
            //method.Invoke(this, new object[] { ctrl });
        }
    }

    /// <summary>
    /// CustomValidator of type T, which is a generic type that will be specified upon instantiation.
    /// </summary>
    /// <typeparam name="T">type of object to be validated</typeparam>
    public class CustomValidator<T> : CustomValidate
    {
        /// <summary>
        /// Delegate function of type T and returns a boolean
        /// </summary>
        private Func<T, bool> Rule;

        //Error message
        private string ErrorMessage;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Rule">function of type T that returns boolean</param>
        /// <param name="ErrorMessage">custom error message</param>
        public CustomValidator(Func<T, bool> Rule, string ErrorMessage)
        {
            this.Rule = Rule;
            this.ErrorMessage = ErrorMessage;
        }

        /// <summary>
        /// Validates the value of type T
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult vr = null;

            try
            {
                //Convert object value to type T so when cast below the code will not error
                var TValue = Convert.ChangeType(value, typeof(T));

                //Cast TValue to type T so compiler won't complain
                if (Rule((T)TValue))
                {
                    vr = new ValidationResult(true, "");
                }
                else
                {
                    vr = new ValidationResult(false, string.Format(ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                String ExErrorMessage = "Error converting object to {0} for validation. Exception error: {1}";
                vr = new ValidationResult(false, string.Format(ExErrorMessage, typeof(T).FullName, ex.Message));
            }
            finally { }
            return vr;
        }
    }

    /// <summary>
    /// Validates that an object is a date
    /// </summary>
    public class DateValidator : CustomValidate
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult vr = null;
            if (Information.IsDate(value) || value == null)
            {
                vr = new ValidationResult(true, "");
            }
            else
            {
                vr = new ValidationResult(false, string.Format("This value must be a date."));
            }

            return vr;
        }
    }

    /// <summary>
    /// Validates that an object is numeric in value
    /// </summary>
    public class NumericValidator : CustomValidate
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult vr = null;
            if (Information.IsNumeric(value))
            {
                vr = new ValidationResult(true, "");
            }
            else
            {
                vr = new ValidationResult(false, string.Format("This value must be numeric."));
            }

            return vr;
        }
    }

    /// <summary>
    /// Validates the length of an object
    /// </summary>
    public class MaxLengthValidator : CustomValidate
    {
        public int MaxLength { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult vr = null;
            if ((value.ToString().Length <= MaxLength))
            {
                vr = new ValidationResult(true, "");
            }
            else
            {
                vr = new ValidationResult(false, string.Format("String must be less than or equal to {0} characters.", MaxLength.ToString()));
            }

            return vr;
        }
    }

    /// <summary>
    /// CustomValidate inherits from ValidationRule and Implements the IDataErrorInfo.
    /// The Class is the base class for the AutoValidation validators.
    /// It is abstract so it cannot be instantiated.
    /// </summary>
    public abstract class CustomValidate : ValidationRule, IDataErrorInfo
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo) { return null; }

        private Dictionary<string, string> validationErrors = new Dictionary<string, string>();

        protected void AddError(string columnName, string msg)
        {
            if (!validationErrors.ContainsKey(columnName))
            {
                validationErrors.Add(columnName, msg);
            }
        }

        protected void RemoveError(string columnName)
        {
            if (validationErrors.ContainsKey(columnName))
            {
                validationErrors.Remove(columnName);
            }
        }

        public bool HasErrors { get { return validationErrors.Count > 0; } }

        string IDataErrorInfo.Error
        {
            get
            {
                if (validationErrors.Count > 0)
                {
                    return string.Format("{0} data is invalid.", Information.TypeName(this));
                }
                else
                {
                    return null;
                }
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (validationErrors.ContainsKey(columnName))
                {
                    return validationErrors[columnName].ToString();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}