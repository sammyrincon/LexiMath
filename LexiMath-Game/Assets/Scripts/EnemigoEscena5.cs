using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private GameObject hitbox;

    public void EnableHitbox()  { if (hitbox) hitbox.SetActive(true); }
    public void DisableHitbox() { if (hitbox) hitbox.SetActive(false); }
}