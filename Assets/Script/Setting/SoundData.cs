using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject
{
    [Range(0f, 1f)] public float bgmvolume = 0.5f;
    [Range(0f, 1f)] public float sfxvolume = 0.5f;
    public bool isBgmOn = true;
    public bool isSfxOn = true;

    [Header("BGM Clips")]
    public AudioClip[] bgmClips; // BGM Clips 배열 (4개)
   //public int currentBgmIndex = 0; // 현재 재생 중인 BGM 인덱스

    [Header("SFX Settings")]
    public AudioClip[] sfxClips; // SFX 클립 배열
}
