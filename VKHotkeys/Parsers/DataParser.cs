using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VKHotkeys.Data;

namespace VKHotkeys.Parsers
{
  public class DataParser
  {
    public static List<Album> ParseAlbumWithSongs(String page)
    {

      List<Album> albums = Album.ParseAlbumsPost(page);
      List<Song> songs =  Song.ParseSongsPost(page);
      
      //Добавляем песни в альбомы
      albums.Add(new Album() { Name = "My music", ID = 0 });
      foreach (var album in albums)
      {
          if (album.ID == 0)
          {
              album.Songs = songs.ToList();
          }
          else
          {
              album.Songs = songs.Where(z => z.AlbumID == album.ID).ToList();
          }
        
      }
       /* for

      if (song.AlbumID == 0 && song.DownloadURL == "") // if My Music and Link broken
      {
          song.DownloadURL =
          BrokenSongs.Add(song);
          continue;
      }*/

      //Создаем альбом неотсортированных записей

      albums.Add(new Album() { Name = "Suggestions", ID = 999 });
      albums.Add(new Album() { Name = "Popular", ID = 1000 });

      return albums;
    }

  }
}
