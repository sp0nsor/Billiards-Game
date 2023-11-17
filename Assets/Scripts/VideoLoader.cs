using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
public class VideoLoader : MonoBehaviour
{
    const String VIDEO_PATH = "Assets/Textures/Other Sprites/IMG_6255.mp4";
    private void Awake()
    {
        LoadVideo();
    }

    async void LoadVideo()
    {
        var videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = VIDEO_PATH;

        //await new WaitUntil(() => videoPlayer.isPrepared);

        while (!videoPlayer.isPrepared)
        {
            return;
        }
        videoPlayer.Play();
    }
}
