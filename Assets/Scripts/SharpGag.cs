using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;

public class SharpGag {

    const string LANG = "en_US";
    const string APP_ID = "com.ninegag.android.app";
    const string COMMENT_CDN = "a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b";

    const string API = "http://api.9gag.com";
    const string API_USER_TOKEN = "/v2/user-token";
    const string POST_LIST = "/v2/post-list";

    string app_id;
    string token;
    string device_uuid;
    string olderThan;

    public SharpGag()
    {
        app_id = APP_ID;
        token = GetRandoSha1();
        device_uuid = Guid.NewGuid().ToString();
    }

    public string Login(string ua, string pw)
    {
        WebClient client = new WebClient();
        client.QueryString.Add("password",GetMd5(pw));
        client.QueryString.Add("pushToken", GetRandoSha1());
        client.QueryString.Add("loginMethod","9gag");
        client.QueryString.Add("loginName", ua);
        client.QueryString.Add("language",LANG);

        string result = MakeRequest(client, API, API_USER_TOKEN);
        Debug.Log(result);
        Dictionary<string,object> res = MiniJSON.Json.Deserialize(result) as Dictionary<string, object>;
        token = ((Dictionary<string, object>)res["data"])["userToken"] as string;
        return result;
    }

    public string GetPosts(string group = "1", string type = "hot", int count = 5, string entryTypes = "animated",string olderThan = null)
    {
        WebClient client = new WebClient();
        client.QueryString.Add("group",group);
        client.QueryString.Add("type",type);
        client.QueryString.Add("itemCounnt",count.ToString());
        client.QueryString.Add("entryTypes", "animated");

        if(string.IsNullOrEmpty(olderThan) == false)
        {
            client.QueryString.Add("olderThan",olderThan);
        }

        return MakeRequest(client, API, POST_LIST);
    }

    string MakeRequest(WebClient client, string APIurl, string APIpath)
    {
        WebHeaderCollection webHeaderCollection = new WebHeaderCollection
        {
            { "9GAG-9GAG_TOKEN", token },
            { "9GAG-TIMESTAMP", GetTimeStamp() },
            { "9GAG-APP_ID", app_id },
            { "X-Package-ID", app_id },
            { "9GAG-DEVICE_UUID", device_uuid },
            { "X-Device-UUID", device_uuid },
            { "9GAG-DEVICE_TYPE", "android" },
            { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
            { "9GAG-REQUEST-SIGNATURE", Sign() }
        };
        client.Headers = webHeaderCollection;
        //Debug.Log(webHeaderCollection.ToString());
        string url = APIurl + APIpath;
        return client.DownloadString(url);
    }

    string GetMd5(string input)
    {
        System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
        StringBuilder sBuilder = new StringBuilder();
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    string GetSha1(string input)
    {
        System.Security.Cryptography.SHA1 sha1Hash = System.Security.Cryptography.SHA1.Create();
        StringBuilder sBuilder = new StringBuilder();
        byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }

    string GetTimeStamp()
    {
        double time = Math.Round(TimeFromEpoch().TotalMilliseconds);
        return time.ToString();
    }

    string Sign()
    {
        string s1 = "*" + GetTimeStamp() + "_._" + app_id + "._." + device_uuid + "9GAG";
        return GetSha1(s1);
    }

    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    TimeSpan TimeFromEpoch()
    {
        return DateTime.Now - epoch;
    }

    string GetRandoSha1()
    {
        string s1 = GetTimeStamp();
        return GetSha1(s1);
        //Encoding ISO = Encoding.GetEncoding("ISO-8859-1");
        //return ISO.GetString(Encoding.Convert(Encoding.UTF8, ISO, Encoding.UTF8.GetBytes(s1)));
    }

    public static string[] postSection = new string[] {"Funny","Animals","WTF","Gaming","Awesome","Food & Drinks","Travel","GIF","Cosplay",
    "NFK - Not For Kids","Timely","Girl","Comic","Spooktober","Guy","Ask 9GAG","Anime & Manga","K-Pop","Sport ","School",
    "Dark Humor", "Countryballs","Horror","DIY & Crafts", "Sci-Tech","Politics","Relationship", "Savage", "Girly Things ",
    "Superhero", "Video","Overwatch","Car","Music","PC Master Race","Wallpaper", "History","Movie & TV","Surreal Memes",
    "Classical Art Memes", "Pic Of The Day","Home Design","Roast Me","Basketball", "Football", "PUBG","Fortnite", "Warhammer",
    "League of Legends","Pokémon","LEGO","Drawing & Illustration","Fan Art " };
    public static string[] postType = new string[] { "Photo", "Animated"};
}