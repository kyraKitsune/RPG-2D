using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    private float playerDetectTime;
    public float playerDetectRate = 0.2f;
    public float chaseRange;
    bool lookRight;
    protected int currentHealth;
    public int maxHealth;

    [Header("Attack")]
    public float attackRange;
    public float attackRate;
    protected float lastAttackTime;
    public int damage;

    [Header("Component")]
    protected Rigidbody2D rb;
    private PlayerController targetPlayer;
    protected Animator anim;
    public GameObject healthBar;
    public Image life;

    [Header("Pathfinding")]
    public float nextWayPointDistance = 2f;
    protected Path path;
    int currentWayPoint = 0;
    bool reachEndPath = false;
    protected Seeker seeker;
    protected float dist;

    public delegate void myAttack(); // Creer une nouvelle methode qui n'existe toujours pas.
    public myAttack myMethod; 

    private void Update()
    {
        Flip();
    }

    public void InitializeCharacter(myAttack _myMethod)
    {
        this.currentHealth = this.maxHealth;
        life.fillAmount = this.maxHealth; // fillAmount c'est la barre de vie qui peut baisser
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        InvokeRepeating("UpdatePath", 0f, .5f); // Permet d'invoquer notre methode updatepath et la repeter toutes les .5sec
        myMethod = _myMethod;
    }

    public void UpdateHealthBar(int value)
    {
        life.fillAmount = (float)value / maxHealth;
    }

    void OnPathComplete(Path p) // Lorsqu'on aura compléter notre chemin
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0; // Reset le chemin
        }

    }

    private void FixedUpdate()
    {
        ReachPlayer(myMethod);
    }

    public void ReachPlayer(myAttack Attack)
    {
        if (targetPlayer != null)
        {
            // Notre float distance sera egale entre notre transform position à notre target player à Nous
            dist = Vector2.Distance(transform.position, targetPlayer.transform.position);
            if (dist < attackRange && Time.time - lastAttackTime >= attackRate) // Time.time = temps depuis que le jeu a commencer
            {
                Attack();
            }
            else if (dist > attackRange)
            {
                if (path == null) // et que path est égal à nul on fait retour
                    return;

                if (currentWayPoint >= path.vectorPath.Count) // Si notre current way point est superieur ou egal à la liste de nombre chemin possible
                {
                    reachEndPath = true;
                    return;
                }
                else
                {
                    reachEndPath = false;
                }
                // Direction = vector path le plus proche de moi - notre position et on normalise tout ça
                Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
                // On multiplie par notre speed, notre RB.velocity = force appliquée  par notre direction et c'est ça qui fait avancer l'ennemi vers le vector path
                Vector2 force = direction * speed * Time.fixedDeltaTime;

                rb.velocity = force;

                // La distance sera le Rb.position de l'ennemi au prochain waypoint
                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

                if (distance < nextWayPointDistance)
                {
                    currentWayPoint++;
                }
            }
            else // Sinon si la distance est supérieure à l'attack range, l'ennemi ne bougera pas
            {
                rb.velocity = Vector2.zero;
            }
        }
        DetectPlayer();
    }
    private void DetectPlayer()
    {
        if (Time.time - playerDetectTime > playerDetectRate) // Calcule la distance entre nous et le joueur
        {
            playerDetectRate = Time.time;
            // Boucler sur tous les PlayerController du jeu
            foreach (PlayerController player in FindObjectsOfType<PlayerController>())
            {
                if (player != null)
                { // Créer une nouvelle variable qui va chercher la distance entre position ennemi et distance
                    float dist = Vector2.Distance(transform.position, player.transform.position);

                    if (player == targetPlayer)
                    {
                        if (dist > chaseRange)
                        {
                            targetPlayer = null;
                            rb.velocity = Vector2.zero; // L'ennemi s'arretera net quand notre personnage sortira de la ChaseRange
                            anim.SetBool("onMove", false); // Si nous sommes plus loin que lorsque notre chase le permet alors false, (dans l'animator)
                        }
                    }
                    else if (dist < chaseRange)
                    {
                        if (targetPlayer == null)
                            targetPlayer = player;
                        anim.SetBool("onMove", true);
                    }
                }
            }
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && targetPlayer != null) // Si notre chemin a été calculer par le seeker
        {
            seeker.StartPath(rb.position, targetPlayer.transform.position, OnPathComplete); // Notre ennemi commence le chemin à notre Player, une fois fait on appelle la methode 
            //OnPathComplete
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Animator anim;
        anim = GetComponent<Animator>();
        anim.SetTrigger("hit");
        UpdateHealthBar(currentHealth);
        healthBar.SetActive(true); // Permet d'activer la healthBar une fois que l'ennemi est touché
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    void Flip()
    {
        if (rb.velocity.x > 0 && lookRight || rb.velocity.x < 0 && !lookRight)
        {
            lookRight = !lookRight; // Egal à l'inverse de se qu'il est 

            transform.Rotate(0, 180f, 0); // Fait faire une rotation a l'ennemi pour qu'il soit dans l'autre sens quand il attaque à gauche
        }
    }
}

