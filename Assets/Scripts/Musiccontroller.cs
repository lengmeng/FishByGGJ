using UnityEngine;
using System.Collections;

public class Musiccontroller : MonoBehaviour
{

    public static Musiccontroller _instance;
    public AudioClip[] _clips;
    public int currMusicNum;
    void Awake()
    {
        _instance = this;
    }

    public void PlayMusic(int i)
    {
        currMusicNum = i;
        this.GetComponent<AudioSource>().clip = _clips[i];
        this.GetComponent<AudioSource>().Play();
    }
}