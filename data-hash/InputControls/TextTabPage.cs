using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class TextTabPage: TabPage
  {
    private TextBox txtText = new TextBox();
    private ComboBox cmbEncoding = new ComboBox();
    private Label lblLength = new Label();
    private HexTextBox txtHex = new HexTextBox();
    private Encoding m_encoding = Encoding.ASCII;

    public TextTabPage ()
    {
      this.Text = this.Name = "Text";

      txtText.Multiline = true;
      txtText.Width = this.Width;
      txtText.Anchor |= AnchorStyles.Right;
      txtText.ScrollBars = ScrollBars.Vertical;
      txtText.TextChanged += Changed;
      this.Controls.Add(txtText);

      cmbEncoding.Location = new Point(0, txtText.Height + 22);
      cmbEncoding.Anchor = AnchorStyles.Left;
      cmbEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
      cmbEncoding.Items.Add("ASCII");
      cmbEncoding.Items.Add("UTF7");
      cmbEncoding.Items.Add("UTF8");
      cmbEncoding.Items.Add("UTF32");
      cmbEncoding.Items.Add("Unicode");
      cmbEncoding.Items.Add("BigEndianUnicode");
      cmbEncoding.SelectedIndex = 0;
      cmbEncoding.SelectedIndexChanged += delegate (object sender, EventArgs e) {
        switch (cmbEncoding.SelectedIndex)
        {
          case 0: m_encoding = Encoding.ASCII; break;
          case 1: m_encoding = Encoding.UTF7; break;
          case 2: m_encoding = Encoding.UTF8; break;
          case 3: m_encoding = Encoding.UTF32; break;
          case 4: m_encoding = Encoding.Unicode; break;
          case 5: m_encoding = Encoding.BigEndianUnicode; break;
          default: MessageBox.Show("cmbEncoding.SelectedIndex = "+ cmbEncoding.SelectedIndex);
            return;
        }
        Changed(null, null);
      };
      this.Controls.Add(cmbEncoding);

      lblLength.Location = new Point(cmbEncoding.Width + 8, txtText.Height + 26);
      lblLength.Anchor = AnchorStyles.Left;
      lblLength.AutoSize = true;
      lblLength.Text = "0 characters; 0 bytes";
      this.Controls.Add(lblLength);

      txtHex.Multiline = true;
      txtHex.ReadOnly = true;
      txtHex.Location = new Point(0, cmbEncoding.Location.Y + cmbEncoding.Height + 8);
      txtHex.Width = this.Width;
      txtHex.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      txtHex.ScrollBars = ScrollBars.Vertical;
      this.Controls.Add(txtHex);
    }

    protected override void OnResize (EventArgs e)
    {
      base.OnResize(e);
      txtText.Height = this.Height/2 - 16;
      txtHex.Height = this.Height - txtHex.Location.Y;
    }

    public void Changed(object sender, EventArgs e)
    {
      byte[] aby = m_encoding.GetBytes(txtText.Text);

      txtHex.ByteArray = aby;

      lblLength.Text = String.Format("{0} character{1}; {2} byte{3}"
        , txtText.Text.Length
        , txtText.Text.Length == 1? "" : "s"
        , aby.Length
        , aby.Length == 1? "" : "s");

      if (null != Trigger)
      {
        this.Invoke(Trigger, aby);
      }
    }

    public delegate void DelegateTrigger(byte[] _aby);
    public DelegateTrigger Trigger = null;
  }
}
