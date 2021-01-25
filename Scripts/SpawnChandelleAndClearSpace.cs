// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
//  Genere les cercle de chandelle a l apparition et fait de l espace pour le boss.
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnChandelleAndClearSpace : MonoBehaviour
{
    // Start is called before the first frame update
    CapsuleCollider colliders;
    
    public float radius;

    public int nombreChandelle;

    public GameObject Chandelle;

    float increment;

    float degreerad=0f;

    Vector3 positionChandelle;

    bool flipex =true;

    public Slider barreVie;

    void Start()
    {
        increment= Mathf.PI*2/nombreChandelle;
        

        colliders =GetComponent<CapsuleCollider>();

        colliders.radius=radius;
        radius=radius -1f;
        spawnCircle();
        //Invoke("turnOff",0.2f);
    }
 // faire apparaitre les chandelle en cercle
    void spawnCircle()
    {
        for (int i = 0; i < nombreChandelle; i++)
        {          
            positionChandelle=new Vector3(radius*Mathf.Sin(degreerad),transform.position.y,radius*Mathf.Cos(degreerad));
            degreerad+=increment;
            GameObject cloneChandel = Instantiate(Chandelle, transform.position+positionChandelle, Chandelle.transform.rotation);
            cloneChandel.SetActive(true);
            cloneChandel.GetComponent<SpriteRenderer>().flipX=flipex;
            flipex=!flipex;
            cloneChandel.transform.parent = gameObject.transform; // rend les chandelle enfant de la zone
        }  
    }
    // detruit les arbres
    private void OnTriggerEnter(Collider autreObjet) 
    {
        if(autreObjet.gameObject.tag == "obstacle" )
        {
           Destroy(autreObjet.gameObject);
           
        }
    }

    public void turnOff()
    {
        gameObject.SetActive(false);
    }
}
