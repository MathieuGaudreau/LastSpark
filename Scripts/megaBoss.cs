// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault & Mathieu Gaudreau
// ===============================
// DESCRIPTION:
// Script qui permet de gérer le déplacement, l'attaque, la mort, et les sons du boss final
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class megaBoss : MonoBehaviour
{
    //tableau pour le nom du boss
    string[] NomBoss = {"Vhervad","Bhithlirh","Z'aiodhr'kax","Hiogd'zho","Vep'zha","Iagruthlu","Evrijhar","Etroulth'las","Ighogno","Omhaaxh'endag","Shiozhish","Dull'xitl","Bhaiobrixr","Naagh'dro","Duro","Aiuhogr'zhulb","Iatalu","Yokthilparc","Yohugnn'xerc", "Ev'indoxz" };

    // Le personnage principale
    public GameObject Sparky;

    //tentacules du boss
    public GameObject tentacule;

    //ui de la vie du boss
    public GameObject leUi;

    //barre de vie du boss de base
    public GameObject laBarreVieBoss;
    
    //clone de la vie du boss
    public GameObject laBarreVieBossClone;
    
    // Le vecteur vers entre le monstre et spark
    public  Vector3 positionspark ;

    public  Vector3 positionsparkviser ;

    // vecteur direction vers spark
    public  Vector3 flecheDirection;

    // vecteur direction vers l obstacle
    public  Vector3 flecheDirection2;

    //distance entre perso et le cercle
    public  Vector3 distanceSparkDuCercle;

    // vecteur pour se placer a coté de spark;
    public  Vector3 nextToSpark;

    // vitesse de marche
    public float vitesse =4f;

    // vitesse d attaque
     float inversevitesse =1;

    //variable d attente entre la marche et les pause
    float tempsarret ;

    // periode pendant la marche rapide 
    public float tempsDash =1f ;

    // pause avant de faire le bon d attaque
    public float pauseAvantAttaque=0.25f;

    // une pause avant de recoomencer une attaque
    public float BreakAvantProchaineAttaque=3f;

    // bool qui permet la marche vers le personnage
    public bool marche=false;

    // bool losque un monstre fonce dans un arbre
    public bool PAsobstuer=true;

    // bool pour faire un seul invoke dans update
    bool oneShot=true;
    // devien off apres la premiere phase et déclenche une animation
    bool phase1=true;

    // devien off apres la 2iem phase et déclenche une animation
    bool phase2=true;
    // lorsque le monstre se montre la premiere fois , devient off apres.
    bool revealed=true;
    
    // animator pour l attaque
    Animator m_Animator;

    // les colliders 
    CapsuleCollider[] grosCollider;

    //audisource du monstre
    AudioSource audiosourceMonstre;

    //son du monstre
    public AudioClip hurt;

    //son attaque
    public AudioClip sonAttaque;

    //son mort
    public AudioClip mort;

    //son sortie de la terre
    public AudioClip terre;
    //savoir si monstre attaque
  
    //vie maximale du boss
    public int vieMax;

    //slider de la vie
    public Slider vie;

    //zone du boss
    public GameObject laZone;

    //zone qui ce creer
    GameObject laZoneCloner;

    //musique du boss
    public AudioClip musiqueBoss;

    //variables qui font fonctionner la fonction dps
    public float Delay = 1;
    public float NbDefoisDps = 3;
    public float DpsSec = 1;
    private int NbFoisFrapper = 0;

    // Start is called before the first frame update
    void Start()
    {
       Sparky=GameObject.Find("Spark");
       leUi=GameObject.Find("UI");
       m_Animator = gameObject.GetComponent<Animator>();
       grosCollider =GetComponents<CapsuleCollider>();
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
        {  // les phase ou le monstre se cache de nouveau.
            if(phase1 && vie.value <=(vieMax/2)&&vie.value >=(vieMax/4))
            {
                phase1=false;
                m_Animator.SetBool("activation",false);
                grosCollider[1].enabled = false;
                Invoke("retourBattle",5f);
            }

            if(phase2 && vie.value <(vieMax/4))
            {
                phase2=false;
                m_Animator.SetBool("activation",false);
                grosCollider[1].enabled = false;
                Invoke("retourBattle",5f);
            }

            //trouver le vecteur qui pointe vers spark 
            positionspark = Sparky.transform.position;

            //flecheDirection= positionspark-transform.position;
            if(positionspark.x> transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX=true;
                nextToSpark = new Vector3(-3f,0,0);
            }

            else
            {
                GetComponent<SpriteRenderer>().flipX=false;
                nextToSpark = new Vector3(3f,0,0);
            }
            // vecteur ver sparky 
            flecheDirection= positionspark+nextToSpark-transform.position;
            // vecteur ver sparky a une distance de 8 le boss se revele
            if(flecheDirection.magnitude < 8 && !m_Animator.GetBool("activation")&&revealed)
            {
                m_Animator.SetBool("activation",true);
                grosCollider[1].enabled = true;
                laBarreVieBossClone.GetComponent<CanvasGroup>().alpha = 1;
                revealed=false;
            }
            // a une distance de 30 le titre et la musique embarque
            if(flecheDirection.magnitude < 30&&!revealed)
            { 
                laBarreVieBossClone.GetComponent<CanvasGroup>().alpha = 1;

                if(!audiosourceMonstre.isPlaying)
                {
                    audiosourceMonstre.clip = musiqueBoss;
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
            if(flecheDirection.magnitude < 15 && oneShot && m_Animator.GetBool("activation"))
            {  
                oneShot=false;
                marche=false;
                GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesse*0;
                CancelInvoke();
                Invoke("OpenMouthattack",pauseAvantAttaque);
            }
                // si spark est assez eloingner le boss retourne dans ca zone
            if((laZoneCloner.transform.position-positionspark).magnitude>50)
            {
                flecheDirection = laZoneCloner.transform.position-transform.position;
            }
        }

        if(vie.value<=0 && m_Animator.GetBool("dead") == false)
        {   
            gameObject.GetComponent<SpawnRemains>().SpawnRemain();
            GameObject.Find("Spark").GetComponent<AudioSource>().PlayOneShot(mort, 0.4f);
            PAsobstuer=false;
            vie.GetComponent<CanvasGroup>().alpha = 0;
            m_Animator.SetBool("dead", true);
        }
    }
    // marche vers sparky
     void FixedUpdate() 
    {
        if(marche&&PAsobstuer)
        { 
           GetComponent<Rigidbody>().velocity =flecheDirection.normalized*vitesse*inversevitesse;
        }
    } 
    // contournement des obstacle et effet effet des degats
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
    //sortir dune collision
     private void OnCollisionExit(Collision other) 
    {
        PAsobstuer=true;
    }
    // fonction pour permettre d attaquer
    void tempsEntreAttaque()
    {
        oneShot=true;
    }
    //fonction qui altener entre marche vers sparky et evation aleatoir
    void marcher()
    {  
        tempsarret =Random.Range(2f,3f);

        if(!m_Animator.GetBool("activation")&&!phase1)
        {
            tempsarret =Random.Range(0.5f,1f);
            inversevitesse=-1;
        }

        else
        {
            inversevitesse=1;
        }
      
        marche=true;
        PAsobstuer=true;
        Invoke("fuir",tempsarret);
    } 
    //evation aleatoir sauf si sparky est loins
    void fuir()
    {   
        if(!oneShot)
        {
            Invoke("tempsEntreAttaque",Random.Range(0f,2.4f));
        }

        tempsarret =Random.Range(0.5f,1f);
        marche=false;

        Invoke("marcher",tempsarret);

        if(flecheDirection.magnitude < 15)
        {
            GetComponent<Rigidbody>().velocity=new Vector3(Random.Range(-vitesse*2f,vitesse*2f),0,Random.Range(-vitesse*2f,vitesse*2f));
        }

        else
        {
            marche=true;
        } 
    } 
    //debut de l animation et prise de la position pour les tentacule
    void OpenMouthattack()
    {
        m_Animator.SetTrigger("attaque");
        positionsparkviser=positionspark;
    } 
    // instanciation des tentacule.
    void tentaculation()
    {
        GameObject cloneTentacule = Instantiate(tentacule,new Vector3(positionsparkviser.x,0.22f,positionsparkviser.z), Quaternion.Euler(0,0, 0));
    }
    // retour du gros collider
    void retourBattle()
    {
        m_Animator.SetBool("activation",true);
        grosCollider[1].enabled = true;
    } 
    //mort du monstre
    void mortMonstre()
    {
        Destroy(gameObject);
        Destroy(laZoneCloner);
        Destroy(laBarreVieBossClone);
        //laZoneCloner.GetComponent<SpawnChandelleAndClearSpace>().turnOff();  
    }

    //fait jouer le son quand il sort de la terre
    void SortTerre()
    {
        audiosourceMonstre.PlayOneShot(terre);
    }

    //fait arreter la musique
    public void stopMusique()
    {
        audiosourceMonstre.Stop();
    }

    //permet de faire des degat par seconde au boss
    IEnumerator Dps() 
    {
    yield return new WaitForSeconds(Delay);

        while(NbFoisFrapper < NbDefoisDps && vie.value >0)
        {
            audiosourceMonstre.PlayOneShot(hurt, 0.8f);
            vie.value -= GameObject.Find("Spark").GetComponent<SparkyInventaire>().attributes[0].value.ValeurModifier *  1.33f;
            m_Animator.SetTrigger("hurt");
            yield return new WaitForSeconds(DpsSec);
            NbFoisFrapper++;
        }
    }
}
