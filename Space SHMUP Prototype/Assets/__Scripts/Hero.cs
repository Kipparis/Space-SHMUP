using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    public static Hero S;

    public float gameRestartDelay = 2f;

    // Поля контролящи движение
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    // Инфа о корабле
    [SerializeField]
    private float _shieldLevel = 1;

    public Weapon[] weapons;

    public bool ____________________;

    public Bounds bounds;

    // Создаём делегативный тип WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // Создаём у этого типа поле с именем fireDelegate
    public WeaponFireDelegate fireDelegate;

    void Awake() {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }

    void Start() {
        // Возвращает оружие чтобы герой начал с 1 бластером
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    // Update is called once per frame
    void Update() {
        // Извлекаем инфу из ввода
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Изменяем позицию в зависимости от ввода
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;   // Можно в самом конце приравнять, после всех махинаций

        bounds.center = transform.position;
        // Держим корабль на экране
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
        if (off != Vector3.zero) {
            pos -= off;
            transform.position = pos;
        }

        // Поворачиваем корабль чтобы выглядело покруче
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        // Используем fireDelegate чтобы выстрелить, убеждаемся что 
        // ось "прыжок" нажата
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
            fireDelegate();
        }
	}

    // Последний тригер 
    public GameObject lastTriggerGo = null;

    void OnTriggerEnter(Collider other) {
        GameObject go = Utils.FindTaggedParent(other.gameObject);
        if (go != null) {
            if (go == lastTriggerGo) {
                return;
            }
            lastTriggerGo = go;
            if (go.tag == "Enemy") {
                // Если мы ударились врагом уменьшаем уровень щита
                shieldLevel--;
                // Уничтожаем врага
                Destroy(go);
            } else if (go.tag == "PowerUp"){
                AbsorbPowerUp(go);
            } else {
                print("Triggered: " + go.name);
            }
        } else {
            print("Triggered: " + other.gameObject.name);
        }
    }

    public void AbsorbPowerUp(GameObject go) {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type) {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                // Это какой то из типо оружия
                if (pu.type == weapons[0].type) {
                    // Увеличиваем количество оружий этого типа
                    Weapon w = GetEmptyWeaponSlot();    // Находим доступный слот
                    if (w != null) {
                        w.SetType(pu.type);
                    }
                } else {
                    // Если это другое оружие
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot() {
        for (int i = 0; i < weapons.Length; i++) {
            if(weapons[i].type == WeaponType.none) {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons() {
        foreach(Weapon w in weapons) {
            w.SetType(WeaponType.none);
        }
    }

    public float shieldLevel {
        get { return (_shieldLevel); }
        set {
            _shieldLevel = Mathf.Min(value, 4);
            // Если щит опускается ниже нуля
            if (value < 0) {
                Destroy(this.gameObject);
                // Говорим мэйну перезапустить игру после задержки
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
