using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;

public class Starter : MonoBehaviour {
    List<Post> posts;
    VideoPlayer vid;
    RenderTexture actualRenderTexture;
    public RawImage videoRenderer;
    public Text videoTitle;
    public RectTransform videoBack;
    public Animator anim;
    public Animator music;
    public UnityEngine.UI.Image progessBar;
    public Animator intro;
    public Animator outro;
    public float minutes = 10f;
    System.DateTime startTime;
    
    //public System.Action StopRecording;

    void Start () {

        SharpGag sharpGag = new SharpGag();
        sharpGag.Login("topfunnyvidsan","pt181179");
        posts = new List<Post>(sharpGag.GetPosts("funny", "hot", 300));

        PostComparer comparer = new PostComparer();
        posts.Sort(comparer);

        foreach(Post p in posts)
        {
            Debug.Log(p.totalVoteCount + "  " + p.title + ": " + p.images.image460sv.url);
        }

        vid = GetComponent<VideoPlayer>();
        vid.EnableAudioTrack(0, true);
        vid.loopPointReached += OnVideoFinish;
        vid.prepareCompleted += OnVideoPrepaded;
        startTime = System.DateTime.Now;
        
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(0.5f);
        music.GetComponent<AudioSource>().Play();
        intro.SetTrigger("Start");
        yield return new WaitForSeconds(3.5f);
        PlayNextVideo();
    }

    void PlayNextVideo()
    {
        vid.url = posts[0].images.image460sv.url;
        actualRenderTexture = new RenderTexture(posts[0].images.image460sv.width, posts[0].images.image460sv.height,16);
        vid.targetTexture = actualRenderTexture;
        videoRenderer.texture = actualRenderTexture;
        videoRenderer.rectTransform.sizeDelta = new Vector2(actualRenderTexture.width * 775 / actualRenderTexture.height, 775);
        videoBack.sizeDelta = new Vector2(videoRenderer.rectTransform.sizeDelta.x + 40, videoRenderer.rectTransform.sizeDelta.y + 40);
        videoTitle.text = System.Net.WebUtility.HtmlDecode(posts[0].title).ToUpper();
        posts.RemoveAt(0);
        vid.Prepare();
    }

    void OnVideoPrepaded(VideoPlayer vp)
    {
        vid.Play();
        vid.time = 0.2;
        vid.Pause();
        anim.SetTrigger("In");
        StartCoroutine(PlaySequence());
        Debug.Log(vid.GetAudioChannelCount(0));
        music.SetBool("on", vid.GetAudioChannelCount(0).ToString() == "0");
    }

    IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(1.7f);
        vid.Play();
    }

    void OnVideoFinish(VideoPlayer vp)
    {
        anim.SetTrigger("Out");
        StartCoroutine(FinishSequence());
    }

    IEnumerator FinishSequence()
    {
        music.SetBool("on", true);
        yield return new WaitForSeconds(0.667f);
        if (posts.Count > 0 && GetTimePassed() < minutes)
        {
            PlayNextVideo();
        }
        else
        {
            StartCoroutine(OutroSequence());
        }
    }

    IEnumerator OutroSequence()
    {
        outro.SetTrigger("Play");
        yield return new WaitForSeconds(12f);

        EditorApplication.isPlaying = false;
    }

    private void Update()
    {
        progessBar.fillAmount = (float)vid.frame / (float)vid.frameCount;
    }

    double GetTimePassed()
    {
       return (startTime - System.DateTime.Now).TotalMinutes;
    }
}

class PostComparer : IComparer<Post>
{
    public int Compare(Post a, Post b)
    {
        return b.totalVoteCount.CompareTo(a.totalVoteCount);
    }
}