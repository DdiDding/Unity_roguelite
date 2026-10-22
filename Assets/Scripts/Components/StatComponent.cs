using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class StatComponent : MonoBehaviour
{
    [SerializeField, Min(0)] private int attack = 10;
    [SerializeField, Min(0)] private int defense = 0;
    [SerializeField, Min(1)] private int maxHp = 100;
    [SerializeField] private SpriteRenderer damageSprite;

    private Color originalColor;
    private Coroutine damageFlashRoutine;

    public int Attack => Mathf.Max(0, attack);
    public int Defense => Mathf.Max(0, defense);
    public int MaxHp => Mathf.Max(1, maxHp);
    public int CurrentHp { get; private set; }
    public bool IsDead => CurrentHp <= 0;

    private void Awake()
    {
        if (damageSprite == null) damageSprite = GetComponentInChildren<SpriteRenderer>(true);
        if (damageSprite != null) originalColor = damageSprite.color;
        ResetHealth();
    }

    private void OnDisable()
    {
        if (damageFlashRoutine == null) return;
        StopCoroutine(damageFlashRoutine);
        damageFlashRoutine = null;
        if (damageSprite != null) damageSprite.color = originalColor;
    }

    // shortcut: Move hit feedback to a visual component when more effects are added.
    private IEnumerator FlashDamage()
    {
        damageSprite.color = new Color(1f, 0f, 0f, originalColor.a);
        yield return new WaitForSeconds(0.1f);
        if (damageSprite != null) damageSprite.color = originalColor;
        damageFlashRoutine = null;
    }

    public void ResetHealth()
    {
        CurrentHp = MaxHp;
    }

    public int TakeDamage(int incomingAttack)
    {
        if (incomingAttack <= 0 || IsDead) return 0;

        int damage = Mathf.Max(0, incomingAttack - Defense);
        int appliedDamage = Mathf.Min(CurrentHp, damage);
        CurrentHp -= appliedDamage;

        if (appliedDamage > 0 && damageSprite != null && isActiveAndEnabled)
        {
            if (damageFlashRoutine != null) StopCoroutine(damageFlashRoutine);
            damageFlashRoutine = StartCoroutine(FlashDamage());
        }
        Debug.Log($"{gameObject.name} 가 데미지 {incomingAttack} 받음, 현재 HP : {CurrentHp}");
        return appliedDamage;
    }

    public int Heal(int amount)
    {
        if (amount <= 0 || IsDead) return 0;

        int restoredHp = Mathf.Min(amount, MaxHp - CurrentHp);

        CurrentHp += restoredHp;
        return restoredHp;
    }
}
