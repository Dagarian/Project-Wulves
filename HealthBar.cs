using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Vector3 permanentRotation;
    public GameObject creature;
            
    private void Update()
    {
        //updates healthbar rotation so it can be seen by fixed camera
        //transform.rotation = Quaternion.Euler(permanentRotation);
    }
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
