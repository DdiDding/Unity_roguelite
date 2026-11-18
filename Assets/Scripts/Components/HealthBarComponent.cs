using UnityEngine;

public sealed class HealthBarComponent : MonoBehaviour
{
    [SerializeField] private StatComponent stats;
    [SerializeField] private Transform fill;

    private void LateUpdate()
    {
        if (stats == null || fill == null)
            return;

        fill.localScale = new Vector3(
            Mathf.Clamp01((float)stats.CurrentHp / stats.MaxHp), 1f, 1f);
    }
}
