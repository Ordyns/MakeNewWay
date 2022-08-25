using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewRequestPanel : MonoBehaviour
{
    [SerializeField] private int targetLevelNumber;
    [Space]
    [SerializeField] private Canvas canvas;
    [SerializeField] private AnimatedPanel animatedPanel;

    private void Start() {
        
        int levelNumber = ScenesLoader.Instance.LastLoadedLevelNumber;
        SaveSystem saveSystem = SaveSystem.Instance;

        if(levelNumber >= targetLevelNumber && saveSystem.Data.ReviewRequested == false){
            saveSystem.Data.ReviewRequested = true;
            canvas.enabled = true;
            animatedPanel.Open();
        }
        else{
            canvas.enabled = false;
            gameObject.SetActive(false);
        }
    }

    public void ClosePanel(){
        animatedPanel.Close(() => Destroy(gameObject));
    }

    public void RequestReview() => Reviews.RequestGooglePlayReview();
}
