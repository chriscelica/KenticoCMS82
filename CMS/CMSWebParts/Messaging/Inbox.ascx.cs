using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_Messaging_Inbox : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the text which is displayed when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), ucInbox.ZeroRowsText), ucInbox.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            ucInbox.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the size of the page when paging is used.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), ucInbox.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            ucInbox.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the original message should be pasted to the current message.
    /// </summary>
    public bool PasteOriginalMessage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PasteOriginalMessage"), ucInbox.PasteOriginalMessage);
        }
        set
        {
            SetValue("PasteOriginalMessage", value);
            ucInbox.PasteOriginalMessage = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the original message should be shown.
    /// </summary>
    public bool ShowOriginalMessage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOriginalMessage"), ucInbox.ShowOriginalMessage);
        }
        set
        {
            SetValue("ShowOriginalMessage", value);
            ucInbox.ShowOriginalMessage = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            ucInbox.StopProcessing = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            ucInbox.StopProcessing = true;
        }
        else
        {
            ucInbox.ZeroRowsText = ZeroRowsText;
            ucInbox.PageSize = PageSize;
            ucInbox.PasteOriginalMessage = PasteOriginalMessage;
            ucInbox.ShowOriginalMessage = ShowOriginalMessage;
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
    }
}