using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class HexTabPage: TabPage
  {
    private HexTextBox txtHex = new HexTextBox();
    private Label lblLength = new Label();

    public HexTabPage ()
    {
      this.Text = this.Name = "Hex";

      txtHex.Multiline = true;
      txtHex.Width = this.Width;
      txtHex.Height = this.Height - 24;
      txtHex.Anchor |= AnchorStyles.Right | AnchorStyles.Bottom;
      txtHex.ScrollBars = ScrollBars.Vertical;
      txtHex.TextChanged += Changed;
      this.Controls.Add(txtHex);

      lblLength.AutoSize = true;
      lblLength.Location = new Point(0, txtHex.Height + 8);
      lblLength.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      lblLength.Text = "0 bytes";
      this.Controls.Add(lblLength);
    }

    public void Changed(object sender, EventArgs e)
    {
      byte[] aby = txtHex.ByteArray;

      lblLength.Text = String.Format("{0} byte{1}"
        , aby.Length, aby.Length == 1? "" : "s");

      if (null != Trigger)
      {
        this.Invoke(Trigger, aby);
      }
    }

    public delegate void DelegateTrigger(byte[] _aby);
    public DelegateTrigger Trigger = null;
  }
}
