using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Используется для теста на границы
public enum BoundsTest {
    center,
    onScreen,
    offScreen
}

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
            if (_camBounds.size == Vector3.zero) {
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

    // Проверяет границы внутри границы камеры
    public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center) {
        return (BoundsInBoundsCheck(camBounds, bnd, test));
    }

    public static Vector3 BoundsInBoundsCheck(Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen) {
        // Центр lilB
        Vector3 pos = lilB.center;

        // Создаём смещение
        Vector3 off = Vector3.zero;

        switch (test) {
            case BoundsTest.center:
                if (bigB.Contains(pos)) {
                    return (Vector3.zero);
                }
                if (pos.x > bigB.max.x) {
                    off.x = pos.x - bigB.max.x;
                } else if (pos.x < bigB.min.x) {
                    off.x = pos.x - bigB.min.x;
                }
                if (pos.y > bigB.max.y) {
                    off.y = pos.y - bigB.max.y;
                } else if (pos.y < bigB.min.y) {
                    off.y = pos.y - bigB.min.y;
                }
                if (pos.z > bigB.max.z) {
                    off.z = pos.z - bigB.max.z;
                } else if (pos.z < bigB.min.z) {
                    off.z = pos.z - bigB.min.z;
                }
                return (off);
            // Что нужно сделать, чтобы держать lilB внутри bigB    
            case BoundsTest.onScreen:
                if (bigB.Contains(lilB.min) && bigB.Contains(lilB.max)) {
                   return (Vector3.zero);
                }
                if(lilB.max.x > bigB.max.x) {
                    off.x = lilB.max.x - bigB.max.x;
                } else if(lilB.min.x < bigB.min.x) {
                    off.x = lilB.min.x - bigB.min.x;
                }
                if (lilB.max.y > bigB.max.y) {
                    off.y = lilB.max.y - bigB.max.y;
                } else if (lilB.min.y < bigB.min.y) {
                    off.y = lilB.min.y - bigB.min.y;
                }
                if (lilB.max.z > bigB.max.z) {
                    off.z = lilB.max.z - bigB.max.z;
                } else if (lilB.min.z < bigB.min.z) {
                    off.z = lilB.min.z - bigB.min.z;
                }
                return (off);
            case BoundsTest.offScreen:
                break;
            default:
                break;
        }
    }
}
