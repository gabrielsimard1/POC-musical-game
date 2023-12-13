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
    bool canShoot = true;
    Coroutine firingCoroutine;

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

    public void SetCanShoot(bool canShoot)
    {
        this.canShoot = canShoot;
    }

    void Fire()
    {
        if (canShoot && isFiring && firingCoroutine == null)
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
        while(canShoot)
        {
            Vector3 position = transform.position;
            position.x += xOffset * Mathf.Sign(transform.localScale.x);
            GameObject instance = Instantiate(projectilePrefab, position, Quaternion.identity);

            if (instance.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.velocity = transform.right * projectileSpeed * Mathf.Sign(transform.localScale.x);
            }
            Destroy(instance, projectileLifetime);
            yield return new WaitForSeconds(firingRate);
        }
    }
}
