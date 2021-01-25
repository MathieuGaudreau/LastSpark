// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
//  Script qui permet de créer le menu d'inventaire selon les espaces et le nombre de cases passées dans l'inspecteur et de gerer les interactions
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{
    //prefab de l'inventaire
    public GameObject inventoryPrefab;

    //position debut en x
    public int X_START;

    //position debut en y
    public int Y_START;

    //espacement en x entre les slots
    public int X_SPACE_ENTRE_ITEMS;

    //nombre de colone
    public int NOMBRE_COLONNE;

    //ESPACEMENT EN Y DES ITEMS
    public int Y_SPACE_ENTRE_ITEMS;

    //prend la fonction du parent
    public override void CreerSlots()
    {
       slotsSurInterface = new Dictionary<GameObject , InventorySlot>();
       for (int i = 0; i < inventaire.trouveSlots.Length; i++)
       {
           //creer les slots dans l'ecran d,inventaire
           var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
           obj.GetComponent<RectTransform>().localPosition = GetPostition(i);

           //creer les evenement pour le drag des items
           AddEvent(obj, EventTriggerType.PointerEnter, delegate {OnEnter(obj);}); 
           AddEvent(obj, EventTriggerType.PointerExit, delegate {OnExit(obj);});
           AddEvent(obj, EventTriggerType.BeginDrag, delegate {OnDragStart(obj);});
           AddEvent(obj, EventTriggerType.EndDrag, delegate {OnDragEnd(obj);});
           AddEvent(obj, EventTriggerType.Drag, delegate {OnDrag(obj);});
           inventaire.trouveSlots[i].slotDisplay = obj;
           slotsSurInterface.Add(obj, inventaire.trouveSlots[i]);
       }
    }

    //place les slots selon les positions choisies
    private Vector3 GetPostition(int i)
    {
        return new Vector3(X_START + (X_SPACE_ENTRE_ITEMS * (i % NOMBRE_COLONNE)), Y_START + (-Y_SPACE_ENTRE_ITEMS * (i/NOMBRE_COLONNE)), 0f);
    }
}
