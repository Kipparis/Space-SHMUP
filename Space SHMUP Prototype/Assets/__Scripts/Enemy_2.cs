using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {
    // Второй враг использует синусовую волну чтобы изменить 
    // двухточечную линейный переход
    public Vector3[] points;
    public float birthTime;
    public float lifeTime = 10;
    // Определяем как сильно волна будет влиять на движение
    public float sinEccentricity = 0.6f;
	
    void Start() {
        // Объявляем две точки
        points = new Vector3[2];

        // Находим границы камеры
        Vector3 cbMin = Utils.camBounds.min;
        Vector3 cbMax = Utils.camBounds.max;

        Vector3 v = Vector3.zero;
        // Выбираем любую точку на левой стороне экрана
        v.x = cbMin.x - Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[0] = v;

        // Выбираем любую точку на правой стороне экрана
        v.x = cbMax.x + Main.S.enemySpawnPadding;
        v.y = Random.Range(cbMin.y, cbMax.y);
        points[1] = v;

        // Меняем сторонами
        if(Random.value < 0.5f) {
            points[0].x *= -1;
            points[1].x *= -1;
        }

        // Задаём время рождения
        birthTime = Time.time;
    }

    public override void Move() {
        // Бизеровая кривая основанная на значание между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;

        // Если значение больше 1, значит разница между текущим временем и рождением больше длинны жизни
        if (u > 1) {
            Destroy(this.gameObject);
            return;
        }

        // Редактируем u добавляя смягчающую кривую основанную на синусовой волне
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        // Сливаем два значения
        pos = (1 - u) * points[0] + u * points[1];
    }
}
