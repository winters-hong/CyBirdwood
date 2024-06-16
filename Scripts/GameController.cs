using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public ParticleSystem feather;
    public ClockControl clock;
    public GameObject[] player_objs;
    public GameObject local_player;
    public bool isDay;

    public Button exitBtn;
    public Button playBtn;

    private AudioSource audioData;
    public AudioClip[] audios_calm;
    public AudioClip[] audios_hot;
    private AudioClip[] audios_playing;
    public int audio_i;

    public GameObject bubble;
    public GameObject feather_obj;
    public AnimationClip[] colorClips;
    public AnimationClip[] featherClips;
    public Animation anim_bubble;
    public Animation anim_feather;
    public int anim_i;

    void Start()
    {
        audio_i = anim_i = 0;
        isDay = false;
        audios_playing = audios_calm;

        anim_bubble = bubble.GetComponent<Animation>();
        anim_feather = feather_obj.GetComponent<Animation>();

        audioData = GetComponent<AudioSource>();
        clock = GetComponent<ClockControl>();
    }

    void Update() { }

    public void HitBubble()
    {
        feather.Play();
        PlayAudio();
        StartCoroutine(
            DelayToInvoke.DelayToInvokeDo(
                () =>
                {
                    feather.Stop();
                },
                3f
            )
        );
    }

    public void PlayAudio()
    {
        audioData.clip = audios_playing[audio_i++];
        audioData.Play();

        if (audio_i >= audios_playing.Length)
            audio_i = 0;
    }

    public void ChangeMode()
    {
        player_objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in player_objs)
        {
            if (p.GetComponent<Player>().isLocalPlayer)
                local_player = p;
        }
        local_player.GetComponent<Player>().CmdChangeMode();
    }

    public void ChangeMode_Controller()
    {
        // Access the color parameter of Bubble shader
        anim_bubble.clip = colorClips[anim_i];
        anim_bubble.Play();

        anim_feather.clip = featherClips[anim_i++];
        anim_feather.Play();

        // 'isDay == true' means now is warm/hot scene
        clock.DayNightSwap(isDay ? 8.5f : 3.5f);
        audios_playing = isDay ? audios_calm : audios_hot; // 'Now is Day' means next should be night/cold
        isDay = !isDay;

        Debug.Log("Mode changed.");

        if (anim_i >= colorClips.Length || anim_i >= featherClips.Length)
            anim_i = 0;
    }

    public void AppQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AppResume()
    {
        SceneManager.LoadScene(0);
    }
}
