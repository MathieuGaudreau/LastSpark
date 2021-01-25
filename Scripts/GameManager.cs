// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gerer la musique de chaque scene et le curseur de la souris
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //image pour le curseur
    public Texture2D cursorTexture;

    //musique du menu
    public AudioClip musicMenu;

    //musique de la scène jeu
    public AudioClip musicJeu;

    //référence au audio source du game manager
    AudioSource audioSource;

    //permet de savoir si le game manager à déja été créé avant
    private static bool dejaCreer = false;

    //fait référence au audio source
    //donne la position au sprite du cursor
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Vector2 cursorOffset = new Vector2(cursorTexture.width/2, cursorTexture.height/2);
        Cursor.SetCursor(cursorTexture, cursorOffset, CursorMode.Auto);
    }

    //si le game manager na pas déja été créé, il ne ce détruit pas
    //mais si il l'est déja il est détruit pour éviter les copies
     void Awake ()
    {
        if (!dejaCreer)
        {
            DontDestroyOnLoad(this.gameObject);
            dejaCreer = true;
        }
     
        else
        {
            Destroy(this.gameObject);
        }
    }


    //donne la musique au audio source du game manager dépendament de la scène qui est active
    void Update()
    {   
        Scene scene = SceneManager.GetActiveScene();

        if(scene.name == ("menuAccueil") ||scene.name == ("sceneFin"))
        {
            audioSource.clip = musicMenu;
            StartCoroutine(FadeAUdio.StartFade(audioSource, 3,0.3F));
        }

        if(scene.name == ("sceneFin"))
        {
            GameObject.Find("compteurJourTotal").GetComponent<Text>().text = "Nombre de jours survécus : " + DayNightCycle.nbJourTotal.ToString();
            StartCoroutine(FadeAUdio.StartFade(audioSource, 3,0.3F));
        }

        if(scene.name == ("jeu"))
        {
            var musiqueBoss = GameObject.FindGameObjectWithTag("miniboss1").GetComponent<AudioSource>();
            audioSource.clip = musicJeu;
            if(musiqueBoss.isPlaying)
            {
                StartCoroutine(FadeAUdio.StartFade(audioSource, 4,0));
            }

            else if(!musiqueBoss.isPlaying)
            {
                StartCoroutine(FadeAUdio.StartFade(audioSource, 1 , 0.3F));
            }
        }

        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
