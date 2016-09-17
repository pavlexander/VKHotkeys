using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

using System.IO;
using System.Windows.Forms;

using VKHotkeys.VK;
using VKHotkeys.Data;
using VKHotkeys.Parsers;

using HtmlAgilityPack;
using System.Text.RegularExpressions;

using System.Security.Cryptography;

namespace VKHotkeys
{
    partial class Form1
    {
        private VKUserInfo UserInfo;
        public List<Album> albums = new List<Album>();

        public List<Song> BrokenSongs;

        public Cookie cook;

        public bool authorized;
        public string location;

        public WMPLib.IWMPPlaylist pl;
        public WMPLib.IWMPMedia temp;
        public static Album currect_album;

        public static int current_playing_id;

        public bool repeat;
        public bool shuffle;

        public void InitializeCommands()
        {
            authorized = false;
            location = "none";
            repeat = false;
            shuffle = false;
        }

        //private static Dictionary<string, string> settings { get { return Form1.settings; } }
        //private static int someints { get { return Form1.; } }

        public void PopulateAlbum(String url)
        {
            currect_album = null;
            albums = new List<Album>();

            int id = UserInfo.UserID;
            //string fullUrl = url + id;
            string fullUrl = url.Replace("$PROFILE$", id.ToString());
            string page = VKUtils.PostRequest(fullUrl, cook);

            /////////////////////////////

            //add_hash
            int index_add_hash = page.IndexOf("add_hash");
            add_hash = page.Substring(index_add_hash + 11, 18);

            //reorder_hash
            int index_reorder_hash = page.IndexOf("reorder_hash");
            reorder_hash = page.Substring(index_reorder_hash + 15, 18);

            //delete_hash
            int index_delete_hash = page.IndexOf("delete_hash");
            delete_hash = page.Substring(index_delete_hash + 14, 18);
           
            /////////////////////////////

            //MessageBox.Show(add_hash + reorder_hash + delete_hash);

            //javascript: window.location.href = 'http://www.vk.com';
            albums = DataParser.ParseAlbumWithSongs(page);
        }

        public List<string> ids_values;
        public List<string> friend_name_values;
        public int Endoflist;

        public void PopulateFriendList()
        {

            string fullUrl = "http://vk.com/audio?act=more_friends&al=1";
            string page = VKUtils.PostRequest(fullUrl, cook);
            
            /////////////////////////////

            // friend ids
            string id_string = "";
            string search_id = @"<!><!json>[";
            int index_ids = page.IndexOf(search_id);
            id_string = page.Substring(index_ids + search_id.Length, page.Length - (index_ids + search_id.Length));
            
            string search_id_end = @"]<!>";
            int index_ids_end = id_string.IndexOf(search_id_end);
            id_string = id_string.Substring(0, index_ids_end);
            ids_values = id_string.Split(',').ToList();

            if (ids_values.Count() == 0)
                return;

            // friend names
            friend_name_values = getFriendNames(page);
            if (friend_name_values.Count() == 0)
                return;

            //check
            if (ids_values.Count() != friend_name_values.Count())
                return;

            bool end_of_friends = false;

            while (!end_of_friends)
            {
                string newurl = fullUrl;
                for (int ii = 0; ii < ids_values.Count(); ii++)
                {
                    newurl = newurl + "&ids[" + ii + "]=" + ids_values[ii];
                }

                page = VKUtils.PostRequest(newurl, cook);
                index_ids = page.IndexOf(search_id);
                id_string = page.Substring(index_ids + search_id.Length, page.Length - (index_ids + search_id.Length));
                index_ids_end = id_string.IndexOf(search_id_end);
                id_string = id_string.Substring(0, index_ids_end);

                string[] ids_values_TEMP = id_string.Split(',');

                if (ids_values_TEMP.Count() == 0)
                {
                    end_of_friends = true;
                    break;
                }

                List<string> addedindexes = new List<string>();
                for (int iii = 0; iii < ids_values_TEMP.Count(); iii++)
                {
                    if (!ids_values.Contains(ids_values_TEMP[iii]))
                    {
                        ids_values.Add(ids_values_TEMP[iii]);
                        addedindexes.Add(iii.ToString());
                    }
                }

                if (addedindexes.Count > 0)
                {
                    List<string> friend_name_values_TEMP = getFriendNames(page);

                    for (int iiii = 0; iiii < friend_name_values_TEMP.Count(); iiii++)
                    {
                        if (addedindexes.Contains(iiii.ToString()))
                        {
                            friend_name_values.Add(friend_name_values_TEMP[iiii]);
                        }
                    }

                }
                else
                {
                    end_of_friends = true;
                    break;
                }
            }

            

            for (int i = 0; i < ids_values.Count(); i++ )
            {
                comboBox2.Items.Add("[" + ids_values[i] + "] " + friend_name_values[i]);
            }
        }

