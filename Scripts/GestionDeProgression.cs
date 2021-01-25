// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
// Code pour délencher la nouvelle map , 
// enleve le decord et en génere un nouveau avec plus d enemie.
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionDeProgression : MonoBehaviour
{
      public GameObject lefeu;
     public GameObject terrain; // le terrain

     public GameObject spaceClearer; // creer un colider pour enlever tout les arbres

     public GameObject ecrantnoir; 

     public static int lvlRendu=1;
     public static int nombreDeRemains; // nombre de reste.
      public static float viePlus =1f;
     bool darken=false;

    // Start is called before the first frame update
    void Start()
    {     nombreDeRemains=1;
      //terrain.GetComponent<generationNiveau>().remplissageBoss();
    }

    // Update is called once per frame   inventoryUI.GetComponent<CanvasGroup>().alpha = 0f;  
    void Update()
    {
      /*if(Input.GetKeyDown(KeyCode.N))
      {
           
            //Invoke("nextLvl",0.1f);
      }*/
      if(darken)
      {
            ecrantnoir.GetComponent<CanvasGroup>().alpha=Mathf.Lerp(ecrantnoir.GetComponent<CanvasGroup>().alpha, 1, 0.1f);
      }

      else
      {
            ecrantnoir.GetComponent<CanvasGroup>().alpha=Mathf.Lerp(ecrantnoir.GetComponent<CanvasGroup>().alpha, 0, 0.1f);
      }
    }

      public void nextLvl()

      {     // // old static way
           // nombremonstre=  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0];
           // nombremonstre= Mathf.RoundToInt(nombremonstre*1.5f);
            lvlRendu++;
            // pour acceder le tableau de gestion d object.
            //  generationNiveau.TableauNbSpawnlimiter[4]=generationNiveau.TableauNbSpawnlimiter[4]*2;
            terrain.GetComponent<generationNiveau>().pourcentageVide=terrain.GetComponent<generationNiveau>().pourcentageVide*6/7;

            //relique
            terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[3]=1 +Random.Range(0,2);
            terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[4]=1 +Random.Range(0,2);
            
            viePlus= viePlus*1.07f;
           if(lvlRendu==2){
                 //mouth
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0]+=10;
                  //miniboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]=2;
                  nombreDeRemains=2;
                  //relique
                  
                 
           }
            else if(lvlRendu==3){
                    //mouth
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0]+=10;
                  //miniboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]=3;
                  nombreDeRemains=3;

                 
                 

            }
             else if(lvlRendu==4){
                  //mouth
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0]+=10;
                  //miniboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]=3;
                   //megaboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[2]=1;
                  nombreDeRemains=4;
                  //relique
                  
            }
            else if(lvlRendu % 3 -2 ==0){

                   //mouth
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0]+=5;
                  //miniboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]+=1;
                   //megaboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[2]=1;

                  //relique
                 

                  nombreDeRemains= terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0] + terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]+terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0];
            }
            else{       
                 
                   //megaboss
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[2]=0;
                   //mouth
                  terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0]+=5;
                  //miniboss
                  //terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]+=1;
                   //megaboss
                 // terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[2]=0;
                  nombreDeRemains= terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0] + terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[1]+terrain.GetComponent<generationNiveau>().TableauNbSpawnlimiter[0];
            }
             
            
            
            darken=true;
            Invoke("nextLvlStep2",1f);
      }
      // fonction qui active leffaceur d arbre 
      public void nextLvlStep2()
      {
            spaceClearer.SetActive(true);
            Invoke("nextLvlStep3",1f);
      }
      // fonction qui active ferme leffaceur d arbre 
      // et generer la nouvelle map
      public void nextLvlStep3()
      {    
            spaceClearer.SetActive(false);
            terrain.GetComponent<generationNiveau>().CommeLeStart();
            Invoke("fadeout",1f);
      }

    public void fadeout()
    {
      darken=false; 
       lefeu.SetActive(true);
       Invoke("finfeu",0.5f);    
    }
     public void finfeu()
    {
      darken=false; 
       lefeu.SetActive(false);     
    }
}
