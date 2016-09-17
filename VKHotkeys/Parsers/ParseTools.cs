using System;
using System.Collections.Generic;

using System.Text;

namespace VKHotkeys.Parsers
{
  public class TextParseTools
  {
    /// <summary> Строка без первого и последнего символа </summary>    
    public static string RemoveStartEndChars(String s)
    {
      if ((s != null) && (s.Length > 2))
        return s.Substring(1, s.Length - 2);
      else
        return s;
    }

    /// <summary>Обычный String.Split, но с учетом квалификаторов выражений.
    /// Например, для того, чтобы не разбирать выражение внутри кавычек (qualifier="\"")
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="delimiter"></param>
    /// <param name="qualifier"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static string[] Split(string expression, string[] delimiter, string qualifier, bool ignoreCase)
    {
      bool _QualifierState = false;
      int _StartIndex = 0;
      System.Collections.ArrayList _Values = new System.Collections.ArrayList();

      for (int _CharIndex = 0; _CharIndex < expression.Length - 1; _CharIndex++)
      {
        if ((qualifier != null)
         & (string.Compare(expression.Substring(_CharIndex, qualifier.Length), qualifier, ignoreCase) == 0))
        {
          _QualifierState = !(_QualifierState);
        }
        else if (!(_QualifierState) & (delimiter != null) )
        {
          for (int i=0;i<delimiter.Length;i++)
          {
            if (string.Compare(expression.Substring (_CharIndex, delimiter[i].Length), delimiter[i], ignoreCase) == 0)
            {
              _Values.Add(expression.Substring (_StartIndex, _CharIndex - _StartIndex));
              _StartIndex = _CharIndex + 1;
              break;
            }  
          }        
        }          
      }

      if (_StartIndex < expression.Length)
        _Values.Add(expression.Substring(_StartIndex, expression.Length - _StartIndex));

      string[] _returnValues = new string[_Values.Count];
      _Values.CopyTo(_returnValues);
      return _returnValues;
    }
  }
}
