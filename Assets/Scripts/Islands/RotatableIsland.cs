using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[SelectionBase]
public class RotatableIsland : Island
{
    public override void OnClick(){
        base.OnClick();

        if(Parent){
            if(Parent.AllChildIslandsCanBeUpdated(DirectionExtensions.GetDirectionFromAngle(Parent.transform.eulerAngles.y + 90)) == false)
                return;
        }
            
        Transform targetTransform = Parent ? Parent.transform : transform;
        targetTransform.DOLocalRotate(transform.eulerAngles + new Vector3(0, 90, 0), 0.275f).SetEase(Ease.OutCubic).OnComplete(UpdatingFinished);
        PlayerInput.Instance.IslandUpdating();

        AudioPlayer.PlayClip(base.updatingSound);
    }
}