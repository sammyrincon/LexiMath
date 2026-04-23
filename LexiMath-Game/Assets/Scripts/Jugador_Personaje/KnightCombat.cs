using UnityEngine;

public class KnightCombat : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox; // arrastra KnightHitbox aquí

    void Awake()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    // Animation Event: en el frame donde pega
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(true);
    }

    // Animation Event: cuando deja de pegar
    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }
}
