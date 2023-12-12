using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10;
    [SerializeField] float projectileLifetime = 5;
    [SerializeField] float firingRate = .2f;

    const float xOffset = .27f;
    bool isFiring;
    Coroutine firingCoroutine;

    void Start()
    {
        
    }

    void Update()
    {
        Fire();
    }

    public void SetIsFiring(bool isFiring)
    {
        this.isFiring = isFiring;
    }

    public bool GetIsFiring()
    {
        return isFiring;
    }

    void Fire()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while(true)
        {
            Vector3 position = transform.position;
            position.x += xOffset * Mathf.Sign(transform.localScale.x);
            GameObject instance = Instantiate(projectilePrefab, position, Quaternion.identity);

            Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
            if (rb != null )
            {
                rb.velocity = transform.right * projectileSpeed * Mathf.Sign(transform.localScale.x);
            }
            Destroy(instance, projectileLifetime);
            yield return new WaitForSeconds(firingRate);
        }
    }
}
