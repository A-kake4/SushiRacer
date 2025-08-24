using UnityEngine;

public abstract class BaseSelectViewMonoBehaviour<T> : MonoBehaviour
    where T : ScriptableObject
{
    [SerializeField]
    protected T item;
    public T Item
    {
        get { return item; }
    }
}
