using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy {
    // Так как это наследственный класс bool ______; будет работать неправильно

    // # секунд для полной волны
    public float waveFrequency = 2; // Период
    // Длинна волны в метрах
    public float waveWidth = 4; // Амплитуда лол
    public float waveRotY = 25;

    private float x0 = -12345;  // Начальное значение x
    private float birthTime;

    void Start() {
        // Начальная позиция уже задана основным классом
        // в Awake()
        x0 = pos.x;

        birthTime = Time.time;
    }

    public override void Move() {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // Немного вращаем вокруг y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // Стандартный мув всё ещё удерживает движение по y
        base.Move();
    }
}
