using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject Bullet;
    public Transform FirePoint;
    private StatComponent ownerStats;

    public void SetOwner(StatComponent owner)
    {
        ownerStats = owner;
    }

    public void LaunchBullet()
    {
        if (ownerStats == null || ownerStats.IsDead || Bullet == null || FirePoint == null) return;
        Camera mainCamera = Camera.main;
        if (mainCamera == null || Mouse.current == null) return;

        Vector2 mousePositon = Mouse.current.position.ReadValue();
        Vector2 fireScreenPosition = mainCamera.WorldToScreenPoint(FirePoint.position);

        Vector2 direction = mousePositon - fireScreenPosition;
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject instance = Instantiate(Bullet, FirePoint.position, rotation);
        if (instance.TryGetComponent(out TestBullet bullet))
        {
            bullet.Init(ownerStats);
        }
        else
        {
            Debug.LogError("LaunchProjectile: Bullet prefab needs a TestBullet component.", instance);
            Destroy(instance);
        }
    }
}
