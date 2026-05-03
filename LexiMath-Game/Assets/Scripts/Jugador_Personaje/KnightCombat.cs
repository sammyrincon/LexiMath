using UnityEngine;

public class KnightCombat : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;

    void Awake()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}
