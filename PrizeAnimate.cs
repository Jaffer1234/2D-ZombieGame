using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeAnimate : MonoBehaviour
{
    private bool isMenu;
    private float speed = 100;
    private Transform targetTransform;
    private Transform target;

    private int index;

    private bool goingUp = false;
    private bool isWaiting = false;
    //private Vector3 positionUp;
    // Start is called before the first frame update

    public void AssignIndex(int _index, bool _isMenu, Transform _target = null)
    {
        return;
        isMenu = _isMenu;
        index = _index;
        targetTransform = _target;
        targetTransform.gameObject.GetComponent<PrizeAssigner>().StopCoroutines();
        target = targetTransform.gameObject.GetComponent<PrizeAssigner>().icon.transform;
        if (!isMenu)
        {
            targetTransform.gameObject.GetComponent<PrizeAssigner>().AssignThings(index);
            Animator anim = targetTransform.gameObject.GetComponent<Animator>();
            anim.Rebind();
            anim.Update(0f);
            anim.SetTrigger("in");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(goingUp)
        //{
        //    transform.position = Vector3.MoveTowards(this.transform.position, positionUp, 10 * Time.deltaTime);

        //    if (Utility.Distance(this.transform.position, positionUp) < 0.1f)
        //    {
        //        if (isWaiting == false)
        //        {
        //            speed = 0;
        //            isWaiting = true;
        //            StartCoroutine(ResetSpeed());
        //        }
        //    }
        //}
        //else
        {
            //if (targetTransform == null)
            //{
            //    DisplayItemValues[] itemValues = FindObjectsOfType<DisplayItemValues>();
            //    foreach (var item in itemValues)
            //    {
            //        item.ShowCryptoValues();
            //    }

            //    Destroy(this.gameObject);
            //}

            if (Time.timeScale > 0)
                transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed);

            if (Utility.Distance(this.transform.position, target.position) < 0.1f)
            {
                //if(isMenu)
                {
                    SoundManager.Instance.PlayCoinResting();
                    targetTransform.gameObject.GetComponent<PrizeAssigner>().StopCoroutines();
                    targetTransform.gameObject.GetComponent<PrizeAssigner>().IncreaseCount();
                }
                //else
                //{
                //    targetTransform.gameObject.transform.parent.gameObject.GetComponentInChildren<DisplayItemValues>().ShowCryptoValues();
                //}
                Destroy(this.gameObject);
            }
        }    
    }
    IEnumerator ResetSpeed()
    {
        WaitForSeconds ws = new WaitForSeconds(0.02f);
        yield return ws;
        speed = 100;
        goingUp = false;
    }
}
