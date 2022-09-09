using UnityEngine;

public class InfoTutorialStep : TutorialStep
{
    [Space]
    [SerializeField] private InfoTutorialView infoView;
    [Space]
    [SerializeField] private Transform anchorTransform;
    [SerializeField] private Vector3 pivotOffsetOfAnchorTransform;
    
    public override void Enter(RectTransform canvas){
        base.Enter(canvas);

        infoView.Init(WorldToCanvasPoint(anchorTransform.transform.position + pivotOffsetOfAnchorTransform));
        infoView.Show();
    }

    private void Update() {
        if(IsActive == false)
            return;

        if(Input.GetMouseButtonDown(0))
            Complete();
    }

    public override void Exit(){
        base.Exit();

        infoView.Hide();
    }
}
    
