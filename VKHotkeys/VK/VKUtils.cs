using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;

namespace VKHotkeys.VK
{
  public class VKUserInfo
  {
    public String SID { get; set; }
    public int UserID { get; set; }
  }
    public static class VKUtils
  {
        public static CookieContainer defCol = new CookieContainer();

        public static string[] Get_ip_h_Zero()
        {

            var postRequest = (HttpWebRequest)WebRequest.Create("http://vk.com/login?act=mobile");

            postRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
            //Запрещаем редирект
            postRequest.AllowAutoRedirect = true;
            //Выставляем таймаут
            postRequest.Timeout = 10000;
            postRequest.CookieContainer = defCol;
            postRequest.Method = "GET";
            postRequest.ContentLength = 0;

            //postRequest.Headers.Add("Accept-Encoding", "gzip,deflate"); 

            //var stream = postRequest.GetRequestStream();
            //stRequest.ContentLength =  stream.Length;

            //получаем весь ответ
            var myHttpWebResponse = (HttpWebResponse)postRequest.GetResponse();

            var stream = myHttpWebResponse.GetResponseStream();

            var reader = new StreamReader(stream);
            var page = reader.ReadToEnd();

            Log("page: " + page);

            var match1 = Regex.Match(page, "ip_h: *'(?<ip_h>.*?)'");
            //var match2 = Regex.Match(page, "lg_h: *'(?<lg_h>.*?)'");

            var match2 = Regex.Match(page, "input(.*)name=\"lg_h\"(.*)value=\"(?<lg_h>.*?)\"");

            if (!match1.Success)// || !match2.Success)
            {
                return null;//ошибка возвращаем пустое    
            }
            else
            {
                string ip_h = match1.Groups["ip_h"].Value;
                string lg_h = match2.Groups["lg_h"].Value;
                return new string[] { ip_h, lg_h };
            }
        }


        public static string[] Get_ip_h()
    {
      var wrGETURL = (HttpWebRequest)WebRequest.Create("http://vk.com/login?act=mobile");
      HttpWebResponse myHttpWebResponse = (HttpWebResponse)wrGETURL.GetResponse();
      StreamReader pageStreamReadermy = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.GetEncoding(1251));

      string page = pageStreamReadermy.ReadToEnd();

      Log("page: " + page);

      var match1 = Regex.Match(page, "ip_h: *'(?<ip_h>.*?)'");
            //var match2 = Regex.Match(page, "lg_h: *'(?<lg_h>.*?)'");

            var match2 = Regex.Match(page, "input(.*)name=\"lg_h\"(.*)value=\"(?<lg_h>.*?)\"");


            if (!match1.Success)// || !match2.Success)
      {
        return null;//ошибка возвращаем пустое    
      }
      else
      {
        string ip_h = match1.Groups["ip_h"].Value;
        string lg_h = match2.Groups["lg_h"].Value;
        return new string[] { ip_h , lg_h };
      }

    }

    public static string Get_ip_h2()
    {
        var wrGETURL = (HttpWebRequest)WebRequest.Create("https://vk.com/audio");
        HttpWebResponse myHttpWebResponse = (HttpWebResponse)wrGETURL.GetResponse();
        StreamReader pageStreamReadermy = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.GetEncoding(1251));

        string page = pageStreamReadermy.ReadToEnd();

        Log("page: " + page);

        var match = Regex.Match(page, "ip_h: *'(?<ip_h>.*?)'");


