using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeAssigner : MonoBehaviour
{
    public Sprite[] sprites;
    public Image icon;
    public Text count;
    // Start is called before the first frame update

    public void AssignThings(int index)
    {
        icon.sprite = sprites[index];

        switch (index)
        {
            case 0:
                count.text = (IngameUI.Instance.BTCCollectedAtStart).ToString();
                break;
            case 1:
                count.text = (IngameUI.Instance.ETHCollectedAtStart).ToString();
                break;
            case 2:
                count.text = (IngameUI.Instance.BNBCollectedAtStart).ToString();
                break;
            case 3:
                count.text = (IngameUI.Instance.USDTCollectedAtStart).ToString();
                break;
            case 4:
                count.text = (IngameUI.Instance.XRPCollectedAtStart).ToString();
                break;
            case 5:
                count.text = (IngameUI.Instance.DOGECollectedAtStart).ToString();
                break;
        }
    }
    public void IncreaseCount()
    {
        int countValue= int.Parse(count.text.ToString());
        countValue++;
        count.text = countValue.ToString();
        StartCoroutine(Hide());
    }
    public void StopCoroutines()
    {
        StopAllCoroutines();
    }
    IEnumerator Hide()
    {
        WaitForSeconds ws = new WaitForSeconds(1f);
        yield return ws;
        GetComponent<Animator>().SetTrigger("out");
    }
}
