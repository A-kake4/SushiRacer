using UnityEngine;

public class BombCount_Tsuji : MonoBehaviour
{
    private float bombCount = 0.0f;
    private float countMax = 1.0f;

    private bool isCounting = false;

    private void OnEnable()
    {
        // ‚±‚±‚ÅƒTƒEƒ“ƒh‚ð–Â‚ç‚·
        bombCount = 0.0f;
        isCounting = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isCounting)
        {
            bombCount += Time.fixedDeltaTime;
        }

        if(bombCount >= countMax)
        {
            isCounting = false;
            gameObject.SetActive(false);
        }
    }
}
