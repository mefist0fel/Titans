using UnityEngine;

public class LivesView : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer[] sprites = new SpriteRenderer[13];
    [SerializeField]
    private Vector2 size = new Vector2(0.1f, 0.1f);
    [SerializeField]
    private int livePoint = 10;
    [SerializeField]
    private int fullLives = 100;
    [SerializeField]
    private int lives = 0;

    public void UpdateState (int armor, int maxArmor, int shield, int maxShield) {
        fullLives = maxArmor + maxShield;
        lives = armor + shield;
        UpdateLives();
    }

    [ContextMenu("Update")]
    private void UpdateLives() {
        int fullCount = Mathf.CeilToInt(fullLives / 10f);
        for (int i = 0; i < sprites.Length; i++) {
            if (sprites[i] == null)
                continue;
            sprites[i].gameObject.SetActive(i < fullCount);
            sprites[i].transform.localPosition = new Vector3(i * size.x - ((fullCount - 1) * 0.5f) * size.x, 0, 0);
        }
    }
}
