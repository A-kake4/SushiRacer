using UnityEngine;

public class SelectManagerSend : MonoBehaviour
{
    [SerializeField]
    private int playerNum = 0;
    [SerializeField]
    private CountupText CountupText;

    private void Update()
    {
        PlayerSelectManager.Instance.SetSelectedCharacterIndex( playerNum, CountupText.CharaNum);
    }
}
