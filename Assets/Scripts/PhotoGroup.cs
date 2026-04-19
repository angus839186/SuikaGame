using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "PhotoGroup", menuName = "ScriptableObjects/PhotoGroup", order = 1)]
public class PhotoGroup : ScriptableObject
{
    [Header("劇照")]
    public Sprite ChineseBackground;
    public Sprite EnglishBackground;

    public VideoClip videoClip;


}