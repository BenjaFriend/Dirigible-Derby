using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip ProtoMusic;

    private PooledAudioSource _source;

    // Use this for initialization
    void Start()
    {
        _source = AudioManager.Instance.Play(ProtoMusic, AudioPoolType.Music, Constants.Mixer.MixerGroups.Master.BackgroundMusic.Name, true, () => { _source = null; });
    }

    //private void OnDestroy()
    //{
    //    _source.Stop();
    //}

    // Update is called once per frame
    void Update()
    {

    }
}
