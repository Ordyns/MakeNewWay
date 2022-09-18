using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    // TODO: Change to auto-implemented property with a private setter or make this field private
    public string LocalizationKey;

    private TextMeshProUGUI _textMesh;

    private void OnValidate() {
        _textMesh = GetComponent<TextMeshProUGUI>();    
    }

    [Zenject.Inject]
    private void Init(System.Action<LocalizedText> addLocalizedText){
        addLocalizedText.Invoke(this);
    }

    public void UpdateText(string text) => _textMesh.text = text;
}