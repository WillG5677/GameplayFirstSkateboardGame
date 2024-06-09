using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteExplosionTest : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> splitSprites = new List<Sprite>();
    [SerializeField]
    private float explosionForce = 1.5f;
    [SerializeField]
    private float explosionRadius = 1.0f;

    private bool exploded = false;

    private void OnMouseDown()
    {
        if (!exploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        foreach (Sprite sprite in splitSprites)
        {
            GameObject go = new GameObject(sprite.name);
            go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            go.transform.SetPositionAndRotation(transform.position, Random.rotation);
            go.AddComponent<SpriteRenderer>().sprite = sprite;
            go.AddComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius);
            go.AddComponent<BoxCollider>();
        }
        Destroy(gameObject);
    }
}
