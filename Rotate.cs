using UnityEngine;
public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 20;

    void Update()
    {
        if(IngameUI.isPlaying)
            this.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.unscaledDeltaTime));
    }

    public void ChangeRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
}