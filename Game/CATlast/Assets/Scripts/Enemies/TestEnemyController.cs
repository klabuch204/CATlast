using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : Character
{
    public float speed = 4.0f;
    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded = false;
    public Transform raycastStartPoint;

    public void Start()
    {
        raycastStartPoint = transform.Find("RaycastStartPoint");
        DoSomethingNew(false);
    }

    public void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
    }

    public override void Die()
    {
        animator.Play("die");
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// Объект бесцельно идёт ВЛЕВО time секунд
    /// </summary>
    /// <param name="time">Время, указывающее сколько объект будет ходить.</param>
    /// <returns></returns>
    IEnumerator WalkLeft(float time)
    {
        animator.SetFloat("Speed", speed);
        if (isFacedRight) Flip();
        while (time > 0)
        {
            rigidbody.velocity = new Vector2(-1 * speed, rigidbody.velocity.y);
            if (!Physics2D.Raycast(raycastStartPoint.position, new Vector3(0, -1), 0.7f))
            {
                StopAllCoroutines();
                MomentalStop();
                StartCoroutine("Wait", Random.Range(1.0f, 2.5f));
            }
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        MomentalStop();
        DoSomethingNew(false);
    }

    private void MomentalStop()
    {
        animator.SetFloat("Speed", 0);
        animator.Play("idle");
    }

    /// <summary>
    /// Объект бесцельно идёт ВПРАВО time секунд
    /// </summary>
    /// <param name="time">Время, указывающее сколько объект будет ходить.</param>
    /// <returns></returns>
    IEnumerator WalkRight(float time)
    {
        animator.SetFloat("Speed", speed);
        if (!isFacedRight) Flip();
        while (time > 0)
        {
            rigidbody.velocity = new Vector2(1 * speed, rigidbody.velocity.y);
            if (!Physics2D.Raycast(raycastStartPoint.position, new Vector3(0, -1), 0.7f))
            {
                StopAllCoroutines();
                MomentalStop();
                StartCoroutine("Wait", Random.Range(1.0f, 2.5f));
            }
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        MomentalStop();
        DoSomethingNew(false);
    }

    /// <summary>
    /// Объект стоит time секунд на месте.
    /// </summary>
    /// <param name="time">Время, указывающее сколько объект будет стоять.</param>
    /// <returns></returns>
    IEnumerator Wait(float time)
    {
        animator.SetFloat("Speed", 0);
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        yield return new WaitForSeconds(time);
        DoSomethingNew(true);
    }

    /// <summary>
    /// Объект проигрывает анимацию получения урона
    /// </summary>
    /// <returns></returns>
    IEnumerator Hit()
    {
        animator.SetFloat("Speed", 0);
        animator.Play("hit");
        yield return new WaitForSeconds(0.07f);
        Wait(1.5f);
    }

    /// <summary>
    /// Разворачивает объект
    /// </summary>
    private void Flip()
    {
        isFacedRight = !isFacedRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    /// Генерирует новое действие для объекта.
    /// </summary>
    private void DoSomethingNew(bool isCalledFromWait)
    {
        if (isCalledFromWait & isGrounded)
        {
            if (!Physics2D.Raycast(raycastStartPoint.position, new Vector3(0, -1), 0.7f))
            {
                if (isFacedRight)
                {
                    StartCoroutine("WalkLeft", Random.Range(0.5f, 2.0f));
                }
                else
                {
                    StartCoroutine("WalkRight", Random.Range(0.5f, 2.0f));
                }
            }
            else
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        StartCoroutine("WalkLeft", Random.Range(0.5f, 3.5f));
                        break;
                    case 1:
                        StartCoroutine("WalkRight", Random.Range(0.5f, 3.5f));
                        break;
                }
            }
        }
        else
        {
            StartCoroutine("Wait", Random.Range(2.0f, 4.5f));
        }
    }



    public override void RecieveDamage(float damage)
    {
        StopAllCoroutines();
        HP -= damage;
        if (HP < float.Epsilon)
        {
            Die();
        }
        else
        {
            StartCoroutine("Hit");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("RecieveDamage", 10.0f);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            if (isFacedRight)
            {
                StopAllCoroutines();
                StartCoroutine("WalkLeft", Random.Range(0.5f, 1.0f));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine("WalkRight", Random.Range(0.5f, 1.0f));
            }
        }
    }
}
