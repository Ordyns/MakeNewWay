using UnityEngine;

public class ReviewRequestPanel : MonoBehaviour
{
    public event System.Action PanelClosed;

    [SerializeField] private int targetLevelNumber;
    [Space]
    [SerializeField] private Canvas canvas;
    [SerializeField] private AnimatedPanel animatedPanel;
    [Space]
    [SerializeField] private AnimatedButton rateGameButton;
    [SerializeField] private AnimatedButton closeButton;

    public bool IsTargetLevel(int levelNumber) => levelNumber == targetLevelNumber;

    public void ShowPanel(){
        canvas.enabled = true;
        rateGameButton.OnClick.AddListener(RequestReview);
        closeButton.OnClick.AddListener(ClosePanel);
        animatedPanel.Open();
    }

    public void Disable(){
        canvas.enabled = false;
        gameObject.SetActive(false);
    }

    private void ClosePanel(){
        animatedPanel.Close(() => {
            gameObject.SetActive(false);
            PanelClosed?.Invoke();
        });
    } 

    private void RequestReview() => Reviews.RequestGooglePlayReview();
}
