using UnityEngine;

public class ScenesTransitions : MonoBehaviour
{
    [SerializeField] private Color transitionsColor = Color.black;
    [Space]
    [SerializeField] private Transition transitionPanel;

    private void Awake() {
        transitionPanel.Init(transitionsColor);
    }

    public void CreateNewTransition(System.Action onInAnimationFinished){
        transitionPanel.gameObject.SetActive(true);
        transitionPanel.StartInAnimation();

        transitionPanel.InAnimationFinished = onInAnimationFinished;
    }

    public void CloseCurrentTransition() => transitionPanel.Close();
}