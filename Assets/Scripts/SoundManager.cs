using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioClip attack;
    [SerializeField] private AudioClip boom;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip hit;

    private new AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);   
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AttackSoundPlay()
    {
        audio.clip = attack;
        audio.Play();
    }

    public void BoomSoundPlay()
    {
        audio.clip = boom;
        audio.Play();
    }

    public void DieSoundPlay()
    {
        audio.clip = die;
        audio.Play();
    }

    public void HitSoundPlay()
    {
        audio.clip = die;
        audio.Play();
    }
}
