using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class Level3Story : MonoBehaviour {

    [SerializeField]
    private Camera2DFollow m_CameraFollow;

    [SerializeField]
    private Transform m_Ship;

    public void StartCameraFollow()
    {
        m_CameraFollow.enabled = true;
    }

    public void SwitchCameraFollow()
    {
        m_CameraFollow.target = m_Ship;
    }
}
