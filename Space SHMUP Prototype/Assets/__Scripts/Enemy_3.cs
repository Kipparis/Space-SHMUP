using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy {
    // Бизеровая кривая для более чем двух точек
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;

    // Start() работает хорошо, потому что его нет в Enemy
    void Start() {
        points = new Vector3[3];    // Объявляем точки

        // Начальная позиция уже задана Main.SpawnEnemy()
        points[0] = pos;

        float xMin = Utils.camBounds.min.x + Main.S.enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - Main.S.enemySpawnPadding;

        Vector3 v;
        // Выбираем рандомную точку в нижней части экрана
        v = Vector3.zero;
        v.x = Random.Range(xMin, xMax);
        v.y = Random.Range(Utils.camBounds.min.y, 0);
        points[1] = v;

        // Выбираем конечную позицию над верхом экрана
        v = Vector3.zero;
        v.y = pos.y;
        v.x = Random.Range(xMin, xMax);
        points[2] = v;

        // Задаём время рождения
        birthTime = Time.time;
    }

    public override void Move() {
        float u = (Time.time - birthTime) / lifeTime;

        if(u > 1) {
            Destroy(this.gameObject);
            return;
        }

        // Варьируем между тремя значениями
        Vector3 p01, p12;
        u = u - 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        p01 = (1 - u) * points[0] + u * points[1];
        p12 = (1 - u) * points[1] + u * points[2];
        pos = (1 - u) * p01 + u * p12;
    }
}
