// ===============================
// AUTEUR(S) : Justin Marques & Mathieu Gaudreau
// ===============================
// DESCRIPTION:
//  Script qui permet de changer les scènes et de gerer les sons des boutons
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangementScenes : MonoBehaviour
{

    //permet de jouer le son du hover
    public void MouseHover()
    {
        GetComponent<AudioSource>().Play();    
    }

    // vers scène du jeu
    public void DemarerJeu()
    {
        SceneManager.LoadScene(3);
    }

    // vers scène "How to play" pour clavier
    public void GoHTPkeyboard()
    {
        SceneManager.LoadScene(1);
    }
    // ---------------------------- À FAIRE : DÉTECTER SI SUR XBOX OU ORDI -----------------------------

    // vers scène "How to play" pour xbox
    public void GoHTPxbox()
    {
        SceneManager.LoadScene(2);
    }

    //retourner au menu principal
    public void RetourMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    // ferme l'onglet du jeu
     public void quitterJeu() 
     {
        Application.Quit();
     }

    //Ferme les aides de jeu
    public void fermerBouton()
    {        
        GameObject.Find("AideDeJeu").GetComponent<AudioSource>().Play();
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    // résume le jeu et ferme le menu pause
    public void ContinuerJeu()
    {
        GameObject.Find("Spark").GetComponent<ControleSparky>().ResumeGame();
    }
}
