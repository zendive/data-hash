using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace datahash
{
  public class HashTabPage: TabPage
  {
    public delegate void DelegateRequestRecalculate();
    public DelegateRequestRecalculate RequestRecalculate = null;

    private CheckBox chkKey = new CheckBox();
    private Button btnGenerate = new Button();
    private HexTextBox txtKey = new HexTextBox();
    private Label lblMatcher = new Label(); private TextBox txtMatcher = new TextBox();
    private Label lblMD5 = new Label(); private TextBox txtMD5 = new TextBox();
    private Label lblSHA1 = new Label(); private TextBox txtSHA1 = new TextBox();
    private Label lblRIPEMD160 = new Label(); private TextBox txtRIPEMD160 = new TextBox();
    private Label lblSHA256 = new Label(); private TextBox txtSHA256 = new TextBox();
    private Label lblSHA384 = new Label(); private TextBox txtSHA384 = new TextBox();
    private Label lblSHA512 = new Label(); private TextBox txtSHA512 = new TextBox();
    private Label lblCRC32 = new Label(); private TextBox txtCRC32 = new TextBox();

    public HashTabPage ()
    {
      this.Text = this.Name = "Hash";
      this.OnFileHash_Result = new Delegate_OnFileHash_Result(FileHash_Result);

      chkKey.Checked = false;
      chkKey.AutoSize = true;
      chkKey.Location = new Point(8, 8);
      chkKey.Text = "Use &Key";
      chkKey.CheckedChanged += chkKey_CheckedChanged;
      this.Controls.Add(chkKey);

      txtKey.Multiline = true;
      txtKey.ScrollBars = ScrollBars.Vertical;
      txtKey.Location = new Point(100, 8);
      txtKey.Anchor |= AnchorStyles.Right;
      txtKey.Width = this.Width - txtKey.Location.X - 8;
      txtKey.Height = 56;
      txtKey.Enabled = false;
      txtKey.TextChanged += txtKey_TextChanged;
      this.Controls.Add(txtKey);

      btnGenerate.Text = "&Generate";
      btnGenerate.Location = new Point(8, chkKey.Location.Y + chkKey.Height + 8);
      btnGenerate.Enabled = false;
      btnGenerate.Click += btnGenerate_Click;
      this.Controls.Add(btnGenerate);

      int iRow = 2;
      AddFieldRow("Matcher", lblMatcher, txtMatcher, this, iRow++);
      txtMatcher.ReadOnly = false;
      txtMatcher.TextChanged += txtMatcher_TextChanged;
      AddFieldRow("MD5", lblMD5, txtMD5, this, iRow++);
      AddFieldRow("RIPEMD160", lblRIPEMD160, txtRIPEMD160, this, iRow++);
      AddFieldRow("SHA1", lblSHA1, txtSHA1, this, iRow++);
      AddFieldRow("SHA256", lblSHA256, txtSHA256, this, iRow++);
      AddFieldRow("SHA384", lblSHA384, txtSHA384, this, iRow++);
      AddFieldRow("SHA512", lblSHA512, txtSHA512, this, iRow++);
      AddFieldRow("CRC32", lblCRC32, txtCRC32, this, iRow++);
    }

    private void btnGenerate_Click(object sender, EventArgs e)
    {
      RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
      byte[] aby = new byte[64];

      rngCsp.GetBytes(aby);
      txtKey.ByteArray = aby;
    }

    private void AddFieldRow(
      string _sTitle, Control _ctrl1, Control _ctrl2, Control _container, int _iRow)
    {
      _ctrl1.Text = _sTitle;
      _ctrl1.Location = new Point(0, _iRow*(24) + 43);
      _ctrl1.AutoSize = true;

      _ctrl2.Location = new Point(100, _iRow*(24) + 40);

      if (_ctrl2 is Label)
      {
        (_ctrl2 as Label).TextAlign = ContentAlignment.BottomLeft;
      }
      else if (_ctrl2 is TextBox)
      {
        (_ctrl2 as TextBox).ReadOnly = true;
        _ctrl2.Font = new Font(FontFamily.GenericMonospace, 9f, FontStyle.Regular);
        _ctrl2.Width = _container.Width - _ctrl2.Location.X - 8;
        _ctrl2.Anchor |= AnchorStyles.Right;
      }

      _container.Controls.Add(_ctrl1);
      _container.Controls.Add(_ctrl2);
    }

    public void HashFields_Empty()
    {
      txtMD5.Text = txtRIPEMD160.Text = txtSHA1.Text = txtSHA256.Text
        = txtSHA384.Text = txtSHA512.Text = txtCRC32.Text = string.Empty;
    }

    public void HashFields_ResetColour()
    {
      txtMD5.BackColor = txtRIPEMD160.BackColor = txtSHA1.BackColor = txtSHA256.BackColor
        = txtSHA384.BackColor = txtSHA512.BackColor = txtCRC32.BackColor
        = SystemColors.Control;
      txtMatcher.BackColor = SystemColors.Window;

      if (chkKey.Checked)
      {
        txtKey.BackColor = SystemColors.Window;
      }
      else
      {
        txtKey.BackColor = SystemColors.Control;
      }
    }

    private void chkKey_CheckedChanged(object sender, EventArgs e)
    {
      HashFields_Empty();
      HashFields_ResetColour();

      txtKey.Enabled = chkKey.Checked;
      btnGenerate.Enabled = chkKey.Checked;

      if (chkKey.Checked)
      {
        chkKey.Font = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Bold);
        lblMD5.Text = "HMACMD5";
        lblRIPEMD160.Text = "HMACRIPEMD160";
        lblSHA1.Text = "HMACSHA1";
        lblSHA256.Text = "HMACSHA256";
        lblSHA384.Text = "HMACSHA384";
        lblSHA512.Text = "HMACSHA512";
      }
      else
      {
        chkKey.Font = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Regular);
        lblMD5.Text = "MD5";
        lblRIPEMD160.Text = "RIPEMD160";
        lblSHA1.Text = "SHA1";
        lblSHA256.Text = "SHA256";
        lblSHA384.Text = "SHA384";
        lblSHA512.Text = "SHA512";
      }

      lblCRC32.Enabled = !chkKey.Checked;
      txtCRC32.Enabled = !chkKey.Checked;

      if (null != this.RequestRecalculate)
      {
        this.Invoke(this.RequestRecalculate);
      }
    }

    private void txtMatcher_TextChanged(object sender, EventArgs e)
    {
      TestMatcher();
    }

    private void txtKey_TextChanged(object sender, EventArgs e)
    {
      HashFields_Empty();
      HashFields_ResetColour();

      if (null != this.RequestRecalculate)
      {
        this.Invoke(this.RequestRecalculate);
      }
    }

    public void Calculate(byte[] _aby)
    {
      HashFields_Empty();
      HashFields_ResetColour();

      if (chkKey.Checked)
      {
        if (!txtKey.IsValid)
        {
          HashFields_Empty();
          HashFields_ResetColour();
          txtKey.BackColor = Color.Salmon;
          return;
        }
        
        byte[] abyKey = txtKey.ByteArray;

        ComputeHash(txtMD5, new HMACMD5(abyKey), _aby);
        ComputeHash(txtRIPEMD160, new HMACRIPEMD160(abyKey), _aby);
        ComputeHash(txtSHA1, new HMACSHA1(abyKey), _aby);
        ComputeHash(txtSHA256, new HMACSHA256(abyKey), _aby);
        ComputeHash(txtSHA384, new HMACSHA384(abyKey), _aby);
        ComputeHash(txtSHA512, new HMACSHA512(abyKey), _aby);
      }
      else
      {
        ComputeHash(txtMD5, MD5.Create(), _aby);
        ComputeHash(txtRIPEMD160, RIPEMD160.Create(), _aby);
        ComputeHash(txtSHA1, SHA1.Create(), _aby);
        ComputeHash(txtSHA256, SHA256.Create(), _aby);
        ComputeHash(txtSHA384, SHA384.Create(), _aby);
        ComputeHash(txtSHA512, SHA512.Create(), _aby);
        ComputeHash(txtCRC32, new CRC32(), _aby);
      }

      TestMatcher();
    }

    public void Calculate(Stream _stream)
    {
      if (chkKey.Checked)
      {
        if (!txtKey.IsValid)
        {
          HashFields_Empty();
          HashFields_ResetColour();
          txtKey.BackColor = Color.Salmon;
          return;
        }

        byte[] abyKey = txtKey.ByteArray;

        ComputeHash(txtMD5, new HMACMD5(abyKey), _stream);
        ComputeHash(txtRIPEMD160, new HMACRIPEMD160(abyKey), _stream);
        ComputeHash(txtSHA1, new HMACSHA1(abyKey), _stream);
        ComputeHash(txtSHA256, new HMACSHA256(abyKey), _stream);
        ComputeHash(txtSHA384, new HMACSHA384(abyKey), _stream);
        ComputeHash(txtSHA512, new HMACSHA512(abyKey), _stream);
      }
      else
      {
        ComputeHash(txtMD5, MD5.Create(), _stream);
        ComputeHash(txtRIPEMD160, RIPEMD160.Create(), _stream);
        ComputeHash(txtSHA1, SHA1.Create(), _stream);
        ComputeHash(txtSHA256, SHA256.Create(), _stream);
        ComputeHash(txtSHA384, SHA384.Create(), _stream);
        ComputeHash(txtSHA512, SHA512.Create(), _stream);
        ComputeHash(txtCRC32, new CRC32(), _stream);
      }

      TestMatcher();
    }

    private void TestMatcher()
    {
      HashFields_ResetColour();
      string str = txtMatcher.Text.Replace(" ", "");

      if (str.Length == 0)
      {
        return;
      }

      if (String.Compare(txtMD5.Text, str, true) == 0)
      {
        txtMD5.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtRIPEMD160.Text, str, true) == 0)
      {
        txtRIPEMD160.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtSHA1.Text, str, true) == 0)
      {
        txtSHA1.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtSHA256.Text, str, true) == 0)
      {
        txtSHA256.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtSHA384.Text, str, true) == 0)
      {
        txtSHA384.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtSHA512.Text, str, true) == 0)
      {
        txtSHA512.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
      else if (String.Compare(txtCRC32.Text, str, true) == 0)
      {
        txtCRC32.BackColor = txtMatcher.BackColor = Color.LightGreen;
      }
    }

    private void ComputeHash(TextBox _txtBox, HashAlgorithm _ha, byte[] _aby)
    {
      byte[] aby = _ha.ComputeHash(_aby);
      _ha.Clear();
      _ha = null;

      _txtBox.Text = ByteArrayToString(aby);
    }

    private void ComputeHash(TextBox _txtBox, HashAlgorithm _ha, Stream _stream)
    {
      _stream.Seek(0, SeekOrigin.Begin);

      byte[] aby = _ha.ComputeHash(_stream);
      _ha.Clear();
      _ha = null;

      _txtBox.Text = ByteArrayToString(aby);
    }

    private string ByteArrayToString(byte[] _aby)
    {
      StringBuilder sb = new StringBuilder(2 * _aby.Length);

      for (int c = 0, length = _aby.Length; c < length; c++)
      {
        sb.Append(_aby[c].ToString("X2"));
      }

      return sb.ToString();
    }

    public void FileHash_Start(string _sFilename)
    {
      HashFields_Empty();
      HashFields_ResetColour();

      if (chkKey.Checked)
      {
        if (!txtKey.IsValid)
        {
          HashFields_Empty();
          HashFields_ResetColour();
          txtKey.BackColor = Color.Tomato;
          return;
        }

        byte[] abyKey = txtKey.ByteArray;

        m_iTotalHashAlgorithms = 6;
        FileHash_ThreadInit(_sFilename, new HMACMD5(abyKey), txtMD5);
        FileHash_ThreadInit(_sFilename, new HMACRIPEMD160(abyKey), txtRIPEMD160);
        FileHash_ThreadInit(_sFilename, new HMACSHA1(abyKey), txtSHA1);
        FileHash_ThreadInit(_sFilename, new HMACSHA256(abyKey), txtSHA256);
        FileHash_ThreadInit(_sFilename, new HMACSHA384(abyKey), txtSHA384);
        FileHash_ThreadInit(_sFilename, new HMACSHA512(abyKey), txtSHA512);
      }
      else
      {
        m_iTotalHashAlgorithms = 7;
        FileHash_ThreadInit(_sFilename, MD5.Create(), txtMD5);
        FileHash_ThreadInit(_sFilename, RIPEMD160.Create(), txtRIPEMD160);
        FileHash_ThreadInit(_sFilename, SHA1.Create(), txtSHA1);
        FileHash_ThreadInit(_sFilename, SHA256.Create(), txtSHA256);
        FileHash_ThreadInit(_sFilename, SHA384.Create(), txtSHA384);
        FileHash_ThreadInit(_sFilename, SHA512.Create(), txtSHA512);
        FileHash_ThreadInit(_sFilename, new CRC32(), txtCRC32);
      }
    }

    private void FileHash_ThreadInit(string _sFilename, HashAlgorithm _ha, TextBox _txtBox)
    {
      Thread thread = new Thread(FileHash_Thread);
      thread.Priority = ThreadPriority.Normal;
      thread.IsBackground = true;
      thread.Start(new object[] {_sFilename, _ha, _txtBox});
    }

    private void FileHash_Thread(object _aArgs)
    {
      object[] args = _aArgs as object[];
      string sFilename = args[0] as string;
      HashAlgorithm ha = args[1] as HashAlgorithm;
      TextBox txtBox = args[2] as TextBox;
      FileInfo fi = null;
      FileStream fs = null;
      byte[] data = null;
      string str = string.Empty;

      try
      {
        // assume file exists
        fi = new FileInfo(sFilename);
        fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

        data = ha.ComputeHash(fs);
        str = ByteArrayToString(data);
      }
      catch (Exception xcp)
      {
        str = xcp.Message;
      }
      finally
      {
        if (null != fs) { fs.Close(); }
      }

      this.Invoke(this.OnFileHash_Result, new object[] {txtBox, str});
    }

    /// <summary> provided to the thread results display </summary>
    private Delegate_OnFileHash_Result OnFileHash_Result = null;
    private delegate void Delegate_OnFileHash_Result(TextBox _txtBox, string _str);

    private int m_iTotalHashAlgorithms = 7;
    private void FileHash_Result(TextBox _txtBox, string _str)
    {
      _txtBox.Text = _str;
      _txtBox.UseWaitCursor = false;
      TestMatcher();

      if (--m_iTotalHashAlgorithms == 0 && null != this.OnFileHash_End)
      {
        this.Invoke(this.OnFileHash_End);
      }
    }

    /// <summary> provided for the control owner to end long-process animation </summary>
    public Delegate_OnFileHash_End OnFileHash_End = null;
    public delegate void Delegate_OnFileHash_End();
  }
}
