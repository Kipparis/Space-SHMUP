using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    public float rotationsPerSecond = 0.1f;
    public bool _________________;
    public int levelShown = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Считываем текущий уровень из класса Hero
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        // Если показываемый уровень различается
        if (levelShown != currLevel) {
            levelShown = currLevel;
            Material mat = this.GetComponent<Renderer>().material;
            // Приравниваем сдвиг текстуры, чтобы показать разный уровень
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        // Поворачиваем щит немножко каждую секунду
        float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
	}
}
