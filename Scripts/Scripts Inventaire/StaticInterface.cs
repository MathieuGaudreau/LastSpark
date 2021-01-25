// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gerer les interactions sur les écrans d'inventaire qui ne sont pas dynamiques
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInterface : UserInterface
{
    //reference au slots de l'inventaire
    public GameObject[] slots;

    //prend la fonction du parent
    public override void CreerSlots()
    {
        //creer dictionnaire avec les valeur de la slots
        slotsSurInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventaire.trouveSlots.Length; i++)
        {
            var obj = slots[i];
        
           //donne les event au slots
           AddEvent(obj, EventTriggerType.PointerEnter, delegate {OnEnter(obj);});
           AddEvent(obj, EventTriggerType.PointerExit, delegate {OnExit(obj);});
           AddEvent(obj, EventTriggerType.BeginDrag, delegate {OnDragStart(obj);});
           AddEvent(obj, EventTriggerType.EndDrag, delegate {OnDragEnd(obj);});
           AddEvent(obj, EventTriggerType.Drag, delegate {OnDrag(obj);});
           inventaire.trouveSlots[i].slotDisplay = obj;
           slotsSurInterface.Add(obj, inventaire.trouveSlots[i]);
        }
    }
}
