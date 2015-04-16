using System;
using System.Text;

using CMS.ExtendedControls;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_AnalyticsLog : AnalyticsLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (Log != null)
        {
            // Update user agent and IP
            gridAnalytics.RowDataBound += GetIPAndAgent;

            Visible = true;

            // Setup header texts
            gridAnalytics.SetHeaders("", "General.CodeName", "General.Object", "General.Count", "General.SiteName", "General.Context");

            HeaderText = GetString("AnalyticsLog.Info");

            gridAnalytics.DataSource = dt;
            gridAnalytics.DataBind();
        }
    }
}