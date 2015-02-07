using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class OutputTabControl: TabControl
  {
    public HashTabPage tabHash = new HashTabPage();
    public EncodeTabPage tabEncode = new EncodeTabPage();
    public DecodeTabPage tabDecode = new DecodeTabPage();

    public OutputTabControl ()
    {
      this.Controls.Add(tabHash);
      //this.Controls.Add(tabEncode);
      //this.Controls.Add(tabDecode);
    }
  }
}
