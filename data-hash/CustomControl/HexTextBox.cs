using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Windows.Forms
{
  public class HexTextBox: TextBox
  {
    public class InvalidHexStringException : ApplicationException { }

    private string m_sDelimiters = "|.,:;+-=_`'\"*[]<>()@#&\t\x20\xA0\x0D\x0A\x00";
    private Regex m_rHexChars = new Regex(@"[\dABCDEF\s\b\r\n]"
      , RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public HexTextBox()
    {
      this.Font = new Font(FontFamily.GenericMonospace, 9f, FontStyle.Regular);
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      string sKeyChar = e.KeyChar.ToString();
      e.Handled = true; // deny press

      if (m_rHexChars.IsMatch(sKeyChar))
      { // allow press
        e.KeyChar = sKeyChar.ToUpper()[0];
        e.Handled = false;
      }
      else if (e.KeyChar == 3 || e.KeyChar == 22)
      { // ^+C and ^+V
        e.Handled = false;
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      if (this.IsValid)
      {
        base.OnTextChanged(e);
      }
    }

    /// <summary> Validate Hex string </summary>
    public bool IsValid
    {
      get
      {
        string str = this.Text.Replace(" ", "").ToUpper();

        if (0 == str.Length)
        {
          return true;
        }
        else if (str.Length < 2)
        {
          return false;
        }

        char[] ac = str.ToCharArray();
        char c;
        bool bLow4bits = false;

        for (int iStrIndex = 0; iStrIndex < ac.Length; iStrIndex++)
        {
          c = ac[iStrIndex];

          if (m_sDelimiters.IndexOf(c, 0) != -1)
          { // skip delimiter
            continue;
          }

          if (c >= 48 && c <= 57)
          { // numbers 0 <= 9
            c -= (char)48;
          }
          else if (c >= 65 && c <= 70)
          { // A <= F
            c -= (char)55;
          }
          else
          { // not a HEX character
            return false;
          }

          bLow4bits = !bLow4bits;
        }

        if (bLow4bits)
        {
          return false;
        }

        return true;
      }
    }

    /// <summary> Retrieve hex string as byte array </summary>
    /// <exception cref="InvalidHexStringException"></exception>
    public byte[] ByteArray
    {
      get
      {
        string str = this.Text.Replace(" ", "").ToUpper();

        if (0 == str.Length)
        {
          return new byte[] {};
        }
        else if (str.Length < 2)
        {
          throw new InvalidHexStringException();
        }

        // approximate buffer size (larger or equal to the real data)
        uint uiSignSize = (uint)Math.Ceiling(str.Length / 2.0);
        byte[] ab = new byte[uiSignSize];
        char[] ac = str.ToCharArray();
        char c;
        bool bLow4bits = false;
        int iSignIndex = 0;

        // start string parsing
        int iStrIndex = 0;
        for (; iStrIndex < ac.Length; iStrIndex++)
        {
          c = ac[iStrIndex];

          if (m_sDelimiters.IndexOf(c, 0) != -1)
          { // skip delimiter
            continue;
          }

          if (c >= 48 && c <= 57)
          { // numbers 0 <= 9
            c -= (char)48;
          }
          else if (c >= 65 && c <= 70)
          { // A <= F
            c -= (char)55;
          }
          else
          { // not a HEX character
            throw new InvalidHexStringException();
          }

          if (bLow4bits == false)
          { // set high-order 4 bits
            ab[iSignIndex] = (byte)(c << 4);
          }
          else
          { // set low-order 4 bits
            ab[iSignIndex++] |= (byte)c;
          }

          bLow4bits = !bLow4bits;
        }

        if (bLow4bits)
        {
          throw new InvalidHexStringException();
        }

        // change signature buffers
        byte[] rv = null;
        if (uiSignSize != iSignIndex)
        { // copy
          rv = new byte[iSignIndex];
          for (int j = 0; j < iSignIndex; j++)
          {
            rv[j] = ab[j];
          }
          ab = null;
        }
        else
        {
          rv = ab;
        }

        return rv;
      }

      set
      {
        StringBuilder sb = new StringBuilder(3 * value.Length);

        for (int c = 0, length = value.Length; c < length; c++)
        {
          if (c == 0) {}
          else if (c%32 == 0)
          {
            sb.Append("\r\n");
          }
          else if (c%8 == 0)
          {
            sb.Append("|");
          }
          else
          {
            sb.Append(" ");
          }

          sb.Append(value[c].ToString("X2"));
        }

        this.Text = sb.ToString();
      }
    }
  }
}
