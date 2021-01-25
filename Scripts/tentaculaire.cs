// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
// Animation des tentacule , déclencher le collider et l auto destruction.
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tentaculaire : MonoBehaviour
{
    AudioSource audioTentacule;

    CapsuleCollider petitCollider;

    public AudioClip sonAttaque;

    // Start is called before the first frame update
    void Start()
    {
       petitCollider =GetComponent<CapsuleCollider>();
       audioTentacule = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void activerCollider()
    {
        petitCollider.enabled = true;
    }
   
    void activerMort()
    {
        Destroy(gameObject);
    }

    //fait jouer le son des tentacules
    void sonTentqcules()
    {
        audioTentacule.PlayOneShot(sonAttaque , 0.5f);
    }
}
