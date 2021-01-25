// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de construire les murs
// Cette fonction sera seulemetn disponible dans une version future du jeu
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMurs : MonoBehaviour
{
    //image du mur
    SpriteRenderer spriteMur;

    //collider du mur
    BoxCollider colliderMur;

    //alpha du mur
    public static float alpha =0.2f;

    // Va chercher les components de l'objet
    void Start()
    {   
        colliderMur = gameObject.GetComponent<BoxCollider>();
        spriteMur = gameObject.GetComponent<SpriteRenderer>();
        colliderMur.enabled = false;
    }

    //achange l'alpha du mur et lui donne un collider si l'alpah est au max
    void Update()
    {
        if(alpha == 1)
        {
            colliderMur.enabled = true;
        }

        spriteMur.color = new Color(1f,1f,1f,alpha);
    }

    //enleve l'alpha si le jeu quitte
    private void OnApplicationQuit() 
    {
        alpha = 0.2f;
    }
}
