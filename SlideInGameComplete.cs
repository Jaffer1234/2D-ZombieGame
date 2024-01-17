using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideInGameComplete : MonoBehaviour
{
    public GameObject prizePlace;
    public Transform BTCSpawnPosition;
    public Transform ETHSpawnPosition;
    public Transform BNBSpawnPosition;
    public Transform USDTSpawnPosition;
    public Transform XRPSpawnPosition;
    public Transform DOGESpawnPosition;
    public GameObject BTCPrefab;
    public GameObject ETHPrefab;
    public GameObject BNBPrefab;
    public GameObject USDTPrefab;
    public GameObject XRPPrefab;
    public GameObject DOGEPrefab;
    public GameObject BTCObject;
    public GameObject ETHObject;
    public GameObject BNBObject;
    public GameObject USDTObject;
    public GameObject XRPObject;
    public GameObject DOGEObject;

    public Text BTC;
    public Text ETH;
    public Text BNB;
    public Text USDT;
    public Text XRP;
    public Text DOGE;

    private int BTCCollected;
    private int ETHCollected;
    private int BNBCollected;
    private int USDTCollected;
    private int XRPCollected;
    private int DOGECollected;

    void OnEnable()
    {
        StartCoroutine(SlideValues());
    }
    IEnumerator SlideValues()
    {
        BTCObject.SetActive(false);
        ETHObject.SetActive(false);
        BNBObject.SetActive(false);
        USDTObject.SetActive(false);
        XRPObject.SetActive(false);
        DOGEObject.SetActive(false);

        BTCCollected = IngameUI.Instance.CollectionOfBTC - IngameUI.Instance.BTCCollectedAtStart;
        ETHCollected = IngameUI.Instance.CollectionOfETH - IngameUI.Instance.ETHCollectedAtStart;
        BNBCollected = IngameUI.Instance.CollectionOfBNB - IngameUI.Instance.BNBCollectedAtStart;
        USDTCollected = IngameUI.Instance.CollectionOfUSDT - IngameUI.Instance.USDTCollectedAtStart;
        XRPCollected = IngameUI.Instance.CollectionOfXRP - IngameUI.Instance.XRPCollectedAtStart;
        DOGECollected = IngameUI.Instance.CollectionOfDOGE - IngameUI.Instance.DOGECollectedAtStart;

        if (BTCCollected > 0)
        {
            BTCObject.SetActive(true);
            BTC.text = BTCCollected.ToString();
        }
        if (ETHCollected > 0)
        {
            ETHObject.SetActive(true);
            ETH.text = ETHCollected.ToString();
        }
        if (BNBCollected > 0)
        {
            BNBObject.SetActive(true);
            BNB.text = BNBCollected.ToString();
        }
        if (USDTCollected > 0)
        {
            USDTObject.SetActive(true);
            USDT.text = USDTCollected.ToString();
        }
        if (XRPCollected > 0)
        {
            XRPObject.SetActive(true);
            XRP.text = XRPCollected.ToString();
        }
        if (DOGECollected > 0)
        {
            DOGEObject.SetActive(true);
            DOGE.text = DOGECollected.ToString();
        }

        yield return null;

        while (IngameUI.Instance.justifiedHeadsRewardsPanel.activeInHierarchy)
        {
            yield return null;
        }
        while (IngameUI.Instance.justifyHeadPanel.activeInHierarchy)
        {
            yield return null;
        }

        if (BTCCollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.BTCCollectedAtStart).ToString();
            AnimatePrizePlace(0);
            yield return new WaitForSeconds(1f);
            float divisions = BTC.GetComponent<SlideNumber>().textAnimationTime / (float)BTCCollected;
            BTC.GetComponent<SlideNumber>().SetNumberForSliding(BTCCollected, 0);

            for (int i = 0; i < BTCCollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(BTCPrefab, BTCSpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(0, true, prizePlace.transform);
                if(i != BTCCollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(BTCObject);
        }
        if (ETHCollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.ETHCollectedAtStart).ToString();
            AnimatePrizePlace(1);
            yield return new WaitForSeconds(1f);
            float divisions = ETH.GetComponent<SlideNumber>().textAnimationTime / (float)ETHCollected;
            ETH.text = ETHCollected.ToString();
            ETH.GetComponent<SlideNumber>().SetNumberForSliding(ETHCollected, 0);

            for (int i = 0; i < ETHCollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(ETHPrefab, ETHSpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(1, true, prizePlace.transform);
                if (i != ETHCollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(ETHObject);
        }
        if (BNBCollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.BNBCollectedAtStart).ToString();
            AnimatePrizePlace(2);
            yield return new WaitForSeconds(1f);
            float divisions = BNB.GetComponent<SlideNumber>().textAnimationTime / (float)BNBCollected;
            BNB.text = BNBCollected.ToString();
            BNB.GetComponent<SlideNumber>().SetNumberForSliding(BNBCollected, 0);

            for (int i = 0; i < BNBCollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(BNBPrefab, BNBSpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(2, true, prizePlace.transform);
                if (i != BNBCollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(BNBObject);
        }
        if (USDTCollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.USDTCollectedAtStart).ToString();
            AnimatePrizePlace(3);
            yield return new WaitForSeconds(1f);
            float divisions = USDT.GetComponent<SlideNumber>().textAnimationTime / (float)USDTCollected;
            USDT.text = USDTCollected.ToString();
            USDT.GetComponent<SlideNumber>().SetNumberForSliding(USDTCollected, 0);

            for (int i = 0; i < USDTCollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(USDTPrefab, USDTSpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(3, true, prizePlace.transform);
                if (i != USDTCollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(USDTObject);
        }
        if (XRPCollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.XRPCollectedAtStart).ToString();
            AnimatePrizePlace(4);
            yield return new WaitForSeconds(1f);
            float divisions = XRP.GetComponent<SlideNumber>().textAnimationTime / (float)XRPCollected;
            XRP.text = XRPCollected.ToString();
            XRP.GetComponent<SlideNumber>().SetNumberForSliding(XRPCollected, 0);

            for (int i = 0; i < XRPCollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(XRPPrefab, XRPSpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(4, true, prizePlace.transform);
                if (i != XRPCollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(XRPObject);
        }
        if (DOGECollected > 0)
        {
            prizePlace.GetComponent<PrizeAssigner>().count.text = (IngameUI.Instance.DOGECollectedAtStart).ToString();
            AnimatePrizePlace(5);
            yield return new WaitForSeconds(1f);
            float divisions = DOGE.GetComponent<SlideNumber>().textAnimationTime / (float)DOGECollected;
            DOGE.text = DOGECollected.ToString();
            DOGE.GetComponent<SlideNumber>().SetNumberForSliding(DOGECollected, 0);

            for (int i = 0; i < DOGECollected; i++)
            {
                SoundManager.Instance.PlayCoinGoing();
                GameObject obj = Instantiate(DOGEPrefab, DOGESpawnPosition.position, Quaternion.identity);
                obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                obj.GetComponent<PrizeAnimate>().AssignIndex(5, true, prizePlace.transform);
                if (i != DOGECollected - 1)
                    yield return new WaitForSeconds(divisions);
            }
            yield return new WaitForSeconds(0.25f);
            Hide(DOGEObject);

            yield return new WaitForSeconds(1f);
            this.gameObject.SetActive(false);
        }
    }
    void AnimatePrizePlace(int index)
    {
        return;
        Animator anim = prizePlace.gameObject.GetComponent<Animator>();
        anim.Rebind();
        anim.Update(0f);
        prizePlace.gameObject.GetComponent<PrizeAssigner>().AssignThings(index);
        prizePlace.gameObject.GetComponent<PrizeAssigner>().StopCoroutines();
        anim.SetTrigger("in");
    }
    void Hide(GameObject obj)
    {
        //Image[] images = obj.GetComponentsInChildren<Image>();
        //foreach (var item in images)
        //{
        //    Color c = item.color;
        //    c.a = 0f;
        //    item.color = c;
        //}
        //Text[] texts = obj.GetComponentsInChildren<Text>();
        //foreach (var item in texts)
        //{
        //    Color c = item.color;
        //    c.a = 0f;
        //    item.color = c;
        //}
    }
}
