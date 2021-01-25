// ===============================
// AUTEUR(S) : Jean-Philippe Filiatrault & Mathieu Gaudreau & Justin Marques
// ===============================
// DESCRIPTION:
//  Script qui permet de gerer les déplacement, la mort, les interractions, les sons, l'attaque et la mort du personnage
//==================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControleSparky : MonoBehaviour
{   
    //vitesse horizontale Maximale désirée
    public float vitesseMax;   

    // direction de la boule de feu
    Vector3 directionFeu;

    //vitesse horizontale actuelle
    float Horizontal;

    //angle de la boule de feu
    float anglefire;

    //angle de la boule de feu
    float anglefireSpriteSpecial;
    
    //vitesse verticale 
    float Vertical;

    //permet de savoir si le personnage est deja en marche
    public static bool enMarche =false;

    bool mauvaiseDirection =true;

    //permet de savoir si le perso lance une boule de feu
    public static bool isCasting =false;

    bool boolsol=false;

    //compteur de la vie du personnage
    public Slider PointDeVie;

    //compteur des points de mana
    public Slider Mana;

    //référence au texte pop up du feu de camp
    public GameObject restoreText;

    //texte d'interraction des remains
    public GameObject brulerRemains;
    
    //texte d'interraction du mur
    public GameObject construire;

    //hitbox du personnage
    public GameObject hitbox;

    //Créer boule de feu d.origine
    public GameObject fireballOriginale;

    public GameObject finDeLaMainSpells;

    //référence au audio source 
    AudioSource audioSrc;

    //reference au collider
    CapsuleCollider colliders;

    //reference au collider de l'epee
    CapsuleCollider Epeecollider;

    //référence à l'image pour parer
    public GameObject parry;

    //son de mort
    public AudioClip sonMort;

    //son dommage
    public AudioClip sparkHurt;

    //son de la marche du perso
    public AudioClip marche;

    //reference au animator
    Animator sparkAnim;

    //écran de crafting
    public GameObject craftingUi;

    //permet de savoir si l'écran de crafting est ouverte
    public static bool craftOuv = false;

    //offfest pour la postion de l'écran de craftiongf
    Vector3 offset;

    //position de base du ui
    Vector3 positionUi;

    //menu de crafting
    public GameObject craftMenu;
    
    //boussole
    public GameObject boussole;
    
    //grosseur de la boussole
    Vector3 scaleBoussoleBase;

    //position de la boussole de base
    Vector3 posBoussoleBase;

    //offset pour la position de la boussole
    Vector3 offsetBoussole;

    //cooldown entre les sorts
    public float tempsCooldown;

    //temps entre les sorts
    float tempsNextFire = 0;

    //son quand on regagne la vie
    public AudioClip heal;  

    bool aPerduVie;

    // Le jeu est sur pause ou non
    public static bool jeuEstPause = false;

    //référence au UI du menu pause
    public GameObject menuPauseUi;

    //aide pour craft
    public GameObject aideCraft;

    //permet de savoir si le craft a deja ete ouvert
    bool dejaEteOuvert;

    //aide generale sur le jeu
    public GameObject aideProg;

    //assigne les components du personnage et met les éléments du ui à off
    private void Start() 
    { 
      aideProg.SetActive(true);
      aideCraft.SetActive(false);
      offsetBoussole = new Vector3(-97f,100f,0f);  
      scaleBoussoleBase = boussole.transform.localScale;
      posBoussoleBase = boussole.transform.position;
      offset  = new Vector3(0,175,0);
      positionUi = gameObject.GetComponent<SparkyInventaire>().inventoryUI.transform.position;
      craftingUi.SetActive(false);
      menuPauseUi.SetActive(false);
      audioSrc = GetComponent<AudioSource>();
      PointDeVie.value = 10;
      restoreText.SetActive(false);
      brulerRemains.SetActive(false);
      construire.SetActive(false);
      parry.SetActive(false);
      colliders =GetComponent<CapsuleCollider>();
      sparkAnim = gameObject.GetComponent<Animator>();
      Epeecollider = hitbox.GetComponent<CapsuleCollider>();
    }

    //controle de la marche du personnage, l'attaque le sort, la mort, la boussole
    void Update()
    {

    //si le perso est mort il ne peut plus avancer ou attaquer
    if(sparkAnim.GetBool("mort") == false && !SparkyInventaire.invOpen && !craftOuv)
    {   
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");

        //met le jeu sur pause avec la touche Esc.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(jeuEstPause==false)
            {
                PauseGame();
            }
            else if(jeuEstPause==true)
            {
                ResumeGame();
            }
        }

        //fait avancer le personnage
        if(enMarche==false && (Mathf.Abs(Horizontal) > 0.1f || Mathf.Abs(Vertical) > 0.1f))
        {
            audioSrc.clip = marche;
            audioSrc.loop = true;
            sparkAnim.SetBool("marche", true);
            enMarche=true;
            audioSrc.Play();
        }

        //si le personnage ne marche pas
        if( enMarche==true && Mathf.Abs(Horizontal) == 0.0f && Mathf.Abs(Vertical) == 0.0f) 
        {
            audioSrc.loop = false;
            enMarche=false;
            sparkAnim.SetBool("marche", false);   
            audioSrc.Stop();
        }

        //change la direction du sprite
        if(Horizontal<0&&enMarche&&mauvaiseDirection&&!isCasting)
        {
            mauvaiseDirection=false;
            GetComponent<SpriteRenderer>().flipX=true;
            hitbox.transform.localPosition=new Vector3(-1.7f,0.9f,0); 
            finDeLaMainSpells.transform.localPosition=new Vector3(-1.44f,1.015f,0);    
        }

        else if(Horizontal>0&&enMarche&&!mauvaiseDirection&&!isCasting)
        {
             mauvaiseDirection=true;
             GetComponent<SpriteRenderer>().flipX=false;
             hitbox.transform.localPosition=new Vector3(1.7f,0.9f,0);
             finDeLaMainSpells.transform.localPosition=new Vector3(1.44f,1.015f,0); 
        }

        //si le personnage est mort
        if(PointDeVie.value <= 0)
        {   
            craftingUi.SetActive(false);
            SparkyInventaire.epeeEquip = false;
            colliders.enabled = false;
            sparkAnim.SetLayerWeight(sparkAnim.GetLayerIndex("epee"), 0); 
            GetComponent<Rigidbody>().velocity=Vector3.zero;
            audioSrc.PlayOneShot(sonMort, 0.7f);
            sparkAnim.SetBool("mort", true);
            Invoke("sceneMort",3f);
            ScriptMurs.alpha = 0.2f;
            SparkyInventaire.nbRemains =0;
            SparkyInventaire.nbRoche = 0;
            gameObject.GetComponent<SparkyInventaire>().inventory.Clear();
            gameObject.GetComponent<SparkyInventaire>().equipment.Clear();
            gameObject.GetComponent<SparkyInventaire>().crafting.Clear();
        }

        // animation du coup d'épée avec LMB et si l'épée est équipée
        if(Input.GetMouseButtonDown(0) && SparkyInventaire.epeeEquip == true)
        {
            sparkAnim.SetTrigger("slash");
        }

        //peu lancer un sort si le cooldown est passe
        if(Time.time > tempsNextFire)
        {
            // animation du sort de boule de feu avec RMB et si le sort est équipé
            if(Input.GetMouseButtonDown(1) && SparkyInventaire.fireballEquip == true && !isCasting && Mana.value >0)
            {
                isCasting=true;
                sparkAnim.SetTrigger("fireballcasting");
                Invoke("genereFireball",0.2f);
                Mana.value -= 2;
                tempsNextFire = Time.time + tempsCooldown;
            }
        }

    // activation du parry
        if(Input.GetKeyDown(KeyCode.Space))
        {
            parry.SetActive(true);
            colliders.enabled=false;
            Invoke("desactiverParry", .8f);
        }

        //fait grandir la boussole si on appui du M
        if(Input.GetKeyDown(KeyCode.M)/*&&!boolsol*/)
        {
            //boussole.transform.localScale = new Vector3(2,2,2);
            // boussole.transform.position = offsetBoussole + posBoussoleBase;
            boolsol=!boolsol;
            CancelInvoke("ScaleBoussole");
            Invoke("ScaleBoussole", 5f);
        }

        if(PointDeVie.value != PointDeVie.maxValue || Mana.value != Mana.maxValue)
        {
            aPerduVie = true;
        }

        else
        {
            aPerduVie = false;
        }
    }
}

    
    private void FixedUpdate() 
    {
        if(PointDeVie.value > 0)
        {
            GetComponent<Rigidbody>().velocity = (transform.forward * (Vertical )+transform.right * (Horizontal )).normalized*vitesseMax;
        }

        if(boolsol)
        {
          boussole.transform.localScale = Vector3.Lerp(boussole.transform.localScale,new Vector3(2,2,2),0.1f);
          boussole.transform.position =Vector3.Lerp(boussole.transform.position,offsetBoussole + posBoussoleBase,0.1f);
        }

        if(!boolsol)
        {
            boussole.transform.localScale = Vector3.Lerp(boussole.transform.localScale,scaleBoussoleBase,0.1f);
            boussole.transform.position =Vector3.Lerp(boussole.transform.position, posBoussoleBase,0.1f);
        }
    } 

    //creer la boule de feu
    void genereFireball()
    {   
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit infoCollision;

        if( Physics.Raycast(camRay.origin, camRay.direction, out infoCollision , 5000, LayerMask.GetMask("Plancher")))
        {
            directionFeu = infoCollision.point-transform.position;

            directionFeu = directionFeu.normalized;
            
            if(directionFeu.x>0)
            {
             mauvaiseDirection=true;
             GetComponent<SpriteRenderer>().flipX=false;
             hitbox.transform.localPosition=new Vector3(1.7f,0.9f,0);
             finDeLaMainSpells.transform.localPosition=new Vector3(1.44f,1.015f,0); 
            }
            
            else
            {
                mauvaiseDirection=false;
                GetComponent<SpriteRenderer>().flipX=true;
                hitbox.transform.localPosition=new Vector3(-1.7f,0.9f,0); 
                finDeLaMainSpells.transform.localPosition=new Vector3(-1.44f,1.015f,0);
            }

            directionFeu = infoCollision.point-fireballOriginale.transform.position;
            directionFeu = directionFeu.normalized;

            if(directionFeu.z<0)
            {
                anglefire= Mathf.Acos(directionFeu.x/directionFeu.magnitude)*Mathf.Rad2Deg ;
            }
            
            else
            {
                anglefire= -Mathf.Acos(directionFeu.x/directionFeu.magnitude)*Mathf.Rad2Deg ;
            }

            anglefireSpriteSpecial=Mathf.Abs(anglefire); 
        }
        //clone boule de feu
       Invoke("shootfire",0.2f); 
    }

    //tire la boule de feu
    void shootfire()
    {
        GameObject cloneFireball = Instantiate(fireballOriginale, fireballOriginale.transform.position, Quaternion.Euler(anglefireSpriteSpecial, anglefire, 0));
        cloneFireball.SetActive(true);
        cloneFireball.GetComponent<Rigidbody>().velocity = directionFeu* 30;
        isCasting=false;
    }

    //ferme le collider de l'epee
    void fermeCollider()
    {
        Epeecollider.enabled=false;
    }

    //ouvre le collider de l'epee
    void ouvreCollider()
    {   
        Epeecollider.enabled=true;
    }
  
    //permet de gérer les collisions avec les objets
    private void OnTriggerEnter(Collider autreObjet) 
    {   
        //si le personnage rentre en contact avec le feu de camp, le texte apparait
        if(autreObjet.gameObject.name == "campFireInt")
        {
            if(aPerduVie)
            {
                restoreText.SetActive(true);
            }
           
            if(SparkyInventaire.nbRemains > 0 && !aPerduVie)
            {
                brulerRemains.SetActive(true);
            }

            if(!craftOuv)
            {
                craftMenu.SetActive(true);
            }
        }

        //prend du degat si l'ennemi le touche
        if(autreObjet.gameObject.tag == "enemieBouche" /*&& BadMouthMovingAndAttacking.isAttacking*/)
        {
             sparkAnim.SetTrigger("hurt");
             audioSrc.PlayOneShot(sparkHurt,2.5f);
             PointDeVie.value -= 2;
        }
        
        //prend du degat si le boss le touche
        if(autreObjet.gameObject.tag == "miniboss1" /*&& BadMouthMovingAndAttacking.isAttacking*/)
        {
             sparkAnim.SetTrigger("hurt");
             audioSrc.PlayOneShot(sparkHurt,2.5f);
             PointDeVie.value -= 4;
        }

        if(autreObjet.gameObject.tag == "tenta" /*&& BadMouthMovingAndAttacking.isAttacking*/)
        {
             sparkAnim.SetTrigger("hurt");
             audioSrc.PlayOneShot(sparkHurt,2.5f);
             PointDeVie.value -= 4;
        }


        //disponible dans une version fututre du jeu
        // if(autreObjet.gameObject.tag == "mur")
        // {
        //     if(ScriptMurs.alpha < 1 && SparkyInventaire.nbRoche >0)
        //     {
        //         construire.SetActive(true);
        //     }
        // }
    }

    //permet de gerer les interractions si le perso est dans le collider d'un objet
    private void OnTriggerStay(Collider autreObjet) 
    {    
        //si le perso est proche du feu de camp, il peu restore ces points de vies, le texte disparait
        if(autreObjet.gameObject.name == "campFireInt")
        {   
            if(SparkyInventaire.nbRemains > 0 && !aPerduVie)
            {
                brulerRemains.SetActive(true);
            }

            if(!SparkyInventaire.invOpen && !enMarche)
            {
                craftMenu.SetActive(true);
            }

            if(Input.GetKeyDown(KeyCode.F))
            {
                if(aPerduVie)
                {
                    RestoreVie();
                }
                
                if(SparkyInventaire.nbRemains > 0 && !aPerduVie)
                {   
                    SparkyInventaire.nbRemains -=1;
                    gameObject.GetComponent<SparkyInventaire>().BrulerReste();
                }

                if(SparkyInventaire.nbRemains == 0)
                {
                    brulerRemains.SetActive(false);
                }
                
                if(PointDeVie.value == 10)
                {
                    restoreText.SetActive(false);
                }
            }

            //ouvre le menu de crafting 
            if(Input.GetKeyDown(KeyCode.E))
            {   
                if(!dejaEteOuvert)
                {
                    aideCraft.SetActive(true);
                    dejaEteOuvert = true;
                }
                if (!SparkyInventaire.invOpen && !enMarche)
                {
                    audioSrc.PlayOneShot(gameObject.GetComponent<SparkyInventaire>().ouvreFerme, 0.4f);
                    gameObject.GetComponent<SparkyInventaire>().Show();
                    gameObject.GetComponent<SparkyInventaire>().inventoryUI.transform.position = positionUi + offset;
                    craftingUi.SetActive(true);
                    craftOuv = true;
                    Time.timeScale = 0;
                }
            }

            if(craftOuv)
            {
                craftMenu.SetActive(false);
            }
        }

        //FONCTION DISPONIBLE DANS FUTURS UPDATE DU JEU
        //  if(autreObjet.gameObject.tag == "mur" && Input.GetKeyDown(KeyCode.F))
        // {
        //     if(SparkyInventaire.nbRoche >0 && ScriptMurs.alpha < 1)
        //     {
        //         SparkyInventaire.nbRoche -=1;
        //         gameObject.GetComponent<SparkyInventaire>().ConstruireMur();
        //         ScriptMurs.alpha += 0.2f;
        //     }
        //     else
        //     {
        //         construire.SetActive(false);
        //     }
        // }
    }

    //permet de gerer quand le personnage quitte le collider
    private void OnTriggerExit(Collider autreObjet) 
    {   
        //si le personnage quitte le feu de camp, le texte disparait
        if(autreObjet.gameObject.name == "campFireInt")
        {
            restoreText.SetActive(false);
            brulerRemains.SetActive(false);
            craftMenu.SetActive(false);
        }  

        // if(autreObjet.gameObject.tag =="mur")
        // {
        //     construire.SetActive(false);
        // }
    }

    //ferme le menu de crafting
    public void fermerCraft()
    {
      if(craftOuv)
        {
            audioSrc.PlayOneShot(gameObject.GetComponent<SparkyInventaire>().ouvreFerme, 0.4f);
            gameObject.GetComponent<SparkyInventaire>().Hide();
            craftingUi.SetActive(false);
            gameObject.GetComponent<SparkyInventaire>().inventoryUI.transform.position = positionUi;
            craftOuv = false;
            Time.timeScale = 1;
        }
    }

    //redonne 10 points de vie au perso si il en a moins que 10
    public void RestoreVie()
    {
        audioSrc.PlayOneShot(heal);
        if(PointDeVie.value < 10)
        {
            PointDeVie.value = 10;
        }

        if(Mana.value < 10)
        {
            Mana.value = 10;
        }
    }

    //désactiver le parry
    public void desactiverParry()
    {
        colliders.enabled=true;
        parry.SetActive(false);
    }

    //load la scene
    public void sceneMort()
    {
        SceneManager.LoadScene(4);
    }

    
    public void ScaleBoussole()
    { 
        boolsol=false;
    }

    //met le jeu sur pause et active le menu
    void PauseGame()
    {
        jeuEstPause = true;
        Time.timeScale = 0;
        menuPauseUi.SetActive(true);
    }

    //redémarre le jeu et ferme le menu
    public void ResumeGame()
    {
        jeuEstPause = false;
        Time.timeScale = 1;
        menuPauseUi.SetActive(false);
    }
}



