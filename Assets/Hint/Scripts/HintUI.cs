using UnityEngine;
using DG.Tweening;

public class HintUI : MonoBehaviour
{
    public bool HintOpened { get; private set; }

    [Header("Background")]
    [SerializeField] private CanvasGroup background;
    
    [Header("Buttons")]
    [SerializeField] private AnimatedButton viewAdButton;
    [SerializeField] private AnimatedButton hintButton;
    [SerializeField] private AnimatedButton closeButton;
    
    [Header("Hint Panel")]
    [SerializeField] private HintView hintView;

    private HintViewModel _hintViewModel;

    private System.Func<string, string> _getLocalizedValueFunc;

    [Zenject.Inject]
    private void Init(HintViewModel hintViewModel, System.Func<string, string> getLocalizedValueFunc) {
        _hintViewModel = hintViewModel;

        _getLocalizedValueFunc = getLocalizedValueFunc;
        hintView.Init(hintViewModel, getLocalizedValueFunc);

        InitHintButtons();
    }

    private async void InitHintButtons(){
        viewAdButton.OnClick.AddListener(ViewAd);
        hintButton.OnClick.AddListener(OpenPanel);
        closeButton.OnClick.AddListener(ClosePanel);

        hintButton.gameObject.SetActive(_hintViewModel.IsAdViewed && _hintViewModel.StepsCount > 0);
        viewAdButton.gameObject.SetActive(false);

        if(_hintViewModel.IsAdViewed == false && _hintViewModel.StepsCount > 0){
            bool isInternetReachable = await _hintViewModel.IsInternetReachable();
            viewAdButton?.gameObject.SetActive(isInternetReachable);
        }
    }

    public void ViewAd(){
        _hintViewModel.ViewAd(AdViewed);                
    }

    private void AdViewed(){
        _hintViewModel.IsAdViewed = true;

        viewAdButton.gameObject.SetActive(false);
        hintButton.gameObject.SetActive(true);

        OpenPanel();
    }

    public void OpenPanel(){
        HintOpened = true;

        hintView.Show();

        _hintViewModel.ActivateHintCommand.Execute();

        ChangeBackgroundVisibility(true);
    }

    public void ClosePanel(){
        HintOpened = false;

        hintView.Hide();

        _hintViewModel.DeactivateHintCommand.Execute();

        ChangeBackgroundVisibility(false);
    }

    private void ChangeBackgroundVisibility(bool visible, float delay = 0){
        background.gameObject.SetActive(true);
        
        background.DOFade(visible ? 1 : 0, 0.25f).SetDelay(delay).OnComplete(() => {
            if(visible == false) 
                background.gameObject.SetActive(false);
        });
    }
}
