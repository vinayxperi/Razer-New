//Dependency : user will need access to the appropriate network locations for the
//             file copy command to work.
using System;   
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace RazerBase
{
    public class AttachmentHelper
    {
        private cBaseBusObject CurrentBusObj = null;
        private List<string> TempStorageList = null;

        public AttachmentHelper(cBaseBusObject _businessObject)
        {
            CurrentBusObj = _businessObject;
        }

        /// <summary>
        /// checks for new attachments, gets new razer created file name, copy file from old path to new path with new razer name
        /// </summary>
        /// <param name="dtAttachments">Attachments datatable</param>
        /// <returns></returns>
        public void doAttachments(DataTable dtAttachments,List<string> attachCopyList)
        {
            TempStorageList = attachCopyList;
            string strToPath = "";
            Int32 AttachmentID = 0;
            Boolean AttachGood = false;
            //iterate through local files
            //for (int i = 0; i < cGlobals.GlobalTempStorageList.Count; i++)
            for (int i = 0; i < TempStorageList.Count; i++)
            {
                strToPath = "";
                //get the newly created name for each local file
                //dt.AsEnumerable().Where(dataRow => dataRow.RowState != DataRowState.Deleted &&
                //RES phase 3.1 have LINQ skip deleted rows in datatable
                var results = from x in dtAttachments.AsEnumerable()
                              where x.RowState != DataRowState.Deleted && x.Field<string>("filename") == System.IO.Path.GetFileName(TempStorageList[i])
                              select new
                              {
                                  prodFilename = x.Field<string>("prod_filename"),
                                  locationId = x.Field<int>("location_id"),
                                  attachmentId = x.Field<int>("attachment_id")
                              };
                foreach (var newFileName in results)
                {
                    //it is possible the same file is in the grid more than once.  In this case the old file could be diff from new file
                    //even though names are the same so copy anyway.
                    string RazerCreatedFileName = newFileName.prodFilename;
                    AttachmentID = newFileName.attachmentId;
                    //strToPath = @"\laser_staging\attachments\rec_account" + RazerCreatedFileName;
                    strToPath = getDBFileLocationPath(newFileName.locationId.ToString()) + "\\" + RazerCreatedFileName;
                }
                //check for path
                if (strToPath != "")
                {
                    try
                    {
                        //copy attachment files
                        System.IO.File.Copy(TempStorageList[i], strToPath, true);
                    }
                    catch (Exception ex)
                    {
                        //log the error/////
                        //TODO : logging code here
                        ///////////////////
                        Messages.ShowError(ex.Message);
                    }
                    //RES 12/15/15 verify Copy command succeeded
                    //if (!System.IO.File.Exists(strToPath))
                    //{
                    //    Messages.ShowError("Attach File " + TempStorageList[i] + " failed!  Check for special characters like foreign language in subject.");
                    //    for (int j = 0; j < dtAttachments.Rows.Count; j++)
                    //    {
                    //        if (Convert.ToInt32(dtAttachments.Rows[j]["attachment_id"]) == AttachmentID)
                    //        {
                    //            dtAttachments.Rows[j].Delete();
                    //            //break;
                    //        }
                    //    }
                    //}
                    //else
                        AttachGood = true;
                        //Messages.ShowInformation("Attachments Saved");
                }
                //else
                //{
                //    Messages.ShowError("Attach File " + TempStorageList[i] + " failed!  Check for special characters like foreign language in subject.");
                   
                //    for (int j = 0; j < dtAttachments.Rows.Count; j++)
                //    {
                //        strToPath = getDBFileLocationPath(dtAttachments.Rows[j]["location_id"].ToString()) + "\\" + dtAttachments.Rows[j]["prod_filename"].ToString();
                //        if (!System.IO.File.Exists(strToPath))
                //        //if (Convert.ToInt32(dtAttachments.Rows[j]["attachment_id"]) == AttachmentID)
                //        {
                //            dtAttachments.Rows[j].Delete();
                //            break;
                //        }
                //        else
                //            AttachGood = true;
                //    }
                //}
            }
            if (AttachGood)
                Messages.ShowInformation("Attachments Saved");
        }

        /// <summary>
        /// gets razer created file name, copy file from old path to new path with new razer name
        /// </summary>
        /// <param name="dtAttachments">Attachments datatable</param>
        /// <returns></returns>
        public string getAttachmentFileLocationId(DataTable dtAttachments, string Filename)
        {
            string strToPath = "";

            //iterate through local files

            //get the newly created name for each local file
            //RES 1/4/16 look up prod filename instead of original filename in case there are dups
            var results = from x in dtAttachments.AsEnumerable()
                            //where x.Field<string>("filename") == Filename
                            where x.Field<string>("prod_filename") == Filename
                            select new
                            {
                                prodFilename = x.Field<string>("prod_filename"),
                                locationId = x.Field<int>("location_id")
                            };
            foreach (var newFileName in results)
            {
                //it is possible the same file is in the grid more than once.  In this case the old file could be diff from new file
                //even though names are the same so copy anyway.
                string RazerCreatedFileName = newFileName.prodFilename;

                //strToPath = @"\laser_staging\attachments\rec_account" + RazerCreatedFileName;
                strToPath = getDBFileLocationPath(newFileName.locationId.ToString()) + "\\" +  RazerCreatedFileName;
                return strToPath;
            }
            return "";
        }

        /// <summary>
        /// get path from location table
        /// </summary>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        private string getDBFileLocationPath(string LocationId)
        {
            //set location id in cur bus obj
            this.CurrentBusObj.changeParm("@location_id", LocationId);
            //pop the attachment location table with current location id
            this.CurrentBusObj.LoadTable("attachment_location");
            if (this.CurrentBusObj.HasObjectData == true)
                //return location info
                if (this.CurrentBusObj.ObjectData.Tables["attachment_location"].Rows.Count > 0)
                    return this.CurrentBusObj.ObjectData.Tables["attachment_location"].Rows[0].ItemArray[2].ToString() + this.CurrentBusObj.ObjectData.Tables["attachment_location"].Rows[0].ItemArray[3].ToString();
                else
                    Messages.ShowInformation("File Cannot Be Found");
            //path not found
            return "";

        }

    }
}
