// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
//  Script qui permet de ramasser un objet au sol et qui lui donne une animation
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundItem : MonoBehaviour, ISerializationCallbackReceiver
{   
    //reference a l'item au sol
    public ItemObject item;

    //reference au data de l'item
    public Item ItemData;

    //nombre d'item
    public int amount;

    //permet de savoir si il a ete pris
    public bool looted = false;

    //vitesse de l'aanimation
    float vitesse = 1f;

    //hauteur de l'animation
    float hauteur = 0.17f;

    //offset de la position
    Vector3 posOffset = new Vector3 ();

    //position temporaire de l'objet
    Vector3 tempPos = new Vector3 ();
    
    //donne la position de depart de l'objet
    private void Start() 
    {
        posOffset = transform.position;
    }

    //fait monter et dscendre l'objet quand il est au sol
    private void Update() 
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * vitesse) * hauteur;
        transform.position = tempPos;
    }

    public void OnAfterDeserialize(){}

    public void OnBeforeSerialize()
    {
    #if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = item.SpriteUi;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    #endif
    }
}
