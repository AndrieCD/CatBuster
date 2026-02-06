using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Sounds")]
    [SerializeField] private AudioSource angelDamage;
    [SerializeField] private AudioSource angelDefeat;
    [SerializeField] private AudioSource angelShoot;
    [SerializeField] private AudioSource angelWalkRun;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource catAttack;
    [SerializeField] private AudioSource catDefeat;
    [SerializeField] private AudioSource catHit;
    [SerializeField] private AudioSource gameBGM;
    [SerializeField] private AudioSource loseBGM;



    // --- MUSIC ---
    public void PlayGameBGM()
    {
        gameBGM.loop = true;
        gameBGM.Play();
    }

    public void StopGameBGM()
    {
        gameBGM.Stop();
    }

    public void PlayLoseBGM()
    {
        loseBGM.loop = true;
        loseBGM.Play();
    }

    public void StopLoseBGM()
    {
        loseBGM.Stop();
    }



    // --- ANGEL ---
    public void PlayAngelDamage()
    {
        angelDamage.Play();
    }

    public void PlayAngelDefeat()
    {
        angelDefeat.Play();
    }

    public void PlayAngelShoot()
    {
        angelShoot.Play();
    }

    public void PlayAngelWalkRun()
    {
        angelWalkRun.Play();
    }

    public void StopAngelWalkRun()
    {
        angelWalkRun.Stop();
    }



    // --- CAT ---
    public void PlayCatAttack()
    {
        catAttack.Play();
    }

    public void PlayCatDefeat()
    {
        catDefeat.Play();
    }

    public void PlayCatHit()
    {
        catHit.Play();
    }



    // --- UI ---
    public void PlayButtonClick()
    {
        buttonClick.Play();
    }

}
