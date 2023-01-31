using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public int healthValue;

    public int UseItem()
    {

        return healthValue;
    }
}
