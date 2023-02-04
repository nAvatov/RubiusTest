using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] Image cardImage, charImage;
    [SerializeField] Sprite frontView;
    [SerializeField] Sprite backView;
    [SerializeField] RectTransform rt;
    private bool isShown = false;
    private float flipAngle = 180f;
    private double spriteChangeEdge = 0.6;

    public Sprite CharImageSprite {
        set {
            charImage.sprite = value;
        }

        get {
            return charImage.sprite;
        }
    }

    #region Public Methods

    /// <summary>
    /// Flipping the card by y-axis depending on initial rotation state. 
    /// </summary>
    public void Flip() {
        rt.DORotate(new Vector3(0f, rt.rotation.y == 0 ? flipAngle : 0f, 0f), 0.5f).OnUpdate(() => {
            if (Math.Round(rt.rotation.y, 1) == spriteChangeEdge) {
                if (isShown) {
                    cardImage.sprite = backView;
                    charImage.sprite = null;
                } else {
                    cardImage.sprite = frontView;
                }
            }
        }).OnComplete(() => {
            isShown = !isShown;
        });
    }
        
    #endregion
}