        public List<string> getFriendNames(string page)
        {
            List<string> list_of_names = new List<string>();

            string search_names = @"from_pad: ''})"">";
            string search_names_end = @"</a>";

            List<int> name_starts = page.AllIndexesOf(search_names);
            List<int> name_ends = page.AllIndexesOf(search_names_end);

            if (name_ends.Count == 0 || name_starts.Count == 0 || name_ends.Count != name_starts.Count)
                return new List<string>();

            for (int i = 0; i< name_starts.Count; i ++)
            {
                list_of_names.Add(WebUtility.HtmlDecode(page.Substring(name_starts[i] + search_names.Length, name_ends[i] - name_starts[i] - search_names.Length)));

            }

            return list_of_names;
        }

        public Album FindSongs(String search_phrase)
        {
            Album search_result = new Album();

            int id = UserInfo.UserID;
            string fullUrl = "http://vk.com/audio?act=search&al=1&autocomplete=1&gid=0&offset=0&performer=0&q="+ HttpUtility.UrlEncode(search_phrase) + @"&sort=0&id=" + id;
            string page = VKUtils.PostRequest(fullUrl, cook);

            if (page.Contains("searchCount\":0")) { return search_result; }

            //Clipboard.SetText(page);

            int cut_start = page.IndexOf("<div");
            int cut_end = page.IndexOf("<!><!json>");
            page = page.Substring(cut_start, cut_end - cut_start);


            //Clipboard.SetText(page);

            //
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(page);

            HtmlNodeCollection songs_node = htmlDoc.DocumentNode.SelectNodes("//div[@class='area clear_fix']");

            //int i = 0;
            //foreach (HtmlNode node_of_song in songs_node)
            for (int i = 0; i < songs_node.Count; i++ )
            {
                HtmlNode node_of_song = songs_node[i];
                HtmlNode one_node = node_of_song.SelectSingleNode(".//input[@type='hidden']");
                //if (one_node.Attributes.Contains("id"))
                //{
                string song_id = one_node.Attributes["id"].Value.ToString(); // audio_info17492045_97998409
                string song_value_oid = song_id.Split('_')[1].Substring(4);
                string song_value_aid = song_id.Split('_')[2];
                //}
                //if (one_node.Attributes.Contains("value"))
                //{
                string song_value = one_node.Attributes["value"].Value.ToString(); // http://cs9-4v4.vk.me/p3/e1fa57591bcef6.mp3,217
                string song_value_link = song_value.Split(',')[0];
                //}

                HtmlNode two_node = node_of_song.SelectSingleNode(".//div[@class='title_wrap fl_l']");
                HtmlNode to_remove = two_node.SelectSingleNode(".//span[@class='user']");
                if (to_remove != null) { two_node.RemoveChild(to_remove, false); }

                string song_title_and_name = HttpUtility.HtmlDecode(two_node.InnerText);

                Song scrapped_song = new Song();
                scrapped_song.aid = song_value_aid;
                scrapped_song.oid = song_value_oid;
                scrapped_song.DownloadURL = song_value_link;
                scrapped_song.FileNameForSave = song_title_and_name;

                search_result.Songs.Add(scrapped_song);
                //MessageBox.Show(song_value_oid + song_value_aid + song_value_link + song_title_and_name);

                if (i == 60) { break; }
            }

            //MessageBox.Show(songs_node.Count.ToString());

            return search_result;
            

            //search_result = DataParser.ParseSearch(page);
        }

        private void getfriends()
        {
            int id = UserInfo.UserID;
            //string url = "http://vk.com/al_friends.php?__query=friends&al=-1&al_id=" + id;
            string url = "https://api.vk.com/method/friends.get?user_id=" + id +"&fields=first_name,last_name";
            string page = VKUtils.PostRequest(url, cook);
            page = page.Remove(page.Length - 1); // remove last
            page = page.Remove(0, 1); // remove first

            var pattern = @"\{(.*?)\}";
            var matches = Regex.Matches(page, pattern);

            Clipboard.SetText(matches[0].ToString());
            //MessageBox.Show(page);

        }

