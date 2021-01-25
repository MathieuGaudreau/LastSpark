// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de faire apparaitre le texte d'interaction a cote de l'objet
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texteInteract : MonoBehaviour
{   
    //texte a faire apparaitre
    public GameObject txt;

    //Fait apparaitre le texte a la position de l'objet vide
    void Update()
    {
        Vector3 positionTxt = Camera.main.WorldToScreenPoint(this.transform.position);
        txt.transform.position = positionTxt;
    }
}
