using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
    // Используем Vector2 чтобы удерживать минимальные
    // и максимыльные значения для Random.Range()
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; // Секунд усиление существует
    public float fadeTime = 4f; // Секунд оно угасает

    public bool ___________________;

    public WeaponType type; // Тип улучшения
    public GameObject cube; // Ссылка на куб
    public TextMesh letter; // Ссылка на тект
    public Vector3 rotPerSecond;    // Скорость вращения Euler
    public float birthTime;

	void Awake() {
        // Находим ссылку на куб
        cube = transform.Find("Cube").gameObject;
        // Находим текст
        letter = GetComponent<TextMesh>();

        // Задаём рандомную скорость
        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;  // Усиление не может улетать вдаль
        vel.Normalize();    // Задаём длинну вектора 1

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        GetComponent<Rigidbody>().velocity = vel;

        // Задаём поворот как 0,0,0
        transform.rotation = Quaternion.identity;

        // Задаём вращение в секунду
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y));
        // Проверяем конец экрана каждые 2 секунды
        InvokeRepeating("CheckOffscreen", 2f, 2f);

        birthTime = Time.time;
    }

    void Update() {
        // Вручную вращаем куб по времени
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        // Угасание усиления по времени
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // Во время жизни u < 0, затем оно приближается к единице во время угасания
        if (u >= 1) {
            Destroy(this.gameObject);
            return;
        }
        if (u > 0) {
            // Цвет бледнеет
            Color c = cube.GetComponent<Renderer>().material.color;
            c.a = 1f - u;
            cube.GetComponent<Renderer>().material.color = c;
            // Буква угасает, но не так сильно
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
    }

    public void SetType(WeaponType wt) {
        // Узнаём определение из класса Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        // Задаём цвет для куба
        cube.GetComponent<Renderer>().material.color = def.color;
        letter.text = def.letter;   // Задаём букву
        type = wt;
    }

    public void AbsorbedBy(GameObject target) {
        // Можем сделать визуальные приколюхи, но пока просто уничтожаем
        Destroy(this.gameObject);
    }

    void CheckOffscreen() {
        if(Utils.ScreenBoundsCheck(cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero) {
            Destroy(this.gameObject);
        }
    }
}
