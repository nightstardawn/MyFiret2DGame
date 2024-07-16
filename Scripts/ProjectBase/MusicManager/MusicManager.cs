using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : SingleBaseManger<MusicManager>
{
    //唯一背景音乐组件
    private AudioSource bkMusic=null;
    //音效依附组件
    private GameObject soundObj=null;
    //存储音效列表
    private List<AudioSource> soundList=new List<AudioSource>();
    //背景音乐大小
    private float bkValue=1;
    //音效大小
    private float soundValue=1;

    public MusicManager()
    {
        MonoManager.Instance.AddUpdateListener(myUpdate);
    }

    private void myUpdate()
    {
        for (int i=soundList.Count-1; i>=0;i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="path">音乐路径Music/BK/</param>
    public void PlayBKMusic(string path)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject("BkMusicObj");
            bkMusic = obj.AddComponent<AudioSource>();
        }
        //异步加载背景音乐 并播放
        ResourcesManager.Instance.LoadAsync<AudioClip>("Music/BK/" + path, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.volume = bkValue;
            bkMusic.loop = true;
            bkMusic.Play();
        });
    }
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }
    /// <summary>
    /// 改变背景音量大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeBKValue(float value)
    {
        bkValue = value;
        if (bkMusic != null)
            return;
        bkMusic.volume=bkValue;
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="path">音效路径Music/Sound/</param>
    /// <param name="isLoop">音效是否循环</param>
    /// <param name="callBack">回调函数</param>
    public void PlaySound(string path, bool isLoop=false,UnityAction<AudioSource> callBack=null)
    {
        if (soundObj == null)
        {
            soundObj= new GameObject("soundObj");
        }
        //异步加载音效 并播放
        ResourcesManager.Instance.LoadAsync<AudioClip>("Sound" + path, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = bkValue;
            source.Play();
            soundList.Add(source);

            if (callBack != null)
                callBack(source);
        });

    }
    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="source"></param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundVaule(float value)
    {
        soundValue= value;
        for (int i = 0; i < soundList.Count; i++)
            soundList[i].volume=soundValue;
    }

}
