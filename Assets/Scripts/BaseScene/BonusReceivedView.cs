using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AnimatedPanel))]
public class BonusReceivedView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bonusRecievedText;

    private AnimatedPanel _animatedPanel;

    private void Awake() {
        _animatedPanel = GetComponent<AnimatedPanel>();
    }

    public void Show(int stepsForBonus){
        bonusRecievedText.text = string.Format(bonusRecievedText.text, stepsForBonus);
        _animatedPanel.Open();
    }
}
