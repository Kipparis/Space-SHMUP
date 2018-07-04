using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    // Три поля должны быть определенны в инспекторе
    public string name; // Имя этой части
    public float health;    // Количество здоровья у этой части
    public string[] protectedBy;    // Другие части которые её защищают

    // Два поля которые задаются автоматом в Start()
    public GameObject go;   // ио этой части
    public Material mat;    // Материал чтоб показать урон
}

public class Enemy_4 : Enemy {
    // Начинает вне экрана, затем выбирает рандомную точку на экране и движется к ней
    // Когда достигнет её, выбирает новую

    public Vector3[] points;  // Записывает р0 и р1 для перехода
    public float timeStart; // Время рождения
    public float duration = 4;  // Время для передвижения

    public Part[] parts;    // Массив частей корабля

    // Use this for initialization
    void Start() {
        points = new Vector3[2];

        // Позиция уже задана Main.SpawnEnemy()
        points[0] = pos;
        points[1] = pos;

        InitMovement();

        // Находим игровой объект и материал каждой части
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    private void InitMovement()
    {
        // Выбираем рандомную точку на экране чтобы двигаться к ней
        Vector3 p1 = Vector3.zero;
        float esp = Main.S.enemySpawnPadding;
        Bounds cBounds = Utils.camBounds;
        p1.x = UnityEngine.Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
        p1.y = UnityEngine.Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);

        points[0] = points[1];  // Смещаем массив
        points[1] = p1; // Добавляем точку

        // Перезагружаем время
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;
        if (u >= 1)
        {
            InitMovement(); // Создаём движение к новой точке
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);    // Смягчение

        pos = (1 - u) * points[0] + u * points[1];
    }

    // Это перезапишет OnCollisionEnter в Enemy
    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Враги не получают урон пока они не на экране
                bounds.center = transform.position + boundsCenterOffset;
                if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero)
                {
                    Destroy(other);
                    break;
                }

                // Наносим урон врагу
                // Находим часть с которой был произведёт контакт
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    // Если не найдено, это может быть потому что в коллайдере
                    // будет снаряд героя, а не этот корабль
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Проверем защищена ли эта часть
                if (prtHit.protectedBy != null) {
                    foreach (string s in prtHit.protectedBy)
                    {
                        // Если хотя бы одна из частей не была разрушена
                        if (!Destroyed(s))
                        {
                            // Тогда пока не наносим этой части урона
                            Destroy(other);
                            return;
                        }
                    }
                }
                // Если не защищена, наносим ей урон
                prtHit.health -= Main.W_DEFS[p.type].damageOnHit;
                // Показываем урон на этой части
                ShowLocalizedDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    // Вместо того чтобы разрушать врага, убираем часть
                    prtHit.go.SetActive(false);
                }
                // Проверяем разрушен ли весь корабль
                bool allDestroyed = true;   // Предположим он разрушен
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))    // Если не разрушена хотя бы одна часть
                    {
                        allDestroyed = false;   // Корабль не разрушен
                        break;
                    }
                }
                if (allDestroyed)
                {   // Если корабль полностью разрушен
                    // говорим мэйн скрипту что корабль разрушен
                    Main.S.ShipDestroyed(this);
                    // Разрушаем врага
                    Destroy(this.gameObject);
                }
                Destroy(other); //Уничтожаем снаряд
                break;
        }
    }

    // Эти две функции находят часть по имени или ио
    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if (prt.name == n)
            {
                return(prt);
            }
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if (go == prt.go)
            {
                return (prt);
            }
        }
        return (null);
    }


    // Функция возвращает истину если часть разрушена
    bool Destroyed(GameObject gameObject)
    {
        return (Destroyed(FindPart(gameObject)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part part)
    {
        if(part == null)
        {   // Если части не существует
            return (true);  // разрушена
        }
        // Если здоровья ниже или равно нулю, часть разрушена
        return (part.health <= 0);
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        remainingDamageFrames = showDamageForFrames;
    }
}
