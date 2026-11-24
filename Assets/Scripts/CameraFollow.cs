using UnityEngine;
namespace Aurora
{
    public sealed class CameraFollow : MonoBehaviour
    {
        public Transform target; Vector3 velocity;
        Vector3 Destination()
        {
            var camera = GetComponent<Camera>(); float halfWidth = camera.orthographicSize * camera.aspect;
            return new Vector3(Mathf.Clamp(target.position.x, halfWidth - .5f, Mathf.Max(halfWidth - .5f, WorldBuilder.Width - halfWidth - .5f)), Mathf.Clamp(target.position.y, -WorldBuilder.Height + camera.orthographicSize + .5f, -camera.orthographicSize + .5f), -10);
        }
        public void Snap() { if (target != null) { transform.position = Destination(); velocity = Vector3.zero; } }
        void LateUpdate() { if (target != null) transform.position = Vector3.SmoothDamp(transform.position, Destination(), ref velocity, .12f); }
    }
}
