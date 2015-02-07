using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class EncodeTabPage: TabPage
  {
    public EncodeTabPage ()
    {
    }

    protected override void InitLayout ()
    {
      base.InitLayout ();
      this.Text = "Encode";
    }
  }
}
