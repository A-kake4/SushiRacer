using UnityEngine;

//----------------------------------------  
// �쐬���F2025/06/18 �쐬�ҁF����  
// �X�V���F2025/07/07 �X�V�ҁF����  
//----------------------------------------  
// MonoBehaviour���p�������V���O���g���̊��N���X  
// dontDestroyOnLoad�̑I���ɂ���ăV�[�����܂����ł��C���X�^���X���j������Ȃ��悤�ɂ���  
// �V�[�����ɃC���X�^���X���Ȃ��ꍇ�AManagerScene��ǉ����ăC���X�^���X��T��  
// ����ł��C���X�^���X���Ȃ��ꍇ�A�G���[���O���o�͂���  
//----------------------------------------
public abstract class SingletonMonoBehaviour<T> : SingletonSealableMonoBehaviour
                                            where T : SingletonMonoBehaviour<T>
{
    [SerializeField]
    protected bool dontDestroyOnLoad = true;

    private static T instance;

    /// <summary>
    /// �V���O���g���C���X�^���X�擾
    /// </summary>
    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            if ( instance == null )
            {
                Debug.LogWarning( $"SingletonMonoBehaviour<{typeof( T ).Name}>�̃C���X�^���X�����݂��܂���B" );
            }
#endif
            return instance;
        }
    }
    protected sealed override void Awake()
    {
        if ( instance == null )
        {
            instance = this as T;
            if ( dontDestroyOnLoad )
            {
                DontDestroyOnLoad( gameObject );
            }
            AwakeSingleton();
        }
        else if ( instance != this )
        {
            Destroy( gameObject );
            return;
        }
    }

    /// <summary>
    /// �V���O���g���̏����������B�p����ŃI�[�o�[���C�h���Ă��������B
    /// </summary>
    protected abstract void AwakeSingleton();

    /// <summary>
    /// �V���O���g���̏I�������B�p����ŃI�[�o�[���C�h���Ă��������B
    /// </summary>
    protected abstract void OnDestroySingleton();

    protected sealed override void OnDestroy()
    {
        if ( instance == this )
        {
            OnDestroySingleton();
            instance = null; // �C���X�^���X���
        }
    }
}
