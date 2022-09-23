using UnityEngine;

public class MoveIslandTutorialStep : TutorialStep, IIslandUpdateHandler
{
    [Space]
    [SerializeField] private MoveIslandTutorialView moveIslandTutorialView;
    [Space]
    [SerializeField] private Transform islandTransform;
    [SerializeField] private Transform targetIslandPositionTransform;
    [SerializeField] private Vector3 islandPivotOffset;

    private void OnValidate() {
        RequiresIslandsUpdater = true;
    }

    public override void Enter(RectTransform canvas){
        base.Enter(canvas);

        Vector3 IslandScreenPosition = WorldToCanvasPoint(islandTransform.position + islandPivotOffset);
        Vector3 TargetScreenPosition = WorldToCanvasPoint(targetIslandPositionTransform.position + islandPivotOffset);
        moveIslandTutorialView.SetPositions(IslandScreenPosition, TargetScreenPosition);
        moveIslandTutorialView.Show();
    }

    private void Update() {
        if(IsActive == false)
            return;

        if(Input.GetMouseButtonDown(0))
            moveIslandTutorialView.Hide();
        else if(Input.GetMouseButtonUp(0))
            moveIslandTutorialView.Show();
    }

    public void OnIslandUpdating(){
        moveIslandTutorialView.Hide();
        moveIslandTutorialView.StopAnimations();
    }

    public void OnIslandUpdated(){
        Complete();
    }

    public override void Exit(){
        base.Exit();

        moveIslandTutorialView.StopAnimations();
    }
}
