using UnityEngine;
using UnityEngine.UI;

public class ReviewRequestPanel : MonoBehaviour
{
    [SerializeField] private int targetLevelNumber;
    [Space]
    [SerializeField] private Canvas canvas;
    [SerializeField] private AnimatedPanel animatedPanel;
    [Space]
    [SerializeField] private AnimatedButton rateGameButton;
    [SerializeField] private AnimatedButton closeButton;

    private void Start() {
        int levelNumber = ProjectContext.Instance.ScenesLoader.LastLoadedLevelNumber;
        SaveSystem saveSystem = ProjectContext.Instance.SaveSystem;

        if(levelNumber >= targetLevelNumber && saveSystem.Data.ReviewRequested == false){
            saveSystem.Data.ReviewRequested = true;
            canvas.enabled = true;
            rateGameButton.OnClick.AddListener(RequestReview);
            closeButton.OnClick.AddListener(ClosePanel);
            animatedPanel.Open();
        }
        else{
            canvas.enabled = false;
            gameObject.SetActive(false);
        }
    }

    private void ClosePanel() => animatedPanel.Close(() => gameObject.SetActive(false));

    private void RequestReview() => Reviews.RequestGooglePlayReview();
}
