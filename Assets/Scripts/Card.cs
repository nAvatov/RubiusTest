using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class Card : MonoBehaviour
{
    private const float flipAngle = 180f;
    private const double spriteChangeEdge = 90d;
    [SerializeField] private Image _cardImage, _charImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _frontView;
    [SerializeField] private Sprite _backView;
    [SerializeField] private RectTransform _rt;
    [SerializeField] private float _flipDuration = 0.5f;
    private bool _isShown = false;
    private DG.Tweening.Tween _flipTween;
    private Coroutine _downloadCoroutine;
    public Sprite CharImageSprite {
        set {
            _charImage.sprite = value;
        }

        get {
            return _charImage.sprite;
        }
    }
    public bool IsShown => _isShown;

    // Flipping the card by y-axis depending on isShown flag state. 
    public void Flip(System.Action callback = null) {
        _flipTween =  _rt.DORotate(new Vector3(0f, _isShown ? flipAngle : 0f, 0f), _flipDuration)
        .OnComplete(() => {
            _isShown = !_isShown;
            if (callback != null) {
                callback();
            }
            _flipTween.onComplete = null;
        })
        .OnUpdate(() => {
            // If card is shown at the moment
            if (_isShown) {
                if (_rt.eulerAngles.y > spriteChangeEdge) {
                    _cardImage.sprite = _backView;
                    _charImage.sprite = null;

                    _flipTween.onUpdate = null;
                }
            } else {
                if (_rt.eulerAngles.y < spriteChangeEdge) {
                    _cardImage.sprite = _frontView;

                    _flipTween.onUpdate = null;
                }
            }
        });
    }

    // Method for initializing charImage.sprite of the card by using ResourceCreator class methods.
    public void LoadSprite(string url, System.Action callback = null) {
        Sprite createdSprite;

        _downloadCoroutine = StartCoroutine(RequestHandler.SendRequest(url, (UnityWebRequest response) => {
            // If request is fine and response code is 200 OK
            if (response != null) {
                createdSprite = ResourceCreator.CreateSpriteFrom(ResourceCreator.GenerateTextureFrom(response.downloadHandler.data));
                _charImage.sprite = createdSprite == null ? _defaultSprite : createdSprite;
            // If something went wrong with server request or response
            } else { 
                _charImage.sprite = _defaultSprite;
            }

            if (callback != null) callback();
        }));
    }

    // Method for handling runtime interruption of flipping process by checking current rotation state.
    public void RepairState() {
        // If loading coroutine is stuck on parent coroutine schedule
        if (_downloadCoroutine != null) {
            StopCoroutine(_downloadCoroutine);
        }
        
        InterruptFlipping();

        // When card at least further than unflipped (backward) rotation
        if (_rt.eulerAngles.y is >= 0 and < flipAngle) {
            _isShown = true;
            return;
        }

        _isShown = false;
    }

    private void InterruptFlipping() {
        _flipTween.Kill();
        _flipTween = null;
    }
}
