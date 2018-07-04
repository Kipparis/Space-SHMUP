using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    public GameObject poi;  // Корабль игрока
    public GameObject[] panels; // Прокручивающийся бэк
    public float scrollSpeed = -30f;
    // motionMult показывает как сильно панели реагирует на движение игрока
    public float motionMult = 0.25f;

    private float panelHt;  // Высота каждой панели
    private float depth;    // Позиция z

    void Start() {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        // Задаём позиции для панелей
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    void Update() {
        float tY, tX = 0;
        tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);

        if(poi != null) {
            tX = -poi.transform.position.x * motionMult;
        }

        // Ставим панель 0 
        panels[0].transform.position = new Vector3(tX, tY, depth);
        // Затем ставим панель[1] куда надо чтобы был эффект продолжающегося неба
        if(tY >= 0) {
            panels[1].transform.position = new Vector3(tX, tY - panelHt, depth);
        } else {
            panels[1].transform.position = new Vector3(tX, tY + panelHt, depth);
        }
    }
}
