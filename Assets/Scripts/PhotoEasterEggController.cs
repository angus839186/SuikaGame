using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoEasterEggController : MonoBehaviour
{
    public EasterEgg[] easterEggs;
}

public class EasterEgg
{
    public GameObject instance;
    public bool isActive;
}