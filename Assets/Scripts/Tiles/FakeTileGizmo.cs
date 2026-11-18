using UnityEngine;

public class FakeTileGizmo : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(base.transform.position, Vector3.one * 0.5f);
	}
}
