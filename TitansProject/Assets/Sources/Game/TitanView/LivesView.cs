using System;
using System.Collections.Generic;
using UnityEngine;

public class LivesView : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer spritePrefab; // Set from editor
    [SerializeField]
    private int livePoint = 5;
    [SerializeField]
    private int armor = 20;
    [SerializeField]
    private int maxArmor = 20;
    [SerializeField]
    private int shield = 20;
    [SerializeField]
    private int maxShield = 20;
    [SerializeField]
    private LivesViewSettings settings; // Set from editor

    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private SpriteRenderer armorSprite;
    private SpriteRenderer shieldSprite;

    private void Awake() {
        spritePrefab.gameObject.SetActive(false);
        armorSprite = Instantiate(spritePrefab, spritePrefab.transform.parent);
        armorSprite.color = settings.ArmorColor;
        shieldSprite = Instantiate(spritePrefab, spritePrefab.transform.parent);
        shieldSprite.color = settings.ShieldColor;
    }

    public void UpdateState (int armor, int maxArmor, int shield, int maxShield) {
        this.maxArmor = maxArmor;
        this.armor = armor;
        this.shield = shield;
        this.maxShield = maxShield;
        UpdateLives();
    }

    [ContextMenu("Update")]
    private void UpdateLives() {
        var fullLives = maxShield + maxArmor;
        int fullCount = fullLives / livePoint;
        FillSpriteCount(fullCount);
        for (int i = 0; i < sprites.Count; i++) {
            sprites[i].gameObject.SetActive(i < fullCount);
            sprites[i].transform.localPosition = new Vector3(i * settings.Distance.x - ((fullCount - 1) * 0.5f) * settings.Distance.x, 0, 0);
            sprites[i].transform.localScale = settings.Size;
            if (i < maxArmor / livePoint) {
                if (i < armor / livePoint) {
                    sprites[i].color = settings.ArmorColor;
                } else {
                    sprites[i].color = settings.ArmorBackColor;
                }
            } else {
                if (i < (shield + maxArmor) / livePoint) {
                    sprites[i].color = settings.ShieldColor;
                } else {
                    sprites[i].color = settings.ShieldBackColor;
                }
            }
        }
        if (armor < maxArmor) {
            armorSprite.gameObject.SetActive(true);
            int position = armor / livePoint;
            float armorLevel = (float)(armor % livePoint) / livePoint;
            armorSprite.transform.localPosition = new Vector3(position * settings.Distance.x - settings.Size.x * (1f - armorLevel) - ((fullCount - 1) * 0.5f) * settings.Distance.x, 0, 0);
            armorSprite.transform.localScale = new Vector3(settings.Size.x * armorLevel, settings.Size.y);
        } else {
            armorSprite.gameObject.SetActive(false);
        }
        if (shield < maxShield) {
            shieldSprite.gameObject.SetActive(true);
            int position = (shield + maxArmor) / livePoint;
            float shieldLevel = (float)(shield % livePoint) / livePoint;
            shieldSprite.transform.localPosition = new Vector3(position * settings.Distance.x - settings.Size.x * (1f - shieldLevel) - ((fullCount - 1) * 0.5f) * settings.Distance.x, 0, 0);
            shieldSprite.transform.localScale = new Vector3(settings.Size.x * shieldLevel, settings.Size.y);
        } else {
            shieldSprite.gameObject.SetActive(false);
        }
    }

    private void FillSpriteCount(int fullCount) {
        if (sprites.Count >= fullCount)
            return;
        for (int i = sprites.Count; i < fullCount; i++) {
            sprites.Add(Instantiate(spritePrefab, spritePrefab.transform.parent));
        }
    }
}
