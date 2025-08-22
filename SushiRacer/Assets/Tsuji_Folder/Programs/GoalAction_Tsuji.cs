using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalAction_Tsuji : MonoBehaviour
{
    private GameObject goalObj = null;
    [SerializeField]
    private Camera goalCamera = null;

    [SerializeField]
    private Camera cameraP1 = null;

    [SerializeField]
    private Camera cameraP2 = null;

    [SerializeField]
    private Canvas canvasTsuji = null;

    [SerializeField]
    private float waitTime = 0.0f;

    private bool onGoalFlag = false;

    private void FixedUpdate()
    {
        if (onGoalFlag == true)
        {
            if (canvasTsuji != null)
            {
                canvasTsuji.gameObject.SetActive(false);

                StartCoroutine(CameraChange());
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        if (collider.gameObject.layer == playerLayer && goalObj == null)
        {
            goalObj = collider.gameObject;
            onGoalFlag = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        if (collision.gameObject.layer == playerLayer && goalObj == null)
        {
            goalObj = collision.gameObject;
            onGoalFlag = true;
        }
    }

    IEnumerator CameraChange()
    {
        yield return new WaitForSeconds(waitTime);
        goalCamera.gameObject.SetActive(true); // ÉSÅ[ÉãÉJÉÅÉâÇóLå¯Ç…Ç∑ÇÈ
        NonActives();
    }

    void NonActives()
    {
        cameraP1.gameObject.SetActive(false);
        cameraP2.gameObject.SetActive(false);
    }

    public bool GetGoalFlag()
    {
        return onGoalFlag;
    }
}
