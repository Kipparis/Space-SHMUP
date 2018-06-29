using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    public static Hero S;

    // Поля контролящи движение
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    // Инфа о корабле
    public float shieldLevel = 1;

    public bool ____________________;

    public Bounds bounds;

    void Awake() {
        S = this;
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        // Извлекаем инфу из ввода
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Изменяем позицию в зависимости от ввода
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Поворачиваем корабль чтобы выглядело покруче
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
	}
}
