using System.Collections;
using UnityEngine;
using Google.Play.Review;

public class Reviews : MonoBehaviour
{
    private static Reviews _instance;

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;

    private void Awake() => _instance = this;

    private static IEnumerator RequestGooglePlayReviewRoutine(){
        var reviewManager = new ReviewManager();

        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            yield break;

        var playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            yield break;
    }

    public static void RequestGooglePlayReview() => Reviews._instance.StartCoroutine(Reviews.RequestGooglePlayReviewRoutine());
}
