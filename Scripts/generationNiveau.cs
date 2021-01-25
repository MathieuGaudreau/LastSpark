// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault
// ===============================
// DESCRIPTION:
//  Script qui modifie les valeur du script Generation Niveau, augmenet le nombre de boss et donne le nombre de cendre a bruler.
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generationNiveau : MonoBehaviour
{
    // table avec tout les arbre et decords 
    public GameObject[] arbresEtAutres;

    // table avec les % d apparition de chacun
    public int[] TableauDesPourcentage;

    //pourcentage vide d apparation
    public int pourcentageVide=100;

    // tableau utilie pour effectueur les pourcentage dans une boucle
    int[] TableauDesLimitesPourcentage;

    // variable utile pour remplir le tableau des limites de pourcentage
    int limiteaccumulerPourcent=0 ;

    // variable qui sera trouver avec un random 
    int pourcentage;
  
    // tableau des object qui on un nombre définie d'appartion
    public GameObject[] LimitedSpawnGO;

    // tableau ou on decide le nombre d'apparition
    public int[] TableauNbSpawnlimiter;

    // tableau utile pour les placer dans la boucle final
     int[] TableauDesLimitesSpawnlimiter;

    //variable pour cerrer le tableau  
    int borneInferieur;

    int limiteSpawnaccumuler=0;
    
    // limite du terrain 
    public int positionZmin =0; 
    public int positionZmax =100;
    public int positionXmin =0;
    public int positionXmax =100;
    public int tailleCarrer =10;
  
    Vector3 myVector1;
   
    int nbObjectLimiter =0;

    //tableau des positions limiter 
    public int[,,] PositionOjectLimiter ;

    //bool pour definir si c est un object limiter
    bool boss =false;
  
    // nombre de spawn pour une longeur de terrain
    int nbdeSpawn;

    // limite pour distancier les object des autres
    public float demiLmax = 1f;

    // Start is called before the first frame update
    void Start()
    {
      
    // tableau accumulatif des limites pour passez au prochain object a spawner, exemble 2 roche et 2 arbre donne {2,4} 
    TableauDesLimitesSpawnlimiter = new int[TableauNbSpawnlimiter.Length+1];

    for (int i = 0; i < TableauNbSpawnlimiter.Length; i++)

    {
        limiteSpawnaccumuler+=TableauNbSpawnlimiter[i];
        TableauDesLimitesSpawnlimiter[i]+=limiteSpawnaccumuler;
    }
        
   
    // tableau des position des object x et z des object limiter remplie avec des -1 dans les troue vide
    nbObjectLimiter=TableauNbSpawnlimiter.Length;
    PositionOjectLimiter = new int[limiteSpawnaccumuler,2,nbObjectLimiter];

    for (int d = 0; d < limiteSpawnaccumuler; d++) // nombre d object limiter au total
    {
        for (int i = 0; i < 2; i++) // position x , position y
        {
            for (int b = 0; b <nbObjectLimiter; b++) // nombre d object au total.
            {
                PositionOjectLimiter[d,i,b]=-1;
            }
        }
    }

    // tableau des limite des pourcentage , ex 4% arbre 8% roche -> {0,4,12} 
    TableauDesLimitesPourcentage = new int[TableauDesPourcentage.Length+1];
    TableauDesLimitesPourcentage[0]=0;

    for (int i = 1; i < TableauDesLimitesPourcentage.Length; i++)
    {
        limiteaccumulerPourcent+= TableauDesPourcentage[i-1];
        TableauDesLimitesPourcentage[i]=limiteaccumulerPourcent;
    }

    // division du cadriller des spawn, vu que le terrain commence  a 0 
    nbdeSpawn = (positionZmax-positionZmin)/tailleCarrer;

    remplissageBoss();
    }

    public void CommeLeStart()
    {
    ////////////// bout rajouter pour la progression/////////////////////////
    // old static way
    //TableauNbSpawnlimiter[0]= GestionDeProgression.nombremonstre;
    
    

    ///////////////////////////////////////////////////////////////////////////////////
    // tableau accumulatif des limites pour passez au prochain object a spawner, exemble 2 roche et 2 arbre donne {2,4} 
    TableauDesLimitesSpawnlimiter = new int[TableauNbSpawnlimiter.Length+1];
    limiteSpawnaccumuler=0 ;

    for (int i = 0; i < TableauNbSpawnlimiter.Length; i++)
    {
        limiteSpawnaccumuler+=TableauNbSpawnlimiter[i];
        TableauDesLimitesSpawnlimiter[i]+=limiteSpawnaccumuler;  
    }
        
    // tableau des position des object x et z des object limiter remplie avec des -1 dans les troue vide
    nbObjectLimiter=TableauNbSpawnlimiter.Length;
    PositionOjectLimiter = new int[limiteSpawnaccumuler,2,nbObjectLimiter];

    for (int d = 0; d < limiteSpawnaccumuler; d++) // nombre d object limiter au total
    {
        for (int i = 0; i < 2; i++) // position x , position y
        {
            for (int b = 0; b <nbObjectLimiter; b++) // nombre d object au total.
            {
                PositionOjectLimiter[d,i,b]=-1;
            }
        }
    }
    
    // tableau des limite des pourcentage , ex 4% arbre 8% roche -> {0,4,12} 
    TableauDesLimitesPourcentage = new int[TableauDesPourcentage.Length+1];
    TableauDesLimitesPourcentage[0]=0;
    limiteaccumulerPourcent=0 ;
       
    for (int i = 1; i < TableauDesLimitesPourcentage.Length; i++)
    {
        limiteaccumulerPourcent+= TableauDesPourcentage[i-1];
        TableauDesLimitesPourcentage[i]=limiteaccumulerPourcent;
    }
 
    // division du cadriller des spawn, vu que le terrain commence  a 0 
    nbdeSpawn = (positionZmax-positionZmin)/tailleCarrer;
      
    remplissageBoss();
    }


    //La carte est séparer dans un grand cadriller [][] et un object sera placer au centre et puis deplacer de maniere aléatoire a l'interieur
    //La boucle for se déplace d'un carrer à l'autre et verifie si tout d'abord un object limiter occupe cette place dans la table positionobjectlimiter et le place
    // sinon elle choisira un pourcentage aleatoire et ensuite vera comparara se pourcentage avec la table des limites des pourcentage.

void PositionMod()
{
    // parcour de tout les division de la map
    for (int i = positionXmin; i < positionXmax; i+= tailleCarrer)
    {
        
        for (int b = positionZmin; b < positionZmax; b+= tailleCarrer)
        {
        // parcour de chacun des object limiter
        for (int caseObjet = 0; caseObjet < limiteSpawnaccumuler; caseObjet++)
        {   // verifie pour chaque c'est quoi le type d object possible ou il en aurait un assigner (-1 serait vide)
            for (int typeobjet = 0; typeobjet < nbObjectLimiter; typeobjet++) 
            {   
                // object 1 position x          //object 1 position y   // correspond t'il a une case dans la map?
                if( PositionOjectLimiter[caseObjet,0,typeobjet]==i&& PositionOjectLimiter[caseObjet,1,typeobjet]==b&& !boss)
                {
                    myVector1= new Vector3((i*1.0f+tailleCarrer/2)+Random.Range(-tailleCarrer/2f+demiLmax, tailleCarrer/2f-demiLmax),0.0f , (b*1.0f+tailleCarrer/2f)+Random.Range(-tailleCarrer/2f+demiLmax, tailleCarrer/2f-demiLmax));
                    GameObject ObjetCloner1 = Instantiate(LimitedSpawnGO[typeobjet],myVector1,LimitedSpawnGO[typeobjet].transform.rotation);
                    boss =true; // un object a été trouver alors on ne mettrera pas un arbre a la meme place
                }
            }
        }
            
        // si boss est false on peut mettre un object aleatoir non compté.
        if(boss)
        {
        boss=false;

        }
        else
        {  
            // on choisi un pourcentage et ensuite on regarde dans quel plage de pourcentage se retrouve t il.chaque zone corresponde a un object.
            pourcentage =Random.Range (0,limiteaccumulerPourcent+pourcentageVide);

            for (int obj = 0; obj < arbresEtAutres.Length; obj++)
            {

                if(pourcentage>=TableauDesLimitesPourcentage[obj]&& pourcentage<TableauDesLimitesPourcentage[obj+1] )
                {  
                        
                    myVector1= new Vector3((i*1.0f+tailleCarrer/2)+Random.Range(-tailleCarrer/2f+demiLmax, tailleCarrer/2f-demiLmax),0.0f , (b*1.0f+tailleCarrer/2f)+Random.Range(-tailleCarrer/2f+demiLmax, tailleCarrer/2f-demiLmax));
                    GameObject ObjetCloner1 = Instantiate(arbresEtAutres[obj],myVector1,arbresEtAutres[obj].transform.rotation);
                }
                
                else
                {
                //le pourcentage etais trop grand cela correspondon a un espace vide}
                }
                       
            }
        }
      }   
    }
}

// TableauNbSpawnlimiter.length-> pour chaque catégorie d'object limiter. exemple: boss , roche , vache ->3
//limiteSpawnaccumuler -> Pour couvrire le total d object limiter 3boss , 2roche, 5vache ->10 
//TableauDesLimitesSpawnlimiter ->{0,3,5,10}
//pour 0 jusqu'a 2 , place un boss. pour 3 jusqu'a 4 , place une roche , pour 5 jusqu'a 10 place une vache
// PositionOjectLimiter[numeroDel'object en question ,position x-> (0) ou y->(1),object en question]
//le tableau sera ensuite verifier dans la boucle final de spawn car ses posistions sont bien specifiquement
// des points de spawn.
public void remplissageBoss()
{
    for (int objet = 0; objet < TableauNbSpawnlimiter.Length; objet++)
    {
        for (int i = 0; i < limiteSpawnaccumuler; i++)
        {

            if( objet ==0)
            {
                borneInferieur=0;
                    
            }
                
            else
            {
                borneInferieur=TableauDesLimitesSpawnlimiter[objet-1];
            }

            if(i>=borneInferieur&&i<TableauDesLimitesSpawnlimiter[objet]){
            PositionOjectLimiter[i,0,objet]=  Random.Range(0,nbdeSpawn)*tailleCarrer;
            PositionOjectLimiter[i,1,objet]=  Random.Range(0,nbdeSpawn)*tailleCarrer;

                //limite inferieur superieur campsite nospawn boss
                 while ((PositionOjectLimiter[i,0,objet]>167f&& PositionOjectLimiter[i,0,objet]<277f&&PositionOjectLimiter[i,1,objet]>193f&& PositionOjectLimiter[i,1,objet]<301)||( PositionOjectLimiter[i,0,objet]<30f || PositionOjectLimiter[i,0,objet]>470f||PositionOjectLimiter[i,1,objet]<30f|| PositionOjectLimiter[i,1,objet]>460f))
                {
                   
                PositionOjectLimiter[i,0,objet]=  Random.Range(0,nbdeSpawn)*tailleCarrer;
                PositionOjectLimiter[i,1,objet]=  Random.Range(0,nbdeSpawn)*tailleCarrer;
                }
            }
        }   
        
    }   
   PositionMod();
}
}




