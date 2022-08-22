using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    public string LocalizationKey;

    private Localization localization;
    private TextMeshProUGUI textMesh;

    private void Awake() {
        localization = Localization.Instance;
        textMesh = GetComponent<TextMeshProUGUI>();

        if(localization){
            localization.LanguageChanged += UpdateText;
            UpdateText();
        }
    }

    private void UpdateText() => textMesh.text = localization.GetLocalizedValue(LocalizationKey);

    private void OnDestroy() {
        if(localization) localization.LanguageChanged -= UpdateText;
    }
}