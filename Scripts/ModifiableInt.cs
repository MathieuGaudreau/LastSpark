// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de modifier les attributs du personnage avec  celui de l'armes equiper
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();
[System.Serializable]
public class ModifiableInt
{   
    //valeur de base
    [SerializeField]
    private int valeurBase;

    public int ValeurdeBase {get {return valeurBase;} set {valeurBase = value; UpdateValeurModifier();} }

    //valeur a ajouter
    [SerializeField]
    private int valeurModifier;

    public int ValeurModifier {get {return valeurModifier;} private set {valeurModifier = value;} }

    //liste des attributs
    public List<IModifiers> modifiers = new List<IModifiers>();

    //la valeur qui est modifier
    public event ModifiedEvent ValeurQuiEstModifier;

    //donne la variable method
    public ModifiableInt(ModifiedEvent method = null)
    {
        valeurModifier = ValeurdeBase;

        if(method != null)
        {
            ValeurQuiEstModifier += method;
        }
    }

    public void RegisterModEvent(ModifiedEvent method)
    {
        ValeurQuiEstModifier += method;
    }

    public void UnregisterModEvent(ModifiedEvent method)
    {
        ValeurQuiEstModifier -= method;
    }

    //fait updater la valeur
    public void UpdateValeurModifier()
    {
        var valueToAdd = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].AjouteValeur(ref valueToAdd);
        }

        valeurModifier = valeurBase + valueToAdd;
        
        if(ValeurQuiEstModifier != null)
        {
            ValeurQuiEstModifier.Invoke();
        }
    }

    //ajouter les modificateurs
    public void AjouteModifications(IModifiers _modifier)
    {
        modifiers.Add(_modifier);
        UpdateValeurModifier();
    }

    //enleve les modificateurs
    public void EnleveModifications(IModifiers _modifier)
    {
        modifiers.Remove(_modifier);
        UpdateValeurModifier();
    }
}
