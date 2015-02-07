using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace datahash
{
  public class WebTabPage: TabPage
  {
    private Button btnRefresh = new Button();
    private TextBox txtUrl = new TextBox();
    private Label lblStatus = new Label();
    private ComboBox cmbUserAgent = new ComboBox();

    public delegate void DelegateTrigger(Stream _stream);
    public DelegateTrigger Trigger = null;

    public delegate void DelegateTriggerSwitch();
    public DelegateTriggerSwitch Start = null;
    public DelegateTriggerSwitch End = null;

    public WebTabPage ()
    {
      this.Text = this.Name = "Web";

      btnRefresh.Text = "&Get";
      btnRefresh.Location = new Point(8, 8);
      btnRefresh.Click += btnRefresh_Click;
      this.Controls.Add(btnRefresh);

      txtUrl.Location = new Point(btnRefresh.Width + 16, 10);
      txtUrl.Width = this.Width - txtUrl.Location.X - 16;
      txtUrl.Anchor |= AnchorStyles.Right;
      txtUrl.KeyPress += txtUrl_KeyPress;
      txtUrl.Text = "http://";
      this.Controls.Add(txtUrl);

      cmbUserAgent.Location = new Point(btnRefresh.Width + 16, txtUrl.Height + 16);
      cmbUserAgent.Width = this.Width - cmbUserAgent.Location.X - 16;
      cmbUserAgent.Anchor |= AnchorStyles.Right;
      cmbUserAgent.Items.Add("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)");
      cmbUserAgent.Items.Add("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
      cmbUserAgent.Items.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");
      cmbUserAgent.Items.Add("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");
      cmbUserAgent.Items.Add("Mozilla/4.0 (compatible; MSIE 5.5; Windows 98; Win 9x 4.90)");
      cmbUserAgent.Items.Add("Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1");
      cmbUserAgent.Items.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1");
      cmbUserAgent.Items.Add("Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.8.1.19) Gecko/20081216 Ubuntu/8.04 (hardy) Firefox/2.0.0.19");
      cmbUserAgent.Items.Add("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.24 (KHTML, like Gecko) Chrome/11.0.696.71 Safari/534.24");
      cmbUserAgent.Items.Add("Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.21.1 (KHTML, like Gecko) Version/5.0.5 Safari/533.21.1");
      cmbUserAgent.Items.Add("Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11");
      cmbUserAgent.Items.Add("");
      cmbUserAgent.Items.Add("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");
      cmbUserAgent.Items.Add("Googlebot/2.1 (+http://www.googlebot.com/bot.html)");
      cmbUserAgent.Items.Add("msnbot/1.0 (+http://search.msn.com/msnbot.htm)");
      cmbUserAgent.Items.Add("Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)");
      cmbUserAgent.Items.Add("SearchExpress");
      cmbUserAgent.Items.Add("Microsoft URL Control - 6.00.8862");
      cmbUserAgent.SelectedIndex = 0;
      this.Controls.Add(cmbUserAgent);

      lblStatus.AutoSize = true;
      lblStatus.Location = new Point(btnRefresh.Width + 16, txtUrl.Height + cmbUserAgent.Height + 24);
      this.Controls.Add(lblStatus);
    }

    private void txtUrl_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == 13)
      {
        e.Handled = true;
        btnRefresh_Click(null, null);
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      if (null == this.Start || null == this.End || null == this.Trigger)
      {
        return;
      }

      string url = txtUrl.Text;
      HttpWebRequest request = null;
      HttpWebResponse response = null;

      this.Invoke(this.Start);

      try
      {
        request = (HttpWebRequest) WebRequest.Create(url);
        request.UserAgent = cmbUserAgent.Text;
        request.BeginGetResponse(new AsyncCallback(ResponseCallback), (object)request);
      }
      catch (Exception xcp)
      {
        this.Invoke(this.End);
        lblStatus.Text = xcp.Message;
        if (null != response) { response.Close(); }
        return;
      }
    }

    private MemoryStream CopyStream(Stream _stream, int _iCapacity)
    {
      MemoryStream rv = new MemoryStream(_iCapacity);
      byte[] buff = new byte[4096];
      int rn;

      while ((rn = _stream.Read(buff, 0, buff.Length)) > 0)
      {
        rv.Write(buff, 0, rn);
      }

      return rv;
    }

    private void ResponseCallback(IAsyncResult _asyncResult)
    {
      HttpWebRequest request = (HttpWebRequest) _asyncResult.AsyncState;
      HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(_asyncResult);

      Stream stream = null;
      MemoryStream copy = null;

      try
      {
        stream = response.GetResponseStream();
        copy = CopyStream(stream, ((int)response.ContentLength > 0? (int)response.ContentLength : 0));

        lblStatus.Text = String.Format("Status: {0}; Content-Length: {1}"
          , response.StatusCode
          , copy.Length);
      }
      catch (Exception xcp)
      {
        lblStatus.Text = xcp.Message;
        return;
      }
      finally
      {
        this.Invoke(this.End);
        if (null != stream) { stream.Close(); }
        if (null != response) { response.Close(); }
      }

      this.Invoke(this.Trigger, copy);
    }
  }
}
