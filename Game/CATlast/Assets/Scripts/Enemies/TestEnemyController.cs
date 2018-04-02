using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : Character
{
    private const float CHECK_GROUND_RAY_MAX_DISTANCE = 0.7f;
    private const float GROUND_RADIUS = 0.2f;
    private Vector3 bottom = new Vector3(0, -1, 0);

    public float speed = 4.0f;
    public Vector3 groundCheck;
    public Vector3 raycastStartPoint;

    private bool isGrounded = false;



    public void Start()
    {
        raycastStartPoint = transform.Find("RaycastStartPoint").position;
        DoSomethingNew(false);
    }

    public void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck, GROUND_RADIUS);
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
            if (!Physics2D.Raycast(raycastStartPoint, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
            {
                StopAllCoroutines();
                MomentalAnimationStop();
                StartCoroutine(Wait(Random.Range(1.0f, 2.5f)));
            }
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        MomentalAnimationStop();
        DoSomethingNew(false);
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
            if (!Physics2D.Raycast(raycastStartPoint, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
            {
                StopAllCoroutines();
                MomentalAnimationStop();
                StartCoroutine(Wait(Random.Range(1.0f, 2.5f)));
            }
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        MomentalAnimationStop();
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
        Wait(0.7f);
    }

    /// <summary>
    /// Разворачивает объект
    /// </summary>
    private void Flip()
    {
        isFacedRight = !isFacedRight;
        transform.localScale.Scale(new Vector3(1,-1,1));
    }

    private void MomentalAnimationStop()
    {
        animator.SetFloat("Speed", 0);
        animator.Play("idle");
    }

    /// <summary>
    /// Генерирует новое действие для объекта.
    /// </summary>
    private void DoSomethingNew(bool isCalledFromWait)
    {
        var random = Random.Range(0.5f, 3.5f);
        if (isCalledFromWait & isGrounded)
        {            
            if (Physics2D.Raycast(raycastStartPoint, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
            {
                WalkToRandomWay(random);
            }
            else
            {
                WalkToOppositeSide(random);
            }
        }
        else
        {
            StartCoroutine(Wait(random));
        }
    }

    private void WalkToRandomWay(float time)
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                StartCoroutine(WalkLeft(time));
                break;
            case 1:
                StartCoroutine(WalkRight(time));
                break;
        }
    }

    private void WalkToOppositeSide(float time)
    {
        StopAllCoroutines();
        if (isFacedRight)
        {
            StartCoroutine(WalkLeft(time));
        }
        else
        {
            StartCoroutine(WalkRight(time));
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
        if (collision.gameObject.tag == "Wall")
        {
            WalkToOppositeSide(Random.Range(0.5f, 1.5f));
        }
    }
}
