// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui contieent la fonction qui ajoute les valeurs des attributs des objets au personnage
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//creer la valeur a ajouter
public interface IModifiers
{
    void AjouteValeur(ref int valeurBase);
}
