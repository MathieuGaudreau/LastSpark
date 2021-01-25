// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui vérifie quand un objet entre ou sort dans un écran d'inventaire et qui permet de drag un item 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public abstract class UserInterface : MonoBehaviour
{
    //creer inventaire
    public InventoryObject inventaire;
    
    // popup de description des objets
    public GameObject PopUpItem;
    
    //image de placeholder pour l'epee
    public Sprite epee;
    
    //image de placeholder pour le sort
    public Sprite sort;

    //image de placeholder pour les trinkets
    public Sprite trinket;
    
    //sprite vide
    public Sprite blank;

    //creer dictionnaire avec valur des slots
    public Dictionary<GameObject, InventorySlot> slotsSurInterface = new Dictionary<GameObject , InventorySlot>();

    bool interfaceOuvert;
    
    //trouver le nombre de lsots et lui donne le parent
    //ajoute l'event quand l'item arrive et quitte interface
    void Start()
    {
        PopUpItem.SetActive(false);
        for (int i = 0; i < inventaire.trouveSlots.Length; i++)
        {
            inventaire.trouveSlots[i].parent = this;
            inventaire.trouveSlots[i].OnAfterUpdate += OnSlotUpdate;
        }

        CreerSlots();

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate {OnEnterInterface(gameObject);});
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate {OnExitInterface(gameObject);});
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(SparkyInventaire.invOpen || ControleSparky.craftOuv)
        {
            interfaceOuvert = true;
        }
        else
        {
            interfaceOuvert = false;
        }
    }

    //update les slots qaund un item est ajouter
    private void OnSlotUpdate(InventorySlot _slot)
    {       
            //update le sprite de la slots avec celui de l'item, le nombre et la couleur
            if(_slot.item.Id >=0)
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.SpriteUi;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().SetNativeSize();
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
                _slot.slotDisplay.transform.GetComponentInChildren<Text>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }

            //remet l,image de base de la slot
            else
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,0);
                _slot.slotDisplay.transform.GetComponentInChildren<Text>().text = "";
            }
    }

    //permet de creer les slots
    public abstract void CreerSlots();
    
    //donne les trigger au slots pour swap les items
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    //data de l'objet est stocker sur la souris
    public void OnEnter(GameObject obj)
    {
        MouseData.slotSourisAuDessus = obj;

        //si la souris hover par dessus un objet, un popup de description apparait
        if (slotsSurInterface[obj].item.Id >= 0 && interfaceOuvert)
        {   
            var offset = new Vector3(150,-200,0);

            if (Input.mousePosition.y < 200)
            {
                offset = new Vector3(150,200,0);
            }

            //offset du popup
            

            PopUpItem.SetActive(true);

            //change la position du popup
            PopUpItem.transform.position = offset + obj.transform.position;

            //fait apparaitre la description de l'objet
            PopUpItem.transform.Find("DescriptionItem").GetComponentInChildren<Text>().text = slotsSurInterface[obj].ItemObject.description;
            
            //si l'objet est une arme l'image change
            if(slotsSurInterface[obj].ItemObject.type == ItemType.Weapons)
            {
                PopUpItem.transform.Find("Type").GetComponent<Image>().sprite = epee;
            }

            //si l'objet est un sort l'image sort
            else if (slotsSurInterface[obj].ItemObject.type == ItemType.Spells)
            {
                PopUpItem.transform.Find("Type").GetComponent<Image>().sprite = sort;
            }

            //si l'objet est un trinket l'image trinket
            else if (slotsSurInterface[obj].ItemObject.type == ItemType.trinkets)
            {
                PopUpItem.transform.Find("Type").GetComponent<Image>().sprite = trinket;
            }

            //si c'est un autre sorte d'objet aucune image apparait
            else
            {
                PopUpItem.transform.Find("Type").GetComponent<Image>().sprite = blank;
            }

            // affiche les attributs de l'objet
            if (slotsSurInterface[obj].item.buffs.Length >0)
            {
                PopUpItem.transform.Find("BuffItem").GetComponentInChildren<Text>().text = slotsSurInterface[obj].item.buffs[0].attributes.ToString();
                PopUpItem.transform.Find("Stats").GetComponentInChildren<Text>().text = slotsSurInterface[obj].item.buffs[0].value.ToString();
            }

            else
            {
                PopUpItem.transform.Find("BuffItem").GetComponentInChildren<Text>().text = "";
                PopUpItem.transform.Find("Stats").GetComponentInChildren<Text>().text = "";
            }
        }
    } 
   
    //le popup ce ferme si la souris n'est pas sur l'objet
    public void OnExit(GameObject obj)
    {
        PopUpItem.SetActive(false);
        MouseData.slotSourisAuDessus = null;
    }

    //détecte si la souris rentre sur un interface
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseOver = obj.GetComponent<UserInterface>();
    }

    //détecte si la souris quitte un interface
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseOver = null;
    }

    // detecte si un objet est dragged et creer un objet temporaire
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemDragged = CreateTempItem(obj);
    }

    //fonction qui créer un objet temporaire pour transporter l'objet d'un inventaire à un autre
    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;

        if(slotsSurInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50,50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsSurInterface[obj].ItemObject.SpriteUi;
            img.SetNativeSize();
            img.transform.localScale = new Vector3(0.35f,0.35f,0.35f);
            img.raycastTarget = false;
        }
            return tempItem;
    }

    //donne les propriétés de l'objet temporaire au slot sur lequel on le dépose
    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemDragged);

        if(MouseData.interfaceMouseOver == null)
        {
            slotsSurInterface[obj].Enleveitem();
            return;
        }

        if(MouseData.slotSourisAuDessus)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseOver.slotsSurInterface[MouseData.slotSourisAuDessus];
            inventaire.SwapItems(slotsSurInterface[obj], mouseHoverSlotData);
        }
    }
    
    //donne la position de la souris à l'objet temporaire 
    public void OnDrag(GameObject obj)
    {
        if(MouseData.tempItemDragged != null)
        {
            MouseData.tempItemDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}

// classe qui contient les objets qui interragisse avec la souris
public static class MouseData
{
    public static UserInterface interfaceMouseOver;
    public static GameObject tempItemDragged;
    public static GameObject slotSourisAuDessus;
}

// Fonctions qui permet d'updater les slots quand un objet est déposé dessus
public static class ExtensionMethods
{
    public static void UpdateSlotsDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
         foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            if(_slot.Value.item.Id >=0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.SpriteUi;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
                _slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }

            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,0);
                _slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}