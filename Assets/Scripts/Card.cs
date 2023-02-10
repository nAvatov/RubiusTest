using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class Card : MonoBehaviour
{
    private const float flipAngle = 180f;
    private const double spriteChangeEdge = 90d;
    [SerializeField] private Image cardImage, charImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite frontView;
    [SerializeField] private Sprite backView;
    [SerializeField] private RectTransform rt;
    [SerializeField] private float flipDuration = 0.5f;
    private bool isShown = false;
    private DG.Tweening.Tween flipTween;
    private Coroutine downloadCoroutine;
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
    /// Flipping the card by y-axis depending on isShown flag state. 
    /// </summary>
    public void Flip(System.Action callback = null) {
        flipTween =  rt.DORotate(new Vector3(0f, isShown ? flipAngle : 0f, 0f), flipDuration)
        .OnComplete(() => {
            isShown = !isShown;
            if (callback != null) callback();
            flipTween.onComplete = null;
        })
        .OnUpdate(() => {
            if (isShown) { // If card is shown at the moment
                if (rt.eulerAngles.y > spriteChangeEdge) {
                    cardImage.sprite = backView;
                    charImage.sprite = null;

                    flipTween.onUpdate = null;
                }
            } else {
                if (rt.eulerAngles.y < spriteChangeEdge) {
                    cardImage.sprite = frontView;

                    flipTween.onUpdate = null;
                }
            }
        });
    }

    /// <summary>
    /// Method for initializing charImage.sprite of the card by using ResourceCreator class methods.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void LoadSprite(string url, System.Action callback = null) {
        Sprite createdSprite;

        downloadCoroutine = StartCoroutine(RequestHandler.SendRequest(url, (UnityWebRequest response) => {
            if (response != null) { // If request is fine and response code is 200 OK
                createdSprite = ResourceCreator.CreateSpriteFrom(ResourceCreator.GenerateTextureFrom(response.downloadHandler.data));
                charImage.sprite = createdSprite == null ? defaultSprite : createdSprite;
            } else { // If something went wrong with server request or response
                charImage.sprite = defaultSprite;
            }

            if (callback != null) callback();
        }));
    }

    /// <summary>
    /// Method for handling runtime interruption of flipping process by checking current rotation state.
    /// </summary>
    public void RepairState() {
        if (downloadCoroutine != null) { // If loading coroutine is stuck on parent coroutine schedule
            StopCoroutine(downloadCoroutine);
        }
        
        InterruptFlipping();

        if (rt.eulerAngles.y is >= 0 and < flipAngle) { // When card at least further than unflipped (backward) rotation
            isShown = true;
            return;
        }

        isShown = false;
    }
        
    #endregion

    #region Private Methods

    private void InterruptFlipping() {
        flipTween.Kill();
        flipTween = null;
    }
        
    #endregion
}
