using System;
using System.Collections.Generic;

using System.Text;
using System.Web.UI.HtmlControls;
using System.IO;
using VKHotkeys.Parsers;


namespace VKHotkeys.Data
{
  public class Album
  {
    public int ID
    {
      get;
      set;
    }

    public String Name
    {
      get;
      set;
    }

    private List<Song> songs;
    public List<Song> Songs
    {
      get
      {
        if (songs == null)
          songs = new List<Song>();

        return songs;
      }
      set
      {
        songs = value;
      }
    }


    public override string ToString()
    {
      return Name;
    }

    public static List<Album> ParseAlbumsPost(String page)
    {
      List<Album> albums = new List<Album>();
      
      String startTag = "albums\":{";
      int index_start = page.IndexOf(startTag);
      if (index_start == -1)
        return albums;

      index_start += startTag.Length;

      int index_end = page.IndexOf("}}", index_start + 1);
      if (index_end == -1)
        return albums;

      String all = page.Substring(index_start, index_end - index_start);

      //String[] items = all.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries).Where(z => z.Length > 1).ToArray();
      String[] items = TextParseTools.Split(all, new String[]{ "{","}"}, "\"", false);

      for (int i = 0; i < items.Length; i++)
      {
        //String[] fields = items[i].Split(',',':');
        String[] fields = TextParseTools.Split(items[i], new String[] { ",", ":" }, "\"", false);

        if (fields.Length <= 3)
          continue;

        Album album = new Album();
        //album.id = fields[1];

        int id;
        if (!Int32.TryParse(TextParseTools.RemoveStartEndChars(fields[1]), out id))
          continue;

        album.ID = id;
        album.Name = System.Web.HttpUtility.HtmlDecode(TextParseTools.RemoveStartEndChars(fields[3]));
           
        albums.Add(album);
      }

      return albums;
    }

  }
}
