using UnityEngine;

public class EffectController : MonoBehaviour
{
    public string EffectId { get; set; }
    public System.Action<string, GameObject> OnEffectDisabled;

    private void OnDisable()
    {
        OnEffectDisabled?.Invoke(EffectId, gameObject);
    }
}
