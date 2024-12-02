using System.Collections;
using UnityEngine;

public class RulesCheckers : MonoBehaviour
{
    public void Show()
    {
        GetComponent<Animation>().Play("rulesCheckerOpen");
    }
    public void Hide()
    {
        GetComponent<Animation>().Play("rulesCheckerClose");
    }
}
