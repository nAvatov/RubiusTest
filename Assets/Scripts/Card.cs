using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] Image cardImage;
    [SerializeField] Sprite frontView;
    [SerializeField] Sprite backView;
    [SerializeField] RectTransform rt;
    private bool isHidden = true;

    #region Public Methods

    public void Flip() {
        rt.DORotate(new Vector3(0f, rt.rotation.y == 0 ? 180f : 0f, 0f), 1f).OnUpdate(() => {
            if (Math.Round(rt.rotation.y, 1) == 0.6) {
                cardImage.sprite = !isHidden ? backView : frontView;
            }
        }).OnComplete(() => {
            isHidden = !isHidden;
        });
    }
        
    #endregion
}
