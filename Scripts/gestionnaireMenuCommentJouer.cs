// ===============================
// AUTEUR(S) : Justin Marques
// ===============================
// DESCRIPTION:
// Changement entre les différerents onglets d'aide de jeu du menu "Comment Jouer"
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gestionnaireMenuCommentJouer : MonoBehaviour
{
    public GameObject aideControles;
    public GameObject aideProgression;
    public GameObject aideInventaire;
    public GameObject aideCrafting;

    // rend actif l'onglet des contrôles et désactive les autres
    void Start()
    {
        aideControles.SetActive(true);
        aideProgression.SetActive(false);
        aideInventaire.SetActive(false);
        aideCrafting.SetActive(false);
    }

    //active l'onglet contrôles
    public void activationControles()
    {
        aideControles.SetActive(true);
        aideProgression.SetActive(false);
        aideInventaire.SetActive(false);
        aideCrafting.SetActive(false);
    }

    //active l'onglet Progression
    public void activationProgression()
    {
        aideControles.SetActive(false);
        aideProgression.SetActive(true);
        aideInventaire.SetActive(false);
        aideCrafting.SetActive(false);
    }

    //active l'onglet Inventaire
    public void activationInventaire()
    {
        aideControles.SetActive(false);
        aideProgression.SetActive(false);
        aideInventaire.SetActive(true);
        aideCrafting.SetActive(false);
    }

    //active l'onglet Crafting
    public void activationCrafting()
    {
        aideControles.SetActive(false);
        aideProgression.SetActive(false);
        aideInventaire.SetActive(false);
        aideCrafting.SetActive(true);
    }
}
