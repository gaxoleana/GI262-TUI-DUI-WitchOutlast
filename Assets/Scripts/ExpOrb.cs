using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ExpOrb : MonoBehaviour
{
    [SerializeField, Min(1)] private int expAmount = 20;

    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        Player player = other.GetComponent<Player>();

        if (player == null || player.CurrentHealth <= 0)
        {
            return;
        }

        collected = true;
        player.GainExp(expAmount);
        Destroy(gameObject);
    }
}
