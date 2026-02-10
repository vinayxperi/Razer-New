using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using RazerWS;

namespace RazerBase
{
    public class cGlobals
    {
        /// <summary>
        /// Allocate current class so for private constructor (for singleton only)
        /// </summary>
        //static readonly cGlobals _instance = new cGlobals();
        //public static cGlobals Instance
        //{
        //    get { return _instance; }
        //}

        public static IRazerService LCService;

        public static IBillingService BillService;

        //public static IWHTCalcEntryService WHTCalcEntryService;


        public static System.Data.DataTable SecurityDT;

        public static string UserName;

        public static string DatabaseName;

        public static string OSVersion;


        /// <summary>
        /// Global variable that is constant.
        /// </summary>
        //public const RazerWCFService.RazerServiceClient LCService = new RazerWCFService.RazerServiceClient();

        /// <summary>
        /// Static value protected by access routine.
        /// </summary>
        static ArrayList _ReturnParms = new ArrayList();
        /// <summary>
        /// Access routine for global variable.
        /// </summary>
        public static ArrayList ReturnParms
        {
            get
            {
                return _ReturnParms;
            }
            set
            {
                _ReturnParms = value;
            }
        }

        public cGlobals(IRazerService LCService, IBillingService billService)
        {
            cGlobals.LCService = LCService;
            cGlobals.BillService = billService;            
        }

        static AccessLevel mFlashSecurity;
        public static AccessLevel FlashSecurity
        {
            get
            {
                return mFlashSecurity;
            }
            set
            {
                mFlashSecurity = value;
            }
        }

        /// <summary>
        /// static protected list
        /// </summary>
        static List<string> _GlobalAttachmentsStorageList = new List<string>();
        /// <summary>
        /// Public list for temporary storage of attachment strings anywhere in the application.
        /// </summary>
        public static List<string> GlobalAttachmentsStorageList
        {
            get
            {
                return _GlobalAttachmentsStorageList;
            }
            set
            {
                _GlobalAttachmentsStorageList = value;
            }
        }

        /// <summary>
        /// static protected list
        /// </summary>
        static List<string> _GlobalCommentAttachmentsStorageList = new List<string>();
        /// <summary>
        /// Public list for temporary storage of attachment strings anywhere in the application.
        /// </summary>
        public static List<string> GlobalCommentAttachmentsStorageList
        {
            get
            {
                return _GlobalCommentAttachmentsStorageList;
            }
            set
            {
                _GlobalCommentAttachmentsStorageList = value;
            }
        }

        public static string Environment { get; set; }

        public static string DisableSecurity { get; set; }

        private static List<string> NonEssentialEnvironments = new List<string>(new[] { "Local", "Dev", "Test" });

        public static bool SetSecurity
        {
            get
            {
                //commented the correct version to allow all environments to have their security disabled;
                //bool TF = !((DisableSecurity == "YES") && NonEssentialEnvironments.Contains(Environment));

                //remove this line to restore the correct security settings and uncomment the line above
                bool TF = !(DisableSecurity == "YES");
                return TF;
            }
        }

        /// <summary>
        /// gets OS Major Version
        /// </summary>
        /// <returns></returns>
        public static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = System.Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else
                            operatingSystem = "7";
                        break;
                    default:
                        break;
                }
            }
            if (operatingSystem != "")
            {
                OSVersion = operatingSystem;
                //operatingSystem = "Windows " + operatingSystem;
                //if (os.ServicePack != "")
                //{
                //    operatingSystem += " " + os.ServicePack;
                //}
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            else
            {
                OSVersion = "";
            }
        
            return OSVersion;
        }

        /// <summary>
        /// Supports getOSInfo() method
        /// </summary>
        /// <returns></returns>
        private int getOSArchitecture()
        {
            string pa = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return ((String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
        }


    }
}
