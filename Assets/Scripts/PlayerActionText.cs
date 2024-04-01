using UnityEngine;

public class PlayerActionText : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float fadeTime = 5f;


    private void Update()
    {
        // Yukar� do�ru kayd�r
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime, 0f);

        Destroy(gameObject, fadeTime);
    }
}
