// ===============================
// AUTEUR(S) : Mathieu Gaudreau & Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
//  Script qui permet de changer l'intensite de la lumiere selon le temps de la journee
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    //référence au directional light
    public GameObject lumiere;

    //référence à l'intensité de la lumière
    public float intensiteLumiere;

    //détermine si un cycle a été complété
    public bool Cycle;

    //compteur du nombre de jours complétés
    public static int nbJourTotal = 1;

    //référence au texte du UI
    public Text counter;

    //durée de la journée
    public float tempsJours;

    //durée de la nuit
    public float tempsNuits;

    //image du soleil qui tourne
    public GameObject soleil; 

    //rotation du soleil
    float rotSoleil;


    //donne l"intensité de base de la lumière
    void Start()
    {
        intensiteLumiere = lumiere.GetComponent<Light>().intensity;
        nbJourTotal = 1;
        //((intensiteLumiere) * 360f)
    }

    //Change l'intensite de la lumiere et fait tourner l'image de la lumiere selon le temps du jour
    void Update()
    {   
        rotSoleil=intensiteLumiere*(360f)/6f-3*360/6;

        //si le cycle n'est pas complet, l'intensité de la lumière suit l'heure de la journée
        //et augmente le compteur de jour de 1
        if(!Cycle)
        {
            soleil.transform.localRotation = Quaternion.Euler(0,0,rotSoleil);
            lumiere.GetComponent<Light>().intensity = intensiteLumiere -= Time.deltaTime * tempsJours;

            if(intensiteLumiere <=0)
            {
                Cycle = true;
                nbJourTotal++;
            }
        }
        
        //si le cycle est complet, le jour reviens et le cycle recommence
        else if (Cycle)
        {   
            soleil.transform.localRotation = Quaternion.Euler(0,0,rotSoleil);
            lumiere.GetComponent<Light>().intensity = intensiteLumiere += Time.deltaTime * tempsNuits; 
            
            if(intensiteLumiere >=3)
            {
                Cycle = false;
            }
        }
        //augmemte le compteur de jour du UI
        counter.text ="JOUR " + nbJourTotal.ToString();
    }
}
