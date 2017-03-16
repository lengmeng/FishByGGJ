using UnityEngine;

using UnityStandardAssets.CrossPlatformInput;

public class UserController : MonoBehaviour
{
    [SerializeField]
    private Shoal m_Character;

    private bool m_IsCloseInput = false;

    private void FixedUpdate()
    {
        if (m_IsCloseInput) return;
        float xMove = CrossPlatformInputManager.GetAxis("Horizontal");
        float yMove = CrossPlatformInputManager.GetAxis("Vertical");
        m_Character.Move(xMove, yMove);
        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_Character.SwitchFish(Shoal.FishEnum.Speed);
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            m_Character.SwitchFish(Shoal.FishEnum.Hide);
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            m_Character.SwitchFish(Shoal.FishEnum.GreatlyVision);
        }
    }

    public void CloseInput()
    {
        m_IsCloseInput = true;
    }

    public void OpenInput()
    {
        m_IsCloseInput = false;
    }
}
