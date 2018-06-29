using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

    //============================= Bounds Functions =============================\\

    // Создаёт границы которые включают две переданные границы
    public static Bounds BoundsUnion(Bounds b0, Bounds b1) {
        // Если размер какого то вектора нулевой просто игнорируем
        if (b0.size == Vector3.zero) return b1;
        if (b1.size == Vector3.zero) return b0;
        // Растягиваем b0 чтобы включить b1 мин и макс
        b0.Encapsulate(b1.min);
        b0.Encapsulate(b1.max);
        return (b0);
    }

    public static Bounds CombineBoundsOfChildren(GameObject go) {
        // Создаём пустые границы
        Bounds b = new Bounds(Vector3.zero, Vector3.zero);

        // Если объект имеет рендерер
        if (go.GetComponent<Renderer>() != null) {
            b = BoundsUnion(b, go.GetComponent<Renderer>().bounds);
        }

        // Если объект имеет коллайдер 
        if (go.GetComponent<Collider>() != null) {
            b = BoundsUnion(b, go.GetComponent<Collider>().bounds);
        }

        // Повторяем внутри детей
        foreach (Transform t in go.transform) {
            b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
        }

        return (b);
    }

    // Статичное, только для чтения поле camBounds
    static public Bounds camBounds {
        get {
            // Если ещё нет _camBounds
            if(_camBounds.size == Vector3.zero) {
                SetCameraBounds();
            }
            return (_camBounds);
        }
    }
    static private Bounds _camBounds;

    // Функция вызывается чтобы задать границы камеры, может быть вызвана напрямую
    public static void SetCameraBounds(Camera cam = null) {
        // Если нет переданной камеры, использует основную
        if (cam == null) cam = Camera.main;
        // Пара важных предположений о камере:
        // 1 Камера орфографичная
        // 2 Поворот камеры 0,0,0

        // Создаём вектора верхнего левого и праваого нижнего угла
        Vector3 topLeft = new Vector3(0, 0, 0);
        Vector3 bottomRight = new Vector3(Screen.width, Screen.height, 0);

        // Конвертируем это в мировые координаты
        Vector3 boundTLN = cam.ScreenToWorldPoint(topLeft);
        Vector3 boundBRF = cam.ScreenToWorldPoint(bottomRight);

        // Присваиваем z координату
        boundTLN.z += cam.nearClipPlane;
        boundBRF.z += cam.farClipPlane;

        // Находим центр границ
        Vector3 center = (boundBRF + boundTLN) / 2f;
        _camBounds = new Bounds(center, Vector3.zero);
        _camBounds.Encapsulate(boundTLN);
        _camBounds.Encapsulate(boundBRF);
    }
}
