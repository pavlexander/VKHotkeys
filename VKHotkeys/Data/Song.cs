using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;

using BinaryAnalysis.UnidecodeSharp;

using VKHotkeys.Parsers;

using System.Text.RegularExpressions;

namespace VKHotkeys.Data
{
    public class Song
    {

        public String Author
        { get; set; }

        public String Name
        { get; set; }

        public String DownloadURL
        { get; set; }

        public String aid
        { get; set; }

        public String oid
        { get; set; }

        private String fileNameForSave;

        public String FileNameForSave
        {
            get
            {
                if (fileNameForSave == null)
                {
                    fileNameForSave = Regex.Replace(HttpUtility.HtmlDecode(Author + "_" + Name), @"[^\w\s:]", "", RegexOptions.IgnorePatternWhitespace);
                }

                //fileNameForSave = Author + " - " + Name + ".mp3";

                return fileNameForSave;
            }
            set { fileNameForSave = value; }
        }

        public int AlbumID
        { get; set; }

        public override string ToString()
        {
            return Author + " - " + Name;
        }

        public static List<Song> ParseSongsPost(String page)
        {
            List<Song> songs = new List<Song>();


            String leftBracket = "{";
            String rightBracket = "<!>";

            int index_start = page.IndexOf(leftBracket);
            if (index_start == -1)
                return songs;

            int index_end = page.IndexOf(rightBracket, index_start + 1);

            String all = page.Substring(index_start, index_end - index_start);

            //      String[] items = all.Split(new char[]{'[', ']' }, StringSplitOptions.RemoveEmptyEntries).Where(z=>z.Length>1).ToArray();
            String[] items = TextParseTools.Split(all, new String[] { "[", "]" }, "'", false);

            for (int i = 0; i < items.Length; i++)
            {
                //String[] fields = items[i].Split(',');
                String[] fields = TextParseTools.Split(items[i], new String[] { "," }, "'", false);

                if (fields.Length <= 7)
                    continue;

                Song song = new Song();
                song.aid = TextParseTools.RemoveStartEndChars(fields[1]);
                song.oid = TextParseTools.RemoveStartEndChars(fields[0]);
                song.Author = fields[5];
                song.Name = fields[6];
                song.DownloadURL = fields[2];


                song.DownloadURL = TextParseTools.RemoveStartEndChars(song.DownloadURL);
                song.Author = System.Web.HttpUtility.HtmlDecode(TextParseTools.RemoveStartEndChars(song.Author));

                song.Name = System.Web.HttpUtility.HtmlDecode(TextParseTools.RemoveStartEndChars(song.Name));

                int id;
                if (Int32.TryParse(TextParseTools.RemoveStartEndChars(fields[8]), out id))
                    song.AlbumID = id;

                songs.Add(song);
            }


            return songs;
        }

    }
  
}
