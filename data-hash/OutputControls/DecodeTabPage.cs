using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class DecodeTabPage: TabPage
  {
    public DecodeTabPage ()
    {
    }

    protected override void InitLayout ()
    {
      base.InitLayout ();
      this.Text = "Decode";
    }
  }
}
