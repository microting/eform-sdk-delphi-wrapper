using eFormCore;
using eFormData;
using eFormShared;
using Newtonsoft.Json;
using RGiesecke.DllExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace eFormSDK.Wrapper
{
    public static class CoreW
    {
        private static Core core;
        private static MainElement mainElement;

        private static Int32 caseCreatedCallbackPointer;
        public delegate void CaseCreatedCallback([MarshalAs(UnmanagedType.BStr)]String jsonCaseDto);

        #region Core_Create
        [DllExport("Core_Create")]
        public static int Core_Create()
        {
            int result = 0;
            try
            {
                core = new Core();
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }

            return result;
        }
        #endregion

        #region Core_Start
        [DllExport("Core_Start")]
        public static int Core_Start([MarshalAs(UnmanagedType.BStr)]String serverConnectionString)
        {
            int result = 0;
            try
            {
                core.Start(serverConnectionString);
                //core.HandleCaseCreated += Core_HandleCaseCreated;
                //IntPtr ptr = (IntPtr)startCallbackPointer;
                //NativeCallback callbackMethod =  (NativeCallback)Marshal.GetDelegateForFunctionPointer(ptr, typeof(NativeCallback));
                //callbackMethod(100);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }

            return result;
        }
        #endregion

        [DllExport("Core_HandleCaseCreated")]
        public static int Core_HandleCaseCreated(Int32 callbackPointer)
        {
            int result = 0;
            try
            {
                caseCreatedCallbackPointer = callbackPointer;               
                core.HandleCaseCreated += Core_HandleCaseCreated; 
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }

        private static void Core_HandleCaseCreated(object sender, EventArgs e)
        {
            Case_Dto caseDto = (Case_Dto)sender;
     
            Packer packer = new Packer();
            String jsonCaseDto = packer.PackCaseDto(caseDto);

            IntPtr ptr = (IntPtr)caseCreatedCallbackPointer;
            CaseCreatedCallback caseCreatedCallbackMethod = (CaseCreatedCallback)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CaseCreatedCallback));
            caseCreatedCallbackMethod(jsonCaseDto);
        }

  
        #region TemplateFromXml
        [DllExport("Core_TemplateFromXml")]
        public static int Core_TemplateFromXml([MarshalAs(UnmanagedType.BStr)] string xml, [MarshalAs(UnmanagedType.BStr)] ref string json)
        {
            int result = 0;
            try
            {
                MainElement mainElement = core.TemplateFromXml(xml);          
                json = new Packer().PackCoreElement(mainElement);              
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateCreate
        [DllExport("Core_TemplateCreate")]
        public static int Core_TemplateCreate([MarshalAs(UnmanagedType.BStr)]String json, ref int templateId)
        {
            int result = 0;
            try
            {
                MainElement mainElement = new Packer().UnpackMainElement(json);
                List<String> errors = core.TemplateValidation(mainElement);
                if (errors.Count > 0)
                {
                    string totalError = "";
                    foreach(string error in errors)
                        totalError += error + "\n";
                    throw new Exception(totalError);
                }
                templateId = core.TemplateCreate(mainElement);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateRead
        [DllExport("Core_TemplateRead")]
        public static int Core_TemplateRead(int templateId, [MarshalAs(UnmanagedType.BStr)] ref string json)
        {
            int result = 0;
            try
            {
                MainElement mainElement = core.TemplateRead(templateId);
                json = new Packer().PackCoreElement(mainElement);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateValidation
        [DllExport("Core_TemplateValidation")]
        public static int Core_TemplateValidation([MarshalAs(UnmanagedType.BStr)] string jsonMainElement,
                [MarshalAs(UnmanagedType.BStr)] ref string jsonValidaitonErrors)
        {
            int result = 0;
            try
            {
                Packer packer = new Packer();
                MainElement mainElement = packer.UnpackMainElement(jsonMainElement);
                List<String> errors = core.TemplateValidation(mainElement);
                jsonValidaitonErrors = packer.PackStringList(errors); 
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateDelete
        [DllExport("Core_TemplateDelete")]
        public static int Core_TemplateDelete(int templateId, ref bool deleteResult)
        {
            int result = 0;
            try
            {
                deleteResult = core.TemplateDelete(templateId);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateUploadData
        [DllExport("Core_TemplateUploadData")]
        public static int Core_TemplateUploadData([MarshalAs(UnmanagedType.BStr)] string jsonMainElementIn,
                [MarshalAs(UnmanagedType.BStr)] ref string jsonMainElementOut)
        {
            int result = 0;
            try
            {
                Packer packer = new Packer();
                MainElement mainElementIn = packer.UnpackMainElement(jsonMainElementIn);
                MainElement mainElementOut = core.TemplateUploadData(mainElementIn);
                jsonMainElementOut = packer.PackCoreElement(mainElementOut);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region Core_Advanced_SiteItemReadAll
        [DllExport("Core_Advanced_SiteItemReadAll")]
        public static int Core_Advanced_SiteItemReadAll([MarshalAs(UnmanagedType.BStr)] ref string json)
        {
            int result = 0;
            try
            {
                List<SiteName_Dto> siteNameDtoList = core.Advanced_SiteItemReadAll();
                Packer packer = new Packer();
                json = packer.PackSiteNameDtoList(siteNameDtoList);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region TemplateItemRead
        [DllExport("Core_TemplateItemRead")]
        public static int Core_TemplateItemRead(int templateId, [MarshalAs(UnmanagedType.BStr)] ref string json)
        {
            int result = 0;
            try
            {
                Template_Dto templateDto = core.TemplateItemRead(templateId);
                json = new Packer().PackTemplateDto(templateDto);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region CaseCreate
        [DllExport("Core_CaseCreate")]
        public static int Core_CaseCreate([MarshalAs(UnmanagedType.BStr)] string jsonMainElement,
            [MarshalAs(UnmanagedType.BStr)] string caseUId, int siteUId,
            [MarshalAs(UnmanagedType.BStr)] ref string resultCase)
        {
            int result = 0;
            try
            {
                MainElement mainElement = new Packer().UnpackMainElement(jsonMainElement);
                resultCase = core.CaseCreate(mainElement, caseUId, siteUId);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region CaseCreate2
        [DllExport("Core_CaseCreate2")]
        public static int Core_CaseCreate2([MarshalAs(UnmanagedType.BStr)] string jsonMainElement,
            [MarshalAs(UnmanagedType.BStr)] string caseUId, [MarshalAs(UnmanagedType.BStr)] string jsonSiteUIds,
            [MarshalAs(UnmanagedType.BStr)] string custom, [MarshalAs(UnmanagedType.BStr)] ref string jsonResultCases)
        {
            int result = 0;
            try
            {
                Packer packer = new Packer();
                MainElement mainElement = packer.UnpackMainElement(jsonMainElement);
                List<int> siteUIds = packer.UnpackIntList(jsonSiteUIds);
                List<string> resultCases = core.CaseCreate(mainElement, caseUId, siteUIds, custom);
                jsonResultCases = packer.PackStringList(resultCases);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region CaseRead
        [DllExport("Core_CaseRead")]
        public static int Core_CaseRead([MarshalAs(UnmanagedType.BStr)] string microtingUId,
            [MarshalAs(UnmanagedType.BStr)] string checkUId, [MarshalAs(UnmanagedType.BStr)] ref string jsonReplyElement)
        {
            int result = 0;
            try
            {
                ReplyElement replyElement = core.CaseRead(microtingUId, checkUId);
                jsonReplyElement = new Packer().PackCoreElement(replyElement);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion

        #region CaseDelete
        [DllExport("Core_CaseDelete")]
        public static int Core_CaseDelete([MarshalAs(UnmanagedType.BStr)] string microtingUId, ref bool deleteResult)
        {
            int result = 0;
            try
            {
                deleteResult = core.CaseDelete(microtingUId);
            }
            catch (Exception ex)
            {
                LastError.Value = ex.Message;
                result = 1;
            }
            return result;
        }
        #endregion
    }

}
