using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    static public Main S;
    static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;    // Частота
    public float enemySpawnPadding = 1.5f;  // Разброс для позиций
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {WeaponType.blaster,
                                    WeaponType.blaster, WeaponType.spread, WeaponType.shield};

    public bool ______________;

    public WeaponType[] activeWeaponTypes;
    public float enemySpawnRate;    // Задержка между спауном

    void Awake() {
        S = this;
        // Задаём границы камеры
        Utils.SetCameraBounds(this.GetComponent<Camera>());
        enemySpawnRate = 1f / enemySpawnPerSecond;
        Invoke("SpawnEnemy", enemySpawnRate);

        // Словарь с WeaponType как ключом
        W_DEFS = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions) {
            W_DEFS[def.type] = def;
        }
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
        // Проверяем существует ли ключ
        if (W_DEFS.ContainsKey(wt)) {
            return (W_DEFS[wt]);
        }
        // Возвратит значение для none, что означает ничего не найдено
        return (new WeaponDefinition());
    }

    void Start() {
        activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
        for (int i = 0; i < weaponDefinitions.Length; i++) {
            activeWeaponTypes[i] = weaponDefinitions[i].type;
        }
    }

    public void SpawnEnemy() {
        // Выбираем рандомного врага чтобы создать
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;
        // Распологаем врага над экраном с рандомным x значением
        Vector3 pos = Vector3.zero;
        float xMin = Utils.camBounds.min.x + enemySpawnPadding;
        float xMax = Utils.camBounds.max.x - enemySpawnPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = Utils.camBounds.max.y + enemySpawnPadding;
        go.transform.position = pos;

        // Создаём нового работягу
        Invoke("SpawnEnemy", enemySpawnRate);
    }

    public void DelayedRestart(float delay) {
        Invoke("Restart", delay);
    }

    public void Restart() {
        Application.LoadLevel("_Scene_0");
    }

    public void ShipDestroyed(Enemy e) {
        // Делаем дроп с шансом
        if(Random.value <= e.powerUpDropChance) {
            // Выбираем усиление
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            // Создаём улучшение
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Задаём нужный тип
            pu.SetType(puType);

            // Задаём позицию от уничтоженного врага
            pu.transform.position = e.transform.position;
        }
    }
}
