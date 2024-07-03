using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private LookAtMode _lookAtMode;
     
    private enum LookAtMode
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    private void LateUpdate()
    {
        switch (_lookAtMode)
        {
            case LookAtMode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case LookAtMode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;           
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case LookAtMode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
                case LookAtMode.CameraForwardInverted:
                    transform.forward = -Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
