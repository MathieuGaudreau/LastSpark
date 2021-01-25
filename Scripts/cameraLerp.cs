// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
//  scipt de gestion de la camera , pour quelle suive et puisse zommer ou quitter le personnage
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLerp : MonoBehaviour
{
    public GameObject sparky;

    public  Vector3 selfieStick = new Vector3(0f,11f,-33f);

    public  Vector3 selfieStickproche = new Vector3(0f,5.52f,-8f);

    public float  amortissment = 0.1f;

    public float  amortissmentproche = 0.1f;

    bool stop = false;

    public float tempsAvantProche =3f;
    
    bool stopbool=true;

    float camX;

    float camZ;

    private void Start() 
    {
        camX = gameObject.transform.position.x;
        camZ = gameObject.transform.position.z;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.LookAt(sparky.transform);
        if(stopbool && sparky.GetComponent<Rigidbody>().velocity.magnitude ==0)
        {
           Invoke("getclose",tempsAvantProche);
           stopbool=false;
        }

        else
        {
           CancelInvoke();
        }
        
        if( sparky.GetComponent<Rigidbody>().velocity.magnitude >0 ||stop==false)
        {
            stopbool=true;
            stop=false;
            Vector3 positionFinal = sparky.transform.TransformPoint(selfieStick);

            if(positionFinal.x <= 39f)
            {
                positionFinal.x = 39f;
            }
                        
            if(positionFinal.x >= 461f)
            {
                positionFinal.x = 461f;
            }

            if(positionFinal.z <= -19)
            {
                positionFinal.z = -19f;
            }

            if(positionFinal.z >= 410f)
            {
                positionFinal.z = 410f;
            }

            transform.position = Vector3.Lerp(transform.position,positionFinal,amortissment);
        }

        else if(stop)
        {
            Vector3 positionFinal1 = sparky.transform.TransformPoint(selfieStickproche);
            transform.position = Vector3.Lerp(transform.position,positionFinal1,amortissmentproche);
        }
    }
    
    void getclose()
    {
        stop=true;
    }
}