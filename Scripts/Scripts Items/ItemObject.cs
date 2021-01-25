// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de creer un nouvel item et lui donne tout les caracteristiques necessaire (Nom, description, attributs, type) 
//==================================
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//enumeration des type d'item retrouver dans le jeu
public enum ItemType
{
    Healing,
    Remains,
    Weapons,
    Spells,
    Default,
    trinkets
}

//enumeration des diffrents attributs des objets
public enum Attributes
{
    Dégat_Physique,
    Dégat_Feu
}

//permet de creer un nouvel objet dans l,inspecteur
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Item")]
public class ItemObject : ScriptableObject
{   
    //sprite qui apparait dans le ui
    public Sprite SpriteUi;

    //permet de dire si l'item peut etre stack ou non dans l'inventaire
    public bool stackable;

    //type de l'item
    public ItemType type;

    //permet de donner description a l'item
    [TextArea(15,20)]
    public string description;

    //creer l'item data
    public Item data = new Item();

    //creer le nouvel item
    public Item CreerItem()
    {
        Item newItem = new Item(this);
        return newItem;
    } 
}

//classe pour l<item
[System.Serializable]
public class Item
{   
    //nom de l'item
    public string Name;

    //id de l,item de base
    public int Id = -1;

    //liste des buffs que l'item donne au personnnage
    public ItemBuff[] buffs;

    //creer item
    public Item()
    {
        Name = "";
        Id = -1;
    }

    //pass les donner au nouvel item
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attributes = item.data.buffs[i].attributes
            };
        }
    }
}

//classe qui donne les attributs au items
[System.Serializable]
public class ItemBuff : IModifiers
{
    //type d'attribut
    public Attributes attributes;

    //value du buff
    public int value;

    //valeur minimu du buff
    public int min;

    //valeur max du buff
    public int max;

    //genere aleatoirement les valeur du buiff
    public ItemBuff(int _min, int _max)
    {
        min= _min;
        max = _max;
        genereValeur();
    }

    //ajoute 
    public void AjouteValeur(ref int valeurBase)
    {
        valeurBase += value;
    }

    //genere valeur aleatoire
    public void genereValeur()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
