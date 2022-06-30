using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewRequestPanel : MonoBehaviour
{
    [SerializeField] private AnimatedPanel animatedPanel;

    public void ClosePanel(){
        animatedPanel.Close(() => Destroy(gameObject));
    }

    public void RequestReview() => Reviews.RequestGooglePlayReview();
}
