using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;
//using System.Reflection;

namespace RazerBase
{
    //Class to handle opening up outlook with a default message and attachments.
    public class cMailMessage
    {

        //Get the current Outlook instance if Open
        //If not create a new instance of Outlook.
        private Outlook.Application GetApplicationObject()
        {
            Outlook.Application application = null;

            if (Process.GetProcessesByName("OUTLOOK").Count() > 0)
            {
                application = Marshal.GetActiveObject("Outlook.Application") as Outlook.Application;

            }
            else
            {
                application = new Outlook.Application();

            }
            return application;

        }

 
        public void CreateEmail(string FromAddress, string ToAddress, string Subject, string Body, string[] AttachmentList)
        {
            int cnt = 1;

            Outlook.Application AppObject = GetApplicationObject();

            var olApp = AppObject;

            var olMail = olApp.CreateItem(Outlook.OlItemType.olMailItem) as Outlook.MailItem;
            olMail.Subject = Subject;
            olMail.To = ToAddress;
            olMail.Body = Body;
            
            foreach (string s in AttachmentList)
            {
                string name = System.IO.Path.GetFileName(s);
                olMail.Attachments.Add(s, Outlook.OlAttachmentType.olByValue, cnt,name);
                cnt++;
            }
            //olMail.Attachments.Add(AttachmentFileName, Outlook.OlAttachmentType.olByValue, 1, name);
            olMail.Display();

        }





    }
}

