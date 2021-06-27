using UnityEngine;

public class Updraft : MonoBehaviour
{
    [SerializeField]
    private float force = 10.0f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player")) return;

        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.AddForce(0, force, 0);
    }
}
