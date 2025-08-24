using UnityEngine;

public class RotateSet : MonoBehaviour
{
    [SerializeField,Header("���ʂ��ǂ̕����Ɍ����邩�H")]
    private Vector3 rotateValue;

    [SerializeField,Header("������܂ł̎���")]
    private float rotateSpeed = 1.0f;

    private Transform myTransform;

    private bool isRotate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTransform = this.transform;
    }

    public void StartRotate(Vector3 addRotate)
    {
        rotateValue = addRotate;
        isRotate = true;
    }

    private void FixedUpdate()
    {
        if (isRotate)
        {
            Quaternion target = Quaternion.Euler(rotateValue);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, target, Time.deltaTime * rotateSpeed);
            if (Quaternion.Angle(myTransform.rotation, target) < 0.1f)
            {
                myTransform.rotation = target;
                isRotate = false;
            }
        }
    }

}
