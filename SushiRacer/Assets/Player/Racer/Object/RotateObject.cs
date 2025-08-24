using UnityEngine;

public class RotationObject : MonoBehaviour
{
    [SerializeField,Header("‰ñ“]Ž²")]
    private Vector3 rotationAxis = new Vector3(0, 1, 0);

    [SerializeField,Header("‰ñ“]‘¬“x")]
    private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    [SerializeField,Header("‰ñ“]‚³‚¹‚é‚©")]
    private bool isRotate = true;
    public bool IsRotate
    {
        get => isRotate;
        set => isRotate = value;
    }

    private Transform myTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myTransform = this.transform;
    }

    private void FixedUpdate()
    {
        if (!isRotate) return;

        myTransform.Rotate(rotationAxis, rotationSpeed.magnitude * Time.fixedDeltaTime);

        if(myTransform.localEulerAngles.x >= 360)
        {
            myTransform.localEulerAngles = new Vector3(0, myTransform.localEulerAngles.y, myTransform.localEulerAngles.z);
        }
    }

}
