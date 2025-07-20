using UnityEngine;

public abstract class SingletonSealableMonoBehaviour : MonoBehaviour
{
    protected abstract void Awake();

    protected abstract void OnDestroy();
}
