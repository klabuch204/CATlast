using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : Character
{
    private const float CHECK_GROUND_RAY_MAX_DISTANCE = 0.7f;
    private const float GROUND_RADIUS = 0.2f;
    private Vector3 bottom = new Vector3(0, -1, 0);
    private Vector3 minusX = new Vector3(-1, 1, 1);

    public float speed = 4.0f;

    private GameObject groundCheck;
    private GameObject raycastStartPoint;
    private bool isGrounded = false;
    private bool lastActionWasWaiting = false;



    public void Start()
    {
        groundCheck = transform.Find("GroundCheck").gameObject;
        raycastStartPoint = transform.Find("RaycastStartPoint").gameObject;
        DoSomethingNew();
        InvokeRepeating("CheckMessage", 0, 1);
    }

    public void CheckMessage()
    {
        Debug.Log("See!");
    }

    public void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, GROUND_RADIUS);
        animator.SetBool("isGrounded", isGrounded);        
    }

    public override void Die()
    {
        animator.SetBool("isDying", true);
        animator.Play("die");
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }   

    /// <summary>
    /// Объект бесцельно идёт ВПЕРЁД time секунд
    /// </summary>
    /// <param name="time">Время(сек.), указывающее сколько объект будет ходить.</param>
    /// <returns></returns>
    IEnumerator Walk(float time)
    {
        lastActionWasWaiting = false;
        animator.SetFloat("Speed", speed);
        float direction;
        while (time > 0)
        {
            if (isFacedRight)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            rigidbody.velocity = new Vector2(direction * speed, rigidbody.velocity.y);
            if (!Physics2D.Raycast(raycastStartPoint.transform.position, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
            {
                StopAllCoroutines();
                AnimationStop();
                StartCoroutine(Wait(Random.Range(1.0f, 2.5f)));
            }
            time -= Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
        AnimationStop();
        DoSomethingNew();
    }

    /// <summary>
    /// Объект стоит time секунд на месте.
    /// </summary>
    /// <param name="time">Время(сек.), указывающее сколько объект будет стоять.</param>
    /// <returns></returns>
    IEnumerator Wait(float time)
    {
        lastActionWasWaiting = true;
        animator.SetFloat("Speed", 0);
        rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        yield return new WaitForSeconds(time);
        DoSomethingNew();
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

    private void Jump()
    {
        AnimationStop();
        rigidbody.AddForce(new Vector2(0, 600));
    }

    /// <summary>
    /// Разворачивает объект
    /// </summary>
    private void Flip()
    {
        isFacedRight = !isFacedRight;
        transform.localScale = Vector3.Scale(transform.localScale, minusX);
    }

    private void AnimationStop()
    {
        animator.SetFloat("Speed", 0);
        animator.Play("idle");
    }

    /// <summary>
    /// Генерирует новое действие для объекта.
    /// </summary>
    private void DoSomethingNew()
    {
        var random = Random.Range(1.0f, 3.0f);
        if (!lastActionWasWaiting)
        {
            StartCoroutine(Wait(random));
            return;
        }
        // Если перед ним есть поверхность
        if (Physics2D.Raycast(raycastStartPoint.transform.position, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
        {
            WalkToRandomWay(random);
        }
        // Если перед ним нет поверхности
        else
        {
            StopAllCoroutines();
            Flip();
            StartCoroutine(Walk(random));
        }
    }

    private void WalkToRandomWay(float time)
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                StartCoroutine(Walk(time));
                break;
            case 1:
                Flip();
                StartCoroutine(Walk(time));
                break;
        }
    }

    public override void RecieveDamage(float damage)
    {
        StopAllCoroutines();
        HP -= damage;
        if (HP < float.Epsilon)
        {
            HP = 0;
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
            Flip();
        }
        // VVV Тестовый код! VVV
        else if (collision.gameObject.tag == "Player")
        {
            Jump();
        }
        // ^^^ Тестовый код! ^^^
    }

    // VVV Тестовый код! VVV
    public void OnMouseDown()
    {
        RecieveDamage(25.0f);
    }
    // ^^^ Тестовый код! ^^^

}

///// <summary>
///// Объект бесцельно идёт ВПРАВО time секунд
///// </summary>
///// <param name="time">Время(сек.), указывающее сколько объект будет ходить.</param>
///// <returns></returns>
//IEnumerator WalkRight(float time)
//{
//    animator.SetFloat("Speed", speed);
//    if (!isFacedRight) Flip();
//    while (time > 0)
//    {
//        rigidbody.velocity = new Vector2(1 * speed, rigidbody.velocity.y);
//        if (!Physics2D.Raycast(raycastStartPoint, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
//        {
//            StopAllCoroutines();
//            MomentalAnimationStop();
//            StartCoroutine(Wait(Random.Range(1.0f, 2.5f)));
//        }
//        time -= Time.deltaTime;
//        yield return new WaitForSeconds(0.1f);
//    }
//    MomentalAnimationStop();
//    DoSomethingNew(false);
//}

///// <summary>
///// Объект бесцельно идёт ВЛЕВО time секунд
///// </summary>
///// <param name="time">Время(сек.), указывающее сколько объект будет ходить.</param>
///// <returns></returns>
//IEnumerator WalkLeft(float time)
//{
//    animator.SetFloat("Speed", speed);
//    if (isFacedRight) Flip();
//    while (time > 0)
//    {
//        rigidbody.velocity = new Vector2(-1 * speed, rigidbody.velocity.y);
//        if (!Physics2D.Raycast(raycastStartPoint, bottom, CHECK_GROUND_RAY_MAX_DISTANCE))
//        {
//            StopAllCoroutines();
//            MomentalAnimationStop();
//            StartCoroutine(Wait(Random.Range(1.0f, 2.5f)));
//        }
//        time -= Time.deltaTime;
//        yield return new WaitForSeconds(0.1f);
//    }
//    MomentalAnimationStop();
//    DoSomethingNew(false);
//}
