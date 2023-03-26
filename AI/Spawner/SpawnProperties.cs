using UnityEngine;

public class SpawnProperties : MonoBehaviour
{
    GameObject npc = null;
    SpawnController controller = null;
    public void SetNpc(GameObject input)
    {
        npc = input;
    }

    public GameObject GetNpc()
    {
        return npc;
    }

    public void SetController (SpawnController input)
    {
        controller = input;
    }

    public SpawnController GetController()
    {
        return controller;
    }
}
