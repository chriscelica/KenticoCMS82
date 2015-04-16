using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.Localization;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;

public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_DocumentAttachmentsList : AttachmentsControl
{
    #region "Variables"

    private string mInnerDivClass = "NewAttachment";
    private int? mFilterLimit;
    private int mUpdateIconPanelWidth = 16;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Minimal count of entries for display filter.
    /// </summary>
    public int FilterLimit
    {
        get
        {
            if (mFilterLimit == null)
            {
                mFilterLimit = ValidationHelper.GetInteger(SettingsHelper.AppSettings["CMSDefaultListingFilterLimit"], 25);
            }
            return (int)mFilterLimit;
        }
        set
        {
            mFilterLimit = value;
        }
    }


    /// <summary>
    /// CSS class of the new attachment link.
    /// </summary>
    public string InnerDivClass
    {
        get
        {
            return mInnerDivClass;
        }
        set
        {
            mInnerDivClass = value;
        }
    }


    /// <summary>
    /// Info label.
    /// </summary>
    public Label InfoLabel
    {
        get
        {
            return MessagesPlaceHolder.InfoLabel;
        }
    }


    /// <summary>
    /// Indicates whether grouped attachments should be displayed.
    /// </summary>
    public override Guid GroupGUID
    {
        get
        {
            return base.GroupGUID;
        }
        set
        {
            base.GroupGUID = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.AttachmentGroupGUID = value;
                newAttachmentElem.AttachmentGroupGUID = value;
            }
        }
    }


    /// <summary>
    /// Indicates if paging is allowed.
    /// </summary>
    public override bool AllowPaging
    {
        get
        {
            return base.AllowPaging;
        }
        set
        {
            base.AllowPaging = value;
            if (gridAttachments != null)
            {
                gridAttachments.Pager.DisplayPager = value;
                gridAttachments.GridView.AllowPaging = value;
            }
        }
    }


    /// <summary>
    /// Defines size of the page for paging.
    /// </summary>
    public override string PageSize
    {
        get
        {
            return base.PageSize;
        }
        set
        {
            base.PageSize = value;
            if (gridAttachments != null)
            {
                gridAttachments.PageSize = value;
            }
        }
    }


    /// <summary>
    /// Default page size.
    /// </summary>
    public override int DefaultPageSize
    {
        get
        {
            return base.DefaultPageSize;
        }
        set
        {
            base.DefaultPageSize = value;
            if (gridAttachments != null)
            {
                gridAttachments.Pager.DefaultPageSize = value;
            }
        }
    }


    /// <summary>
    /// Width of the attachment.
    /// </summary>
    public override int ResizeToWidth
    {
        get
        {
            return base.ResizeToWidth;
        }
        set
        {
            base.ResizeToWidth = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToWidth = value;
            }
        }
    }


    /// <summary>
    /// Height of the attachment.
    /// </summary>
    public override int ResizeToHeight
    {
        get
        {
            return base.ResizeToHeight;
        }
        set
        {
            base.ResizeToHeight = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToHeight = value;
            }
        }
    }


    /// <summary>
    /// Maximal side size of the attachment.
    /// </summary>
    public override int ResizeToMaxSideSize
    {
        get
        {
            return base.ResizeToMaxSideSize;
        }
        set
        {
            base.ResizeToMaxSideSize = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.ResizeToMaxSideSize = value;
            }
        }
    }


    /// <summary>
    /// List of allowed extensions.
    /// </summary>
    public override string AllowedExtensions
    {
        get
        {
            return base.AllowedExtensions;
        }
        set
        {
            base.AllowedExtensions = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.AllowedExtensions = value;
            }
        }
    }


    /// <summary>
    /// Specifies the document for which the attachments should be displayed.
    /// </summary>
    public override int DocumentID
    {
        get
        {
            return base.DocumentID;
        }
        set
        {
            base.DocumentID = value;
            if (newAttachmentElem != null)
            {
                newAttachmentElem.DocumentID = value;
            }
        }
    }


    /// <summary>
    /// Specifies the version of the document (optional).
    /// </summary>
    public int VersionHistoryID
    {
        get
        {
            if ((Node != null) && (Node.DocumentID > 0))
            {
                return Node.DocumentCheckedOutVersionHistoryID;
            }

            return 0;
        }
    }


    /// <summary>
    /// Defines the form GUID; indicates that the temporary attachment will be handled.
    /// </summary>
    public override Guid FormGUID
    {
        get
        {
            return base.FormGUID;
        }
        set
        {
            base.FormGUID = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.AttachmentFormGUID = value;
                newAttachmentElem.FormGUID = value;
            }
        }
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            if ((dsAttachments != null) && (newAttachmentElem != null))
            {
                dsAttachments.StopProcessing = value;
                newAttachmentElem.StopProcessing = value;
            }
        }
    }


    /// <summary>
    /// Content container
    /// </summary>
    public Panel Container
    {
        get
        {
            return pnlCont;
        }
    }


    /// <summary>
    /// Label for workflow information
    /// </summary>
    public Label WorkflowLabel
    {
        get
        {
            return lblWf;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Node is not null neither for insert mode
        if ((dsAttachments == null) || (Node == null))
        {
            StopProcessing = true;
        }
        Visible = !StopProcessing;

        if (StopProcessing)
        {
            if (dsAttachments != null)
            {
                dsAttachments.StopProcessing = true;
            }
            if (newAttachmentElem != null)
            {
                newAttachmentElem.StopProcessing = true;
            }
            // Do nothing
        }
        else
        {
            // Register script for tooltips
            if (ShowTooltip)
            {
                ScriptHelper.RegisterTooltip(Page);
            }

            // Ensure info message
            if ((Request[Page.postEventSourceID] == hdnPostback.ClientID) || Request[Page.postEventSourceID] == hdnFullPostback.ClientID)
            {
                string action = Request[Page.postEventArgumentID];

                switch (action)
                {
                    case "insert":
                        ShowConfirmation(GetString("attach.inserted"));
                        break;

                    case "update":
                        ShowConfirmation(GetString("attach.updated"));
                        break;

                    case "delete":
                        ShowConfirmation(GetString("attach.deleted"));
                        break;

                    default:
                        if (action != "")
                        {
                            UpdateEditParameters(action);
                        }
                        break;
                }
            }


            #region "Scripts"

            // Refresh script
            string script = String.Format(@"
function RefreshUpdatePanel_{0}(hiddenFieldID, action) {{
    var hiddenField = document.getElementById(hiddenFieldID);
    if (hiddenField) {{
        __doPostBack(hiddenFieldID, action);
    }}
}}
function FullPageRefresh_{0}(action) {{
    if(RefreshTree != null)
    {{
        RefreshTree();
    }}

    var hiddenField = document.getElementById('{1}');
    if (hiddenField) {{
       __doPostBack('{1}', action);
    }}
}}
function InitRefresh_{0}(msg, fullRefresh, refreshTree, action)
{{
    if((msg != null) && (msg != '')){{ 
        alert(msg); action='error'; 
    }}
    
    if (fullRefresh) {{
        FullPageRefresh_{0}(action);
    }}
    else {{
        RefreshUpdatePanel_{0}('{2}', action);
    }}
}}
", ClientID, hdnFullPostback.ClientID, hdnPostback.ClientID);

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentScripts_" + ClientID, ScriptHelper.GetScript(script));

            // Register dialog script
            ScriptHelper.RegisterDialogScript(Page);

            // Register jQuery script for thumbnails updating
            ScriptHelper.RegisterJQuery(Page);

            #endregion


            // Initialize button for adding attachments
            newAttachmentElem.SourceType = MediaSourceEnum.DocumentAttachments;
            newAttachmentElem.DocumentID = DocumentID;
            newAttachmentElem.NodeParentNodeID = NodeParentNodeID;
            newAttachmentElem.NodeClassName = NodeClassName;
            newAttachmentElem.ResizeToWidth = ResizeToWidth;
            newAttachmentElem.ResizeToHeight = ResizeToHeight;
            newAttachmentElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
            newAttachmentElem.AttachmentGroupGUID = GroupGUID;
            newAttachmentElem.FormGUID = FormGUID;
            newAttachmentElem.AllowedExtensions = AllowedExtensions;
            newAttachmentElem.ParentElemID = ClientID;
            newAttachmentElem.ForceLoad = true;
            newAttachmentElem.Text = GetString("attach.newattachment");
            newAttachmentElem.InnerElementClass = InnerDivClass;
            newAttachmentElem.InnerLoadingElementClass = InnerLoadingDivClass;
            newAttachmentElem.IsLiveSite = IsLiveSite;
            newAttachmentElem.CheckPermissions = CheckPermissions;


            // Grid initialization
            gridAttachments.OnExternalDataBound += gridAttachments_OnExternalDataBound;
            gridAttachments.OnDataReload += gridAttachments_OnDataReload;
            gridAttachments.OnAction += gridAttachments_OnAction;
            gridAttachments.OnBeforeDataReload += gridAttachments_OnBeforeDataReload;
            gridAttachments.ZeroRowsText = GetString("attach.nodatafound");
            gridAttachments.IsLiveSite = IsLiveSite;
            gridAttachments.ShowActionsMenu = !IsLiveSite;
            gridAttachments.Columns = "AttachmentID, AttachmentGUID, AttachmentImageWidth, AttachmentImageHeight, AttachmentExtension, AttachmentName, AttachmentSize, AttachmentOrder, AttachmentTitle, AttachmentDescription";
            gridAttachments.OrderBy = "AttachmentOrder, AttachmentName, AttachmentID";

            // Get all possible column names from appropriate info
            IDataContainer info = (VersionHistoryID == 0) ? (IDataContainer)new AttachmentInfo() : new AttachmentHistoryInfo();
            gridAttachments.AllColumns = SqlHelper.MergeColumns(info.ColumnNames).Replace("AttachmentHistoryID", "AttachmentID");

            DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        }
    }


    void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Refresh control state after undo checkout action or when the workflow is finished
        if ((e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT) || e.WorkflowFinished)
        {
            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            lblWf.Visible = (lblWf.Text != string.Empty);

            // Ensure uploader button
            newAttachmentElem.Enabled = Enabled;

            // Hide actions
            gridAttachments.GridView.Columns[0].Visible = !HideActions;
            gridAttachments.GridView.Columns[1].Visible = !HideActions;
            newAttachmentElem.Visible = !HideActions;

            // Check if filter should be visible
            pnlFilter.Visible = (dsAttachments.TotalRecords > FilterLimit) || !String.IsNullOrEmpty(txtFilter.Text);
            if (pnlFilter.Visible)
            {
                // Set different message when no attachment will be found
                gridAttachments.ZeroRowsText = GetString("attach.nodata");
            }

            // Ensure correct layout
            Visible = !HideActions || pnlFilter.Visible;

            // Dialog for editing attachment
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format(@"
function Edit_{0}(attachmentGUID, formGUID, versionHistoryID, parentId, hash, image) {{ 
  var form = '';
  if (formGUID != '') {{ 
      form = '&formguid=' + formGUID + '&parentid=' + parentId; 
  }}
  {1}
  if (image) {{
      modalDialog('{2}, 'editorDialog', 905, 670); 
  }}
  else {{
      modalDialog('{3}, 'editorDialog', 700, 400); 
  }}
  return false; 
}}",
                                        ClientID,
                                        (((Node != null) ? String.Format("else{{ form = '&siteid=' + {0}; }}", Node.NodeSiteID) : string.Empty)),
                                        ResolveUrl((IsLiveSite ? "~/CMSFormControls/LiveSelectors/ImageEditor.aspx" : "~/CMSModules/Content/CMSDesk/Edit/ImageEditor.aspx") + "?attachmentGUID=' + attachmentGUID + '&versionHistoryID=' + versionHistoryID + form + '&clientid=" + ClientID + "&refresh=1&hash=' + hash"),
                                        AuthenticationHelper.ResolveDialogUrl(String.Format("{0}?attachmentGUID=' + attachmentGUID + '&versionHistoryID=' + versionHistoryID + form + '&clientid={1}&refresh=1&hash=' + hash", (IsLiveSite ? "~/CMSModules/Content/Attachments/CMSPages/MetaDataEditor.aspx" : "~/CMSModules/Content/Attachments/Dialogs/MetaDataEditor.aspx"), ClientID))));

            // Register script for editing attachment
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AttachmentEditScripts_" + ClientID, ScriptHelper.GetScript(sb.ToString()));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Indicates if the control contains some data.
    /// </summary>
    public override bool HasData()
    {
        return (dsAttachments != null && !DataHelper.DataSourceIsEmpty(dsAttachments.DataSource));
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        gridAttachments.ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Updates parameters used by Edit button when displaying image editor.
    /// </summary>
    /// <param name="attachmentGuidString">GUID identifying attachment</param>
    private void UpdateEditParameters(string attachmentGuidString)
    {
        if (ShowTooltip)
        {
            // Try to get attachment GUID
            var attGuid = ValidationHelper.GetGuid(attachmentGuidString, Guid.Empty);
            if (attGuid != Guid.Empty)
            {
                // Get attachment
                AttachmentInfo attInfo = AttachmentInfoProvider.GetAttachmentInfoWithoutBinary(attGuid, SiteName);
                if (attInfo != null)
                {
                    string attName = attInfo.AttachmentName;
                    int documentId = DocumentID;

                    // Get attachment URL
                    string attachmentUrl;

                    if (Node != null)
                    {
                        if (IsLiveSite && (documentId > 0))
                        {
                            attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attGuid, URLHelper.GetSafeFileName(attName, SiteContext.CurrentSiteName)));
                        }
                        else
                        {
                            attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attGuid, URLHelper.GetSafeFileName(attName, SiteContext.CurrentSiteName), null, VersionHistoryID));
                        }
                    }
                    else
                    {
                        attachmentUrl = ResolveUrl(DocumentHelper.GetAttachmentUrl(attGuid, VersionHistoryID));
                    }
                    attachmentUrl = URLHelper.UpdateParameterInUrl(attachmentUrl, "chset", Guid.NewGuid().ToString());

                    // Ensure correct URL for non-temporary attachments
                    if ((OriginalNodeSiteName != SiteContext.CurrentSiteName) && (documentId > 0))
                    {
                        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "sitename", OriginalNodeSiteName);
                    }

                    // Generate new tooltip command
                    string newToolTip = null;
                    string title = attInfo.AttachmentTitle;
                    string description = attInfo.AttachmentDescription;

                    // Optionally trim attachment name
                    string attachmentName = TextHelper.LimitLength(attInfo.AttachmentName, ATTACHMENT_NAME_LIMIT);
                    int imageWidth = attInfo.AttachmentImageWidth;
                    int imageHeight = attInfo.AttachmentImageHeight;
                    bool isImage = ImageHelper.IsImage(attInfo.AttachmentExtension);

                    int tooltipWidth = 300;
                    string url = isImage ? attachmentUrl : null;

                    string tooltipBody = UIHelper.GetTooltip(url, imageWidth, imageHeight, title, attachmentName, description, null, ref tooltipWidth);
                    if (!string.IsNullOrEmpty(tooltipBody))
                    {
                        newToolTip = String.Format("Tip('{0}', WIDTH, -300)", tooltipBody);
                    }

                    // Get update script
                    string updateScript = String.Format("$cmsj(\"#{0}\").attr('onmouseover', '').unbind('mouseover').mouseover(function(){{ {1} }});", attGuid, newToolTip);

                    // Execute update
                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "AttachmentUpdateEdit", ScriptHelper.GetScript(updateScript));
                }
            }
        }
    }


    private string GetWhereConditionInternal()
    {
        return string.IsNullOrEmpty(txtFilter.Text) ? null : String.Format("AttachmentName LIKE '%{0}%'", SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(txtFilter.Text)));
    }

    #endregion


    #region "Grid events"

    protected DataSet gridAttachments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (Node != null)
        {
            dsAttachments.Path = Node.NodeAliasPath;
            // Prefer culture from the node
            dsAttachments.CultureCode = !string.IsNullOrEmpty(Node.DocumentCulture) ? Node.DocumentCulture : LocalizationContext.PreferredCultureCode;
            dsAttachments.SiteName = SiteInfoProvider.GetSiteName(Node.OriginalNodeSiteID);
        }

        // Versioned attachments
        dsAttachments.DocumentVersionHistoryID = VersionHistoryID;
        dsAttachments.AttachmentGroupGUID = GroupGUID;
        dsAttachments.AttachmentFormGUID = FormGUID;
        dsAttachments.WhereCondition = GetWhereConditionInternal();
        dsAttachments.SelectedColumns = columns;
        dsAttachments.OrderBy = currentOrder;
        dsAttachments.LoadPagesIndividually = true;
        dsAttachments.UniPagerControl = gridAttachments.Pager.UniPager;
        dsAttachments.LoadData(true);

        // Ensure right column name (for attachments under workflow)
        if (!DataHelper.DataSourceIsEmpty(dsAttachments.DataSource))
        {
            totalRecords = dsAttachments.PagerForceNumberOfResults;

            DataSet ds = (DataSet)dsAttachments.DataSource;
            if (ds != null)
            {
                DataTable dt = (ds).Tables[0];
                if (!dt.Columns.Contains("AttachmentFormGUID"))
                {
                    dt.Columns.Add("AttachmentFormGUID");
                }
            }
        }

        return (DataSet)dsAttachments.DataSource;
    }


    protected void gridAttachments_OnBeforeDataReload()
    {
        gridAttachments.IsLiveSite = IsLiveSite;
        gridAttachments.GridView.AllowPaging = AllowPaging;
        if (!AllowPaging)
        {
            gridAttachments.PageSize = "0";
        }
        gridAttachments.GridView.AllowSorting = false;
    }


    /// <summary>
    /// UniGrid action buttons event handler.
    /// </summary>
    protected void gridAttachments_OnAction(string actionName, object actionArgument)
    {
        if (Enabled && !HideActions)
        {
            #region "Check permissions"

            if (CheckPermissions)
            {
                if (FormGUID != Guid.Empty)
                {
                    if (!RaiseOnCheckPermissions("Create", this))
                    {
                        if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(NodeParentNodeID, NodeClassName))
                        {
                            ShowError(GetString("attach.actiondenied"));
                            return;
                        }
                    }
                }
                else
                {
                    if (!RaiseOnCheckPermissions("Modify", this))
                    {
                        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                        {
                            ShowError(GetString("attach.actiondenied"));
                            return;
                        }
                    }
                }
            }

            #endregion


            Guid attachmentGuid = Guid.Empty;

            // Get action argument (Guid or int)
            if (ValidationHelper.IsGuid(actionArgument))
            {
                attachmentGuid = ValidationHelper.GetGuid(actionArgument, Guid.Empty);
            }

            // Process proper action
            switch (actionName.ToLowerCSafe())
            {
                case "moveup":
                    if (attachmentGuid != Guid.Empty)
                    {
                        // Move attachment up
                        if (FormGUID == Guid.Empty)
                        {
                            PerformAttachmentAction("moveup", () => DocumentHelper.MoveAttachmentUp(attachmentGuid, Node));
                        }
                        else
                        {
                            AttachmentInfoProvider.MoveAttachmentUp(attachmentGuid, 0);
                        }
                    }
                    break;

                case "movedown":
                    if (attachmentGuid != Guid.Empty)
                    {
                        // Move attachment down
                        if (FormGUID == Guid.Empty)
                        {
                            PerformAttachmentAction("movedown", () => DocumentHelper.MoveAttachmentDown(attachmentGuid, Node));
                        }
                        else
                        {
                            AttachmentInfoProvider.MoveAttachmentDown(attachmentGuid, 0);
                        }
                    }
                    break;

                case "delete":
                    if (attachmentGuid != Guid.Empty)
                    {
                        // Delete attachment
                        if (FormGUID == Guid.Empty)
                        {
                            PerformAttachmentAction("delete", () => DocumentHelper.DeleteAttachment(Node, attachmentGuid, TreeProvider));
                        }
                        else
                        {
                            AttachmentInfoProvider.DeleteTemporaryAttachment(attachmentGuid, SiteContext.CurrentSiteName);
                        }

                        ShowConfirmation(GetString("attach.deleted"));
                    }
                    break;
            }
        }
    }


    private void PerformAttachmentAction(string actionName, Action action)
    {
        // Store original values
        var originalStep = Node.WorkflowStep;
        var wasArchived = Node.IsArchived;
        var wasInPublishedStep = Node.IsInPublishStep;

        // Ensure automatic check-in/ check-out
        VersionManager vm = null;
        bool checkin = false;

        // Check out the document
        if (AutoCheck)
        {
            vm = VersionManager.GetInstance(TreeProvider);
            var step = vm.CheckOut(Node, Node.IsPublished, true);

            // Do not check-in document if not under a workflow anymore
            checkin = (step != null);
        }

        // Perform action
        if (action != null)
        {
            action();
        }

        // Check in the document
        if (AutoCheck)
        {
            if ((vm != null) && checkin && (Node.DocumentWorkflowStepID != 0))
            {
                vm.CheckIn(Node, null);
            }

            // Document state changed
            bool fullRefresh = (originalStep == null) || originalStep.StepIsArchived || originalStep.StepIsPublished ||
                               (wasInPublishedStep != Node.IsInPublishStep) || (wasArchived != Node.IsArchived) || (WorkflowManager.GetNodeWorkflow(Node) == null);

            // Ensure full page refresh
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), actionName + "Refresh", ScriptHelper.GetScript(String.Format("InitRefresh_{0}('', {2}, false, '{1}');", ClientID, actionName, fullRefresh ? "true" : "false")));

            // Clear document manager properties
            DocumentManager.ClearProperties();
        }

        // Log synchronization task if not under workflow
        if (!UsesWorkflow)
        {
            DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.UpdateDocument, TreeProvider);
        }
    }


    /// <summary>
    /// UniGrid external data bound.
    /// </summary>
    protected object gridAttachments_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView drv;

        switch (sourceName.ToLowerCSafe())
        {
            case "clone":
                CMSGridActionButton imgClone = sender as CMSGridActionButton;
                if (imgClone != null)
                {
                    if (IsLiveSite)
                    {
                        // Hide cloning on live site
                        imgClone.Visible = false;
                        return imgClone;
                    }

                    string objectType = VersionHistoryID > 0 ? AttachmentHistoryInfo.OBJECT_TYPE : AttachmentInfo.OBJECT_TYPE;

                    int id = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["AttachmentID"], 0);

                    imgClone.PreRender += imgClone_PreRender;
                    imgClone.OnClientClick = "modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?objectType=" + objectType + "&objectId=" + id) + "', 'CloneObject', 750, 470); return false;";
                }
                break;

            case "update":
                {
                    Panel pnlBlock = new Panel
                        {
                            ID = "pnlBlock"
                        };

                    pnlBlock.Style.Add("margin", "0 auto");
                    pnlBlock.PreRender += (senderObject, args) => pnlBlock.Width = mUpdateIconPanelWidth;

                    // Add update control
                    // Dynamically load uploader control
                    var dfuElem = Page.LoadUserControl("~/CMSModules/Content/Controls/Attachments/DirectFileUploader/DirectFileUploader.ascx") as DirectFileUploader;

                    drv = parameter as DataRowView;

                    // Set uploader's properties
                    if (dfuElem != null)
                    {
                        dfuElem.SourceType = MediaSourceEnum.DocumentAttachments;
                        dfuElem.ID = "dfuElem" + DocumentID;
                        dfuElem.IsLiveSite = IsLiveSite;
                        dfuElem.ControlGroup = "update";
                        dfuElem.AttachmentGUID = GetAttachmentGuid(drv);
                        dfuElem.DisplayInline = true;
                        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
                        dfuElem.MaxNumberToUpload = 1;
                        dfuElem.PreRender += dfuElem_PreRender;
                        pnlBlock.Controls.Add(dfuElem);
                    }

                    // Check if external edit allowed by the form
                    bool allowExt = (Form == null) || Form.AllowExternalEditing;

                    if (allowExt && (FormGUID == Guid.Empty))
                    {
                        var ctrl = ExternalEditHelper.LoadExternalEditControl(pnlBlock, FileTypeEnum.Attachment, SiteName, new DataRowContainer(drv), IsLiveSite, Node);
                        if (ctrl != null)
                        {
                            ctrl.Enabled = Enabled;
                            ctrl.ID = "extEdit" + DocumentID;

                            // Ensure form identification
                            if ((Form != null) && (Form.Parent != null))
                            {
                                ctrl.FormID = Form.Parent.ClientID;
                            }
                            ctrl.SiteName = SiteName;

                            ctrl.PreRender += extEdit_PreRender;

                            if (FieldInfo != null)
                            {
                                ctrl.AttachmentFieldName = FieldInfo.Name;
                            }

                            // Adjust styles
                            bool isRTL = (IsLiveSite && CultureHelper.IsPreferredCultureRTL()) || (!IsLiveSite && CultureHelper.IsUICultureRTL());

                            pnlBlock.Style.Add("text-align", isRTL ? "right" : "left");
                            mUpdateIconPanelWidth = 32;
                        }
                    }

                    return pnlBlock;
                }

            case "edit":
                // Get file extension
                string extension = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["AttachmentExtension"], string.Empty).ToLowerCSafe();
                Guid guid = ValidationHelper.GetGuid(((DataRowView)((GridViewRow)parameter).DataItem).Row["AttachmentGUID"], Guid.Empty);
                CMSGridActionButton img = sender as CMSGridActionButton;
                if (img != null)
                {
                    img.ToolTip = String.Format("{0}|{1}", extension, guid);
                    img.PreRender += img_PreRender;
                }
                break;

            case "delete":
                CMSGridActionButton imgDelete = sender as CMSGridActionButton;
                if (imgDelete != null)
                {
                    // Turn off validation
                    imgDelete.CausesValidation = false;
                    imgDelete.PreRender += imgDelete_PreRender;
                }
                break;

            case "moveup":
                CMSGridActionButton imgUp = sender as CMSGridActionButton;
                if (imgUp != null)
                {
                    // Turn off validation
                    imgUp.CausesValidation = false;
                    imgUp.PreRender += imgUp_PreRender;
                }
                break;

            case "movedown":
                CMSGridActionButton imgDown = sender as CMSGridActionButton;
                if (imgDown != null)
                {
                    // Turn off validation
                    imgDown.CausesValidation = false;
                    imgDown.PreRender += imgDown_PreRender;
                }
                break;

            case "attachmentname":
                {
                    drv = parameter as DataRowView;

                    if (drv == null)
                    {
                        break;
                    }

                    // Get attachment GUID
                    Guid attachmentGuid = GetAttachmentGuid(drv);

                    // Get attachment extension
                    string attachmentExt = GetAttachmentExtension(drv);
                    bool isImage = ImageHelper.IsImage(attachmentExt);

                    // Get link for attachment
                    string attachmentUrl;

                    string attName = ValidationHelper.GetString(drv["AttachmentName"], string.Empty);
                    int documentId = DocumentID;

                    if (Node != null)
                    {
                        if (IsLiveSite && (documentId > 0))
                        {
                            attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attachmentGuid, URLHelper.GetSafeFileName(attName, SiteContext.CurrentSiteName)));
                        }
                        else
                        {
                            attachmentUrl = AuthenticationHelper.ResolveUIUrl(AttachmentURLProvider.GetAttachmentUrl(attachmentGuid, URLHelper.GetSafeFileName(attName, SiteContext.CurrentSiteName), null, VersionHistoryID));
                        }
                    }
                    else
                    {
                        attachmentUrl = ResolveUrl(DocumentHelper.GetAttachmentUrl(attachmentGuid, VersionHistoryID));
                    }
                    attachmentUrl = URLHelper.UpdateParameterInUrl(attachmentUrl, "chset", Guid.NewGuid().ToString());

                    // Ensure correct URL for non-temporary attachments
                    if ((OriginalNodeSiteName != SiteContext.CurrentSiteName) && (documentId > 0))
                    {
                        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "sitename", OriginalNodeSiteName);
                    }

                    // Add latest version requirement for live site
                    if (IsLiveSite && (documentId > 0))
                    {
                        // Add requirement for latest version of files for current document
                        string newparams = "latestfordocid=" + documentId;
                        newparams += "&hash=" + ValidationHelper.GetHashString("d" + documentId);

                        attachmentUrl += "&" + newparams;
                    }

                    // Optionally trim attachment name
                    string attachmentName = TextHelper.LimitLength(attName, ATTACHMENT_NAME_LIMIT);

                    // Tooltip
                    string tooltip = null;
                    if (ShowTooltip)
                    {
                        string title = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "AttachmentTitle"), string.Empty);
                        string description = ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "AttachmentDescription"), string.Empty);
                        int imageWidth = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "AttachmentImageWidth"), 0);
                        int imageHeight = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(drv, "AttachmentImageHeight"), 0);

                        tooltip = UIHelper.GetTooltipAttributes(attachmentUrl, imageWidth, imageHeight, title, attachmentName, attachmentExt, description, null, 300);
                    }

                    // Icon
                    string imageTag = UIHelper.GetFileIcon(Page, attachmentExt);

                    if (isImage)
                    {
                        return String.Format("<a class=\"cms-icon-link\" href=\"#\" onclick=\"javascript: window.open('{0}'); return false;\"><span id=\"{1}\" {2}>{3}{4}</span></a>", attachmentUrl, attachmentGuid, tooltip, imageTag, attachmentName);
                    }
                    else
                    {
                        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "disposition", "attachment");

                        // NOTE: OnClick here is needed to avoid loader to show because even for download links, the pageUnload event is fired
                        return String.Format("<a class=\"cms-icon-link\" onclick=\"javascript: {5}\" href=\"{0}\"><span id=\"{1}\" {2}>{3}{4}</span></a>", attachmentUrl, attachmentGuid, tooltip, imageTag, attachmentName, ScriptHelper.GetDisableProgressScript());
                    }
                }

            case "attachmentsize":
                long size = ValidationHelper.GetLong(parameter, 0);
                return DataHelper.GetSizeString(size);
        }

        return parameter;
    }

    #endregion


    #region "Grid actions' events"

    protected void dfuElem_PreRender(object sender, EventArgs e)
    {
        var dfuElem = (DirectFileUploader)sender;

        dfuElem.ForceLoad = true;
        dfuElem.FormGUID = FormGUID;
        dfuElem.AttachmentGroupGUID = GroupGUID;
        dfuElem.DocumentID = DocumentID;
        dfuElem.NodeParentNodeID = NodeParentNodeID;
        dfuElem.NodeClassName = NodeClassName;
        dfuElem.ResizeToWidth = ResizeToWidth;
        dfuElem.ResizeToHeight = ResizeToHeight;
        dfuElem.ResizeToMaxSideSize = ResizeToMaxSideSize;
        dfuElem.AllowedExtensions = AllowedExtensions;
        dfuElem.ShowIconMode = true;
        dfuElem.InsertMode = false;
        dfuElem.ParentElemID = ClientID;
        dfuElem.CheckPermissions = CheckPermissions;
        dfuElem.IsLiveSite = IsLiveSite;
        dfuElem.UploadMode = MultifileUploaderModeEnum.DirectSingle;
        dfuElem.MaxNumberToUpload = 1;

        dfuElem.Enabled = Enabled;
    }


    protected void extEdit_PreRender(object sender, EventArgs e)
    {
        var extEdit = sender as ExternalEditControl;
        if (extEdit != null)
        {
            extEdit.Enabled = Enabled;
        }
    }


    void imgClone_PreRender(object sender, EventArgs e)
    {
        var imgClone = (CMSGridActionButton)sender;
        if (!Enabled || (DocumentID <= 0))
        {
            imgClone.Enabled = false;
        }
    }


    protected void img_PreRender(object sender, EventArgs e)
    {
        var img = (CMSGridActionButton)sender;

        if (AuthenticationHelper.IsAuthenticated())
        {
            if (!Enabled)
            {
                // Disable edit icon
                img.Enabled = false;
                img.Style.Add("cursor", "default");
            }
            else
            {
                string[] args = img.ToolTip.Split('|');
                string strForm = (FormGUID == Guid.Empty) ? string.Empty : FormGUID.ToString();
                string form = null;

                if (!String.IsNullOrEmpty(strForm))
                {
                    form = String.Format("&formguid={0}&parentid={1}", strForm, NodeParentNodeID);
                }
                else
                {
                    if (Node != null)
                    {
                        form += "&siteid=" + Node.NodeSiteID;
                    }
                }

                string isImage = ImageHelper.IsSupportedByImageEditor(args[0]) ? "true" : "false";
                // Prepare parameters
                string parameters = String.Format("?attachmentGUID={0}&versionHistoryID={1}{2}&clientid={3}&refresh=1", args[1], VersionHistoryID, form, ClientID);
                // Create security hash
                string validationHash = QueryHelper.GetHash(parameters);

                img.OnClientClick = String.Format("Edit_{0}('{1}', '{2}', '{3}', {4}, '{5}', {6});return false;", ClientID, args[1], strForm, VersionHistoryID, NodeParentNodeID, validationHash, isImage);
            }

            img.ToolTip = GetString("general.edit");
        }
        else
        {
            img.Visible = false;
        }
    }


    protected void imgDown_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgDown = (CMSGridActionButton)sender;
        if (!Enabled || !AllowChangeOrder)
        {
            // Disable move down icon in case that editing is not allowed
            imgDown.Enabled = false;
            imgDown.Style.Add("cursor", "default");
        }
    }


    protected void imgUp_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgUp = (CMSGridActionButton)sender;
        if (!Enabled || !AllowChangeOrder)
        {
            // Disable move up icon in case that editing is not allowed
            imgUp.Enabled = false;
            imgUp.Style.Add("cursor", "default");
        }
    }


    protected void imgDelete_PreRender(object sender, EventArgs e)
    {
        CMSGridActionButton imgDelete = (CMSGridActionButton)sender;
        if (!Enabled)
        {
            // Disable delete icon in case that editing is not allowed
            imgDelete.Enabled = false;
            imgDelete.Style.Add("cursor", "default");
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Gets GUID value from DataRowView.
    /// </summary>
    /// <param name="drv">Row cell</param>
    /// <returns>GUID of attachment</returns>
    protected static Guid GetAttachmentGuid(DataRowView drv)
    {
        // Get GUID of attachment
        return ValidationHelper.GetGuid(DataHelper.GetDataRowViewValue(drv, "AttachmentGUID"), Guid.Empty);
    }


    /// <summary>
    /// Gets attachment name from DataRowView.
    /// </summary>
    /// <param name="drv">Row cell</param>
    /// <returns>Attachment name</returns>
    protected static string GetAttachmentName(DataRowView drv)
    {
        // Get GUID of attachment
        return ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "AttachmentName"), string.Empty);
    }


    /// <summary>
    /// Gets extension value from DataRowView.
    /// </summary>
    /// <param name="drv">Row view</param>
    /// <returns>Extension of attachment</returns>
    protected static string GetAttachmentExtension(DataRowView drv)
    {
        // Get extension of attachment
        return ValidationHelper.GetString(DataHelper.GetDataRowViewValue(drv, "AttachmentExtension"), string.Empty);
    }

    #endregion
}