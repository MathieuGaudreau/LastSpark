// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui donne un effet de scintillement sur la lumiere du feu
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeuDeCamp : MonoBehaviour
{
   //intensite max de la lumiere
   public float intensiteMax = 28.0f;

   //intensite min de la lumiere
   public float intensiteMin = 30.0f;

   //lumiere du feu de camp
   public Light lumiere;

    //Fait osciller la lumiere du feu 
    void Update()
    {
        lumiere.intensity = Random.Range(intensiteMin,intensiteMax);
    }
}
