using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class iconManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        
    }

    [Header("회전속도 조절")]
    [SerializeField]
    [Range(1f, 100f)] float rotateSpeed = 50f;
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * rotateSpeed, Space.Self);
        //transform.position = new Vector2(Time.deltaTime *rotateSpeed, Time.deltaTime * rotateSpeed);
    }
}
