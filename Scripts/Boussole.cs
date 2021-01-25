// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
//  Script qui permet à la flèche de la boussole de pointer vers le feu de camp
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boussole : MonoBehaviour
{   
    //feu de camp dans la scène
    public GameObject campdeFeux;

    //personnage
    public GameObject sparky;

    //position du personnage
    public Vector3 posSpark;

    //position du feu de camps
    public Vector3 posCampdeFeux;

    //flèche de la boussole
    public Vector3  flecheDirection;

    float anglefire;
    
    float timepassing=0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        posSpark = sparky.transform.position;

        posCampdeFeux = campdeFeux.transform.position;

        flecheDirection =  posSpark-posCampdeFeux;

        flecheDirection = flecheDirection.normalized;

        if(flecheDirection.z<0)
        {
            anglefire= Mathf.Acos(flecheDirection.x/flecheDirection.magnitude)*Mathf.Rad2Deg ;
        }
            
        else
        {
            anglefire= -Mathf.Acos(flecheDirection.x/flecheDirection.magnitude)*Mathf.Rad2Deg ;
        }

        timepassing=timepassing+ Mathf.PI/90; 

        anglefire= anglefire+Mathf.Cos(timepassing)*5;

        if(timepassing>=Mathf.PI*2)
        {
            timepassing=0f;
        }

        transform.rotation=Quaternion.Euler(45, 0, -anglefire-45f);
    }
}
