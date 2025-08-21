using TMPro;
using UnityEngine;

public class CountupText : MonoBehaviour
{
    [SerializeField,ReadOnly,Header("‘I‚ñ‚Å‚¢‚éƒLƒƒƒ‰")]
    private int charaNum = 0;
    public int CharaNum
    {
        get { return charaNum; }
    }

    [SerializeField]
    private string text;

    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        SetText();
    }

    public void CountUp()
    {
        charaNum++;
        SetText();
    }

    public void CountDown()
    {
        charaNum--;
        SetText();
    }

    private void SetText()
    {
        textMeshPro.text = text + charaNum.ToString("00");
    }
}
