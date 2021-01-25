// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script permet d'instancier un objet a la mort d'un ennemi ou d'un boss 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRemains : MonoBehaviour
{   
    //permet de savoir si l'objet a deja ete creer
    public bool dejaCreer;

    //remains du boss
    public GameObject remains;

    //arme donner par le boss
    public GameObject arme;

    //offset entre les objets
    Vector3 offset = new Vector3(3,0,0);

    int chanceDe = 20;

    int randomChance;

    private void Start() 
    {
        randomChance = Random.Range(0,100);
        Debug.Log(randomChance);
    }
    //permet d'instancier les objets si ils n'ont pas deja ete cree
    public void SpawnRemain()
    {
        if(!dejaCreer)
        {
            Instantiate(remains, gameObject.transform.position, gameObject.transform.rotation);
            dejaCreer = true;

            if(arme != null)
            {   
                if(randomChance <= chanceDe )
                {
                    Instantiate(arme, gameObject.transform.position + offset, gameObject.transform.rotation);
                }
            }
        }
    }
}
