using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event Card", menuName = "Cards/Event")]
public class EventCard : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;

}
