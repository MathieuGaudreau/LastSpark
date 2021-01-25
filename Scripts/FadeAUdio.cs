// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
//  Script qui donne un effet de fade audio
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FadeAUdio
{
    //creer un fade audio qui descend ou monte selon un certain temps
    public static IEnumerator StartFade(AudioSource audioSource, float tempsFade, float VolumeVoulu)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < tempsFade)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, VolumeVoulu, currentTime / tempsFade);
            yield return null;
        }
        yield break;
    }
}
