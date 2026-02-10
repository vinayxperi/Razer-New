#region using statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

#endregion


namespace RazerBase
{
    #region Delegates
    public delegate void ItemSelectedCallBack(DataRowView selectedItem);
    #endregion

    #region class BindingObject
    /// <summary>
    /// This class is used to handle the binding for lookup values
    /// </summary>
    public class BindingObject
    {

        #region Private Variables
        private DataTable sourceData;
        private string displayField;
        private string valueField;
        private string title;
        private ItemSelectedCallBack callBackMethod;
        private bool lookupAsString;
        #endregion

        #region Constructors

        #region Default Constructor
        /// <summary>
        /// Create a new instance of a BindingObject.
        /// </summary>
        public BindingObject()
        {
            // perform initializations for this object
            Init();
        }
        #endregion

        #region Parameterized Constructor
        /// <summary>
        /// Create a new instance of a 'BindingObject' and set its properties
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="displayField"></param>
        /// <param name="valueField"></param>
        public BindingObject(DataTable sourceData, string displayField, string valueField)
        {
            // perform initializations for this object
            Init();

            // set the properties passed in
            this.SourceData = sourceData;
            this.DisplayField = displayField;
            this.ValueField = valueField;
        }
        #endregion

        #endregion

        #region Methods

        #region GetKeyValue(string filterText, ref int itemCount)
        /// <summary>
        /// This method finds the value when the Key is a String;
        /// For integer Keys use the Get Value
        /// </summary>
        internal string GetKeyValue(string filterText, ref int itemCount)
        {
            // initial value
            string keyValue = "";

            try
            {
                // if the SourceData exists
                if (this.HasSourceData)
                {
                    // get the results
                    var results = from dataTable in this.SourceData.AsEnumerable()
                                  where

                                      // add the where clause
                                      dataTable.Field<string>(this.DisplayField).IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0

                                  // select statement
                                  select dataTable;

                    // set the return value
                    itemCount = results.Count();

                    // if exactly one record was found
                    if (itemCount == 1)
                    {
                        // set the return value
                        DataTable resultsTable = results.CopyToDataTable();

                        // if the resultsTable exists
                        if (resultsTable != null)
                        {
                            // Get the current dataRow
                            DataRowView dataRow = resultsTable.DefaultView[0];

                            // set the return value
                            keyValue = dataRow[this.ValueField].ToString();
                        }
                    }
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();

                // set the return value to "Error" to indicate an error occurred
                keyValue = "Error";
            }

            // return value
            return keyValue;
        }
        #endregion

        #region GetTextDisplay(object value)
        /// <summary>
        /// This method is used to find the Text record for the value passed in.
        /// </summary>
        public string GetTextDisplay(object value)
        {
            // initial value
            string textDisplay = "";

            try
            {
                // if the SourceData exists
                if ((this.HasSourceData) && (value != null))
                {
                    // if we the binding is using a string key
                    if (this.LookupAsString)
                    {
                        // get a string value value
                        string stringValue = value.ToString();

                        // make sure the cast was successful
                        if (!String.IsNullOrEmpty(stringValue))
                        {
                            // get the results
                            var results = from dataTable in this.SourceData.AsEnumerable()

                                          // add the where clause
                                          where dataTable.Field<string>(this.ValueField) == stringValue

                                          // select statement 
                                          select dataTable;

                            // if exactly one record was found
                            if (results.Count() == 1)
                            {
                                // set the return value
                                DataTable resultsTable = results.CopyToDataTable();

                                // if the resultsTable exists
                                if (resultsTable != null)
                                {
                                    // Get the current dataRow
                                    DataRowView dataRow = resultsTable.DefaultView[0];

                                    // set the return value
                                    textDisplay = dataRow[this.DisplayField].ToString();
                                }
                            }
                        }
                    }
                    else
                    {
                        //Removed KSH 9/26/2011 to allow 0 values to bind to objects
                        // get an integer value for value
                        //int intValue = Convert.ToInt32(value);
                        // make sure the cast was successful
                        //if (intValue > 0)
                        int intTest = -1;
                        if (Int32.TryParse(value.ToString(), out intTest))
                        //////////////////////////////////////////////////////////////
                        {
                            switch (value.GetType().FullName)
                            {
                                case "System.Byte":
                                    textDisplay = ParseNumericForGetTextDisplay<System.Byte>(value);
                                    break;
                                case "System.Int32":
                                    textDisplay = ParseNumericForGetTextDisplay<System.Int32>(value);
                                    break;
                                case "System.Decimal":
                                    textDisplay = ParseNumericForGetTextDisplay<System.Decimal>(value);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();

                // set the return value to 'Error" so we know an error occurred
                textDisplay = "Error";
            }

            // return value
            return textDisplay;
        }

        private string ParseNumericForGetTextDisplay<T>(object value)
        {
            string textDisplay = "";
            //Convert object value to type T so when cast below the code will not error
            var TValue = Convert.ChangeType(value, typeof(T));

            // get the results
            var results = from dataTable in this.SourceData.AsEnumerable()

                          // add the where clause
                          where dataTable.Field<T>(this.ValueField).Equals((T)TValue)

                          // select statement 
                          select dataTable;

            // if exactly one record was found
            if (results.Count() == 1)
            {
                // set the return value
                DataTable resultsTable = results.CopyToDataTable();

                // if the resultsTable exists
                if (resultsTable != null)
                {
                    // Get the current dataRow
                    DataRowView dataRow = resultsTable.DefaultView[0];

                    // set the return value
                    textDisplay = dataRow[this.DisplayField].ToString();
                }
            }
            return textDisplay;
        }
        #endregion

        #region GetValue(string filterText, ref int itemCount)
        /// <summary>
        /// This method returns the Value for the corresponding item
        /// in the Source Data Table
        /// </summary>
        public int GetValue(string filterText, ref int itemCount)
        {
            // initial value
            int value = 0;

            try
            {
                // if the SourceData exists
                if (this.HasSourceData)
                {
                    // get the results
                    var results = from dataTable in this.SourceData.AsEnumerable()
                                  where

                                      // add the where clause
                                      dataTable.Field<string>(this.DisplayField).IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0

                                  // select statement
                                  select dataTable;

                    // set the return value
                    itemCount = results.Count();

                    // if exactly one record was found
                    if (itemCount == 1)
                    {
                        // set the return value
                        DataTable resultsTable = results.CopyToDataTable();

                        // if the resultsTable exists
                        if (resultsTable != null)
                        {
                            // Get the current dataRow
                            DataRowView dataRow = resultsTable.DefaultView[0];

                            // set the return value
                            value = Convert.ToInt32(dataRow[this.ValueField]);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                // for debugging only
                string err = error.ToString();

                // set the return value to -1 to indicate an error occurred
                value = -1;
            }

            // return value
            return value;
        }
        #endregion

        #region Init()
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init()
        {
            // default to false 
            this.LookupAsString = false;
        }
        #endregion

        #endregion

        #region Properties

        #region CallBackMethod
        /// <summary>
        /// This property gets or sets the value for 'CallBackMethod'.
        /// </summary>
        public ItemSelectedCallBack CallBackMethod
        {
            get { return callBackMethod; }
            set { callBackMethod = value; }
        }
        #endregion

        #region DisplayField
        /// <summary>
        /// This property gets or sets the value for 'DisplayField'.
        /// </summary>
        public string DisplayField
        {
            get { return displayField; }
            set { displayField = value; }
        }
        #endregion

        #region HasCallBackMethod
        /// <summary>
        /// This property returns true if this object has a 'CallBackMethod'.
        /// </summary>
        public bool HasCallBackMethod
        {
            get
            {
                // initial value
                bool hasCallBackMethod = (this.CallBackMethod != null);

                // return value
                return hasCallBackMethod;
            }
        }
        #endregion

        #region HasDisplayField
        /// <summary>
        /// This property returns true if the 'DisplayField' exists.
        /// </summary>
        public bool HasDisplayField
        {
            get
            {
                // initial value
                bool hasDisplayField = (!String.IsNullOrEmpty(this.DisplayField));

                // return value
                return hasDisplayField;
            }
        }
        #endregion

        #region HasSourceData
        /// <summary>
        /// This property returns true if this object has a 'SourceData'.
        /// </summary>
        public bool HasSourceData
        {
            get
            {
                // initial value
                bool hasSourceData = (this.SourceData != null);

                // return value
                return hasSourceData;
            }
        }
        #endregion

        #region HasTitle
        /// <summary>
        /// This property returns true if the 'Title' exists.
        /// </summary>
        public bool HasTitle
        {
            get
            {
                // initial value
                bool hasTitle = (!String.IsNullOrEmpty(this.Title));

                // return value
                return hasTitle;
            }
        }
        #endregion

        #region HasValueField
        /// <summary>
        /// This property returns true if the 'ValueField' exists.
        /// </summary>
        public bool HasValueField
        {
            get
            {
                // initial value
                bool hasValueField = (!String.IsNullOrEmpty(this.ValueField));

                // return value
                return hasValueField;
            }
        }
        #endregion

        #region LookupAsString
        /// <summary>
        /// This property gets or sets the value for 'LookupAsString'.
        /// </summary>
        public bool LookupAsString
        {
            get { return lookupAsString; }
            set { lookupAsString = value; }
        }
        #endregion

        #region SourceData
        /// <summary>
        /// This property gets or sets the value for 'SourceData'.
        /// </summary>
        public DataTable SourceData
        {
            get { return sourceData; }
            set { sourceData = value; }
        }
        #endregion

        #region Title
        /// <summary>
        /// This property gets or sets the value for 'Title'.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        #endregion

        #region ValueField
        /// <summary>
        /// This property gets or sets the value for 'ValueField'.
        /// </summary>
        public string ValueField
        {
            get { return valueField; }
            set { valueField = value; }
        }
        #endregion

        #endregion

    }
    #endregion
}
