using UnityEngine;
using System.Collections.Generic;

public class BombsEffect_Tsuji : MonoBehaviour
{
    [SerializeField]
    List<GameObject> boms = new List<GameObject>();
    [SerializeField]
    private GoalAction_Tsuji goalAction;

    private float timer = 0.0f;
    private float coolTimer = 0.0f;
    private bool isCoolOn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(goalAction == null) return;

        if(goalAction.GetGoalFlag() == false)
            return;

        if (boms.Count == 0)
        {
            return;
        }

        //timer += Time.fixedDeltaTime;

        //if (timer < 1.0f)
        //{
        //    return;
        //}

        if (isCoolOn)
        {
            // クールダウン
            coolTimer += Time.fixedDeltaTime;
            if(coolTimer > 0.1f)
            {
                coolTimer = 0.0f;
                isCoolOn = false;
            }
        }
        else
        {
            int activateCount = 2; // 一度に有効化する数（例:3つ）
            int maxTries = 100;

            for (int j = 0; j < activateCount; j++)
            {
                for (int i = 0; i < maxTries; i++)
                {
                    int randomIndex = Random.Range(0, boms.Count);
                    if (!boms[randomIndex].activeSelf)
                    {
                        boms[randomIndex].SetActive(true);
                        break; // 次のbomb探しへ
                    }
                }
            }

            isCoolOn = true;
            return;
        }
    }
}
