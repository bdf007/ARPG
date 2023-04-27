using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{

    public Image fill;

    public static HealthBarUI instance;

    // instantiate HealthBarUi
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void UpdateFill(int curHp, int maxHp)
    {
        fill.fillAmount = (float)curHp / (float)maxHp;
    }
}
