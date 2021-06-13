using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public GameObject deathUI;
    private bool deathUIisOn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<MeshRenderer>().forceRenderingOff = true;
            other.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY;
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(2);
        deathUI.SetActive(true);
        deathUIisOn = true;
    }

    public void Start()
    {
        deathUI.SetActive(false);
        deathUIisOn = false;
    }
}
