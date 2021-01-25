// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui contient toutes les fonctions qui permettent d'ajouter, de modifier et d'enlever les items dans les différents inventaires
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using UnityEditor;

//Types d'nventaires et d'interfaces disponibles
public enum InterfaceType
{
    Inventory,
    Equipment,
    Crafting
}

//permet de creer un nouvel inventaire dans l'inspecteur
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]

public class InventoryObject : ScriptableObject
{   
    //nom du chemin pour sauvegarder inventaire
    public string cheminSauvegarde;

    //database pour savoir les objets disponibles
    public ItemDatabaseObject database;

    //variable pour savoir les types d'ecran inventaire
    public InterfaceType type;

    //inventaire qui va contenir tout les objets
    public Inventory Container;

    //tableau qui va trouver tout les slots
    public InventorySlot[] trouveSlots{get {return Container.Slots;} }

    //va ajouter les items ramasser par le personnage et les mettres dans l,inventaire du perso
    public bool AjouterItem(Item _item, int _amount)
    {   
        //si le nombre de slot vide est de 0 ou moins, il return false donc on ne peut pas ajouter plus d'item
        if(NombreSlotVide <= 0)
         {
            return false;
         }  

        //cherche si l'item ramasse existe deja
        InventorySlot slot = TrouveItemDansInventaire(_item);

        //si item existe pas,on l'ajoute, s'il peux  ce stack  avec les autres pareil
        if(!database.ItemObjects[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        
        //monte le nombre de l'item si il y en a d'autre
        slot.AjouteNombreItem(_amount);
        return true;
    }

    //compte le nombre de slots vide
    public int NombreSlotVide
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < trouveSlots.Length; i++)
            {
                if(trouveSlots[i].item.Id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public InventorySlot TrouveItemDansInventaire(Item _item)
    {
        for (int i = 0; i < trouveSlots.Length; i++)
        {
            if(trouveSlots[i].item.Id == _item.Id)
            {
                return trouveSlots[i];
            }
        }
        return null;
    }

    //si la slot est vide on retourne la slot dans le compteur
    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < trouveSlots.Length; i++)
        {
            if(trouveSlots[i].item.Id <= -1)
            {
                trouveSlots[i].UpdateSlot(_item, _amount);
                return trouveSlots[i];
            }
        }
        return null;
    }

    //permet de changer l'enplacement des items
    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {   
        //si l'item est allowed dans la slot, un objet temporaire est cree pour le transporter dans la nouvelle slot, 
        //es items vont donc echanger d'emplacement
        if(item2.PeutPlacerDansSlot(item1.ItemObject) && item1.PeutPlacerDansSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item,item2.amount);
            item2.UpdateSlot(item1.item,item1.amount);
            item1.UpdateSlot(temp.item,temp.amount);
        }
    }
    
    //permet de vider l,inventaire ou l'equipement dans l,inspecteur
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }

}

//creer l'inventaire de base
//donne le nombre de slot voulu 
//le clear au chargement
[System.Serializable]
public class Inventory
{
        public InventorySlot[] Slots = new InventorySlot[12];
        public void Clear()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].Enleveitem();
            }
        }
}

//prend la meme fonction deja creer ailleur
public delegate void SlotUpdated(InventorySlot _slot);

//creer les slots de l,inventaire
[System.Serializable]
public class InventorySlot
{   
    public ItemType[] ItemsPermis = new ItemType[0];

    [System.NonSerialized]
    public UserInterface parent;

    [System.NonSerialized]
    public GameObject slotDisplay;

    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;

    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;
    public Item item = new Item();
    public int amount;

    //creer un objet et lui odnne un id dans le database
    public ItemObject ItemObject
    {
        get
        {
            if(item.Id >= 0)
            {
                return parent.inventaire.database.ItemObjects[item.Id];
            }
            return null;
        }
    }

    //update l'inventaire quand il y a un nouveau item
    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }

    //update l'inventaire quand il y a deja l'item
    public InventorySlot(Item _item, int _amount)
    {
        UpdateSlot(_item, _amount);
    }

    public void UpdateSlot(Item _item, int _amount)
    {
        if(OnBeforeUpdate != null)
        {
            OnBeforeUpdate.Invoke(this);
        }

        item = _item;
        amount = _amount;

        if(OnAfterUpdate != null)
        {
            OnAfterUpdate.Invoke(this);
        }
    }

    //update l'inventaire quand l'item est enlever
    public void Enleveitem()
    {
        UpdateSlot(new Item(), 0);
    }

    //update le nombre de chaque item
    public void AjouteNombreItem(int value)
    {
        UpdateSlot(item, amount += value);
    }

    //verifie si l'item est allowed dans la slot
    public bool PeutPlacerDansSlot(ItemObject _itemObject)
    {
        if (ItemsPermis.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
        { 
            return true;
        }
         
        for (int i = 0; i < ItemsPermis.Length; i++)
        {
            if(_itemObject.type == ItemsPermis[i])
            {
                return true;
            }
        }
        return false;
    }
}