using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform placeSfxPrefab;

    [SerializeField] private Transform flipSfxPrefab;

    [SerializeField] private Transform passSfxPrefab;

    public void PlayPlaceSfx()
    {
        Transform sfxTransform = Instantiate(placeSfxPrefab);
        Destroy(sfxTransform.gameObject, 1f);
    }
    public void PlayFlipSfx()
    {
        Transform sfxTransform = Instantiate(flipSfxPrefab);
        Destroy(sfxTransform.gameObject, 1f);
    }
    public void PlayTwitchSfx()
    {
        Transform sfxTransform = Instantiate(placeSfxPrefab);
        Destroy(sfxTransform.gameObject, 1f);
    }

    public void PlayPassSfx()
    {
        Transform sfxTransform = Instantiate(passSfxPrefab);
        Destroy(sfxTransform.gameObject, 1f);
    }
}
