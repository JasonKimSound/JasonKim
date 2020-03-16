using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; //곡이름
    public AudioClip clip; //곡
}


public class SoundManager : MonoBehaviour//모노비헤비어가 붙어야 객체에 컴포넌트를 추가할 수 있다.
{
    //싱글톤화 시켜야함! singleton
    
    
    static public SoundManager instance; // 선언만 해준거야 껍데기라궁
    #region singleton
    void Awake() //객체 생성시 최초 실행
    {
        if(instance == null)
        {
            instance = this; // 자기자신을 넣어준다ㅎㅎ
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;

    private int count = 0; //비지엠 교체 카운트

    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
        PlaySE("Bgm");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {

            count++;
            PlaySE(bgmSounds[count].name);
            if (count == 2)
                count = -1;
        }
    }


    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for (int j = 0; j < effectSounds.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name; 
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return; //메소드를 빠져나온다
                    }
                }
                Debug.Log("모든 가용 AudioSource(이펙트)가 사용중입니다.");
                
            }
        }

        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (_name == bgmSounds[i].name)
            {
                playSoundName[audioSourceEffects.Length-1] = bgmSounds[i].name;
                audioSourceBgm.clip = bgmSounds[i].clip;
                audioSourceBgm.Play();
                if (i == 0)
                {
                    audioSourceBgm.volume = 0.5f;
                }
                else if(i == 1)
                {
                    audioSourceBgm.volume = 1.0f;

                }else if(i == 2)
                {
                    audioSourceBgm.volume = 1.0f;

                }
                return; //메소드를 빠져나온다

            }
            Debug.Log("모든 가용 AudioSource(BGM)가 사용중입니다.");
        }

        Debug.Log(_name + "사운가 사운드매니저에 등록되지 않았습니다.");
        return;

    }



    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("재생중인" + _name + "사운드가 없습니다.");
    }
}
