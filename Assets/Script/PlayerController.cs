using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Component")]
    Rigidbody2D rb;
    Animator anim;

    [Header("Stat")]
    [SerializeField] //Pour empêcher un autre script de prendre cela
    float moveSpeed;
    public int currentHealth;
    public int maxHealth;

    [Header("Attack")]
    private float attackTime;
    [SerializeField] float timeBetweenAttack;
    public bool canMove = true, canAttack = true;
    [SerializeField] Transform checkEnemy;
    public LayerMask whatIsEnemy;
    public float range;
    public int damage = 1;

    public static PlayerController instance; // Recuperer toutes les infos du Player

    private void Awake()
    {
        instance = this; // Cette fonction est instance
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(canAttack)
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0)) // Lorsqu'on attaque
        {
            if (Time.time >= attackTime)
            {
                rb.velocity = Vector2.zero; // Notre velocite sera a zero
                anim.SetTrigger("attack"); // Nous faisons un anim.. attack pour recuperer dans l'animator l'animation

                StartCoroutine(Delay());
                IEnumerator Delay() // Pour mettre un temps d'attente avant de rebouger
                {
                    canMove = false;
                    yield return new WaitForSeconds(.5f);
                    canMove = true;
                }
                attackTime = Time.time + timeBetweenAttack;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canMove) 
        Move();
    }

    void Move()
    {
        if(Input.GetAxis("Horizontal") > 0.1) // si mon axe horizontal part vers la gauche
        {
            anim.SetFloat("lastInputX", 1); // On récupere le lastInputX dans l'animator et je lui mets mon GetAxisRow à l'horizontal.
            anim.SetFloat("lastInputY", 0);
        }
        else if (Input.GetAxis("Horizontal") < -0.1)
        {
            anim.SetFloat("lastInputX", -1);
            anim.SetFloat("lastInputY", 0);
        }

        if (Input.GetAxis("Vertical") > 0.1) // Ou si mon axe horizontal part vers la gauche
        {
            anim.SetFloat("lastInputX", 0); // On récupere le lastInputX dans l'animator et je lui mets mon GetAxisRow à l'horizontal.
            anim.SetFloat("lastInputY", 1);
        }
        else if (Input.GetAxis("Vertical") < -0.1)
        {
            anim.SetFloat("lastInputX", 0);
            anim.SetFloat("lastInputY", -1);
        }

        if (Input.GetAxis("Horizontal") > 0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x + range, transform.position.y, 0);
        } 
        else if (Input.GetAxis("Horizontal") < -0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x - range, transform.position.y, 0);
        }

        if (Input.GetAxis("Vertical") > 0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x, transform.position.y +range, 0);
        }
        else if (Input.GetAxis("Vertical") < -0.1)
        {
            checkEnemy.position = new Vector3(transform.position.x, transform.position.y -range, 0);
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(x, y) * moveSpeed * Time.fixedDeltaTime; //.normalized; // normalized fait retourner le vector par 1 ou -1 si le player se déplace

        rb.velocity.Normalize(); // On dit que notre vélocité est normalisée, pour aller plus vite

        if( x != 0 || y != 0) // Pour l'animation de marche
        {
            anim.SetFloat("inputX", x);
            anim.SetFloat("inputY", y);
        }
    }

    public void OnAttack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(checkEnemy.position, 0.5f, whatIsEnemy);

        foreach (var enemys in enemy) // Dégats à l'ennemi
        {
            enemys.GetComponent<EnemyFly>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
