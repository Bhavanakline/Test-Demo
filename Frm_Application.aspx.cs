using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BLL;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace ORA_N
{
    public partial class Frm_Application : System.Web.UI.Page
    {
        #region Variables
        ApplicationBLL oApplicationBLL = new ApplicationBLL();
        int iResult = 0;
        DataSet ods = new DataSet();
        string UserMsg = string.Empty;
        string DeveloperMsg = string.Empty;
        Dbconn odbcon = new Dbconn();
        #endregion

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (Session["UserName"] == null)
                    {
                        Response.Redirect("Login.aspx", false);
                    }
                    else
                    {
                        Page.Form.Attributes.Add("enctype", "multipart/form-data");
                        Pageloadsettings();
                    }
                }
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(lnkLogoPath);
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(lnkIconPath);
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(lnkIconpathupload);

                succuess.Visible = false;
                fail.Visible = false;
                //divuploadalert.Visible = false;
                //divUploadalert2.Visible = false;
                txtDBDescription.Attributes.Add("disabled", "disabled");
                txtDBDescription.Text = System.Configuration.ConfigurationManager.AppSettings["DBNAME"].ToString();
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        #endregion

        #region Click Events
        //Save/Update
        protected void lbtnsave_Click(object sender, EventArgs e)
        {
            try
            {
                if (hdnValue.Value == "-1")
                {
                    InsertApplication();

                    ViewState["SortExpression"] = "CREATEDDATE";
                    ViewState["SortDirection"] = "DESC";
                    funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());

                }
                else
                {
                    UpdateApplication();

                    ViewState["SortExpression"] = "UPDATEDDATE";
                    ViewState["SortDirection"] = "DESC";
                    funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                }

                divEdocuments.Visible = false;
                divApplicationmaster.Visible = true;
                divSeachApplicationDetails.Visible = true;
                divSeachApplicationEdocuments.Visible = false;
                divEdocuments.Visible = false;
                Clear();
                hdnValue.Value = "-1";
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        protected void lbtnclear_Click(object sender, EventArgs e)
        {
            Clear();
            succuess.Visible = false;
            fail.Visible = false;
            hdnValue.Value = "-1";
            ViewState["SortExpression"] = "APP_NAME";
            ViewState["SortDirection"] = "ASC";

            funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
        }

        //Sorting
        protected void lbtn_sortcitycode_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["SortExpression"] = "APP_NAME";
                if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                    ViewState["SortDirection"] = "DESC";
                else
                    ViewState["SortDirection"] = "ASC";

                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }
        protected void lbtn_sortAppcode_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["SortExpression"] = "APP_CODE";
                if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                    ViewState["SortDirection"] = "DESC";
                else
                    ViewState["SortDirection"] = "ASC";

                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }
        protected void lbtn_sortAppDesc_Click(object sender, EventArgs e)
        {
            try
            {
                ViewState["SortExpression"] = "APP_DESC";
                if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                    ViewState["SortDirection"] = "DESC";
                else
                    ViewState["SortDirection"] = "ASC";

                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }

        //Searching
        protected void lbtn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                gvApplication.PageIndex = 0;
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        protected void lnkIconpathupload_Click(object sender, EventArgs e)
        {
            try
            {

                string strFileExtensionAdmin = System.IO.Path.GetExtension(fupUserGuide.PostedFile.FileName);
                if (strFileExtensionAdmin.Trim() != ".doc" && strFileExtensionAdmin.Trim() != ".docx" && strFileExtensionAdmin.Trim() != ".pdf")
                {
                    //divuploadalert.Visible = true;
                    //uploaderrormsg.Text = "Please Upload Word(.doc,.docx) or Pdf(.pdf) only";
                    UserMsg = "Please Upload Word(.doc,.docx) or Pdf(.pdf) only";
                    ScriptManager.RegisterStartupScript(this, GetType(),
                            "Popup", "erroralert('" + UserMsg + "');", true);
                    return;
                }
                else
                {
                    InsertAppUserDocuments();
                    ddlAvailableTypes.ClearSelection();
                    txtRemarks.Text = "";
                }
                BindDocuments();
                //uploadsuccessmsg.Visible = false;
                //uploadsuccessmsg.Visible = false;
                if (hdnValue.Value == "-1")
                {
                    BindBuildgrid("");
                }
                else
                {
                    BindBuildgrid(txtappcode.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        protected void btn_modulecode_Click(object sender, EventArgs e)
        {

        }
        protected void btnclearmodule_Click(object sender, EventArgs e)
        {

        }
        protected void btnok_Click(object sender, EventArgs e)
        {
            try
            {
                /*Update Delete Flag is 1*/
                string UserMsg = "";
                string DeveloperMsg = "";
                int iCID1 = Convert.ToInt32(hdnValue.Value);
                int iResult = DeleteApplication();
                if (iResult > 0)
                {
                    //Deletion Succssful
                    if (iResult == 702)
                    {
                        UserMsg = Dbconn.GetMessage(1002, iResult);
                        ScriptManager.RegisterStartupScript(this, GetType(),
                               "Popup", "successalert('" + UserMsg + "');", true);
                        ViewState["SortExpression"] = "APP_CODE";
                        if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                            ViewState["SortDirection"] = "DESC";
                        else
                            ViewState["SortDirection"] = "ASC";

                        funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                        Clear();
                        divEdocuments.Visible = false;
                        divApplicationmaster.Visible = true;
                        divSeachApplicationDetails.Visible = true;
                        divSeachApplicationEdocuments.Visible = false;
                        divEdocuments.Visible = false;
                    }
                    else if (iResult == 707)
                    {
                        try
                        {
                            UserMsg = "Delete Failed";
                            ScriptManager.RegisterStartupScript(this, GetType(),
                                       "Popup", "successalert('" + UserMsg + "');", true);

                            DeveloperMsg = Dbconn.GetMessage(1002, 707);
                            string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                            StackTrace st = new StackTrace();
                            StackFrame sf = st.GetFrame(0);
                            MethodBase currentMethodName = sf.GetMethod();
                            string fuctionname = currentMethodName.Name;
                            string Procedurename = "USP_IUD_DEPT";


                            Utility.SendErrorMailMessage(DeveloperMsg, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                        }
                        catch (Exception ex)
                        {
                            Utility.LogError(ex);

                            string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                            StackTrace st = new StackTrace();
                            StackFrame sf = st.GetFrame(0);
                            MethodBase currentMethodName = sf.GetMethod();
                            string fuctionname = currentMethodName.Name;
                            string Procedurename = "USP_IUD_DEPT";

                            Utility.SendErrorMailMessage(ex.Message, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                        }
                    }
                    ViewState["SortExpression"] = "APP_CODE";
                    if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                        ViewState["SortDirection"] = "DESC";
                    else
                        ViewState["SortDirection"] = "ASC";

                    funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                }
                else
                {
                    fail.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        protected void btndelcon_Click(object sender, EventArgs e)
        {

        }

        protected void lbtnnext_Click(object sender, EventArgs e)
        {
            divApplicationmaster.Visible = false;
            divSeachApplicationDetails.Visible = false;
            divSeachApplicationEdocuments.Visible = true;
            ddlAvailableTypes.ClearSelection();
            txtRemarks.Text = "";
            lblTitle.Text = "Application References";
            divEdocuments.Visible = true;
            //uploaderrormsg.Visible = false;
            //uploadsuccessmsg.Visible = false;
            //divUploadalert2.Visible = false;
            if (hdnValue.Value == "-1")
            {
                BindBuildgrid("");
            }
            else
            {
                BindBuildgrid(txtappcode.Text.Trim());
            }
        }
        protected void lbtn_previous_Click(object sender, EventArgs e)
        {
            divEdocuments.Visible = false;
            divApplicationmaster.Visible = true;
            divSeachApplicationDetails.Visible = true;
            divSeachApplicationEdocuments.Visible = false;
            divEdocuments.Visible = false;
            lblTitle.Text = "Application Details";
        }

        protected void lnkLogoPath_Click(object sender, EventArgs e)
        {
            //string CurrentFilePath = Path.GetFullPath(fupAppLogPath.PostedFile.FileName);
            string strFileExtension = System.IO.Path.GetExtension(fupAppLogPath.PostedFile.FileName);
            if (strFileExtension.Trim() != ".jpg" && strFileExtension.Trim() != ".png")
            {

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Message", "alert('Please upload .jpg,.png images only.');", true);
                return;
            }
            else
            {
                if (fupAppLogPath.PostedFile != null && fupAppLogPath.PostedFile.ContentLength > 0)
                    UpLoadAndDisplayLogo();
            }

        }
        protected void lnkIconPath_Click(object sender, EventArgs e)
        {
            //string CurrentFilePath = Path.GetFullPath(fupIconPath.PostedFile.FileName);
            string strFileExtension = System.IO.Path.GetExtension(fupIconPath.PostedFile.FileName);
            if (strFileExtension.Trim() != ".jpg" && strFileExtension.Trim() != ".png")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Message", "alert('Please upload .jpg,.png images only.');", true);
                return;
            }
            else
            {
                if (fupIconPath.PostedFile != null && fupIconPath.PostedFile.ContentLength > 0)
                    UpLoadAndDisplayIcon();
            }
        }
        protected void btndocOK_Click(object sender, EventArgs e)
        {
            try
            {
                iResult = DeleteDocument();
                if (iResult > 0)
                {
                    //Deletion Succssful
                    if (iResult == 702)
                    {
                        UserMsg = Dbconn.GetMessage(1002, iResult);
                        ScriptManager.RegisterStartupScript(this, GetType(),
                                "Popup", "successalert('" + UserMsg + "');", true);
                        //lbl_success.Visible = true;
                        //uploadsuccessmsg.Visible = true;
                        //uploadsuccessmsg.Text = UserMsg;
                        BindDocuments();
                        divEdocuments.Visible = true;
                        divApplicationmaster.Visible = false;
                        divSeachApplicationDetails.Visible = false;
                        divSeachApplicationEdocuments.Visible = true;
                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#Deleteconfirm').modal();", true);
                        if (hdnValue.Value == "-1")
                        {
                            BindBuildgrid("");
                        }
                        else
                        {
                            BindBuildgrid(txtappcode.Text.Trim());
                        }
                    }
                    else if (iResult == 707)
                    {
                        try
                        {
                            fail.Visible = true;
                            lbl_errormsg.Visible = true;
                            UserMsg = Dbconn.GetMessage(1002, iResult);
                            DeveloperMsg = Dbconn.GetMessage(1002, 707);
                            string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                            StackTrace st = new StackTrace();
                            StackFrame sf = st.GetFrame(0);
                            MethodBase currentMethodName = sf.GetMethod();
                            string fuctionname = currentMethodName.Name;
                            string Procedurename = "USP_IUD_DEPT";


                            Utility.SendErrorMailMessage(DeveloperMsg, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                        }
                        catch (Exception ex)
                        {
                            Utility.LogError(ex);

                            string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                            StackTrace st = new StackTrace();
                            StackFrame sf = st.GetFrame(0);
                            MethodBase currentMethodName = sf.GetMethod();
                            string fuctionname = currentMethodName.Name;
                            string Procedurename = "USP_IUD_DEPT";
                            Utility.SendErrorMailMessage(ex.Message, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                        }
                    }

                }
                else
                {
                    fail.Visible = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
        protected void btn_deleteversion_Click(object sender, EventArgs e)
        {
            SqlParameter[] sqlParams = {
                            new SqlParameter("@IMODE", 1003),
                            new SqlParameter("@SNO", Convert.ToInt32(hdndeleteBuildid.Value))
                                    };
            int i = odbcon.UAT_ExecuteNonQuery("USP_IUD_BUILDVERSION", CommandType.StoredProcedure, sqlParams);

            if (i == 702)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "name", "hideModal();", true);
                if (hdnValue.Value == "-1")
                {
                    if (hdntempno.Value != string.Empty)

                        BindBuildgrid(hdntempno.Value);
                }
                else
                {
                    if (txtappcode.Text != string.Empty)
                        BindBuildgrid(txtappcode.Text);
                }
            }
        }
        protected void ddlAvailableTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //uploaderrormsg.Visible = false;
            //uploadsuccessmsg.Visible = false;
            //divuploadalert.Visible = false;
            //divUploadalert2.Visible = false;
        }
        protected void txtDevelopedBy_TextChanged(object sender, EventArgs e)
        {
            string s = "";
            if (s == "")
            { }
        }
        protected void cb_activate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //hdnValue.Value = RNO.ToString();
                int ID = Convert.ToInt32(hdnValue.Value);
                string Strac = CheckPartytrInfo(1003, ID);
                if (Strac == "Y")
                {
                    lblppmsg.Text = "This Application Code is used by another transactions, so the Delete operation cannot be Processed.";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#SearchTransactions').modal('show');", true);
                    cb_activate.Checked = false;
                }
                else
                {
                    if (cb_activate.Checked == true)
                    {
                        cb_activate.Checked = true;
                    }
                    else
                    {
                        cb_activate.Checked = false;
                    }
                    //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#Searchconfirm').modal();", true);
                }
                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        #endregion

        #region Functions
        protected void Pageloadsettings()
        {
            try
            {
                fail.Visible = false;
                succuess.Visible = false;
                validationalert.Attributes.Add("style", "display:none");
                lblvalid.Text = "";

                hdnValue.Value = "-1";
                Clear();
                BindDropdowns();
                //BindApplication();
                ViewState["SortExpression"] = "APP_NAME";
                ViewState["SortDirection"] = "ASC";

                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                //funcion_Search_Code(1002, "",
                // "", "");
                divEdocuments.Visible = false;
                divSeachApplicationEdocuments.Visible = false;
                divEdocuments.Visible = false;
                lblTitle.Text = "Application Details";
                lnkIconpathupload.Attributes.Add("disabled", "disabled");
                lnkIconpathupload.Attributes.Add("class", "btn btn-light round");

                lnkLogoPath.Attributes.Add("disabled", "disabled");
                lnkLogoPath.Attributes.Add("class", "btn mr-1 mb-1 btn-light btn-sm");

                lnkIconPath.Attributes.Add("disabled", "disabled");
                lnkIconPath.Attributes.Add("class", "btn mr-1 mb-1 btn-light btn-sm");

                //uploadsuccessmsg.Visible = false;
                imgAppiconPath.Visible = false;
                imgAppLogoPath.Visible = false;
                Gentempno();
                BindBuildgrid("");
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        //Generate Tempno
        protected void Gentempno()
        {
            hdntempno.Value = Utility.Gentempno();
        }
        protected void BindDropdowns()
        {
            try
            {
                DataView odvRefType = new DataView();
                odvRefType = odbcon.GetLookups("REFTYPE", "");
                odbcon.populateDropDown(ddlAvailableTypes, odvRefType, "DESCRIPTION", "CODE");

                DataView odvAppType = new DataView();
                odvRefType = odbcon.GetLookups("APPTYPE", "");
                odbcon.populateDropDown(ddlAppType, odvRefType, "DESCRIPTION", "CODE");

                DataView odvDBType = new DataView();
                odvRefType = odbcon.GetLookups("DBTYPE", "");
                odbcon.populateDropDown(ddlDBType, odvRefType, "DESCRIPTION", "CODE");
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        //Insert Application
        protected void InsertApplication()
        {
            try
            {
                string Bit32 = "0";
                string Bit64 = "0";
                string Deactive = "0";
                string APP_LOGOPATH = "";
                string APP_ICONPATH = "";
                if (ViewState["LOGO"] != null)
                {
                    APP_LOGOPATH = ViewState["LOGO"].ToString();
                }
                if (ViewState["ICON"] != null)
                {
                    APP_ICONPATH = ViewState["ICON"].ToString();
                }
                if (Chk32.Checked)
                {
                    Bit32 = "1";
                }
                if (Chk64.Checked)
                {
                    Bit64 = "1";
                }
                if (cb_activate.Checked)
                {
                    Deactive = "1";
                }
                iResult = oApplicationBLL.InsertApplication(1001, txtappcode.Text.Trim().ToUpper(),
                    txtappname.Text.Trim().ToUpper(), txtAppDesc.Text.Trim().ToUpper(), txtAppAlias.Text.Trim().ToUpper(),
                    ddlAppType.SelectedValue, APP_LOGOPATH, APP_ICONPATH, txtRunon.Text.Trim().ToUpper(), txtKeyFeatures.Text.Trim().ToUpper(), txtOptionAddonApp.Text.Trim().ToUpper()
                    , ddlDBType.SelectedValue, txtDBDescription.Text.Trim().ToUpper(), txtFitSizeofOrg.Text.Trim().ToUpper(), Bit32, Bit64, txtDependencies.Text.Trim().ToUpper()
                    , txtCopyRightsby.Text.Trim().ToUpper(), txtAppVersion.Text.Trim().ToUpper(), txtDevelopedBy.Text.Trim().ToUpper(), Deactive, Session["Username"].ToString().Trim().ToUpper()
                    , Session["OWNERID"].ToString().Trim().ToUpper(), Session["COMPANYID"].ToString().Trim().ToUpper());

                //Record Already Exists
                if (iResult == 222)
                {
                    UserMsg = "User Request Already Exist in the System";
                    ScriptManager.RegisterStartupScript(this, GetType(),
                               "Popup", "successalert('" + UserMsg + "');", true);
                }
                //Insertion Successful
                else if (iResult == 101)
                {
                    UserMsg = Dbconn.GetMessage(1002, iResult);
                    ScriptManager.RegisterStartupScript(this, GetType(),
                               "Popup", "successalert('" + UserMsg + "');", true);
                    UpdateTempno();
                    UPDBuildversion(txtappcode.Text.Trim(), hdntempno.Value.Trim());
                    Clear();
                    imgAppiconPath.Visible = false;
                    imgAppLogoPath.Visible = false;
                    //funcion_Search_Code(1002, "",
                    //"", "");
                }
                //Insertion Failed
                else if (iResult == 103)
                {
                    UserMsg = "Insertion Failed";
                    ScriptManager.RegisterStartupScript(this, GetType(),
                               "Popup", "successalert('" + UserMsg + "');", true);
                    DeveloperMsg = Dbconn.GetMessage(1002, iResult);
                    string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                    StackTrace st = new StackTrace();
                    StackFrame sf = st.GetFrame(0);
                    MethodBase currentMethodName = sf.GetMethod();
                    string fuctionname = currentMethodName.Name;
                    string Procedurename = "USP_INSERT_APPLICATIONMASTER";
                    Utility.SendErrorMailMessage(DeveloperMsg, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        //Update Application
        protected void UpdateApplication()
        {
            try
            {
                string Bit32 = "0";
                string Bit64 = "0";
                string Deactive = "0";
                string APP_LOGOPATH = "";
                string APP_ICONPATH = "";
                if (ViewState["LOGO"] != null)
                {
                    APP_LOGOPATH = ViewState["LOGO"].ToString();
                }
                if (ViewState["ICON"] != null)
                {
                    APP_ICONPATH = ViewState["ICON"].ToString();
                }
                if (Chk32.Checked)
                {
                    Bit32 = "1";
                }
                if (Chk64.Checked)
                {
                    Bit64 = "1";
                }
                if (cb_activate.Checked)
                {
                    Deactive = "1";
                }
                iResult = oApplicationBLL.UpdateApplication(1001, Convert.ToInt32(hdnValue.Value), txtappcode.Text.Trim().ToUpper(),
                    txtappname.Text.Trim().ToUpper(), txtAppDesc.Text.Trim().ToUpper(), txtAppAlias.Text.Trim().ToUpper(),
                    ddlAppType.SelectedValue, APP_LOGOPATH, APP_ICONPATH, txtRunon.Text.Trim().ToUpper(), txtKeyFeatures.Text.Trim().ToUpper(), txtOptionAddonApp.Text.Trim().ToUpper()
                    , ddlDBType.SelectedValue, txtDBDescription.Text.Trim().ToUpper(), txtFitSizeofOrg.Text.Trim().ToUpper(), Bit32, Bit64, txtDependencies.Text.Trim().ToUpper()
                    , txtCopyRightsby.Text.Trim().ToUpper(), txtAppVersion.Text.Trim().ToUpper(), txtDevelopedBy.Text.Trim().ToUpper(), Deactive, Session["Username"].ToString().Trim().ToUpper()
                    );

                if (iResult == 104)
                {
                    UserMsg = Dbconn.GetMessage(1002, iResult);
                    ScriptManager.RegisterStartupScript(this, GetType(),
                               "Popup", "successalert('" + UserMsg + "');", true);
                    Clear();
                    imgAppiconPath.Visible = false;
                    imgAppLogoPath.Visible = false;
                    //funcion_Search_Code(1002, "",
                    //"", "");
                }
                //Updation Failed
                else if (iResult == 704)
                {
                    try
                    {
                        UserMsg = "Updation Failed";
                        ScriptManager.RegisterStartupScript(this, GetType(),
                                   "Popup", "successalert('" + UserMsg + "');", true);
                        DeveloperMsg = Dbconn.GetMessage(1002, 705);
                        string pagename = Utility.GetCurrentPageName(Request.Url.AbsolutePath);
                        StackTrace st = new StackTrace();
                        StackFrame sf = st.GetFrame(0);
                        MethodBase currentMethodName = sf.GetMethod();
                        string fuctionname = currentMethodName.Name;
                        string Procedurename = "USP_IUD_CODE";
                        Utility.SendErrorMailMessage(DeveloperMsg, Convert.ToString(Session["Username"]), pagename, iResult, fuctionname, Procedurename, 0);
                    }
                    catch (Exception ex)
                    {
                        Utility.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        //Delete Application

        protected int DeleteApplication()
        {
            try
            {
                iResult = oApplicationBLL.DeleteApplication(1001, Convert.ToInt32(hdnValue.Value), Session["Username"].ToString().ToUpper());
                return iResult;
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return -1;
            }
        }
        protected int DeleteDocument()
        {
            try
            {
                iResult = oApplicationBLL.DeleteDocument(1001, Convert.ToInt32(hdnDocID.Value), Session["Username"].ToString().ToUpper());
                return iResult;
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return -1;
            }
        }
        //Get Application Info
        protected void BindApplication()
        {
            try
            {
                ods = oApplicationBLL.GetApplication(1001, -1);
                if (ods.Tables.Count > 0 && ods.Tables[0].Rows.Count > 0)
                {
                    gvApplication.DataSource = ods;
                    gvApplication.DataBind();
                }
                else
                {
                    gvApplication.DataSource = null;
                    gvApplication.DataBind();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        protected void Clear()
        {
            txtappcode.Text = "";
            txtappname.Text = "";
            txtAppDesc.Text = "";
            txtAppAlias.Text = "";
            ddlAppType.ClearSelection();
            txtRunon.Text = "";
            txtKeyFeatures.Text = "";
            txtOptionAddonApp.Text = "";
            ddlDBType.ClearSelection();
            //txtDBDescription.Text = "";
            txtFitSizeofOrg.Text = "";
            Chk32.Checked = false;
            Chk64.Checked = false;
            txtDependencies.Text = "";
            txtCopyRightsby.Text = "";
            txtAppVersion.Text = "";
            txtDevelopedBy.Text = "";

            gvDocuments.DataSource = null;
            gvDocuments.DataBind();
            txtappcode.Enabled = true;
        }

        protected void funcion_AutoSearch_Code(int iMode, string Searchkey)
        {
            try
            {
                ods = oApplicationBLL.GetAutoCodeSearch(iMode, Searchkey);
                DataView Odv = ods.Tables[0].DefaultView;
                if (Odv.Table.Rows.Count > 0)
                {
                    string sSortExp = ViewState["SortExpression"] + " " + ViewState["SortDirection"];
                    if (sSortExp != string.Empty)
                        Odv.Sort = sSortExp;
                    gvApplication.DataSource = Odv;
                    gvApplication.DataBind();
                    pager.Visible = true;
                }
                else
                {
                    gvApplication.DataSource = null;
                    gvApplication.DataBind();
                    pager.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }

        }

        protected string CheckPartytrInfo(int iMode, int RNO)
        {
            try
            {
                string Activate = "";
                ods = oApplicationBLL.GetApplication(1003, RNO);
                if (ods.Tables.Count > 0 && ods.Tables[0].Rows.Count > 0)
                {
                    Activate = "Y";
                }
                else
                {
                    Activate = "N";
                }
                return Activate;
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }

        //Rendering
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
        protected void InsertAppUserDocuments()
        {
            try
            {
                string UserFileName = "";
                if (fupUserGuide.HasFile)
                {
                    UserFileName = Path.GetFileName(fupUserGuide.PostedFile.FileName);
                    string path = Server.MapPath("~/USERGUIDE/") + UserFileName;
                    if (File.Exists(path))
                    {
                        //divuploadalert.Visible = true;
                        //uploaderrormsg.Text = UserFileName + "  Already Exists into the System.";
                        //uploadsuccessmsg.Visible = false;
                        //uploaderrormsg.Visible = true;
                        UserMsg = UserFileName + "  Already Exists into the System.";
                        ScriptManager.RegisterStartupScript(this, GetType(),
                                "Popup", "erroralert('" + UserMsg + "');", true);
                        return;
                    }
                    else
                    {
                        string Appcode = "";
                        if (hdnValue.Value == "-1")
                        {
                            Appcode = hdntempno.Value;
                        }
                        else
                        {
                            Appcode = txtappcode.Text.Trim();
                        }
                        //hdntempno.Value
                        fupUserGuide.PostedFile.SaveAs(Server.MapPath("~/USERGUIDE/") + UserFileName);
                        iResult = oApplicationBLL.InsertAppDocuments(1001, -1, Appcode, ddlAvailableTypes.SelectedValue, UserFileName, txtRemarks.Text.Trim(), Session["Username"].ToString().ToUpper());
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        protected void BindDocuments()
        {
            try
            {
                string App_code = "";
                if (hdnValue.Value == "-1")
                {
                    App_code = hdntempno.Value.Trim();
                }
                else
                {
                    App_code = txtappcode.Text.Trim().ToUpper();
                }
                ods = oApplicationBLL.GetApplicationDocuments(1001, -1, App_code);
                if (ods.Tables.Count > 0 && ods.Tables[0].Rows.Count > 0)
                {
                    gvDocuments.DataSource = ods;
                    gvDocuments.DataBind();
                }
                else
                {
                    gvDocuments.DataSource = null;
                    gvDocuments.DataBind();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        private void UpLoadAndDisplayLogo()
        {
            string imgName = fupAppLogPath.FileName;
            string imgPath = "images/" + imgName;
            ViewState["LOGO"] = imgPath;
            int imgSize = fupAppLogPath.PostedFile.ContentLength;
            if (fupAppLogPath.PostedFile != null && fupAppLogPath.PostedFile.FileName != "")
            {
                fupAppLogPath.SaveAs(Server.MapPath(imgPath));
                imgAppLogoPath.ImageUrl = "~/" + imgPath;
                imgAppLogoPath.Visible = true;
            }
        }
        private void UpLoadAndDisplayIcon()
        {
            string imgName = fupIconPath.FileName;
            string imgPath = "images/" + imgName;
            ViewState["ICON"] = imgPath;
            int imgSize = fupIconPath.PostedFile.ContentLength;
            if (fupIconPath.PostedFile != null && fupIconPath.PostedFile.FileName != "")
            {
                fupIconPath.SaveAs(Server.MapPath(imgPath));
                imgAppiconPath.ImageUrl = "~/" + imgPath;
                imgAppiconPath.Visible = true;
            }
        }
        protected void UpdateTempno()
        {
            try
            {
                iResult = oApplicationBLL.UpdateAppCodeByTempNo(1001, hdntempno.Value, txtappcode.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        protected void BindBuildgrid(string APPCODE)
        {
            try
            {

                SqlParameter[] sqlParams = {
                            new SqlParameter("@IMODE", 1001),
                            new SqlParameter("@APPID", APPCODE)
                                         };
                ods = odbcon.UAT_GetDataSet("USP_SELECT_BUILDVERSION", CommandType.StoredProcedure, sqlParams);
                DataView Odv = ods.Tables[0].DefaultView;
                if (Odv.Table.Rows.Count > 0)
                {
                    gvBuldDetails.DataSource = ods;
                    gvBuldDetails.DataBind();
                }
                else
                {
                    ods.Tables[0].Rows.Add(ods.Tables[0].NewRow());
                    gvBuldDetails.DataSource = ods;
                    gvBuldDetails.DataBind();
                    int columncount = gvBuldDetails.Rows[0].Cells.Count;
                    gvBuldDetails.Rows[0].Cells.Clear();
                    gvBuldDetails.Rows[0].Cells.Add(new TableCell());
                    gvBuldDetails.Rows[0].Cells[0].ColumnSpan = columncount;
                    ViewState["gridcount"] = "0";
                    gvBuldDetails.Rows[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        protected void UPDBuildversion(string Appid, string Tempno)
        {
            try
            {
                SqlParameter[] sqlParams = {
                            new SqlParameter("@IMODE", 1004),
        new SqlParameter("@APPID", Appid),
        new SqlParameter("@TEMPNO", Tempno),
        new SqlParameter("@UPDATEDBY", Session["Username"].ToString())
                                   };
                int i = odbcon.UAT_ExecuteNonQuery("USP_IUD_BUILDVERSION", CommandType.StoredProcedure, sqlParams);

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        #endregion

        #region GridEvents
        protected void ddlpagesize_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //Application Grid
        protected void gvApplication_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
                int RNO = Convert.ToInt32(e.CommandArgument);
                hdnValue.Value = RNO.ToString();
                if (e.CommandName == "Editcode")
                {

                    divEdocuments.Visible = false;
                    divApplicationmaster.Visible = true;
                    ods = oApplicationBLL.GetApplication(1002, RNO);
                    if (ods.Tables.Count > 0 && ods.Tables[0].Rows.Count > 0)
                    {
                        txtappcode.Text = ods.Tables[0].Rows[0]["APP_CODE"].ToString();
                        txtappcode.Enabled = false;
                        txtappname.Text = ods.Tables[0].Rows[0]["APP_NAME"].ToString();
                        txtAppDesc.Text = ods.Tables[0].Rows[0]["APP_DESC"].ToString();
                        txtAppAlias.Text = ods.Tables[0].Rows[0]["APP_ALIAS"].ToString();
                        ddlAppType.SelectedValue = ods.Tables[0].Rows[0]["APP_TYPE"].ToString();
                        txtRunon.Text = ods.Tables[0].Rows[0]["RUNON"].ToString();
                        txtKeyFeatures.Text = ods.Tables[0].Rows[0]["KEY_FEATURES"].ToString();
                        txtOptionAddonApp.Text = ods.Tables[0].Rows[0]["OPTION_ADDON_APP"].ToString();
                        ddlDBType.SelectedValue = ods.Tables[0].Rows[0]["DB_TYPE"].ToString();
                        txtDBDescription.Text = ods.Tables[0].Rows[0]["DB_DESCRIPTION"].ToString();
                        txtFitSizeofOrg.Text = ods.Tables[0].Rows[0]["FIT_SIZEOF_ORG"].ToString();
                        if (ods.Tables[0].Rows[0]["32_BIT_VERSION"].ToString() == "1")
                        {
                            Chk32.Checked = true;
                        }
                        else
                        {
                            Chk32.Checked = false;
                        }
                        if (ods.Tables[0].Rows[0]["64_BIT_VERSION"].ToString() == "1")
                        {
                            Chk64.Checked = true;
                        }
                        else
                        {
                            Chk64.Checked = false;
                        }
                        txtDependencies.Text = ods.Tables[0].Rows[0]["DEPENDENCIES"].ToString();
                        txtCopyRightsby.Text = ods.Tables[0].Rows[0]["COPYRIGHTS_BY"].ToString();
                        txtAppVersion.Text = ods.Tables[0].Rows[0]["APP_VERSION"].ToString();
                        txtDevelopedBy.Text = ods.Tables[0].Rows[0]["DEVELOPED_BY"].ToString();
                        if (ods.Tables[0].Rows[0]["ISDEACTIVATED"].ToString() == "True")
                        {
                            cb_activate.Checked = true;
                        }
                        else
                        {
                            cb_activate.Checked = false;
                        }
                        imgAppLogoPath.ImageUrl = ods.Tables[0].Rows[0]["APP_LOGOPATH"].ToString();
                        imgAppiconPath.ImageUrl = ods.Tables[0].Rows[0]["APP_ICONPATH"].ToString();

                        string LOGOPATH = ods.Tables[0].Rows[0]["APP_LOGOPATH"].ToString();
                        string ICONPATH = ods.Tables[0].Rows[0]["APP_ICONPATH"].ToString();
                        if (LOGOPATH != "" && ICONPATH != "")
                        {
                            imgAppLogoPath.Visible = true;
                            imgAppiconPath.Visible = true;
                        }
                        if (ICONPATH == "")
                        {
                            imgAppiconPath.Visible = false;
                        }
                        if (LOGOPATH == "")
                        {
                            imgAppLogoPath.Visible = false;
                        }

                    }
                    BindDocuments();
                    BindBuildgrid(txtappcode.Text.Trim());
                }
                else if (e.CommandName == "Deletecode")
                {
                    hdnValue.Value = RNO.ToString();
                    int ID = Convert.ToInt32(hdnValue.Value);
                    string Strac = CheckPartytrInfo(1003, ID);
                    if (Strac == "Y")
                    {
                        lblppmsg.Text = "This Application Code is used by another transactions, so the Delete operation cannot be Processed.";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#SearchTransactions').modal('show');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#Searchconfirm').modal('show');", true);
                    }

                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        protected void gvApplication_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvApplication.PageIndex = e.NewPageIndex;
                ViewState["SortExpression"] = "APP_CODE";
                if (Convert.ToString(ViewState["SortDirection"]) == "ASC")
                    ViewState["SortDirection"] = "DESC";
                else
                    ViewState["SortDirection"] = "ASC";

                funcion_AutoSearch_Code(1001, txt_Searchtype.Text.Trim());
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }

        }
        protected void gvApplication_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // when mouse is over the row, save original color to new attribute, and change it to highlight color
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#c8daea';");

                // when mouse leaves the row, change the bg color to its original value   
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");



                string Active = e.Row.Cells[5].Text;
                LinkButton lbtnactive = e.Row.Cells[0].FindControl("lbtn_delte") as LinkButton;

                if (Active.ToString().Trim().ToUpper() == "TRUE")
                {
                    e.Row.ForeColor = Color.FromName("#ff6666");
                    lbtnactive.ToolTip = "Please Delete the record";
                    lbtnactive.Enabled = true;
                    lbtnactive.Text = "<span><i class=\"fa fa-trash\" aria-hidden=\"true\" style=\"color:red;font-size:18px;\"></i></span>";
                }
                else
                {
                    e.Row.ForeColor = Color.FromName("#2A3F54");
                    lbtnactive.ToolTip = "This APP code is activated, so the delete operation has been cancelled.";
                    lbtnactive.Enabled = false;
                    lbtnactive.Text = "<span><i class=\"fa fa-trash\" aria-hidden=\"true\" style=\"color:grey;font-size:18px;\"></i></span>";
                }
            }
        }
        //Documents Grid
        protected void gvDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDocuments.PageIndex = e.NewPageIndex;
            BindDocuments();
        }
        protected void gvDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "DOWNLOAD")
            {
                string Filename = Convert.ToString(e.CommandArgument);

                string strExtension = Path.GetExtension(Filename).ToLower();

                if (strExtension == ".pdf" || strExtension == ".docx" || strExtension == ".doc")
                {
                    Response.ContentType = "Application/MIME";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + Filename);
                    Response.TransmitFile(Server.MapPath("~/USERGUIDE/" + Filename));
                    Response.End();
                }
            }
            if (e.CommandName == "DeleteDocument")
            {
                int RNO = Convert.ToInt32(e.CommandArgument);
                hdnDocID.Value = RNO.ToString();

                //ClientScript.RegisterStartupScript(this.GetType(), "Pop", "openModal();", true);
                btndocOK_Click(new object(), new EventArgs());
                //if (!IsPostBack)
                //{

                //}
                //else
                //{
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#SearchconfirmDocs').modal();", true);
                //}

            }
        }
        //Build Version Details
        protected void gvBuldDetails_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvBuldDetails.EditIndex = e.NewEditIndex;
                hdnGridEdit.Value = "Edit";
                int BuildID = Convert.ToInt32(gvBuldDetails.DataKeys[e.NewEditIndex].Values["SNO"]);
                hdndeleteBuildid.Value = BuildID.ToString();
                if (hdnValue.Value == "-1")
                {
                    if (hdntempno.Value != string.Empty)
                        BindBuildgrid(hdntempno.Value);
                }
                else
                {
                    if (txtappcode.Text != string.Empty)
                        BindBuildgrid(txtappcode.Text.Trim());
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }
        protected void gvBuldDetails_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int i = 0;
                //int BuildID = Convert.ToInt32(gvBuldDetails.DataKeys[e.RowIndex].Value.ToString());
                string APPID = gvBuldDetails.DataKeys[e.RowIndex].Values["APPID"].ToString();
                int SNO = Convert.ToInt32(gvBuldDetails.DataKeys[e.RowIndex].Values["SNO"].ToString());
                hdndeleteBuildid.Value = SNO.ToString();
                TextBox Bversion = (TextBox)gvBuldDetails.Rows[e.RowIndex].FindControl("txtBuildversion");

                TextBox BRemarks = (TextBox)gvBuldDetails.Rows[e.RowIndex].FindControl("txtRemarks");


                if (Bversion.Text == "" && BRemarks.Text == "")
                {
                    Bversion.Attributes.Add("class", "form-control validationmsg");
                    BRemarks.Attributes.Add("class", "form-control validationmsg");
                    return;
                }
                else if (Bversion.Text == "")
                {
                    Bversion.Attributes.Add("class", "form-control validationmsg");
                    BRemarks.Attributes.Add("class", "form-control");
                    return;
                }
                else if (BRemarks.Text == "")
                {
                    Bversion.Attributes.Add("class", "form-control");
                    BRemarks.Attributes.Add("class", "form-control validationmsg");
                    return;
                }
                else
                {
                    Bversion.Attributes.Add("class", "form-control");
                    BRemarks.Attributes.Add("class", "form-control");
                }

                SqlParameter[] sqlParams = {
                            new SqlParameter("@IMODE", 1002),
        new SqlParameter("@VERSION", Bversion.Text.Trim()),
        new SqlParameter("@REMARKS", BRemarks.Text.Trim()),
        new SqlParameter("@UPDATEDBY", Session["Username"].ToString()),
        new SqlParameter("@SNO", Convert.ToInt32(SNO))
                                   };
                i = odbcon.UAT_ExecuteNonQuery("USP_IUD_BUILDVERSION", CommandType.StoredProcedure, sqlParams);

                gvBuldDetails.EditIndex = -1;
                if (hdnValue.Value == "-1")
                {
                    if (hdntempno.Value != string.Empty)

                        BindBuildgrid(hdntempno.Value);
                }
                else
                {
                    if (txtappcode.Text != string.Empty)
                        BindBuildgrid(txtappcode.Text);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }

        }
        protected void gvBuldDetails_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                gvBuldDetails.EditIndex = -1;
                if (hdnValue.Value == "-1")
                {
                    if (hdntempno.Value != string.Empty)

                        BindBuildgrid(hdntempno.Value);
                }
                else
                {
                    if (txtappcode.Text != string.Empty)
                        BindBuildgrid(txtappcode.Text);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }
        protected void gvBuldDetails_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int i = 0;
                int SNO = Convert.ToInt32(gvBuldDetails.DataKeys[e.RowIndex].Values["SNO"].ToString());
                string APPID = gvBuldDetails.DataKeys[e.RowIndex].Values["APPID"].ToString();
                int IATArateid = Convert.ToInt32(gvBuldDetails.DataKeys[e.RowIndex].Values["SNO"].ToString());
                hdndeleteBuildid.Value = IATArateid.ToString();

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#BuildDelconfirm').modal('show');", true);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }

        }
        protected void gvBuldDetails_PreRender(object sender, System.EventArgs e)
        {
            try
            {
                gvBuldDetails.ShowFooter = true;
                Label lblcount = new Label();

                if (lblcount.Text != "0")
                    gvBuldDetails.FooterRow.Cells[1].Controls.Add(lblcount);
                hdnBpValue.Value = ViewState["gridcount"].ToString();
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }
        }
        protected void gvBuldDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int i = 0;
                if (e.CommandName.Equals("AddNew"))
                {
                    TextBox Bversion = (TextBox)gvBuldDetails.FooterRow.FindControl("txtftrBuildversion");

                    TextBox Breamrks = (TextBox)gvBuldDetails.FooterRow.FindControl("txtftrRemarks");

                    TextBox BAPPversion = (TextBox)gvBuldDetails.FooterRow.FindControl("txtftrAPPversion");

                    BAPPversion.Text = txtAppVersion.Text;
                    BAPPversion.Attributes.Add("disabled", "disabled");

                    if (Bversion.Text == "" && Breamrks.Text == "")
                    {
                        Bversion.Attributes.Add("class", "form-control validationmsg");
                        Breamrks.Attributes.Add("class", "form-control validationmsg");
                        return;
                    }
                    else if (Bversion.Text == "")
                    {
                        Bversion.Attributes.Add("class", "form-control validationmsg");
                        Breamrks.Attributes.Add("class", "form-control");
                        return;
                    }
                    else if (Breamrks.Text == "")
                    {
                        Bversion.Attributes.Add("class", "form-control");
                        Breamrks.Attributes.Add("class", "form-control validationmsg");
                        return;
                    }
                    else
                    {
                        Bversion.Attributes.Add("class", "form-control");
                        Breamrks.Attributes.Add("class", "form-control");
                    }

                    hdnBreakpintSave.Value = "Save";
                    int item_no;
                    item_no = Convert.ToInt32(hdnBpValue.Value) + 1;
                    string APPID;
                    if (hdnValue.Value == "-1")
                    {
                        //IATARATEID = Convert.ToInt32(hdnIatarateid.Value);
                        APPID = hdntempno.Value;
                    }
                    else
                    {
                        APPID = txtappcode.Text.ToString();
                    }

                    SqlParameter[] sqlParams = {
            new SqlParameter("@IMODE", 1001),
            new SqlParameter("@APPID", APPID),
            new SqlParameter("@VERSION", Bversion.Text.Trim().ToUpper()),
            new SqlParameter("@Remarks", Breamrks.Text.Trim().ToUpper()),
            new SqlParameter("@CREATEDBY", Session["Username"].ToString()),
            new SqlParameter("@OWNERID", Session["OWNERID"].ToString()),
            new SqlParameter("@COMPANYID", Session["COMPANYID"].ToString()),
            new SqlParameter("@APPVERSION", BAPPversion.Text.Trim().ToUpper())
                                        };
                    i = odbcon.UAT_ExecuteNonQuery("USP_IUD_BUILDVERSION", CommandType.StoredProcedure, sqlParams);
                    if (i == 101)
                    {
                        if (hdnValue.Value == "-1")
                        {
                            if (hdntempno.Value != string.Empty)
                                BindBuildgrid(hdntempno.Value);
                        }
                        else
                        {
                            if (txtappcode.Text != string.Empty)
                                BindBuildgrid(txtappcode.Text.Trim());
                        }

                    }
                    if (i == 222)
                    {
                        lblpalert.Text = "Version already created";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#CAlert').modal('show');", true);

                    }

                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                odbcon.DisConnect();
            }

        }
        protected void gvBuldDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
            {
                // when mouse is over the row, save original color to new attribute, and change it to highlight color
                e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#c8daea';");

                // when mouse leaves the row, change the bg color to its original value   
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                TextBox BAPversion = (TextBox)e.Row.FindControl("txtftrAPPversion");
                BAPversion.Text = txtAppVersion.Text;
                BAPversion.Attributes.Add("disabled", "disabled");
            }
        }
        protected void gvDocuments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == System.Web.UI.WebControls.DataControlRowType.DataRow)
                {
                    // when mouse is over the row, save original color to new attribute, and change it to highlight color
                    e.Row.Attributes.Add("onmouseover", "this.originalstyle=this.style.backgroundColor;this.style.backgroundColor='#c8daea';");

                    // when mouse leaves the row, change the bg color to its original value   
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalstyle;");
                }
                LinkButton lb = e.Row.FindControl("lnkDownloadAdminGuide") as LinkButton;
                if (lb != null)
                {
                    ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(lb);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            finally
            {
                
            }
        }
        #endregion
    }
}