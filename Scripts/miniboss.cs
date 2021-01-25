// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault & Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gérer le déplacement, l'attaque, la mort, et les sons du miniboss 
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class miniboss : MonoBehaviour
{
    //tableau des nom pour les boss
    string[] NomBoss = {"Vhervad","Bhithlirh","Z'aiodhr'kax","Hiogd'zho","Vep'zha","Iagruthlu","Evrijhar","Etroulth'las","Ighogno","Omhaaxh'endag","Shiozhish","Dull'xitl","Bhaiobrixr","Naagh'dro","Duro","Aiuhogr'zhulb","Iatalu","Yokthilparc","Yohugnn'xerc", "Ev'indoxz" };

    // Le personnage principale
    public GameObject Sparky;

    //ui de la vie du boss
    public GameObject leUi;

    //barre de vie du boss de base
    public GameObject laBarreVieBoss;
    
    //clone de la vie du boss
    public GameObject laBarreVieBossClone;
    
    // Le vecteur vers entre le monstre et spark
    public  Vector3 positionspark ;

    // vecteur direction vers spark
    public  Vector3 flecheDirection;

    // vecteur direction vers l obstacle
    public  Vector3 flecheDirection2;

    //distance entre perso et le cercle
    public  Vector3 distanceSparkDuCercle;

    // vecteur pour se placer a coté de spark;
    public  Vector3 nextToSpark;

    // vitesse de marche
    public float vitesse =1;

    // vitesse d attaque
    public float vitesseAttaque =5;

    //variable d attente entre la marche et les pause
    float tempsarret ;

    // periode pendant la marche rapide 
    public float tempsDash =1f ;

    // pause avant de faire le bon d attaque
    public float pauseAvantAttaque=0.5f;

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
    
    // animator pour l attaque
    Animator m_Animator;

    // les colliders 
    CapsuleCollider[] colliders;

    //audisource du monstre
    AudioSource audiosourceMonstre;

    //son du monstre
    public AudioClip hurt;

    //son attaque
    public AudioClip sonAttaque;

    //son mort
    public AudioClip mort;

    //savoir si monstre attaque
    public bool attaque;

    //vie maximale du boss
    public int vieMax;

    //slider de la vie
    public Slider vie;

    //zone du boss
    public GameObject laZone;

    //zone qui ce creer
    GameObject laZoneCloner;

    //musique du boss
    public AudioClip[] musiqueBoss;

    //variable pour faire fonctionner le dps
    public float Delay = 1;
    public float NbDefoisDps = 3;
    public float DpsSec = 1;
    private int NbFoisFrapper = 0;

    void Start()
    {  
       Sparky=GameObject.Find("Spark");
       leUi=GameObject.Find("UI");
       marcher();
       m_Animator = gameObject.GetComponent<Animator>();
       colliders =GetComponents<CapsuleCollider>();
       audiosourceMonstre = GetComponent<AudioSource>();
       laZoneCloner = Instantiate(laZone, new Vector3(transform.position.x,-0.02f,transform.position.z), laZone.transform.rotation);
       laBarreVieBossClone = Instantiate(laBarreVieBoss);
       laBarreVieBossClone.transform.SetParent(leUi.transform,false);
       vie =laBarreVieBossClone.GetComponent<Slider>();
       vie.maxValue = vieMax*GestionDeProgression.viePlus;
       vie.value = vie.maxValue;
       laBarreVieBossClone.GetComponentInChildren<Text>().text = NomBoss[Random.Range(0,NomBoss.Length)];
    }
  
    // Update is called once per frame
    void Update()
    {
        if(vie.value>0)
        {
            //trouver le vecteur qui pointe vers spark 
            positionspark = Sparky.transform.position;

            //flecheDirection= positionspark-transform.position;
            if(positionspark.x> transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX=true;
                nextToSpark = new Vector3(-3f,0,0);
                colliders[1].center=new Vector3(4.11f,3.1f,0f);
            }

            else
            {
                GetComponent<SpriteRenderer>().flipX=false;
                nextToSpark = new Vector3(3f,0,0);
                colliders[1].center=new Vector3(-4.11f,3.1f,0f);
            }
            
            flecheDirection= positionspark+nextToSpark-transform.position;

            if(flecheDirection.magnitude < 30)
            { 
                laBarreVieBossClone.GetComponent<CanvasGroup>().alpha = 1;


                if(!audiosourceMonstre.isPlaying)
                {
                    audiosourceMonstre.clip = musiqueBoss[Random.Range(0,musiqueBoss.Length)];
                    audiosourceMonstre.Play();
                    StartCoroutine(FadeAUdio.StartFade(audiosourceMonstre,3.5f,1));
                }
            }

            else
            {
                laBarreVieBossClone.GetComponent<CanvasGroup>().alpha = 0;

                if(audiosourceMonstre.isPlaying)
                {
                    StartCoroutine(FadeAUdio.StartFade(audiosourceMonstre,2f,0));
                    Invoke("stopMusique",2f);
                }
            }
        
            // si le vecteur a une longueur de 10 on declanche l'attaque
            if(flecheDirection.magnitude < 3 && oneShot)
            {
                oneShot=false;
                marche=false;
                GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesse*0;
                CancelInvoke();
                Invoke("OpenMouthattack",pauseAvantAttaque);
            }

            if((laZoneCloner.transform.position-positionspark).magnitude>50)
            {
                flecheDirection = laZoneCloner.transform.position-transform.position;
            }
        }

        if(vie.value<=0 && m_Animator.GetBool("mort") == false)
        {   
            gameObject.GetComponent<SpawnRemains>().SpawnRemain();
            GameObject.Find("Spark").GetComponent<AudioSource>().PlayOneShot(mort, 0.4f);
            m_Animator.SetBool("mort", true);
            Invoke("mortMonstre" ,0.4f);
            PAsobstuer=false;
            vie.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    //fais marcher le monstre vers le joueur
    void FixedUpdate() 
    {
        if(marche&&PAsobstuer)
        { 
           GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesse;
        }
    }

    // contourne un object qui l obstue en marchant en parallele 
    private void OnCollisionEnter(Collision autreObjet) 
    {   
        if(autreObjet.gameObject.tag == "obstacle")
        {
            PAsobstuer=false;
            flecheDirection2= autreObjet.transform.position-transform.position;
            flecheDirection2=new Vector3 (flecheDirection2.z,flecheDirection2.y,-flecheDirection2.x);
            GetComponent<Rigidbody>().velocity =flecheDirection2.normalized*vitesse;
        }
           
        if(autreObjet.gameObject.tag == "arme" && vie.value>0)
        {
            if(SparkyInventaire.armeFeu == true)
            {
                StartCoroutine(Dps());
            }

            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie.value -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[0].value.ValeurModifier;
            m_Animator.SetTrigger("hurt");
        }

        if(autreObjet.gameObject.tag == "fireball" && vie.value>0)
        {
            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie.value -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[1].value.ValeurModifier;
            m_Animator.SetTrigger("hurt");
        }
    }

    // On quite l'obstruction nous pouvons reparcher vers le personnage
    private void OnCollisionExit(Collision other) 
    {
        PAsobstuer=true;
    }

    // detecter si une arme a atteind la zone du monstre.
    // detruire le monstre
    // se remettre a marcher vers le personnage pendant un temps aleatoire
    void marcher()
    {
        tempsarret =Random.Range(1f,4f);
        marche=true;
        PAsobstuer=true;
        Invoke("arreter",tempsarret);
    }
      
    //s'arreter pendant un temps aleatoire
    // et reinisialliser 
    void arreter()
    {
        tempsarret =Random.Range(0f,1f);
        marche=false;
        Invoke("marcher",tempsarret);
        colliders[1].enabled = false;
        GetComponent<Rigidbody>().velocity=new Vector3(Random.Range(-vitesse*0.5f,vitesse*0.5f),0f,Random.Range(-vitesse*0.5f,vitesse*0.5f));
        attaque = false;
    }
    //script qui delenche l attaque et par l animation
    void OpenMouthattack()
    { 
        Invoke("attack",ouvertureBoucheSansAvancer);
        m_Animator.SetTrigger("attacking");
        attaque = true;
    }
    // debut de l attaque
    void attack()
    {
      
        GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesseAttaque;
        Invoke("arreter",tempsDash);
        Invoke("pause",BreakAvantProchaineAttaque);
        Invoke("colliderDelay",0.4f);
    }
    /// enable le collider au bon moment de l attaque.
    void colliderDelay()
    {
        colliders[1].enabled = true;
    }
    // la pause avant la prochaine attaque prend fin
    void pause()
    {
        oneShot=true;  
    }
    // destruction final du monstre
    public void mortMonstre()
    {
        Destroy(gameObject);
        Destroy(laZoneCloner);
        Destroy(laBarreVieBossClone);
    }

    //fait joue le son 
    public void sonEpee()
    {
       audiosourceMonstre.PlayOneShot(sonAttaque, 0.7f);
    }

    //arrete la musique
    public void stopMusique()
    {
        audiosourceMonstre.Stop();
    }

    //permet de faire du dammage per seconds
    IEnumerator Dps() 
    {
    yield return new WaitForSeconds(Delay);

        while(NbFoisFrapper < NbDefoisDps && vie.value >0)
        {
            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie.value -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[0].value.ValeurModifier * 1.33f;
            m_Animator.SetTrigger("hurt");
            yield return new WaitForSeconds(DpsSec);
            NbFoisFrapper++;
        }
    }
}
