using UnityEngine;

public class TempEnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}
