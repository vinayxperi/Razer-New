using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System;

namespace RazerBase
{
    public static class ApplicationSecurity
    {
        //Field Level Security is implemented using DependencyProperty

        #region Field Level Security

        public static string GetFieldLevelSecurityGUID(DependencyObject obj)
        {
            return (string)obj.GetValue(FieldLevelSecurityGUIDProperty);
        }

        public static void SetFieldLevelSecurityGUID(DependencyObject obj, string value)
        {
            obj.SetValue(FieldLevelSecurityGUIDProperty, value);
        }

        // Using a DependencyProperty as the backing store for FieldLevelSecurityGUID.
        public static readonly DependencyProperty FieldLevelSecurityGUIDProperty =
            DependencyProperty.RegisterAttached("FieldLevelSecurityGUID", typeof(string), typeof(ApplicationSecurity), new UIPropertyMetadata(string.Empty, OnFieldContextNameChanged));

        private static void OnFieldContextNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ApplicationSecurityManager.EvaluateField(obj, e.NewValue);
        }

        #endregion
    }

    public static class ApplicationSecurityManager
    {
        /// <summary>
        /// Field level Security, gets type of object, checks sec, and sets vis level
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="p"></param>
        internal static void EvaluateField(DependencyObject obj, object p)
        {
            var controlSecurity = GetControlVisibility(Convert.ToInt32(p), "FIELD");

            if (!cGlobals.SetSecurity)
            {
                controlSecurity.SetAccessLevel(3);
            }

            switch (obj.GetType().FullName) //Get the type of object
            {
                case "System.Windows.Controls.TextBox":
                    var txtBox = obj as System.Windows.Controls.TextBox;
                    txtBox.Visibility = controlSecurity.ControlVisibility;
                    txtBox.IsReadOnly = controlSecurity.IsReadOnly();
                    break;
                case"RazerBase.ucLabelTextBox":
                    var _ucLabelTextBox = obj as RazerBase.ucLabelTextBox;
                    _ucLabelTextBox.Visibility = controlSecurity.ControlVisibility;
                    _ucLabelTextBox.IsReadOnly = controlSecurity.IsReadOnly();
                    break;
                case "RazerBase.ucLabelCheckBox":
                    var _ucLabelCheckBox = obj as RazerBase.ucLabelCheckBox;
                    _ucLabelCheckBox.Visibility = controlSecurity.ControlVisibility;
                    _ucLabelCheckBox.IsEnabled = controlSecurity.IsEnabled();
                    break;
                case "RazerBase.ucLabelDatePicker":
                    var _ucLabelDatePicker = obj as RazerBase.ucLabelDatePicker;
                    _ucLabelDatePicker.Visibility = controlSecurity.ControlVisibility;
                    _ucLabelDatePicker.IsReadOnly = controlSecurity.IsReadOnly();
                    break;
                case "RazerBase.ucLabelMultilineTextBox":
                    var _ucLabelMultiLineTextBox = obj as RazerBase.ucLabelMultilineTextBox;
                    _ucLabelMultiLineTextBox.Visibility = controlSecurity.ControlVisibility;
                    _ucLabelMultiLineTextBox.IsEnabled = controlSecurity.IsEnabled();
                    break;
                case "System.Windows.Controls.Label":
                    var lblCtrl = obj as System.Windows.Controls.Label;
                    lblCtrl.Visibility = controlSecurity.ControlVisibility;
                    break;
                case "System.Windows.Controls.RowDefinition":
                    var RowDef = obj as System.Windows.Controls.RowDefinition;
                    GridLength gval = new GridLength(0);
                    RowDef.Height = gval;
                    break;
                case "Infragistics.Windows.DataPresenter.Field":
                    var field = obj as Infragistics.Windows.DataPresenter.Field;
                    field.Visibility = controlSecurity.ControlVisibility;
                    break;
                case "System.Windows.Controls.MenuItem":
                    var menuItem = obj as System.Windows.Controls.MenuItem;
                    menuItem.Visibility = controlSecurity.ControlVisibility;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// returns AccessLevel enum to caller and sets global FlashSecurity to bubble up to cCommandLibrary
        /// </summary>
        /// <param name="objName">Object to check</param>
        /// <returns>AccessLevel Enum</returns>
        public static ControlSecurity ObjAccess(string objName, out bool objectFound, int? ParentObjectID = null)
        {
            //Set sec obj
            ControlSecurity ControlSec = GetControlVisibility(objName, out objectFound, ParentObjectID);
            if (cGlobals.SetSecurity)
            {
                //Set global flash sec
                cGlobals.FlashSecurity = ControlSec.AccessLevel;
            }
            else
            {
                ControlSec.SetAccessLevel(3);
            }
            return ControlSec;
        }

        /// <summary>
        /// overload 1 reads security table and returns controlSecurity object to be used in determining various levels of security
        /// </summary>
        /// <param name="objID">int32 objID (i.e., 157, 180, 3000)</param>
        /// <param name="objType">string objType (i.e., WINDOW, TAB, FOLDER)</param>
        /// <returns></returns>
        static ControlSecurity GetControlVisibility(Int32 objID, string objType)
        {
            var controlSecurity = new ControlSecurity();
            IEnumerable<DataRow> query = (from rows in cGlobals.SecurityDT.AsEnumerable()
                                          where
                                             rows.Field<int>("object_id") == Convert.ToInt32(objID)
                                             && rows.Field<string>("object_type") == objType
                                          select rows);

            if (query.Count() != 0)
            {
                var qry = query.Take(1).Single();

                controlSecurity.GUID = objID.ToString();
                controlSecurity.ObjectType = qry.Field<string>("object_type");
                controlSecurity.ObjectName = qry.Field<string>("object_name");
                controlSecurity.ObjectID = qry.Field<int>("object_id");
                controlSecurity.MenuSelection = qry.Field<string>("menu_selection");
                controlSecurity.ParentID = qry.Field<int?>("parent_id");
                controlSecurity.SetAccessLevel(qry.Field<int>("access_level"));
            }

            return controlSecurity;
        }

        /// <summary>
        /// overload 2 reads security_object table and returns controlSecurity object to be used in determining various levels of security
        /// </summary>
        /// <param name="objID">string objName (i.e., RecvAcct, w_collections, gl_vchr_gener_tab)</param>
        /// <param name="objType">string objType (i.e., WINDOW, TAB, FOLDER)</param>
        /// <returns></returns>
        static ControlSecurity GetControlVisibility(string objName, string objType, out bool objectFound)
        {
            var controlSecurity = new ControlSecurity();
            IEnumerable<DataRow> query = (from rows in cGlobals.SecurityDT.AsEnumerable()
                                          where
                                             rows.Field<string>("object_name") == objName
                                             && rows.Field<string>("object_type") == objType
                                          select rows);

            if (query.Count() != 0)
            {
                objectFound = true;

                var qry = query.Take(1).Single();

                controlSecurity.GUID = objName;
                controlSecurity.ObjectType = qry.Field<string>("object_type");
                controlSecurity.ObjectName = qry.Field<string>("object_name");
                controlSecurity.ObjectID = qry.Field<int>("object_id");
                controlSecurity.MenuSelection = qry.Field<string>("menu_selection");
                controlSecurity.ParentID = qry.Field<int?>("parent_id");
                controlSecurity.SetAccessLevel(qry.Field<int>("access_level"));
            }
            else
            {
                objectFound = false;
            }


            return controlSecurity;
        }

        /// <summary>
        /// overload 3 reads security_object table and returns controlSecurity object to be used in determining various levels of security
        /// </summary>
        /// <param name="objID">string objName (i.e., RecvAcct, w_collections, gl_vchr_gener_tab)</param>
        /// <returns></returns>
        static ControlSecurity GetControlVisibility(string objName, out bool objectFound, int? ParentObjectID = null)
        {
            var controlSecurity = new ControlSecurity();
            IEnumerable<DataRow> query = (from rows in cGlobals.SecurityDT.AsEnumerable()
                                          where
                                             rows.Field<string>("object_name") == objName
                                          select rows);

            if (ParentObjectID != null)
            {
                query = (from rows in query
                         where rows.Field<int?>("parent_id") == ParentObjectID
                         select rows);
            }

            if (query.Count() != 0)
            {
                objectFound = true;
                var qry = query.Take(1).Single();

                controlSecurity.GUID = objName;
                controlSecurity.ObjectType = qry.Field<string>("object_type");
                controlSecurity.ObjectName = qry.Field<string>("object_name");
                controlSecurity.ObjectID = qry.Field<int>("object_id");
                controlSecurity.MenuSelection = qry.Field<string>("menu_selection");
                controlSecurity.ParentID = qry.Field<int?>("parent_id");
                controlSecurity.SetAccessLevel(qry.Field<int>("access_level"));
            }
            else
            {
                objectFound = false;
            }

            return controlSecurity;
        }

        public static ControlSecurity GetMenuVisibility(string objName)
        {
            var controlSecurity = new ControlSecurity();
            if (cGlobals.SetSecurity)
            {
                IEnumerable<DataRow> query = (from rows in cGlobals.SecurityDT.AsEnumerable()
                                              where
                                                 rows.Field<string>("menu_selection") == objName
                                              select rows);

                if (query.Count() != 0)
                {
                    var qry = query.Take(1).Single();

                    controlSecurity.GUID = objName;
                    controlSecurity.ObjectType = qry.Field<string>("object_type");
                    controlSecurity.ObjectName = qry.Field<string>("object_name");
                    controlSecurity.ObjectID = qry.Field<int>("object_id");
                    controlSecurity.MenuSelection = qry.Field<string>("menu_selection");
                    controlSecurity.ParentID = qry.Field<int?>("parent_id");
                    controlSecurity.SetAccessLevel(qry.Field<int>("access_level"));
                }
                else
                {
                    controlSecurity.SetAccessLevel(0);
                }
            }
            else
            {
                controlSecurity.SetAccessLevel(3);
            }

            return controlSecurity;
        }

    }

    /// <summary>
    /// set props
    /// </summary>
    public class ControlSecurity
    {
        public string GUID { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string MenuSelection { get; set; }
        public int? ParentID { get; set; }
        public int ObjectID { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public Visibility ControlVisibility { get; set; }
        public bool UserHasUpdateAccess { get; set; }
        public bool UserHasDeleteAccess { get; set; }

        public ControlSecurity()
        {
            this.ControlVisibility = Visibility.Visible;
            this.UserHasDeleteAccess = true;
            this.UserHasUpdateAccess = true;
            this.ParentID = null;
        }

        public bool IsReadOnly()
        {
            return !(this.UserHasDeleteAccess || this.UserHasUpdateAccess);
        }

        public bool IsEnabled()
        {
            return (this.UserHasDeleteAccess || this.UserHasUpdateAccess);
        }

        public void SetAccessLevel(int iAccessLevel)
        {
            switch (iAccessLevel)
            {
                case 0:// No Access to the object
                    this.AccessLevel = AccessLevel.NoAccess;
                    this.ControlVisibility = Visibility.Collapsed;
                    this.UserHasDeleteAccess = false;
                    this.UserHasUpdateAccess = false;
                    break;
                case 1:// View Only Access
                    this.AccessLevel = AccessLevel.ViewOnly;
                    this.UserHasDeleteAccess = false;
                    this.UserHasUpdateAccess = false;
                    break;
                case 2:// View, Update, No Delete
                    this.AccessLevel = AccessLevel.ViewUpdate;
                    this.UserHasDeleteAccess = false;
                    break;
                case 3:// View, Update, Delete
                default:// View, Update, Delete
                    this.AccessLevel = AccessLevel.ViewUpdateDelete;
                    break;
            }
        }


    }

}
