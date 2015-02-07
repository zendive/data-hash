using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

[assembly: System.Reflection.AssemblyVersion("1.0.*")]
namespace datahash
{
  /// <summary>
  /// Implementation of Hash viewer:
  /// http://msdn.microsoft.com/en-us/library/system.security.cryptography.hashalgorithm.aspx
  /// </summary>
  static class Program
  {
    public static Regex rgVer3 = new Regex(@"(\d+\.\d+.\d+).*", RegexOptions.Compiled);
    public static string Name = Application.ProductName +" v"+ rgVer3.Replace(Application.ProductVersion, "$1");

    /// <summary> Entry point </summary>
    /// <param name="args"> accept filename </param>
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        string sFilename = String.Empty;
        if (args.Length != 0 && File.Exists(args[0]))
        { // process shell given file
          sFilename = args[0];
        }

        using (MainForm form = new MainForm(sFilename))
        {
          Application.Run(form);
        }
      }
      catch (Exception xcp)
      {
        MessageBox.Show(xcp.ToString(), Program.Name);
      }
    }
  }


}