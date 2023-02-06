using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class CardsController : MonoBehaviour
{
    [SerializeField] Button loadButton;
    [SerializeField] TMPro.TMP_Dropdown dropdownList;
    [SerializeField] List<Card> cards;
    [SerializeField] Sprite defaultSprite; // Incase something wrong happened with download
    private string urlPattern = "http://picsum.photos/200";
    private float tweenFlipDelay = 0.5f;
    private bool cardsAreShown = false;
    private bool CardsAreShown {
        set {
            cardsAreShown = value;

            if (value) {
                loadButton.interactable = true;
            }
        }
    }
    private UniTask[] tasks;

    #region Public Methods

    public void Load() {
        loadButton.interactable = false;
        switch(dropdownList.captionText.text) {
            case "OneByOne" : {
                StartCoroutine(Show(() => {
                    StartCoroutine(ShowCards(
                        null, 
                        (Card loadedCard) => { loadedCard.Flip(); }
                    ));
                }));
                break;
            }
            case "AllAtOnce" : {
                StartCoroutine(Show(() => {
                    StartCoroutine(ShowCards(
                        () => { foreach(Card card in cards) { card.Flip(); } }, 
                        null
                    ));
                }));
                break;
            }
            case "EachAsReady" : {
                StartCoroutine(Show(() => {
                    ShowCardsAsync();
                }));
                break;
            }
        }
    }
        
    #endregion

    #region Private Methods

    private IEnumerator Show(System.Action callback) {
        StartCoroutine(HideAllCards());

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => cardsAreShown == false);

        callback();
    }

    /// <summary>
    /// Method for donwloading resources for cards sprites. Can handle two types of card flipping by pointing direct callback in params.
    /// </summary>
    /// <param name="AllAtOnce"></param>
    /// <param name="OneByOne"></param>
    /// <returns></returns>
    private IEnumerator ShowCards(System.Action AllAtOnce = null, System.Action<Card> OneByOne = null) {
        Sprite createdSprite;
        
        foreach(Card card in cards) {
            StartCoroutine(RequestHandler.SendRequest(urlPattern, (UnityWebRequest response) => {
                if (response != null) { // If request is fine and response code is 200 OK
                    createdSprite = ResourceCreator.CreateSpriteFrom(ResourceCreator.GenerateTextureFrom(response.downloadHandler.data));
                    card.CharImageSprite = createdSprite == null ? defaultSprite : createdSprite;
                } else { // If something went wrong with server request or response
                    card.CharImageSprite = defaultSprite;
                }
            }));
            
            yield return new WaitUntil( () => card.CharImageSprite != null);
            if (OneByOne != null) OneByOne(card);
        }
        
        if (AllAtOnce != null) AllAtOnce();
        
        yield return new WaitForSeconds(tweenFlipDelay);
        CardsAreShown = true;
    }


    /// <summary>
    /// This method creates amount of tasks equal to cards amount. After all tasks is done cardsAreShown flag is changed.
    /// </summary>
    /// <returns></returns>
    private async void ShowCardsAsync() {
        tasks = new UniTask[cards.Count];

        for(int i = 0; i < cards.Count; i++) {
            tasks[i] = GetSpriteForCardAsync(cards[i]);
        }

        await UniTask.WhenAll(tasks);
        await UniTask.Delay(System.TimeSpan.FromSeconds(tweenFlipDelay));
        CardsAreShown = true; // flag is set to true for hiding cards process 
    }

    /// <summary>
    /// Method for particular card async byts download, card charImage sprite inits and card flips after download is succeed.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private async UniTask GetSpriteForCardAsync(Card card) {
        Sprite createdSprite;
        byte[] bytes;

        var task = RequestHandler.GetBytesAsync(UnityWebRequest.Get(urlPattern)); // Setting task for byte[] download

        bytes = await task; 
        createdSprite = ResourceCreator.CreateSpriteFrom(ResourceCreator.GenerateTextureFrom(bytes));

        card.CharImageSprite = createdSprite == null ? defaultSprite : createdSprite;
        card.Flip();
    }

    /// <summary>
    /// Method for refreshing cards state - flipping them all over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideAllCards() {
        if (cardsAreShown) {
            foreach(Card card in cards) {
                card.Flip(); // Tweens are taking some time too! Beware of using coroutines with them..
            }

            yield return new WaitForSeconds(tweenFlipDelay);
            CardsAreShown = false;
        }
    }
        
    #endregion
}


