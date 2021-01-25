// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de donner le id des items dans la base de donne
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{   
    //tableau qui contient les items
    public ItemObject[] ItemObjects;

    //update automatiquement els id des objets dans la database
    [ContextMenu("update id")]
    public void UpdateID()
    {
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            if(ItemObjects[i].data.Id != i)
            {
                ItemObjects[i].data.Id = i;
            }
        }
    } 

    //update les id apres que le jeu ai sauvegarder les objets
    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {
    }
}
