// ===============================
// AUTEUR(S) : Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui faiut en sorte que les objets au sols font face la caméra
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    //permet aux sprites des objets de toujours être face à la caméra 
    private void LateUpdate()
    {
        transform.forward = GameObject.FindGameObjectWithTag("MainCamera").transform.forward;
    }
}
