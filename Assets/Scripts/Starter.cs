using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SharpGag sharpGag = new SharpGag();
        sharpGag.Login("topfunnyvidsan","pt181179");
        string posts = sharpGag.GetPosts("1", "hot", 2);
        Dictionary<string, object> postDic = MiniJSON.Json.Deserialize(posts) as Dictionary<string, object>;
        Debug.Log(posts);
    }
    
    void Update () {
	}
}
