using RazerBase.Interfaces;
using RazerBase;
using RazerBase.Lookups;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using Infragistics.Windows.DockManager;
using System;

namespace COLA
{
    /// <summary>
    /// Interaction logic for COLAFolderMainScreeen.xaml
    /// </summary>
    public partial class COLAMainScreen : ScreenBase, IScreen
    {
        private string windowCaption;
        public string WindowCaption
        {
            get { return windowCaption; }
        }

        /// <summary>
        /// This constructor creates a new instance of a 'FolderMainScreen' object.
        /// The ScreenBaase constructor is also called.
        /// </summary>
        public COLAMainScreen()
            : base()
        {
            // Create Controls
            InitializeComponent();

            // performs initializations for this object.

        }
        
        /// <summary>
        /// This method performs initializations for this object.
        /// </summary>
        public void Init(cBaseBusObject businessObject)
        {
            // Set the ScreenBaseType
            this.ScreenBaseType = ScreenBaseTypeEnum.Folder;
            this.DoNotSetDataContext = true;
            //Set the maintablename for the folder if it has one
            this.MainTableName = "ColaCandidates";
            // set the businessObject - This object will be used by default for all tabs and grids in the folder
            this.CurrentBusObj = businessObject;
            // add the Tab user controls that are of type screen base
            TabCollection.Add(uGeneral);
            this.Load();
            //this.CanExecuteSaveCommand = true;
         }

        #region Save Logic
        ///// <summary>
        /// Override of save method handles save functionality for folder
        /// </summary>
        public override void Save()
        {
            this.PrepareFreeformForSave();
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
        #endregion
    }
}
