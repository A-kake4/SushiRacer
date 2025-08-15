using UnityEngine;
using UnityEngine.UI;

public class LiquidFill_Tsuji : MonoBehaviour
{
    [SerializeField]
    private Image liquidImage;

    [SerializeField] 
    private float nowFill = 0.5f;

    void Update()
    {       
        liquidImage.fillAmount = nowFill;
    }

    public void SetWasabiAmount(float amount)
    {
        nowFill = amount;
    }
}
