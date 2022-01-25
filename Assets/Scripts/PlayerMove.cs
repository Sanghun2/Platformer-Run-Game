using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>() ;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up* jumpPower,ForceMode2D.Impulse);
        }
        //stop speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y );
        }
        //direction sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        //animation
        if (Mathf.Abs(rigid.velocity.normalized.x) == 0)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true) ;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right*h, ForceMode2D.Impulse);
        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed*(-1))
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }

        //landing platform 
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.7f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnAttack(Transform enemy)
    {
        //point
        rigid.AddForce(Vector2.up*10, ForceMode2D.Impulse);
        //enemy die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1,1,1,0.4f);

        //reaction force
        rigid.velocity = Vector2.zero;
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("damaged");
        Invoke("OffDamaged", 3f);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,1);
    }
}
