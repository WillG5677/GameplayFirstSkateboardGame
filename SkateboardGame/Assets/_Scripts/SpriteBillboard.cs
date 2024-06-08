using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    private enum BillboardMode
    {
        lookAtCam,
        copyCamRot,
        lockedXZ
    }

    [SerializeField]
    private BillboardMode billboardMode;

    private Camera sceneCam;

    private void Awake()
    {
        sceneCam = Camera.main;
    }

    private void Update()
    {
        switch (billboardMode)
        {
            case BillboardMode.lookAtCam:
                transform.LookAt(sceneCam.transform);
                break;
            case BillboardMode.copyCamRot:
                transform.rotation = sceneCam.transform.rotation;
                break;
            case BillboardMode.lockedXZ:
                Vector3 camRot = sceneCam.transform.rotation.eulerAngles;
                camRot.x = camRot.z = 0;
                transform.eulerAngles = camRot;
                break;
        }
    }
}
