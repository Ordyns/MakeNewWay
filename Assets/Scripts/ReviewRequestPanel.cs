using UnityEngine;
using UnityEngine.UI;

public class ReviewRequestPanel : MonoBehaviour
{
    [field:SerializeField] public int TargetLevelNumber { get; private set; }
    [Space]
    [SerializeField] private Canvas canvas;
    [SerializeField] private AnimatedPanel animatedPanel;
    [Space]
    [SerializeField] private AnimatedButton rateGameButton;
    [SerializeField] private AnimatedButton closeButton;

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

    private void ClosePanel() => animatedPanel.Close(() => gameObject.SetActive(false));

    private void RequestReview() => Reviews.RequestGooglePlayReview();
}
