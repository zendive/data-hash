using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace datahash
{
  public class InputTabControl: TabControl
  {
    public TextTabPage tabText = new TextTabPage();
    public HexTabPage tabHex = new HexTabPage();
    public FileTabPage tabFile = new FileTabPage();
    public WebTabPage tabWeb = new WebTabPage();

    public InputTabControl ()
    {
      this.Controls.Add(tabText);
      this.Controls.Add(tabHex);
      this.Controls.Add(tabFile);
      this.Controls.Add(tabWeb);

      this.Height = 108;
    }
  }
}
