// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault & Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gérer le déplacement, l'attaque, la mort, et les sons de l'ennemi 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadMouthMovingAndAttacking : MonoBehaviour
    {

    // Le personnage principale
    public GameObject Sparky;

    // Le vecteur vers entre le monstre et spark
    public  Vector3 positionspark ;

    // vecteur direction vers spark
    public  Vector3 flecheDirection;

    // vecteur direction vers l obstacle
    public  Vector3 flecheDirection2;
    
    // vitesse de marche
    public float vitesse =1;
    
    // vitesse d attaque
    public float vitesseAttaque =5;

    //variable d attente entre la marche et les pause
    float tempsarret ;

    // periode pendant la marche rapide 
    public float tempsDash =1f ;

    // pause avant de faire le bon d attaque
    public float pauseAvantAttaque=1f;

    // pause avant de faire le bon d attaque
    public float ouvertureBoucheSansAvancer=0.1f;

    // une pause avant de recoomencer une attaque
    public float BreakAvantProchaineAttaque=3f;

    // bool qui permet la marche vers le personnage
    public bool marche=true;

    // bool losque un monstre fonce dans un arbre
    public bool PAsobstuer=true;

    // bool pour faire un seul invoke dans update
    bool oneShot=true;

    bool jenRentre=false;
    bool jenSort=false;
    
    // animator pour l attaque
    Animator m_Animator;

    // les colliders 
    CapsuleCollider[] colliders;

    //audisource du monstre
    AudioSource audiosourceMonstre;

    //son du monstre
    public AudioClip rire;

    //son attaque
    public AudioClip bite;

    //son mort
    public AudioClip mort;

    //son du monstre quand il se fait frapper
    public AudioClip hurt;

    //savoir si monstre attaque
    public bool attaque;

    //nombre de point de vie du monstre
    public float vie=6;

    //permet de savoir si le monstre est mort
    public bool MonstreMort;

    //varibales pour la fonction de dps
    public float Delay = 1;
    public float NbDefoisDps = 3;
    public float DpsSec = 1;
    private int NbFoisFrapper = 0;

    //Fonction qui va chercher les components du monstre, va trouver le personnage et commence le déplacement du monstre
    void Start()
    { 
        vie= vie*GestionDeProgression.viePlus;
        Sparky=GameObject.Find("Spark");
        marcher();
        m_Animator = gameObject.GetComponent<Animator>();
        colliders =GetComponents<CapsuleCollider>();
        audiosourceMonstre = GetComponent<AudioSource>();
    }
    
    //Gere les deplacement, attaque, mort de l'ennemi
    void Update()
    {
        if (vie > 0)
        {
            //trouver le vecteur qui pointe vers spark 
            positionspark = Sparky.transform.position;
            flecheDirection = positionspark - transform.position;

            if (positionspark.x > transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }

            // si le vecteur a une longueur de 10 on declanche l'attaque
            if (flecheDirection.magnitude < 10 && oneShot)
            {
                oneShot = false;
                marche = false;
                GetComponent<Rigidbody>().velocity = flecheDirection.normalized * vitesse * 0;
                CancelInvoke("marcher");
                CancelInvoke("arreter");

                Invoke("OpenMouthattack", pauseAvantAttaque);
                Invoke("monstreRire", Random.Range(3f, 7f));
            }
        }
        
        // mort du monstre
        if(vie<=0)
        {
            MonstreMort = true;
            audiosourceMonstre.PlayOneShot(mort, 0.1f);
            Invoke("mortMonstre",0.4f);
            PAsobstuer=false;
        }

    }

    //fais marcher le monstre vers le joueur
    void FixedUpdate() 
    {
        if(marche && PAsobstuer && vie > 0)
        { 
           GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesse;
        }
    }
    // trigger pour contourner les obstacle et se tenir loins des zones.
    private void OnTriggerStay(Collider autreObjet) 
    {
      if(autreObjet.gameObject.tag == "zoneBoss" ||autreObjet.gameObject.tag == "zoneRuine")
        {
            if(!jenRentre)
            {
                jenRentre=true;
                Invoke("jesortirai",2f);
            }

            if(!jenSort)
            {
                PAsobstuer=false;
                flecheDirection2= autreObjet.transform.position-transform.position;
                flecheDirection2=new Vector3 (flecheDirection2.z,flecheDirection2.y,-flecheDirection2.x);
                GetComponent<Rigidbody>().velocity =flecheDirection2.normalized*vitesse;
            }
            else
            {
                PAsobstuer=false;
                flecheDirection2= autreObjet.transform.position-transform.position;
                GetComponent<Rigidbody>().velocity =flecheDirection2.normalized*-vitesse;
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
      //PAsobstuer=true;
      jenSort=false;
      jenRentre=false;
    }

    void jesortirai()
    {
        jenSort= true;
    }



    // contourne un object qui l'obstrue en marchant en parallele 
    // Fait perdre de la vie au monstre si le personnage l'attaque
    private void OnCollisionEnter(Collision autreObjet) 
    {   
        if(autreObjet.gameObject.tag == "obstacle")
        {
            PAsobstuer=false;
            flecheDirection2= autreObjet.transform.position-transform.position;
            flecheDirection2=new Vector3 (flecheDirection2.z,flecheDirection2.y,-flecheDirection2.x);
            GetComponent<Rigidbody>().velocity =flecheDirection2.normalized*vitesse;
        }

        if(autreObjet.gameObject.tag == "arme" && vie > 0)
        {
            if(SparkyInventaire.armeFeu == true)
            {
                StartCoroutine(Dps());
            }

            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[0].value.ValeurModifier;
            m_Animator.SetTrigger("hurt");
        }

         if(autreObjet.gameObject.tag == "fireball" && vie >0)
        {
            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[1].value.ValeurModifier;
            m_Animator.SetTrigger("hurt");
        }
    }

    // On quite l'obstruction nous pouvons reparcher vers le personnage
    private void OnCollisionExit(Collision other) 
    {
        PAsobstuer=true;
    }

    // detruire le monstre
    void mortMonstre()
    {
        m_Animator.SetBool("mort", true);
    }

    void detruitMonstre()
    {
        Destroy(gameObject);
    }

    // se remettre a marcher vers le personnage pendant un temps aleatoire
    void marcher()
    {
        tempsarret =Random.Range(1f,2f);
        marche=true;
        PAsobstuer=true;
        Invoke("arreter",tempsarret);
    }
      
    //s'arreter pendant un temps aleatoire et reinisialliser 
    void arreter()
    {
        tempsarret =Random.Range(0f,0.5f);
        marche=false;
        Invoke("marcher",tempsarret);
        colliders[1].enabled = false;
        GetComponent<Rigidbody>().velocity=Vector3.zero;
        attaque = false;
    }

    // démarre l'animation d'attaque
    void OpenMouthattack()
    { 
        Invoke("attack",ouvertureBoucheSansAvancer);
        m_Animator.SetTrigger("attacking");
        attaque = true;
    }
    
    // fait avancer le monstre vers la cible de son attaque
    // fait jouer le son de l'attaque
    void attack()
    {
        audiosourceMonstre.PlayOneShot(bite,0.8f);
        GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesseAttaque;
        Invoke("arreter",tempsDash);
        Invoke("pause",BreakAvantProchaineAttaque);
        colliders[1].enabled = true;
    }
    
    // met le monstre en pause avant de recommencer l'attaque
    void pause()
    {
        oneShot=true;  
    }

    //fait joue le son de rire si il n'attaque pas
    void monstreRire()
    {   
        if(!attaque)
        {
           audiosourceMonstre.PlayOneShot(rire,0.6f); 
        }   
    }

    //permet de faire du degat aux ennemies a chaque seconde
    IEnumerator Dps() {
    yield return new WaitForSeconds(Delay);

    while(NbFoisFrapper < NbDefoisDps)
    {
        audiosourceMonstre.PlayOneShot(hurt, 0.8f);
        vie -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[0].value.ValeurModifier * 1.33f;
        m_Animator.SetTrigger("hurt");
        yield return new WaitForSeconds(DpsSec);
        NbFoisFrapper++;
    }
}
}
