using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSelector : MonoBehaviour
{
    public GameObject[] lockedHeads;
    public GameObject[] unlockedHeads;
    public GameObject[] justifiedPanel;
    public GameObject[] particles;


    void OnEnable()
    {
        foreach (var item in particles)
        {
            if (item)
            {
                item.SetActive(false);
            }
        }
        foreach (var item in justifiedPanel)
        {
            if (item)
            {
                item.SetActive(false);
            }
        }

        int limit = ((GameManager.Instance.levelNumber - 1) / IngameUI.Instance.levelsAfterWhichANewHeadIsSpawned) + 3;
        limit += HeadSpawner.Instance.GetNumberOfJustifiedHeads();
        limit = Mathf.Clamp(limit, 0, HeadSpawner.Instance.headPrefabs.Length);

        for (int i = 0; i < limit; i++)
        {
            unlockedHeads[i].SetActive(true);
            lockedHeads[i].SetActive(false);

            if (HeadSpawner.Instance.CheckIfJustified(i))
            {
                unlockedHeads[i].SetActive(false);
                justifiedPanel[i].SetActive(true);
            }

            if (GameManager.Instance.levelNumber == 1)
            {
                particles[i].SetActive(true);
            }
            else if (i == (limit - 1))
            {
                particles[i].SetActive(true);
            }
        }
        for (int i = limit; i < HeadSpawner.Instance.headPrefabs.Length; i++)
        {
            unlockedHeads[i].SetActive(false);
            lockedHeads[i].SetActive(true);
        }
    }
}