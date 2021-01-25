// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gerer toutes les interractions avec l'inventaire, l'equipement et le crafting
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SparkyInventaire : MonoBehaviour
{       
    //reference au UI de l'inventaire
    public GameObject inventoryUI;

    // pour savoir si inventaire est deja ouvert
    public static bool invOpen = false;

    // reference a inventaire du perso
    public InventoryObject inventory;

    //reference a equipement du perso
    public InventoryObject equipment;

    public InventoryObject crafting;

    //tableau des attribus du perso
    public Attribute[] attributes;

    //audiosource du personnage
    AudioSource audioSource;

    //son de ouverture et fermeture de inventaire
    public AudioClip ouvreFerme;

    //image de l'epee pour ui
    public Image imageArmeEquip;

    //image du sort pour UI
    public Image imageSortEquip;

    //image du sort pour UI
    public Image placeHolderEpee;

    //image du sort pour UI
    public Image placeHolderSort;

    //image du sort pour UI
    public Image placeHolderEpeeCraft;

    //image du sort pour UI
    public Image placeHolderTrinket;   

    public Sprite blank;

    //permet de savoir si le perso a l'epee de départ sur lui
    public static bool epeeEquip = false;

    //permet de savoir si le perso a la boule de feu sur lui
    public static bool fireballEquip = false;
    
    //nombre de reamisn dans inventaire
    public static int nbRemains;

    //nombre de roche dasn inventaire
    public static int nbRoche;

    //popup d'aide pour l'inventaire
    public GameObject aideInv;

    //permet de de savoir si l'inventaire a deja ete ouvert une fois
    public bool dejaEteOuvert;

    //animator du personnage
    Animator animSpark;

    //tableau des sons des armes
    public AudioClip[] tabSonsArmes;

    //permet de savoir si un objet a ete creer
    bool itemCreer;

    //permet de savoir si l'arme dps est equip
    public static bool armeFeu;

    //nombre de remais que le doiut bruler pour changer la foret
    public static int nbRemainsBruler;


    //donne le audio source, cache l'inventaire, donne les attributs au perso et permet d'equipper les items
    void Start()
    {   
        animSpark = gameObject.GetComponent<Animator>();
        aideInv.SetActive(false);
        imageArmeEquip.enabled = false;
        imageSortEquip.enabled = false;
        audioSource = GetComponent<AudioSource>();
        inventoryUI.GetComponent<CanvasGroup>().alpha = 0f;  

        //instentie le tableau des attributs
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }

        //permet de savoir si un objet est depose dans les slots d'equipement
        for (int i = 0; i < equipment.trouveSlots.Length; i++)
        {
            equipment.trouveSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.trouveSlots[i].OnAfterUpdate += OnAddItem;
        }  

        //permet de savoir si un objet est depose dans les slots d'equipement
        for (int i = 0; i < crafting.trouveSlots.Length; i++)
        {
            crafting.trouveSlots[i].OnBeforeUpdate += OnRemoveItem;
            crafting.trouveSlots[i].OnAfterUpdate += OnAddItem;
        }  
    }

    // ouvre et ferme inventaire avec I, de crafter des objets, de changer les animations et le mana;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I) && !ControleSparky.enMarche && !ControleSparky.craftOuv)
        {
            if(invOpen)
            {   
                audioSource.PlayOneShot(ouvreFerme, 0.4f);
                Hide();
                invOpen = false;
                Time.timeScale = 1;
            }

            else if (!ControleSparky.craftOuv)
            {
                audioSource.PlayOneShot(ouvreFerme, 0.4f);
                Show();
                invOpen = true;
                Time.timeScale = 0;
                
            }

            if (!dejaEteOuvert)
            {
                aideInv.SetActive(true);
                dejaEteOuvert = true;
            }
        }

        //change l.opaciti/ du sort si il n'y a pas assez de mana pour le faire jouer
        if(gameObject.GetComponent<ControleSparky>().Mana.value <= 0)
        {
            imageSortEquip.GetComponent<CanvasGroup>().alpha = 0.5f;
        }

        else
        {
            imageSortEquip.GetComponent<CanvasGroup>().alpha = 1f;
        }

        //detecte si l'arme dps est equip
        if(equipment.trouveSlots[0].item.Id == 12 || equipment.trouveSlots[0].item.Id == 13)
        {
            armeFeu = true;
        }
        else
        {
            armeFeu = false;
        }

        //appel les fonctions
        craft();
        updateAnimator();
        OeilDeMana();
    }

    
    //permet de cacher  l'inventaire
    public void Hide() 
    {
     inventoryUI.GetComponent<CanvasGroup>().alpha = 0f; 
     inventoryUI.GetComponent<CanvasGroup>().blocksRaycasts = false; 
    }

    //permet de montrer l'inventaire
    public void Show() 
    {
     inventoryUI.GetComponent<CanvasGroup>().alpha = 1f;
     inventoryUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }  

    //si l'item est enlever de l'equipement, les buffs donnes au perso s'enleve
    //enleve le srpite du ui qui montre4 quel item est equipper
    public void OnRemoveItem(InventorySlot _slot)
    {
        //si il y a pas d'item equipper il return
        if(_slot.ItemObject == null)
        {
            return;      
        }

        //si l"interface sur lequel l'item etait depose est equipement, les buff donner par l'item vont s'enlever
        switch (_slot.parent.inventaire.type)
        {
            case InterfaceType.Inventory:
            break;

            case InterfaceType.Crafting:
            break;

            case InterfaceType.Equipment:

            for (int i = 0; i < _slot.item.buffs.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if(attributes[j].type == _slot.item.buffs[i].attributes)
                    {
                        attributes[j].value.EnleveModifications(_slot.item.buffs[i]);
                    }
                }
            }
            
            //si l'item enlever a un sprite dans UIdisplay, l'epee est dsactive, l'image du ui est enlever
            if(_slot.ItemObject.SpriteUi !=null)
            {
                switch (_slot.ItemsPermis[0])
                {
                   case ItemType.Weapons:
                  placeHolderEpee.enabled = true;
                  epeeEquip = false;
                  imageArmeEquip.sprite = null;
                  imageArmeEquip.enabled = false;
                    break;

                    case ItemType.Spells:
                  placeHolderSort.enabled = true;
                  fireballEquip = false;
                  imageSortEquip.enabled = false;
                  imageSortEquip.sprite =null;
                    break;
                }
            }
            break;
        }
    }

    //si l'item est ajouter dans l'equipement, les buffs sont ajoute au perso 
    //donne le sprite de l'item a ui 
    public void OnAddItem(InventorySlot _slot)
    {
        //si la case est vide il return
        if(_slot.ItemObject == null)
        {
            return;  
        }

        //si l"interface sur lequel l'item est depose est equipement, les buff s'ajoute au personnage
        switch (_slot.parent.inventaire.type)
        {
            case InterfaceType.Inventory:
            break;
            
            case InterfaceType.Crafting:
            break;

            case InterfaceType.Equipment:

            for (int i = 0; i < _slot.item.buffs.Length; i++)
            {
                for (int j = 0; j < attributes.Length; j++)
                {
                    if(attributes[j].type == _slot.item.buffs[i].attributes)
                    {
                        attributes[j].value.AjouteModifications(_slot.item.buffs[i]);
                    }
                }
            }

            //si l'item ajouter a un sprite dans UIdisplay, l'epee est dsactive, l'image du ui est enlever
            if(_slot.ItemObject.SpriteUi !=null)
            {
                switch (_slot.ItemsPermis[0])
                {
                   case ItemType.Weapons:
                  placeHolderEpee.enabled = false;
                  epeeEquip = true;
                  imageArmeEquip.enabled = true;
                  imageArmeEquip.sprite = _slot.ItemObject.SpriteUi;
                  imageArmeEquip.SetNativeSize();
                    break;

                   case ItemType.Spells:
                  placeHolderSort.enabled = false;
                  fireballEquip = true;
                  imageSortEquip.enabled = true;
                  imageSortEquip.sprite = _slot.ItemObject.SpriteUi;  
                  imageSortEquip.SetNativeSize();                  
                    break;
                }
            }
            break;
        }
    }

    //permet de changer les layers des animations selon l'arme 
    public void updateAnimator()
    {
            if(equipment.trouveSlots[0].item.Id == -1)
            {
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("epee"), 0);
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("claymore"), 0);
            }

            if(equipment.trouveSlots[0].item.Id == 2 || equipment.trouveSlots[0].item.Id == 8 || equipment.trouveSlots[0].item.Id == 9 || equipment.trouveSlots[0].item.Id == 13)
            {
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("epee"), 1);
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("claymore"), 0);
            }

            if(equipment.trouveSlots[0].item.Id == 3 || equipment.trouveSlots[0].item.Id == 10 || equipment.trouveSlots[0].item.Id == 11 || equipment.trouveSlots[0].item.Id == 12)
            {
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("claymore"), 1);
                animSpark.SetLayerWeight(animSpark.GetLayerIndex("epee"), 0);
            } 
    }

    //permet de changer le son selon l'arme
    public void SonArmes()
    {
        if(equipment.trouveSlots[0].item.Id==2 || equipment.trouveSlots[0].item.Id == 8 || equipment.trouveSlots[0].item.Id == 9  || equipment.trouveSlots[0].item.Id == 13 && !ControleSparky.isCasting)
        {
            audioSource.PlayOneShot(tabSonsArmes[0],0.7f);
        }

        if(equipment.trouveSlots[0].item.Id==3 || equipment.trouveSlots[0].item.Id == 10 || equipment.trouveSlots[0].item.Id == 11 || equipment.trouveSlots[0].item.Id == 12 && !ControleSparky.isCasting)
        {
            audioSource.PlayOneShot(tabSonsArmes[1],0.7f);
        }

        if(equipment.trouveSlots[1].item.Id == 0 && ControleSparky.isCasting)
        {
            audioSource.PlayOneShot(tabSonsArmes[2], 0.5f);
        }
    }

    //cahnge le nombre de mana
    public void OeilDeMana()
    {      
        for (int i = 0; i < inventory.trouveSlots.Length; i++)
        {
            if(inventory.trouveSlots[i].item.Id == 5)
            {
                 gameObject.GetComponent<ControleSparky>().Mana.maxValue = 10;
            }
        }
    }

    //permet de bruler les restes du boss et de regenerer la carte
    public void BrulerReste()
    {   
        audioSource.PlayOneShot(gameObject.GetComponent<ControleSparky>().heal);
        for (int i = 0; i < inventory.trouveSlots.Length; i++)
        {
            if(inventory.trouveSlots[i].item.Id == 1)
            {
                inventory.Container.Slots[i].amount -=1; 
                inventory.Container.Slots[i].UpdateSlot(inventory.trouveSlots[i].item,inventory.Container.Slots[i].amount);
                nbRemainsBruler++;

                if(inventory.trouveSlots[i].amount ==0)
                {
                    inventory.trouveSlots[i].Enleveitem();
                }

                if(nbRemainsBruler == GestionDeProgression.nombreDeRemains)
                {
                    GameObject.Find("GestionDeProgression").GetComponent<GestionDeProgression>().nextLvl();
                    nbRemainsBruler = 0;
                }
            }
        }
    }

    //Cette fonction sera disponible dans une version future du jeu
    // public void ConstruireMur()
    // {
    //     for (int i = 0; i < inventory.trouveSlots.Length; i++)
    //     {
    //         if(inventory.trouveSlots[i].item.Id == 5)
    //         {
    //             inventory.Container.Slots[i].amount -=1; 
    //             inventory.Container.Slots[i].UpdateSlot(inventory.trouveSlots[i].item,inventory.Container.Slots[i].amount);

    //             if(inventory.trouveSlots[i].amount ==0)
    //             {
    //                 inventory.trouveSlots[i].Enleveitem();
    //             }
    //         }
    //     }
    // }

    //permet de creer de nouvelles armes 
    public void craft()
    {   
        //epee avec radis
        if(crafting.trouveSlots[0].item.Id == 2 & crafting.trouveSlots[1].item.Id == 7)
        {
            crafting.trouveSlots[2].item.Id =8;
        }

        //epee avec plume
        if(crafting.trouveSlots[0].item.Id == 2 & crafting.trouveSlots[1].item.Id == 6)
        {
            crafting.trouveSlots[2].item.Id =9;
        }

        //epee avec gem
        if(crafting.trouveSlots[0].item.Id == 2 & crafting.trouveSlots[1].item.Id == 4)
        {
            crafting.trouveSlots[2].item.Id =13;
        }

        //claymore avec radis
        if(crafting.trouveSlots[0].item.Id == 3 & crafting.trouveSlots[1].item.Id == 7)
        {
            crafting.trouveSlots[2].item.Id =10;
        }

        //claymore avec plume
        if(crafting.trouveSlots[0].item.Id == 3 & crafting.trouveSlots[1].item.Id == 6)
        {
            crafting.trouveSlots[2].item.Id =11;
        }

        //claymore avec gem
        if(crafting.trouveSlots[0].item.Id == 3 & crafting.trouveSlots[1].item.Id == 4)
        {
            crafting.trouveSlots[2].item.Id =12;
        }

        if(crafting.trouveSlots[2].item.Id > -1)
        {
            itemCreer = true;
            GameObject.Find("itemCree").GetComponent<Image>().sprite = crafting.trouveSlots[2].ItemObject.SpriteUi;
            crafting.trouveSlots[2].item.Name = crafting.trouveSlots[2].ItemObject.data.Name;
            crafting.trouveSlots[2].item.buffs = crafting.trouveSlots[2].ItemObject.data.buffs;
            crafting.trouveSlots[2].UpdateSlot(crafting.trouveSlots[2].item,1);
        }

        if(itemCreer == true)
        {
            
            for (int i = 0; i < inventory.trouveSlots.Length; i++)
            {
                if(inventory.trouveSlots[i].item.Id == crafting.trouveSlots[2].item.Id)
                {
                    crafting.trouveSlots[0].Enleveitem();
                    crafting.trouveSlots[1].Enleveitem();
                    crafting.trouveSlots[2].Enleveitem();
                    itemCreer = false;
                }
            }

            for (int i = 0; i < equipment.trouveSlots.Length; i++)
            {
                if(equipment.trouveSlots[i].item.Id == crafting.trouveSlots[2].item.Id)
                {
                    crafting.trouveSlots[0].Enleveitem();
                    crafting.trouveSlots[1].Enleveitem();
                    crafting.trouveSlots[2].Enleveitem();
                    itemCreer = false;
                }
            }
        }
            //change les placeholder des slots
            if(crafting.trouveSlots[0].item.Id > -1)
            {
                placeHolderEpeeCraft.enabled = false;
            }
            else if (crafting.trouveSlots[0].item.Id == -1)
            {
                placeHolderTrinket.enabled = true;
            }

            if(crafting.trouveSlots[1].item.Id > -1)
            {
                placeHolderTrinket.enabled = false;
            }
            else if(crafting.trouveSlots[0].item.Id == -1)
            {
                placeHolderEpeeCraft.enabled =true;
            }
    }

    //si le personnage entre dans le collider de l'item il peut le ramasser et l'item s,ajoute a l'inventaire
    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<GroundItem>();
        if (item)
        {
            Item _item = new Item(item.item);
            if(inventory.AjouterItem(_item,1))
            {
                if(_item.Id == 1)
                {
                    nbRemains +=1;
                }

                //pour version future
                //  if(_item.Id == 5)
                // {
                //     nbRoche +=1;
                // }

                Destroy(other.gameObject);
            }
        }
    }
    
    public void AttributeModified(Attribute attribute)
    {
       return;
    }

    //vide inventaire quand le jeu ce ferme
    private void OnApplicationQuit() 
    {
        inventory.Clear();
        equipment.Clear();
        crafting.Clear();
    }
}

//classe qui permet de donner les attribut des objets au personnage
[System.Serializable]
public class Attribute
{
    [System.NonSerialized]

    //reference au script de l'inventaire
    public SparkyInventaire parent;

    //donne choix de buff au personnage
    public Attributes type;

    //donne la value du buff
    public ModifiableInt value;

    public void SetParent(SparkyInventaire _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }
    
    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
