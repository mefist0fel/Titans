using System.Collections.Generic;
using UnityEngine;

public sealed class BulletController : MonoBehaviour {
    private static BulletController Instance;

    [SerializeField]
    private float speed = 10f; // Set from editor
    [SerializeField]
    private GameObject bulletPrefab; // Set from editor

    private List<GameObject> bulletsCache = new List<GameObject>();

    public static void Fire(Vector3 from, Vector3 to, Vector3 up, Action onHit) {
        if (Instance == null) {
            Debug.LogError("BulletController is not initialised");
            if (onHit != null) {
                onHit();
            }
            return;
        }
        Instance.FireBullet(from, to, up, onHit);
    }

    private void FireBullet(Vector3 from, Vector3 to, Vector3 up, Action onHit) {
        float distance = Vector3.Distance(from, to);
        float height = distance * 0.3f;
        GameObject bullet = CreateBullon();
        bullet.transform.position = from;
        bullet.gameObject.SetActive(true);
        var trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null) {
            trail.Clear();
        }
        Timer.Add(distance / speed,
            (anim) => {
                float trajectory = (anim * 2f - 1f);
                trajectory = trajectory * trajectory * -1 + 1f;
                bullet.transform.position = Vector3.Lerp(from, to, anim) + trajectory * height * up;
            },
            () => {
                Timer.Add(0.2f, () => {
                    bullet.SetActive(false);
                });
                if (onHit != null) {
                    onHit();
                }
            });
    }

    private GameObject CreateBullon() {
        foreach (var bullet in bulletsCache) {
            if (bullet == null) {
                Debug.LogError("Empty bullet in bullet cache!");
                continue;
            }
            if (!bullet.activeSelf) {
                return bullet;
            }
        }
        var newBullet = Instantiate<GameObject>(bulletPrefab, transform);
        newBullet.SetActive(false);
        bulletsCache.Add(newBullet);
        return newBullet;
    }

    private void Awake () {
        Instance = this;
	}
	
	void Update () {
		
	}
}
