using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speed = 10f;   // m/s
    public float fireRate = 0.3f;   // Seconds/shot
    public float health = 10;
    public int score = 100; // Сколько очков за разрушение

    public int showDamageForFrames = 2; // Кадров чтобы показать урон

    public bool _________________;

    public Color[] originalColors;
    public Material[] materials;    // Все материалы
    public int remainingDamageFrames = 0;   // Осталось кадров до показа урона

    public Bounds bounds;   // Границы его и его детей
    public Vector3 boundsCenterOffset;  // Чё

    void Awake() {
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++) {
            originalColors[i] = materials[i].color;
        }
        InvokeRepeating("CheckOffscreen", 0f, 2f);
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        if(remainingDamageFrames > 0) {
            remainingDamageFrames--;
            if(remainingDamageFrames == 0) {
                UnShowDamage();
            }
        }
	}

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    public Vector3 pos {
        get { return (this.transform.position); }
        set { this.transform.position = value; }
    }

    void CheckOffscreen() {
        // Если границы ещё не сделанны
        if (bounds.size == Vector3.zero) {
            bounds = Utils.CombineBoundsOfChildren(this.gameObject);
            // Находим разницу между центром границы и позицией
            boundsCenterOffset = bounds.center - transform.position;
        }

        // Всё время поддерживаем границы
        bounds.center = transform.position + boundsCenterOffset;
        // Проверяем всё время вышли ли полностью границы
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen);
        if (off != Vector3.zero) {
            // Если враг ушёл из нижнего края
            if (off.y < 0) {
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        GameObject other = coll.gameObject;
        switch (other.tag) {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Враги не получают урон пока они не на экране
                bounds.center = transform.position + boundsCenterOffset;
                if(bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
                    Destroy(other);
                    break;
                }
                // Наносим урон врагу
                ShowDamage();
                health -= Main.W_DEFS[p.type].damageOnHit;
                if(health <= 0) {
                    // Уничтожаем врага
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
        }
    }

    void ShowDamage() {
        foreach (Material mat in materials) {
            mat.color = Color.red;
        }
        remainingDamageFrames = showDamageForFrames;
    }

    void UnShowDamage() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = originalColors[i];
        }
    }
}
