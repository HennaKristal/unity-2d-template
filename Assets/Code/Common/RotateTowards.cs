using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private Transform target;

    private void Start()
    {
        if (target == null)
        {
            GameManager.Instance.GetPlayerTransform();
        }
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
