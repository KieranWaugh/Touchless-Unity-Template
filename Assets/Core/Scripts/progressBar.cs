using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class progressBar : MonoBehaviour
{
    public int maxValue;
    public int currentValue;

    [SerializeField] private Image bar;
    [SerializeField] private TMP_Text  text;

    

    public void updateBar(int value){
        currentValue = value;
        bar.fillAmount = (float)value/(float)maxValue;
        text.GetComponent<TMP_Text>().text = value + "/" + maxValue;
    }
}
