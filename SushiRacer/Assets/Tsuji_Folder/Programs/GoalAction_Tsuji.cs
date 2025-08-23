using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct GoalObjects
{
    public GameObject obj;
    public CheckPointDir dir;
    public int count;

    public GoalObjects(GameObject _obj, CheckPointDir _dir, int _count)
    {
        obj = _obj;
        dir = _dir;
        count = _count;
    }
}

public class GoalAction_Tsuji : MonoBehaviour
{
    [SerializeField]
    private Camera goalCamera = null;

    [SerializeField]
    private Camera cameraP1 = null;

    public Camera CameraP1 => cameraP1;

    [SerializeField]
    private Camera cameraP2 = null;

    public Camera CameraP2 => cameraP2;

    [SerializeField]
    private Canvas canvasTsuji = null;

    [SerializeField]
    private float waitTime = 0.0f;

    List<GoalObjects> goalObjs = new List<GoalObjects>();

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
        GameObject obj = collider.gameObject;
        if (obj.layer == playerLayer)
        {
            Vector3 goalPos = transform.position;
            goalPos.y = 0.0f;

            Vector3 racerPos = obj.transform.position;
            racerPos.y = 0.0f;

            Vector3 wallfromPlayerDir = racerPos - goalPos;

            float dot = Vector3.Dot(transform.forward.normalized, wallfromPlayerDir.normalized);

            if (dot < 0.0f)
            {
                for (int i = 0; i < goalObjs.Count; i++)
                {
                    if (goalObjs[i].obj == obj)
                    {
                        var temp = goalObjs[i];
                        temp.dir = CheckPointDir.ForwardDirection;
                        goalObjs[i] = temp;
                        return;
                    }
                }

                goalObjs.Add(new GoalObjects(obj, CheckPointDir.ForwardDirection, 0));

            }
            else if (dot > 0.0f)
            {
                for (int i = 0; i < goalObjs.Count; i++)
                {
                    if (goalObjs[i].obj == obj)
                    {
                        var temp = goalObjs[i];
                        temp.dir = CheckPointDir.ReverseDirection;
                        goalObjs[i] = temp;
                        return;
                    }
                }

                goalObjs.Add(new GoalObjects(obj, CheckPointDir.ReverseDirection, -1));
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        GameObject obj = collider.gameObject;
        if (obj.layer == playerLayer)
        {
            Vector3 goalPos = transform.position;
            goalPos.y = 0.0f;

            Vector3 racerPos = obj.transform.position;
            racerPos.y = 0.0f;

            Vector3 wallfromPlayerDir = racerPos - goalPos;

            float dot = Vector3.Dot(transform.forward.normalized, wallfromPlayerDir.normalized);

            if (dot > 0.0f)
            {
                if (goalObjs.Count > 0 && onGoalFlag == false)
                {
                    for (int i = 0; i < goalObjs.Count; i++)
                    {
                        if (goalObjs[i].obj == obj &&
                            goalObjs[i].dir == CheckPointDir.ForwardDirection)
                        {
                            if (goalObjs[i].count >= 0)
                            {
                                onGoalFlag = true;
                                break;
                            }
                            
                            var temp = goalObjs[i];
                            temp.count = 1;
                            goalObjs[i] = temp; // Œ³‚ÌƒŠƒXƒg‚É”½‰f
                        }
                    }
                }
            }
            else if (dot < 0.0f)
            {
//                Debug.Log("‹t•ûŒü”²‚¯");
            }
        }
    }

    IEnumerator CameraChange()
    {
        yield return new WaitForSeconds(waitTime);
        goalCamera.gameObject.SetActive(true); // ƒS[ƒ‹ƒJƒƒ‰‚ð—LŒø‚É‚·‚é
        NonActives();
    }

    void NonActives()
    {
        PlayerKeeper_Tsuji.instance.GetCamera1().gameObject.SetActive(false);
        PlayerKeeper_Tsuji.instance.GetCamera2().gameObject.SetActive(false);
        //cameraP1.gameObject.SetActive(false);
        //cameraP2.gameObject.SetActive(false);
    }

    public bool GetGoalFlag()
    {
        return onGoalFlag;
    }
}
