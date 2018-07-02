using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    none,   // Обычный / нет оружия
    blaster,    // Просто бластер
    spread, // 2 выстрела одновременно
    phaser, // Выстрелы что идут волной
    missile,    // Самонаводящиеся ракеты
    laser,  // Урон по времени
    shield  // Повышает уровень щита
}

// WeaponDefinition Позволяет описать разные свойства у оружия
// в инспекторе
[System.Serializable]
public class WeaponDefinition {
    public WeaponType type = WeaponType.none;
    public string letter;   // Буква которая покажется на усилении
    public Color color = Color.white;   // Цвет оружия и улучшения
    public GameObject projectilePrefab; // Модель для снаряда
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;   // Количество урона
    public float continuousDamage = 0;  // Для лазера
    public float delayBetweenShots = 0;
    public float velocity = 20; // Скорость снарядов
}

public class Weapon : MonoBehaviour {
    static public Transform PROJECTILE_ANCHOR;

    public bool ________________;
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShot;  // Последний раз когда выстрелил

    void Start() {
        collar = transform.Find("Collar").gameObject;
        // Вызываем SetType() для базового значения
        SetType(_type);

        if(PROJECTILE_ANCHOR == null) {
            GameObject go = new GameObject("_Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // Находим родительский fireDelegate
        GameObject parentGO = Utils.FindTaggedParent(this.gameObject);
        if(parentGO.tag == "Hero") {
            Hero.S.fireDelegate += Fire;
        }
    }
	
    public WeaponType type {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt) {
        _type = wt;
        if(type == WeaponType.none) {
            this.gameObject.SetActive(false);
            return;
        } else {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(wt);
        collar.GetComponent<Renderer>().material.color = def.color;
        lastShot = 0;   // Всегда возможно стрелять сразу же, как только тип приравнян
    }

    public void Fire() {
        // Если объект неактивный, возвращаем
        if (!gameObject.activeInHierarchy) return;
        // Если не прошло достаточно времени от выстрела, возвращаем
        if(Time.time - lastShot < def.delayBetweenShots) {
            return;
        }
        Projectile p;
        switch (type) {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = Vector3.up * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(-.2f, 0.9f, 0) * def.velocity;
                p = MakeProjectile();
                p.GetComponent<Rigidbody>().velocity = new Vector3(.2f, 0.9f, 0) * def.velocity;
                break;
        }
    }

    public Projectile MakeProjectile() {
        GameObject go = Instantiate(def.projectilePrefab) as GameObject;
        if(Utils.FindTaggedParent(this.gameObject).tag == "Hero") {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        } else {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.parent = PROJECTILE_ANCHOR;
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShot = Time.time;
        return (p);
    }
}