        private void PopulateAlbumExtra(String url)
        {
            currect_album = null;

            //int id = UserInfo.UserID;
            string fullUrl = url;// + id;
            string page = VKUtils.PostRequest(fullUrl, cook);

            //getHashForAdd(page);

            int index = page.IndexOf("<!>{\"all");
            string page2 = page.Substring(index + 3);
            albums = DataParser.ParseAlbumWithSongs(page2);
        }

        public string add_hash;
        public string reorder_hash;
        public string delete_hash;

        /*private void getHashForAdd(string page)
        {
            int index_start = page.IndexOf("addShareAudio");
            string subpage = page.Substring(index_start + 14, 100);
            int index_end = subpage.IndexOf(")");
            subpage = subpage.Substring(0, index_end);

            subpage = subpage.Split(',')[3];
            subpage = subpage.Trim();
            hash = TextParseTools.RemoveStartEndChars(subpage);
        }*/

        private bool CheckAuthorization()
        {
            if (!authorized)
                MessageBox.Show("Please Log In!");

            return authorized;
        }

        public void CleanBrokenLinks(List<Album> AlbToClean)
        {
            BrokenSongs = new List<Song>();

            foreach (Album MyAlb in AlbToClean)
            {
                if (MyAlb.Name == "My music")
                {
                    List<Song> CleansedSongs = new List<Song>();

                    for (int i = 0; i < MyAlb.Songs.Count; i++)
                    {
                        if (MyAlb.Songs[i].DownloadURL == "''")
                        {
                            MyAlb.Songs[i].DownloadURL = i.ToString();
                            BrokenSongs.Add(MyAlb.Songs[i]);
                        }
                        else
                        {
                            CleansedSongs.Add(MyAlb.Songs[i]);
                        }
                    }
                    MyAlb.Songs.Clear();
                    MyAlb.Songs = CleansedSongs;
                    CleansedSongs = null;
                    break;
                }
            }

        }

        Dictionary<int, string> friends;
        int mymus;

        private bool RefreshAlbumList(out string error)
        {
            error = "";
            try
            {
                if (!authorized)
                {
                    error = "You are not logged in";
                    return false;
                }

                //PopulateAlbum("https://vk.com/audio?act=load_audios_silent&al=1&please_dont_ddos=2&gid=0&id=");
                PopulateAlbum("https://vk.com/al_audio.php?__query=audios$PROFILE$&_ref=left_nav&al=-1&al_id=$PROFILE$");
                

                //CleanBrokenLinks(albums);

                comboBox2.Items.Clear();

                mymus = 0;
                int i = 0;
                foreach (var alb in albums)
                {
                    if (alb.Name == "My music")
                    {
                        mymus = i;
                    }

                    comboBox2.Items.Add(alb.Name);
                    i++;
                }
                //comboBox2.Items.Add("Suggestions");
                //comboBox2.Items.Add("Popular");

                /*foreach (KeyValuePair<int, string> pair in friends)
                {
                    comboBox2.Items.Add(pair.Value);
                }*/

                Endoflist = comboBox2.Items.Count;

                /*if (comboBox2.Items.Count > 0)
                {
                    //comboBox2.SelectedIndex = albums.Count - 1; // main album
                    comboBox2.SelectedIndex = mymus;
                }*/

                PopulateFriendList();

                return true;
            }
            catch (Exception exc)
            {
                error = exc.Message;
                return false;
            }
        }

