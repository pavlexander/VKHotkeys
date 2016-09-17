using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using BinaryAnalysis.UnidecodeSharp;
using VKHotkeys.Data;


namespace VKHotkeys.VK
{
  public class MusicDownloader
  {
    
    public static string GetPath(Song song, List<Album> albums, String basePath, bool catalogForAlbums, bool catalogForAuthor, bool transliterate)
    {
      String outPath = basePath;

      Directory.CreateDirectory(outPath);

      if (catalogForAlbums)
      {
        Album album = albums.Find(z=>z.ID==song.AlbumID);
        if (album != null)
        {
          String albumPath = transliterate ? album.Name.Unidecode() : album.Name;
          albumPath = RemoveIllegalFileNameChars(albumPath);
          outPath = Path.Combine(outPath, albumPath);
        }
          
        Directory.CreateDirectory(outPath);
      }

      if (catalogForAuthor)
      {
        String authorPath = transliterate? song.Author.Unidecode(): song.Author;
        authorPath = RemoveIllegalFileNameChars(authorPath);
        outPath = Path.Combine(outPath, authorPath);
        Directory.CreateDirectory(outPath);
      }

      String fileName = song.FileNameForSave;

      if (transliterate)
        fileName = fileName.Unidecode();

      fileName = fileName.Replace("\"", "'");//заменяем кавычки апострофом

      fileName = RemoveIllegalFileNameChars(fileName);

      outPath = Path.Combine(outPath , fileName); 
      //outPath = RemoveIllegalPathChars(outPath);
      return outPath;
    }
  
    public static string RemoveIllegalCatalogChars(string catalog)
    {
      string invalid = new string(Path.GetInvalidFileNameChars()); // http://blogs.msdn.com/b/bclteam/archive/2005/01/11/351060.aspx
      String dir = catalog;
      foreach (char c in invalid)
      {
        dir = dir.Replace(c.ToString(), "");
      }
      return dir;
    }

    public static string RemoveIllegalFileNameChars(string fileName)
    {
      string invalid = new string(Path.GetInvalidFileNameChars());
      String file = fileName;
      foreach (char c in invalid)
      {
        fileName = fileName.Replace(c.ToString(), "_");
      }
      return fileName;
    }
    
  }
}
