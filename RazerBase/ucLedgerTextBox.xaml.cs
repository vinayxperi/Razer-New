using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RazerBase
{
    /// <summary>
    /// Interaction logic for ucLedgerTextBox.xaml
    /// </summary>
    public partial class ucLedgerTextBox : UserControl
    {
        private GridLength labelWidth;
        private string labelText;
        private const int DefaultLabelWidth = 84;
        public List<ValidationRule> customRules = new List<ValidationRule>();
        private string bindPath;

        public ucLedgerTextBox()
        {
            InitializeComponent();
            Init();
        }
        public void Init()
        {
            // Set the label width to the default label width
            this.LabelWidth = new GridLength(DefaultLabelWidth);
            EnterActsLikeTab = false;
        }

        /// <summary>
        /// This property gets or sets the value for 'LabelText'.
        /// </summary>
        public string LabelText
        {
            get { return labelText; }
            set
            {
                // set the value
                labelText = value;

                // Set the Context of the lable 
                this.Label.Content = value;
            }
        }

        /// <summary>
        /// This property gets or sets the value for 'LabelWidth'.
        /// </summary>
        public GridLength LabelWidth
        {
            get { return labelWidth; }
            set
            {
                // set the value
                labelWidth = value;

                // set the value of the control
                //this.GridLabelColumn.Width = value;
                this.Label.Width = System.Convert.ToInt32(value.ToString());
                this.TextBox.Width = System.Convert.ToInt32(value.ToString());
                this.lbBorder.Width = System.Convert.ToInt32(value.ToString());
            }
        }

        /// <summary>
        /// This property gets or sets the value for 'BindPath'.
        /// </summary>
        public string BindPath
        {
            get { return bindPath; }
            set
            {
                // set the value
                bindPath = value;

                // if the bindPath is set
                if (bindPath != null)
                {
                    // Set the value
                    Binding binding = new Binding(bindPath);

                    // Set to two way mode
                    binding.Mode = BindingMode.TwoWay;

                    // Set the update source trigger
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                    // set the binding
                    this.SetBinding(TextProperty, binding);
                }
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ucLedgerTextBox),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        /// <summary>
        /// True - Enter behaves like the tab key - False Enter behaves as normal - False is the default
        /// </summary>
        public bool EnterActsLikeTab { get; set; }

        private bool isReadOnly;
        /// <summary>
        /// Sets the readonly value of the textbox in the user control
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                TextBox.IsReadOnly = value;
            }

        }
        private int _ldTabIndex;

        public int CntrlTabIndex
        {
            get { return _ldTabIndex; }
            set
            {
                _ldTabIndex = value;
                TextBox.TabIndex = value;
            }
        }
      

        public void CntrlFocus()
        {
            TextBox.Focus();
        }
    }
}