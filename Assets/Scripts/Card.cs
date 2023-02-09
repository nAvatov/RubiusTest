using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class Card : MonoBehaviour
{
    const float flipAngle = 180f;
    const double spriteChangeEdge = 0.6;
    [SerializeField] Image cardImage, charImage;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite frontView;
    [SerializeField] Sprite backView;
    [SerializeField] RectTransform rt;
    [SerializeField] float flipDuration = 0.5f;
    private bool isShown = false;
    public Sprite CharImageSprite {
        set {
            charImage.sprite = value;
        }

        get {
            return charImage.sprite;
        }
    }

    public bool IsShown {
        get {
            return isShown;
        }
    }

    #region Public Methods

    /// <summary>
    /// Flipping the card by y-axis depending on initial rotation state. 
    /// </summary>
    public void Flip(System.Action callback = null) {
        rt.DORotate(new Vector3(0f, rt.rotation.y == 0f ? flipAngle : 0f, 0f), flipDuration)
        .OnComplete(() => {
            isShown = !isShown;
            if (callback != null) callback();
        })
        .OnUpdate(() => {
            if (Math.Round(rt.rotation.y, 1) == spriteChangeEdge) {
                if (isShown) {
                    cardImage.sprite = backView;
                    charImage.sprite = null;
                } else {
                    cardImage.sprite = frontView;
                }
            }
        });
    }

    public void LoadSprite(string url, System.Action callback = null) {
        Sprite createdSprite;

        StartCoroutine(RequestHandler.SendRequest(url, (UnityWebRequest response) => {
            if (response != null) { // If request is fine and response code is 200 OK
                createdSprite = ResourceCreator.CreateSpriteFrom(ResourceCreator.GenerateTextureFrom(response.downloadHandler.data));
                charImage.sprite = createdSprite == null ? defaultSprite : createdSprite;
            } else { // If something went wrong with server request or response
                charImage.sprite = defaultSprite;
            }

            if (callback != null) callback();
        }));
    }
        
    #endregion
}
