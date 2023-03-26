using System.Linq;
using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
    [Tooltip("The list of objects to be toggled simultaneously.")]
    public GameObject[] inputObjects;
    [Tooltip("The list of tags that will trigger the given object to be toggled.")]
    public string[] triggerTags; 

    bool activated = false;

    //Toggles some gameobject on collision with this gameobject.
    //This requires that the trigger object (The gameobject this script is applied to)
    //have a collider and have "isTrigger" true.
    private void OnTriggerEnter(Collider collider)
    {
        if (!activated) //Activate only once.
        {
            string colTag = collider.gameObject.tag;
            if (triggerTags.Contains(colTag))
            {
                foreach (var obj in inputObjects) //Toggle each object
                {
                    //Debug.Log("Collide! Toggle. From: " + collider.name);
                    bool state = obj.activeSelf;
                    //Debug.Log(state + " to " + (!state) );
                    obj.SetActive(!obj.activeSelf);
                }
                activated = true;
            }
        }
    }
}
