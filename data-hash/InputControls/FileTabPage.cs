using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace datahash
{
  public class FileTabPage: TabPage
  {
    private Button btnFile = new Button();
    public Label lblFile = new Label();
    public Label lblInfo = new Label();

    public FileTabPage ()
    {
      this.Text = this.Name = "File";
      
      btnFile.Text = "&Browse...";
      btnFile.Location = new Point(8, 8);
      btnFile.Click += btnFile_Click;
      this.Controls.Add(btnFile);

      lblFile.AutoEllipsis = true;
      lblFile.Location = new Point(btnFile.Location.X + btnFile.Width + 8, 8);
      lblFile.Width = this.Width - lblFile.Location.X - 16;
      lblFile.Anchor |= AnchorStyles.Right;
      lblFile.BorderStyle = BorderStyle.FixedSingle;
      lblFile.TextAlign = ContentAlignment.MiddleLeft;
      this.Controls.Add(lblFile);

      lblInfo.AutoSize = true;
      lblInfo.Location = new Point(lblFile.Location.X, lblFile.Location.Y + lblFile.Height + 8);
      this.Controls.Add(lblInfo);
    }

    private void btnFile_Click(object sender, EventArgs e)
    {
      lblFile.Text = string.Empty;

      OpenFileDialog dlgFile = new OpenFileDialog();
      dlgFile.Multiselect = false;

      DialogResult dlgResult = dlgFile.ShowDialog();
      if (dlgResult != DialogResult.OK)
      {
        return;
      }

      lblFile.Text = dlgFile.FileName;
      FileInfo fi = new FileInfo(dlgFile.FileName);

      lblInfo.Text = String.Format("Modified: {0}\nSize: {1:N0} Bytes"
        , fi.LastWriteTime.ToString()
        , fi.Length
        );

      if (null != Trigger)
      {
        this.Invoke(Trigger, dlgFile.FileName);
      }
    }

    public delegate void DelegateTrigger(string _sFilename);
    public DelegateTrigger Trigger = null;
  }
}
