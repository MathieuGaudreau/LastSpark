// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:Scipt pour faire de l espace avec un collider
// 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clearSpace : MonoBehaviour
{
    CapsuleCollider colliders;
    
    public float radius;
    
    // Start is called before the first frame update
    void Start()
    {   
        colliders =GetComponent<CapsuleCollider>();
        colliders.radius=radius;
        Invoke("turnOff",2f);
    }

    //gere les collisions avec les objetss
    private void OnTriggerEnter(Collider autreObjet) 
    {
        if(autreObjet.gameObject.tag == "obstacle" )
        {
           Destroy(autreObjet.gameObject);
        }
        if(autreObjet.gameObject.tag == "zoneRuine" )
        {
           Destroy(autreObjet.gameObject);
        }
           
        if(autreObjet.gameObject.tag == "enemieBouche" )
        {
           Destroy(autreObjet.gameObject);
        } 

        if(autreObjet.gameObject.tag == "miniboss1" )
        {
           Destroy(autreObjet.gameObject);
        } 

        if(autreObjet.gameObject.name =="SpawnBossCircle(lazone)(Clone)")
        {
            Destroy(autreObjet.gameObject);
        }
    }

    void turnOff()
    {
        gameObject.SetActive(false);
    }
}
