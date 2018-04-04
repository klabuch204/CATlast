using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Шаблонный класс любого игрового персонажа/врага/союзника, который может сражаться или быть раненым.
/// </summary>
public class Character : MonoBehaviour
{
    // Указывает, повернут ли объект вправо
    protected bool isFacedRight;
    // Параметр здоровья.
    public float HP = 100.0f;
    // Компонент аниматора для данного объекта.
    protected Animator animator;    
    
    // Тело объекта
    protected new Rigidbody2D rigidbody;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacedRight = GetComponent<Transform>().localScale.x > 0;
    }

    /// <summary>
    /// Нанесение данному персонажу урона.
    /// </summary>
    /// <param name="damage">Урон.</param>
    public virtual void RecieveDamage(float damage)
    {
        HP -= damage;
        if (HP < 0.0f) Die();
    }

    /// <summary>
    /// Вызывается при смерти данного объекта. 
    /// Предполагается исользование для проигрывания анимации смерти и последующим удалением объекта.
    /// </summary>
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Лечит данный объект.
    /// </summary>
    /// <param name="hp">Здоровье, которое предполагается восстановить.</param>
    public virtual void Heal(float hp)
    {
        HP += hp;
    }
}

