// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
// Scipte sur la fireball pour la detruire au contact et delencher sont animation
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    // Start is called before the first frame update
    Animator m_Animator;

    public AudioClip explosion;
    void Start()
    {
        Invoke("destruction",3f);
         m_Animator = gameObject.GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision other) 
    {
         if(other.gameObject.name != "Spark"&&other.gameObject.name != "Terrain")
         {
            m_Animator.SetTrigger("toucher");
            //Debug.Log("toucher");
            //Invoke("destruction",0f);
            gameObject.GetComponent<Rigidbody>().velocity=Vector3.zero;
            gameObject.GetComponent<AudioSource>().PlayOneShot(explosion,1);
            Debug.Log(other.gameObject.name);
         }
    }

    void destruction()
    {
        Destroy(gameObject);
    }
}