        public bool CommandLogin(out string error)
        {
            error = "";

            if (authorized) { return true; }

            try
            {
                UserInfo = VKUtils.LoginToVkontakte(settings["login_name"].ToString(), settings["login_pas"].ToString());

                //проверяем,получили ли мы нужную переменную в куки
                if (String.IsNullOrEmpty(UserInfo.SID))
                {
                    authorized = false;
                    error = "Possibly wrong Login/Password.";
                    return false;
                }
                else
                {
                    authorized = true;
                    cook = new Cookie("remixsid", UserInfo.SID); //remixsid
                    cook.Domain = "vk.com";

                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public void CommandMyMysic(bool skipit = false)
        {
            int index = comboBox2.SelectedIndex;
            PopulateAlbum("http://vk.com/audio?act=load_audios_silent&al=1&please_dont_ddos=2&gid=0&id=");

            //CleanBrokenLinks(albums);
            /*
            if (albums.Count + 2 != comboBox2.Items.Count)
            {
                string error;
                bool result2 = RefreshAlbumList(out error);
                if (!result2) { MessageBox.Show(error, "Can't refresh album list"); }
                return;
            }*/

            currect_album = albums[index];
            currect_album.Name = "main";
            CommandUpdatePlaylist();
        }

        public void CommandSuggestions()
        {
            PopulateAlbumExtra("http://vk.com/audio?act=get_recommendations&al=1&offset=0&remix=0&id=" + UserInfo.UserID);
            currect_album = albums[0];
            currect_album.Name = "sug";
            CommandUpdatePlaylist();
        }

        public void CommandPopular()
        {
            PopulateAlbumExtra("http://vk.com/audio?act=get_popular&al=1&offset=0&remix=0&id=" + UserInfo.UserID);
            currect_album = albums[0];
            currect_album.Name = "pop";
            CommandUpdatePlaylist();
        }

        public void CommandFriend(int index)
        {
            int id = UserInfo.UserID;
            PopulateAlbumExtra("http://vk.com/audio?act=load_audios_silent&al=1&please_dont_ddos=3&id=" + ids_values[index]);

            foreach(Album albb in albums)
            {
                if (albb.ID == 0)
                {
                    currect_album = albb;
                    break;
                }
            }

            //currect_album = albums[0];
            currect_album.Name = "friend";
            CommandUpdatePlaylist();
        }

        public void CommandExtendPopular()
        {
            List<Album> albumsExtended = new List<Album>();

            int scroller = currect_album.Songs.Count;  // +1

            string url = "http://vk.com/audio?act=get_popular&al=1&offset=" + scroller + "&remix=0&id=";
            int id = UserInfo.UserID;
            string fullUrl = url + id;
            string page = VKUtils.PostRequest(fullUrl, cook);

            //getHashForAdd(page);

            int index = page.IndexOf("<!>{\"all");
            string page2 = page.Substring(index + 3);
            albumsExtended = DataParser.ParseAlbumWithSongs(page2);

            foreach (Song song in albumsExtended[0].Songs)
            {
                currect_album.Songs.Add(song);
            }

            CommandUpdatePlaylistAdd(albumsExtended[0]);
        }

        public void CommandExtendSuggestions()
        {
            List<Album> albumsExtended = new List<Album>();

            int scroller = currect_album.Songs.Count; // +1

            string url = "http://vk.com/audio?act=get_recommendations&al=1&offset=" + scroller + "&remix=0&recommendation=1&id=";
            int id = UserInfo.UserID;
            string fullUrl = url + id;
            string page = VKUtils.PostRequest(fullUrl, cook);

            //getHashForAdd(page);

            int index = page.IndexOf("<!>{\"all");
            string page2 = page.Substring(index + 3);
            albumsExtended = DataParser.ParseAlbumWithSongs(page2);

            foreach (Song song in albumsExtended[0].Songs)
            {
                currect_album.Songs.Add(song);
            }

            CommandUpdatePlaylistAdd(albumsExtended[0]);
        }

        public void CommandAdd()
        {
            string aid = currect_album.Songs[current_playing_id].aid;
            string oid = currect_album.Songs[current_playing_id].oid;

            string adder = "";
            if (currect_album.Name == "sug") { adder = "&recommendation=1"; }

            string url = ("http://vk.com/audio?act=add&aid=" + aid + "&al=1&oid=" + oid + "&top=0&hash=" + add_hash + adder);

            string page = VKUtils.PostRequest(url, cook);
        }

        public void CommandUpdatePlaylist()
        {
            if ((currect_album != null) && (currect_album.Songs != null))
            {
                pl = axWindowsMediaPlayer1.playlistCollection.newPlaylist("playlist");

                int i = 0;
                foreach (Song eachsong in currect_album.Songs)
                {
                    dataGridView1.Rows.Add(i, null, currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
                    temp = axWindowsMediaPlayer1.newMedia(eachsong.DownloadURL);

                    //.getItemInfo("s_id").ToString())
                    pl.appendItem(temp);
                    pl.Item[i].setItemInfo("s_id", i.ToString());
                    i++;
                }

            }

            current_playing_id = 0;
            axWindowsMediaPlayer1.currentPlaylist = pl;
        }

        public void CommandUpdatePlaylistAdd(Album album_extended)
        {
            if ((album_extended != null) && (album_extended.Songs != null))
            {
                int i = current_playing_id + 1;
                foreach (Song eachsong in album_extended.Songs)
                {
                    dataGridView1.Rows.Add(i, null, eachsong.Author + " - " + eachsong.Name);
                    temp = axWindowsMediaPlayer1.newMedia(eachsong.DownloadURL);

                    pl.appendItem(temp);
                    pl.Item[i].setItemInfo("s_id", i.ToString());
                    i++;
                }

            }

            //current_playing_id = 0;
            //axWindowsMediaPlayer1.currentPlaylist = pl;
        }

        public string hk_current_song_name()
        {
            if (currect_album == null) { return null; }
            if (currect_album.Songs.Count == 0) { return null; }
            return currect_album.Songs[current_playing_id].Author + " - " + currect_album.Songs[current_playing_id].Name;
        }

        public void hk_MyMysic()
        {
            if (comboBox2.Items.Count > 2)
            {
                comboBox2.SelectedIndex = Endoflist - 3;
            }
        }

        public void hk_Suggestions()
        {
            if (comboBox2.Items.Count > 2)
            {
                comboBox2.SelectedIndex = Endoflist - 2;
            }
        }

        public void hk_Popular()
        {
            if (comboBox2.Items.Count > 2)
            {
                comboBox2.SelectedIndex = Endoflist - 1;
            }
        }
        public void PlayIfpaused()
        {
            /*
             if (axWindowsMediaPlayer1.status.Equals("Paused"))
             {
                 

             }
            */
             if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
             {
                 axWindowsMediaPlayer1.Ctlcontrols.play();
             }
        }

        public void hk_Next()
        {
            if (currect_album.Songs.Count > 0)
            {
                PlayIfpaused();

                axWindowsMediaPlayer1.Ctlcontrols.next();
            }
        }

        public void hk_Prev()
        {
            if (currect_album.Songs.Count > 0)
            {
                PlayIfpaused();

                axWindowsMediaPlayer1.Ctlcontrols.previous();
            }
        }

        public void hk_Play_Pause()
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            else if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }

        public void hk_Repeat()
        {
            if (!repeat)
            {
                repeat = true;
                button3.Text = "On";
            }
            else
            {
                repeat = false;
                button3.Text = "Off";
            }
        }

        /*public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }*/

        public void Shuffle(List<Song> list)
        {
            //RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            //int n = list.Count;

            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                Song temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }

            /*while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Song value = list[k];
                list[k] = list[n];
                list[n] = value;
            }*/

            //return list;
        }

        public void hk_Shuffle()
        {
            if (currect_album.Songs.Count > 2)
            {
                List<Song> newsonglist = new List<Song>();

                Shuffle(currect_album.Songs);

                dataGridView1.Rows.Clear();
                CommandUpdatePlaylist();

                //MessageBox.Show(newsonglist[0].Name);
            }

            /*if (!shuffle)
            {
                shuffle = true;
                button10.Text = "On";
            }
            else
            {
                shuffle = false;
                button10.Text = "Off";
            }*/
        }
        public void hk_Download()
        {
            if (current_song != null)
            {
                string download_name = currect_album.Songs[current_playing_id].FileNameForSave;
                string newpath = Path.Combine(@"downloads\", download_name + ".mp3");

                if (!Directory.Exists(@"downloads\")) { Directory.CreateDirectory(@"downloads\"); }
                //if (File.Exists(newpath)) { result = "Error! File with that name already exists"; return; }

                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(current_song.sourceURL, newpath);
                    }
                    //result = "Saved: " + download_name;
                }
                catch
                {
                    MessageBox.Show("Erro");
                    //result = "Error! Try again later.";
                }
            }
        }

        public void hk_Add()
        {
            if (current_song != null)
            {
                if (currect_album.Name == "sug" || currect_album.Name == "pop" || currect_album.Name == "friend")
                {
                    CommandAdd();
                }
            }
        }

        public void hk_First()
        {
            if (currect_album.Songs.Count > 0)
            {
                axWindowsMediaPlayer1.Ctlcontrols.playItem(pl.get_Item(0));
            }
        }

        public List<string> GetMusicListSer()
        {
            if (currect_album.Songs.Count == 0)
            {
                return null;
            }

            List<string> ret = new List<string>();

            for (int i = 0; i < currect_album.Songs.Count; i++)
            {
                ret.Add(currect_album.Songs[i].Author + " - " + currect_album.Songs[i].Name);
            }

            return ret;
        }
    }
    public static class MyExtensions
    {
        /*public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }*/

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }   

}
