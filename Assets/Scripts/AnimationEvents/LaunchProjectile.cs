using UnityEditor.ShaderGraph.Legacy;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject Bullet;
    public Transform FirePoint;

    public void LaunchBullet()
    {
        Vector2 mousePositon = Mouse.current.position.ReadValue();
        Vector2 fireScreenPosition = Camera.main.WorldToScreenPoint(FirePoint.position);

        Vector2 direction = mousePositon - fireScreenPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Instantiate(Bullet, FirePoint.position, rotation);
    }
}