        if (!match.Success)
        {
            return String.Empty;//ошибка возвращаем пустое    
        }
        else
        {
            string ip_h = match.Groups["ip_h"].Value;
            return ip_h;
        }

    }

    public static void Log(string logMessage)
    {
        using (StreamWriter w = File.AppendText("log.txt"))
        {
        w.Write("\r\nLog Entry : ");
        w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
            DateTime.Now.ToLongDateString());
        w.WriteLine("  ", "");
        w.WriteLine("  {0}", logMessage);
        w.WriteLine("-------------------------------");

        }
    }

    /// <summary> Авторизация к Vkontakte.ru с получением id и SID пользователя </summary>    
    public static VKUserInfo LoginToVkontakte(String email, String password)
    {
        //Log("=======================================================");
        //Log("================== Starting logging ===================");

      //Получаем  поле  ip_h
        string[] all_h = Get_ip_h_Zero();
        string ip_h = all_h[0];
        string lg_h = all_h[1];

            //Log("ip_h: '" + ip_h + "'");

            //создаем запрос
            var values = new Dictionary<string, string>
                     {
                       {"act", "login"},
                       {"role","al_frame"},
                       {"expire", ""},

                       {"captcha_sid", ""},
                       {"captcha_key", ""},

                       //{"from_protocol", "http"},

                       {"ip_h", ip_h},//ip_h
                       {"lg_h", lg_h},//ip_h

                       {"email", email},
                       {"pass", password},

                     };

      string link = (  String.Join("", values.Select(z => "&" + z.Key + "=" + z.Value).ToArray()));//HttpUtility.UrlEncode
      string fullUrl = @"https://login.vk.com/?" + link;

      //CookieContainer collectiondd22 = new CookieContainer();

      var postRequest = (HttpWebRequest) WebRequest.Create(fullUrl);
            
      postRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:15.0) Gecko/20100101 Firefox/15.0.1";
      //Запрещаем редирект
      postRequest.AllowAutoRedirect = true;
      //Выставляем таймаут
      postRequest.Timeout = 10000;
      postRequest.CookieContainer = defCol;
      postRequest.Method = "GET";
      postRequest.ContentLength = 0;

      //postRequest.Headers.Add("Accept-Encoding", "gzip,deflate"); 

      //var stream = postRequest.GetRequestStream();
      //stRequest.ContentLength =  stream.Length;
     
      //получаем весь ответ
      var myHttpWebResponse = (HttpWebResponse)postRequest.GetResponse();

      var stream = myHttpWebResponse.GetResponseStream();

      var reader = new StreamReader(stream);
      var html = reader.ReadToEnd();

      //string id_from_cookie = collectiondd22.GetCookies(new Uri(fullUrl))[2].Value;

            //string ssid_from_cookie = collectiondd22.GetCookies(new Uri(fullUrl))[5].Value;

            /*

          string gggg1 = collectiondd22.GetCookies(new Uri(fullUrl))[0].Value;
          string gggg2 = collectiondd22.GetCookies(new Uri(fullUrl))[1].Value;
          string gggg3 = collectiondd22.GetCookies(new Uri(fullUrl))[2].Value;
          string gggg4 = collectiondd22.GetCookies(new Uri(fullUrl))[3].Value;
          string gggg5 = collectiondd22.GetCookies(new Uri(fullUrl))[4].Value;
          string gggg6 = collectiondd22.GetCookies(new Uri(fullUrl))[5].Value;
          //string gggg7 = collectiondd22.GetCookies(new Uri(fullUrl))[6].Value;

          string final = gggg1 + gggg2 + gggg3 + gggg4 + gggg5 + gggg6;// +gggg7;
          //Log("id_from_cookie: '" + id_from_cookie.ToString() + "'");
          //Log("ssid_from_cookie: '" + ssid_from_cookie.ToString() + "'");

            */

            string id_from_cookie = defCol.GetCookies(new Uri(fullUrl))["l"].Value;

            string gggg6 = defCol.GetCookies(new Uri(fullUrl))["remixsid"].Value;
           // string gggg6 = defCol.GetCookies(new Uri(".login.vk.com"))["remixsid"].Value;

      VKUserInfo info = new VKUserInfo() { SID = gggg6, UserID = Convert.ToInt32(id_from_cookie) };    

      return info;
    }

    
    /// <summary> POST-запрос к Vkontakte </summary>    
    public static String PostRequest(String url, Cookie sidCookie)
    {
      //создаем запрос
      HttpWebRequest wrPOSTURL = (HttpWebRequest)System.Net.WebRequest.Create(url);
      wrPOSTURL.Method = "POST";
      //wrGETURL.Headers.Add(cook.ToString());
      wrPOSTURL.Headers["Cookie"] = sidCookie.ToString();
      wrPOSTURL.ContentLength = 0;

      HttpWebResponse myHttpWebResponse = (HttpWebResponse)wrPOSTURL.GetResponse();

      StreamReader myStreamReadermy = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.GetEncoding(1251));      
      string page = myStreamReadermy.ReadToEnd();

      return page;
    }

  }
}
