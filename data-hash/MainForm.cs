using System;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace datahash
{
  public partial class MainForm: Form
  {
    private const string sWORK = "Please, wait...";
    private const string sIDLE = "Ready";

    private StatusStrip ssStatus = new StatusStrip();
    private ToolStripStatusLabel lblStatus = new ToolStripStatusLabel();

    private InputTabControl tcInput = new InputTabControl();
    private OutputTabControl tcOutput = new OutputTabControl();

    public MainForm(string _sFilename)
    {
      this.Text = Program.Name;
      this.MinimumSize = new Size(710, 480);

      this.KeyPreview = true;
      this.KeyDown += this.OnForm_KeyDown;
      this.AllowTransparency = true;
      this.AllowDrop = true;

      this.DragEnter += delegate(object sender, DragEventArgs e) {
        if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
        {
          e.Effect = DragDropEffects.Copy;
        }
      };

      this.DragDrop += delegate(object sender, DragEventArgs e) {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length != 0 && File.Exists(files[0]))
        {
          HashFromShellFile(files[0]);
        }
      };

      this.Load += Form_Load;

      if (!String.IsNullOrEmpty(_sFilename))
      { // process shell given file
        this.Load += delegate {
          HashFromShellFile(_sFilename);
        };
      }

      tcInput.tabText.Trigger = HashFromByteArray;
      tcInput.tabHex.Trigger = HashFromByteArray;
      tcInput.tabFile.Trigger = HashFromFile_Begin;
      tcInput.tabWeb.Start = HashFromStream_Start;
      tcInput.tabWeb.End = HashFromStream_End;
      tcInput.tabWeb.Trigger = HashFromStream;

      tcOutput.tabHash.OnFileHash_End += HashFromFile_End;
      tcOutput.tabHash.RequestRecalculate += RequestRecalculateHash;
    }

    private void RequestRecalculateHash()
    {
      switch (tcInput.SelectedTab.Name)
      {
        case "Text": tcInput.tabText.Changed(null, null); break;
        case "Hex": tcInput.tabHex.Changed(null, null); break;
      }
    }

    private void HashFromStream(Stream _stream)
    {
      lblStatus.Text = sWORK;
      tcOutput.tabHash.Calculate(_stream);
      lblStatus.Text = sIDLE;
    }

    private void HashFromStream_Start()
    {
      lblStatus.Text = sWORK;
      this.UseWaitCursor = true;
      EnableControls(false);
    }

    private void HashFromStream_End()
    {
      lblStatus.Text = sIDLE;
      this.UseWaitCursor = false;
      EnableControls(true);
    }

    private void HashFromShellFile(string _sFilename)
    {
      tcInput.SelectTab("File");
      tcInput.tabFile.lblFile.Text = _sFilename;
      HashFromFile_Begin(_sFilename);
    }

    private void HashFromByteArray(byte[] _aby)
    {
      lblStatus.Text = sWORK;
      tcOutput.tabHash.Calculate(_aby);
      lblStatus.Text = sIDLE;
    }

    private void HashFromFile_Begin(string _sFilename)
    {
      lblStatus.Text = sWORK;
      tcOutput.tabHash.FileHash_Start(_sFilename);
      this.UseWaitCursor = true;
      EnableControls(false);
    }

    private void HashFromFile_End()
    {
      this.UseWaitCursor = false;
      EnableControls(true);
      lblStatus.Text = sIDLE;
      GC.Collect();
    }

    private void EnableControls(bool _bEnable)
    {
      tcInput.Enabled = _bEnable;
      tcOutput.Enabled = _bEnable;
    }

    void Form_Load(object sender, EventArgs e)
    {
      tcInput.Width = this.Width - 8;
      tcInput.Anchor |= AnchorStyles.Right | AnchorStyles.Bottom;
      this.Controls.Add(tcInput);

      tcOutput.Height = this.Height - tcInput.Height - 58;
      tcOutput.AutoSize = true;
      tcOutput.Dock = DockStyle.Bottom;
      this.Controls.Add(tcOutput);

      lblStatus.Text = sIDLE;
      ssStatus.Items.Add(lblStatus);
      this.Controls.Add(ssStatus);
    }

    private void OnForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.F11)
      {
        e.Handled = true;
        if (this.Opacity < 0.11d) { return; }
        this.Opacity -= 0.1d;
      }
      else if (e.KeyCode == Keys.F12)
      {
        e.Handled = true;
        this.Opacity += 0.1d;
      }
      else if (e.KeyCode == Keys.Escape)
      {
        e.Handled = true;
        Application.Exit();
      }
      else if (e.KeyCode == Keys.F1)
      {
        e.Handled = true;
        string link = "https://github.com/zendive/data-hash/";

        DialogResult dlgResult = MessageBox.Show(
          "More information available at: \""+ link +"\"\n\nProceed online?"
          , Program.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (DialogResult.Yes == dlgResult)
        {
          using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
          {
            proc.StartInfo.FileName = link;
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
          }
        }
      }
    }

  }
}
