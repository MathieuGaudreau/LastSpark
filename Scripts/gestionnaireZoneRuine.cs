// ===============================
// AUTEUR(S) : Jutin Marques
// ===============================
// DESCRIPTION:
// Gère les effets lumineux des zones de ruines selon le cycle Jour/nuit
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gestionnaireZoneRuine : MonoBehaviour
{
    // lumière du symbole gravé
   // GameObject lumiereSymbole;
  public GameObject lumiereSymbole;
    
    GameObject lumiereDayNightCycle;

    //intensité de la lumière du symbole pendant la nuit
    public float intensiteSymboleNuit;

    //intensité de la lumière du symbole pendant le jour
    public float intensiteSymboleJour;

    //intensité de la lumière du cycle jour/nuit
    float intensiteDayNightCycle;

    //postition pour instantiate 
    public GameObject positionObjet;

    //objet a instantiate
    public GameObject[] trinkets;

    // cherche l'intensité de la lumière du cycle de jour/nuit
    void Start()
    {   
        lumiereDayNightCycle = GameObject.Find("Lumiere");
       // lumiereSymbole = GameObject.FindGameObjectWithTag("lumiereRuineSymbole");
        intensiteSymboleNuit = 3;
        intensiteSymboleJour = 1;
       

        var trinketsSpawn = Instantiate(trinkets[Random.Range(0,trinkets.Length)] , positionObjet.transform.position, positionObjet.transform.rotation);

        trinketsSpawn.transform.parent = gameObject.transform;
    }

    // Chnage l'intensité des lumières de la zone
    void Update()
    {
      
        //augmente ou baisse l'intensité de la lumière du symbole selon l'heure
        lumiereSymbole.GetComponent<Light>().intensity = 3 - (lumiereDayNightCycle.GetComponent<Light>().intensity*2f)/3f;
    }
   
}
