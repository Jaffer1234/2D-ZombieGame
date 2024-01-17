using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Linq.Expressions;
using UnityEngine.Purchasing;

public class MoveTowards : MonoBehaviour
{
    public enum Action
    {
        move
    }
    public static float speedMultiplier = 1;
    public bool isDevilHead = false;
    public bool isIdGetter = false;
    public bool isCopyHead = false;
    public bool isBossHead = false;

    public bool isFreezingHead = false;
    public Animator freezeAnimator;
    
    public GameObject copyHeadParticles;
    public GameObject trailParticles;

    public Action actionType;
    public Transform[] points;
    private float speed;
    public float _speed { get => speed; set => speed = value; }

    private float reArrangeSpeed;
    public float _reArrangeSpeed { get => reArrangeSpeed; set => reArrangeSpeed = value; }

    private float pipeOpenRearrangeSpeed;
    public float PipeOpenRearrangeSpeed { get => pipeOpenRearrangeSpeed; set => pipeOpenRearrangeSpeed = value; }

    private float reArrangeRotateSpeed;
    public float _reArrangeRotateSpeed { get => reArrangeRotateSpeed; set => reArrangeRotateSpeed = value; }
    
    public int destinationIndex = 0;

    private int index = -1;
    public int _index { get => index; set => index = value; }

    private bool _canMove = false;

    private bool justJumped = false;
    private bool justRearranged = false;
    private bool _jumpToDestination = false;
    private bool onceReachedDestination = false;
    public bool _onceReachedDestination { get => onceReachedDestination; set => onceReachedDestination = value; }
    
    //[HideInInspector]
    public bool atDestination = false;
    public int myLastIndex = 0;
    private GameObject target;
    public GameObject _target { get => target; set => target = value; }

    private GameObject newTarget;
    private bool spawnedAtFirst = false;
    private int levelNumber;

    private bool iHaveBeenCopied = false;

    public bool _iHaveBeenCopied { get => iHaveBeenCopied; set => iHaveBeenCopied = value; }

    public GameObject bossSwapSourceParticles;
    public GameObject bossSwapDestinationParticles;

    private bool isOriginalBossHead;
    public bool IsOriginalBossHead { get => isOriginalBossHead; set => isOriginalBossHead = value; }


    private bool _isFastHead = false;
    public bool isFastHead { get => _isFastHead; set => _isFastHead = value; }

    public static bool bossChangingPosition;

    private void Start()
    {
        levelNumber = GameManager.Instance.levelNumber;
        if (_iHaveBeenCopied)
        {
            return;
        }

        if (!spawnedAtFirst)
        {
            GetDestination();
            this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(350);
        }
        speed = 3;
        reArrangeSpeed = 3;
        PipeOpenRearrangeSpeed = 7;
        reArrangeRotateSpeed = 180;
    }

    public void GetDestination()
    {
        index = HeadPositionsArray.Instance.headPositions.Length;
        if (!HeadPositionsArray.Instance.headsList.Contains(this.gameObject))
        {
            HeadPositionsArray.Instance.headsList.Add(this.gameObject);
        }
        //destinationIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);
        AssignNext();
    }

    public void SpawnedAtStart(int i)
    {
        spawnedAtFirst = true;
        index = i;
        myLastIndex = i;
        destinationIndex = i;
        atDestination = true;
        onceReachedDestination = true;
        this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(0);
        if (!isFreezingHead)
        {
            this.transform.eulerAngles = new Vector3(0, 0, Random.Range(-360, -45));
        }
        else
        {
            freezeAnimator.SetTrigger("freeze");
        }
        if (trailParticles)
        {
            if (trailParticles.activeInHierarchy)
            {
                trailParticles.SetActive(false);
            }
        }
        HeadPositionsArray.Instance.headsList.Add(this.gameObject);
    }

    public void BossDuplicate()
    {
        if ((index - 1) >= 0)
        {
            GameObject leftHead = HeadPositionsArray.Instance.headsList[index - 1];
            if (leftHead.GetComponent<MoveTowards>().isBossHead == false && leftHead.GetComponent<HeadScript>().Id >= 0 && isOriginalBossHead)
            {
                GameObject leftHeadCopy = Instantiate(this.gameObject, leftHead.transform.position, leftHead.transform.rotation);
                MoveTowards leftHeadCopyMovementScript = leftHeadCopy.GetComponent<MoveTowards>();
                MoveTowards leftHeadCopyFromMovementScript = leftHead.GetComponent<MoveTowards>();

                leftHeadCopy.gameObject.name = "BOSS HEAD";

                leftHeadCopyMovementScript._speed = leftHeadCopyFromMovementScript.speed;
                leftHeadCopyMovementScript._reArrangeSpeed = leftHeadCopyFromMovementScript.reArrangeSpeed;
                leftHeadCopyMovementScript.PipeOpenRearrangeSpeed = leftHeadCopyFromMovementScript.PipeOpenRearrangeSpeed;
                leftHeadCopyMovementScript._reArrangeRotateSpeed = leftHeadCopyFromMovementScript.reArrangeRotateSpeed;
                leftHeadCopyMovementScript._index = leftHeadCopyFromMovementScript._index;
                leftHeadCopyMovementScript.destinationIndex = leftHeadCopyFromMovementScript.destinationIndex;
                leftHeadCopyMovementScript.myLastIndex = leftHeadCopyFromMovementScript.myLastIndex;
                leftHeadCopyMovementScript.atDestination = leftHeadCopyFromMovementScript.atDestination;
                leftHeadCopyMovementScript._onceReachedDestination = leftHeadCopyFromMovementScript.onceReachedDestination;
                leftHeadCopyMovementScript._target = leftHeadCopyFromMovementScript.target;
                leftHeadCopyMovementScript._iHaveBeenCopied = true;
                leftHeadCopyMovementScript.gameObject.GetComponentInChildren<Rotate>().ChangeRotationSpeed(leftHeadCopyFromMovementScript.gameObject.GetComponentInChildren<Rotate>().rotationSpeed);

                int leftHeadIndex = HeadPositionsArray.Instance.headsList.IndexOf(leftHead.gameObject);
                //int leftHeadIndex = index - 1;
                HeadPositionsArray.Instance.headsList.Remove(leftHead.gameObject);
                HeadPositionsArray.Instance.headsList.Insert(leftHeadIndex, leftHeadCopy);

                GameObject particle = Instantiate(copyHeadParticles, leftHeadCopy.transform.position, Quaternion.identity);

                Destroy(leftHead.gameObject);
            }
        }
    }

    public void BossChangePosition()
    {
        HeadPositionsArray.Instance.canCheckForPairs = false;
        bossChangingPosition = true;
        int randomPosition = Random.Range(1, HeadPositionsArray.Instance.GetLastAssignedIndex());
        int myIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);

        if (HeadPositionsArray.Instance.headsList[randomPosition].gameObject == this.gameObject)
        {
            if ((randomPosition - 1) == 0)
            {
                randomPosition = randomPosition + 1;
            }
            else
            {
                randomPosition = randomPosition - 1;
            }
        }

        GameObject randomHead = HeadPositionsArray.Instance.headsList[randomPosition].gameObject;

        HeadPositionsArray.Instance.headsList.Remove(randomHead);
        HeadPositionsArray.Instance.headsList.Insert(myIndex, randomHead);
        HeadPositionsArray.Instance.headsList.Remove(this.gameObject);
        HeadPositionsArray.Instance.headsList.Insert(randomPosition, this.gameObject);

        randomHead.GetComponent<MoveTowards>().JumpToDestinationIndex();
        JumpToDestinationIndex();
        StartCoroutine(TeleportEffect(randomHead));
    }

    IEnumerator TeleportEffect(GameObject target)
    {
        Instantiate(bossSwapSourceParticles, this.transform.position, Quaternion.identity);
        Instantiate(bossSwapDestinationParticles, target.transform.position, Quaternion.identity);
        GameObject mySpriteParent = GetComponent<HeadScript>().headSprite.gameObject.transform.parent.gameObject;
        GameObject targetSpriteParent = target.GetComponent<HeadScript>().headSprite.gameObject.transform.parent.gameObject;

        GetComponent<HeadScript>().headSprite.gameObject.GetComponent<Animator>().enabled = false;
        target.GetComponent<HeadScript>().headSprite.gameObject.GetComponent<Animator>().enabled = false;

        GetComponent<HeadScript>().headSprite.gameObject.transform.parent = null;
        target.GetComponent<HeadScript>().headSprite.gameObject.transform.parent = null;
        GetComponent<HeadScript>().bossHealthText.gameObject.SetActive(false);


        StartCoroutine(SpriteFade(GetComponent<HeadScript>().headSprite.GetComponent<SpriteRenderer>()));
        StartCoroutine(SpriteFade(target.GetComponent<HeadScript>().headSprite.GetComponent<SpriteRenderer>()));
        SoundManager.Instance.PlayBossSwapSound();
        yield return new WaitForSeconds(1f);


        GetComponent<HeadScript>().bossHealthText.gameObject.SetActive(true);


        GetComponent<HeadScript>().headSprite.gameObject.GetComponent<Animator>().enabled = true;
        target.GetComponent<HeadScript>().headSprite.gameObject.GetComponent<Animator>().enabled = true;

        GetComponent<HeadScript>().headSprite.gameObject.transform.parent = mySpriteParent.transform;
        target.GetComponent<HeadScript>().headSprite.gameObject.transform.parent = targetSpriteParent.transform;


        GetComponent<HeadScript>().headSprite.gameObject.transform.localPosition = Vector3.zero;
        target.GetComponent<HeadScript>().headSprite.gameObject.transform.localPosition = Vector3.zero;
    }

    IEnumerator SpriteFade(SpriteRenderer sprite)
    {
        float speed = 1f;
        Color c = sprite.color;
        while (c.a > 0.1f)
        {
            c.a = c.a - (Time.deltaTime * speed);

            if (c.a < 0.1f)
            {
                c.a = 0.1f;
            }
            if (sprite != null)
            {
                sprite.color = c;
            }
            else
            {
                StopCoroutine(SpriteFade(sprite));
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        c.a = 1f;
        if (sprite != null)
        {
            sprite.color = c;
        }
        bossChangingPosition = false;
    }

    void Update()
    {
        if (_canMove && IngameUI.isPlaying)
        {
            if (onceReachedDestination)
            {
                speed = reArrangeSpeed;
                int levelNo = levelNumber - 1;
                if (levelNo < 0)
                {
                    levelNo = 0;
                }
                speed += (levelNo * 0.01f);
            }
            transform.position = Vector3.MoveTowards(this.transform.position, target.gameObject.transform.position, Time.deltaTime * speed);

            if (Utility.Distance(this.transform.position, target.gameObject.transform.position) < 0.1f)
            {
                _canMove = false;
                AssignNext();
            }
        }
        if (_jumpToDestination)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, newTarget.gameObject.transform.position, Time.deltaTime * PipeOpenRearrangeSpeed);

            if (Utility.Distance(this.transform.position, newTarget.gameObject.transform.position) < 0.1f)
            {
                this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(0);
                _jumpToDestination = false;
                index = destinationIndex;
                myLastIndex = destinationIndex;
                atDestination = true;
                
                if (isFreezingHead)
                {
                    freezeAnimator.SetTrigger("freeze");
                    freezeAnimator.gameObject.transform.rotation = Quaternion.identity;
                }

                if (isDevilHead)
                {
                    GetComponentInChildren<Rotate>().gameObject.transform.rotation = Quaternion.identity;
                }

                HeadPositionsArray.Instance.jumpCount--;

                if (HeadPositionsArray.Instance.jumpCount <= 0)
                {
                    if (HeadPositionsArray.Instance.jumpCount <= 0)
                    {
                        HeadPositionsArray.Instance.jumpCount = 0;
                    }
                    if (isDevilHead)
                    {
                        if (((index + 1) >= 0 && (index + 1) < HeadPositionsArray.Instance.headPositions.Length) && ((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length))
                        {
                            if (HeadPositionsArray.Instance.headsList.Count > (index + 1))
                            {
                                if (HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().atDestination && HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().atDestination)
                                {
                                    MakeMeDevilHead();
                                    HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().MakeMeDevilHead();
                                    HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().MakeMeDevilHead();
                                    return;
                                }
                            }
                        }
                    }
                    HeadPositionsArray.Instance.canCheckForPairs = true;
                    HeadPositionsArray.Instance.CheckForPairs();
                }
            }
        }
    }

    public void AssignNext()
    {
        if (actionType == Action.move)
        {
            index--;
            destinationIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);

            if (index >= destinationIndex)
            {
                if (target)
                {
                    if (target.GetComponent<IndexProperties>().adoptChange)
                    {
                        if (target.GetComponent<IndexProperties>().showParticles)
                        {
                            this.GetComponent<HeadScript>().PlayHitGroundEffect();
                        }

                        if (target.GetComponent<IndexProperties>().playSound)
                        {
                            if (this.GetComponent<HeadScript>().istBomb || this.GetComponent<HeadScript>().secondBomb)
                            {
                                SoundManager.Instance.PlayBombHitSound();
                            }
                            else
                                SoundManager.Instance.PlayHeadMoveSound();
                        }
                        speed = target.GetComponent<IndexProperties>().movementspeed;
                        if (!onceReachedDestination)
                        {
                            speed = speed * speedMultiplier;
                        }
                        float tempSpeed = speed;
                        int levelNo = levelNumber - 1;
                        if (levelNo < 0)
                        {
                            levelNo = 0;
                        }
                        speed += (levelNo * 0.01f);
                        float increasePercentage = ((speed - tempSpeed) / tempSpeed) * 100;
                        float newRotateSpeed = ((increasePercentage * target.GetComponent<IndexProperties>().rotateSpeed) / 100) + target.GetComponent<IndexProperties>().rotateSpeed;
                        this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(newRotateSpeed);
                    }
                    else
                    {
                        if (justJumped || justRearranged)
                        {
                            if (justRearranged)
                            {
                                justRearranged = false;
                            }
                            if (justJumped)
                            {
                                justJumped = false;
                            }
                            int tempIndex = index;


                            for (int i = tempIndex; i < HeadPositionsArray.Instance.headPositions.Length; i++)
                            {
                                if (HeadPositionsArray.Instance.headPositions[i].GetComponent<IndexProperties>().adoptChange)
                                {
                                    speed = HeadPositionsArray.Instance.headPositions[i].GetComponent<IndexProperties>().movementspeed;
                                    float tempSpeed = speed;
                                    int levelNo = levelNumber - 1;
                                    if (levelNo < 0)
                                    {
                                        levelNo = 0;
                                    }
                                    speed += (levelNo * 0.01f);
                                    float increasePercentage = ((speed - tempSpeed) / tempSpeed) * 100;
                                    float newRotateSpeed = ((increasePercentage * HeadPositionsArray.Instance.headPositions[i].GetComponent<IndexProperties>().rotateSpeed) / 100) + HeadPositionsArray.Instance.headPositions[i].GetComponent<IndexProperties>().rotateSpeed;
                                    this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(newRotateSpeed);

                                    break;
                                }
                            }
                        }
                    }
                }


                target = HeadPositionsArray.Instance.headPositions[index];

                _canMove = true;

                if (isFreezingHead)
                {
                    freezeAnimator.SetTrigger("unfreeze");
                }
            }
            else
            {
                if (destinationIndex != HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject))
                {
                    destinationIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);
                    index++;
                    AssignNext();
                }
                else
                {
                    if ((index + 1) >= 0 && (index + 1) < HeadPositionsArray.Instance.headPositions.Length)
                    {
                        if (HeadPositionsArray.Instance.headPositions[index + 1].gameObject.GetComponent<IndexProperties>().doShake)
                        {
                            EnvironmentScript.Instance.PleaseShakeYourBooty(HeadPositionsArray.Instance.headPositions[index + 1].gameObject.GetComponent<IndexProperties>().shakeDuration);
                        }
                    }
                    StopMoving();
                }
            }
        }
    }
    void CheckForPlaceInList(Collider2D other)
    {
        GameObject aheadHead = other.gameObject.GetComponent<IndexProperties>().temporaryDownHead;

        if (aheadHead)
        {
            int positionInList = HeadPositionsArray.Instance.headsList.IndexOf(aheadHead);
            int myCurrentPosition = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);
            if ((positionInList + 1) != myCurrentPosition)
            {
                HeadPositionsArray.Instance.headsList.RemoveAt(myCurrentPosition);
                HeadPositionsArray.Instance.headsList.Insert((positionInList + 1), this.gameObject);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PipeOpener>())
        {
            other.GetComponentInParent<PipeOpener>().targetHeadPosition.gameObject.GetComponent<IndexProperties>().temporaryHead = this.gameObject;
        }
        if (other.tag == "log")
        {
            if (justJumped && isFastHead)
            {
                CheckForPlaceInList(other);
            }
            other.gameObject.GetComponent<IndexProperties>().temporaryDownHead = this.gameObject;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PipeOpener>())
        {
            other.GetComponentInParent<PipeOpener>().targetHeadPosition.gameObject.GetComponent<IndexProperties>().temporaryHead = null;
        }
    }
    public void JumpToDestinationIndex()
    {
        destinationIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);

        if (myLastIndex != destinationIndex)
        {
            atDestination = false;
            HeadPositionsArray.Instance.jumpCount++;
            newTarget = HeadPositionsArray.Instance.headPositions[destinationIndex];
            this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(reArrangeRotateSpeed);
            _jumpToDestination = true;


            if (isFreezingHead)
            {
                freezeAnimator.SetTrigger("unfreeze");
            }
        }
    }
    public void JumpToIndex(int i, Transform lower = null)
    {
        if (_canMove)
        {
            justJumped = true;
            speed = reArrangeSpeed;
            index = i;
            target = HeadPositionsArray.Instance.headPositions[index];
        }
    }

    public void TurnJustRearrangedTrue()
    {
        justRearranged = true;
    }

    public void StopMoving()
    {
        if (HeadPositionsArray.Instance.rearrangingHeads)
        {
            if (HeadPositionsArray.Instance.matchCount <= 0)
            {
                HeadPositionsArray.Instance.canCheckForPairs = true;
                HeadPositionsArray.Instance.rearrangingHeads = false;
                HeadPositionsArray.Instance.matchCount = 0;
            }
            else
            {
                HeadPositionsArray.Instance.matchCount--;
            }
        }
        atDestination = true;


        if (isFastHead == true)
        {
            isFastHead = false;
        }


        if (iHaveBeenCopied == false)
        {
            index++;
            myLastIndex = index;
            destinationIndex = myLastIndex;
        }
        else
        {
            iHaveBeenCopied = false;
        }

        if (isFreezingHead)
        {
            freezeAnimator.SetTrigger("freeze");
            freezeAnimator.gameObject.transform.rotation = Quaternion.identity;
        }

        if (!onceReachedDestination)
        {
            onceReachedDestination = true;
        }
        if (!isDevilHead)
        {
            if (((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length))
            {
                if (HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().isDevilHead)
                {
                    if (((index - 2) >= 0 && (index - 2) < HeadPositionsArray.Instance.headPositions.Length))
                    {
                        HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().MakeMeDevilHead();
                        MakeMeDevilHead();
                        HeadPositionsArray.Instance.headsList[index - 2].GetComponent<MoveTowards>().MakeMeDevilHead();
                    }
                }
            }
        }
        else
        {
            if (isDevilHead)
            {
                GetComponentInChildren<Rotate>().gameObject.transform.rotation = Quaternion.identity;
            }
            if (((index + 1) >= 0 && (index + 1) < HeadPositionsArray.Instance.headPositions.Length) && ((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length))
            {
                if (HeadPositionsArray.Instance.headsList.Count > (index + 1))
                {
                    if (HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().atDestination && HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().atDestination)
                    {
                        MakeMeDevilHead();
                        HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().MakeMeDevilHead();
                        HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().MakeMeDevilHead();
                    }
                }
            }
        }

        if (isIdGetter)
        {
            if ((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length)
            {
                if (HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().isBossHead == false)
                {
                    int id = HeadPositionsArray.Instance.headsList[index - 1].GetComponent<HeadScript>().Id;

                    HeadPositionsArray.Instance.PopAllHeadsWithSameID(id, this.gameObject);
                    GetComponent<HeadScript>().headSprite.gameObject.GetComponent<Animator>().enabled = false;
                    GetComponent<HeadScript>().headSprite.GetComponent<SpriteRenderer>().sprite = HeadPositionsArray.Instance.headsList[index - 1].GetComponent<HeadScript>().headSprite.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    HeadPositionsArray.Instance.headsList[index - 1].GetComponent<HeadScript>().DecreaseHealth();
                    GetComponent<HeadScript>().PlayEffects();
                    StartCoroutine(BlastHeadAfterSomeTime());
                }
            }
        }

        if (isCopyHead)
        {
            if ((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length)
            {
                GameObject leftHead = HeadPositionsArray.Instance.headsList[index - 1];
                GameObject leftHeadCopy = Instantiate(leftHead, this.transform.position, this.transform.rotation);


                MoveTowards leftHeadCopyMovementScript = leftHeadCopy.GetComponent<MoveTowards>();

                int myIndex = HeadPositionsArray.Instance.headsList.IndexOf(this.gameObject);
                HeadPositionsArray.Instance.headsList.Remove(this.gameObject);
                HeadPositionsArray.Instance.headsList.Insert(myIndex, leftHeadCopy);

                leftHeadCopyMovementScript._speed = speed;
                leftHeadCopyMovementScript._reArrangeSpeed = reArrangeSpeed;
                leftHeadCopyMovementScript._reArrangeRotateSpeed = reArrangeRotateSpeed;
                leftHeadCopyMovementScript.PipeOpenRearrangeSpeed = pipeOpenRearrangeSpeed;
                leftHeadCopyMovementScript._index = myIndex;
                leftHeadCopyMovementScript.myLastIndex = myIndex;
                leftHeadCopyMovementScript.destinationIndex = myIndex;
                leftHeadCopyMovementScript.atDestination = atDestination;
                leftHeadCopyMovementScript._onceReachedDestination = onceReachedDestination;
                leftHeadCopyMovementScript._target = target;
                leftHeadCopyMovementScript._iHaveBeenCopied = true;

                if (HeadPositionsArray.Instance.headsToBlast.Contains(leftHead))
                {
                    HeadPositionsArray.Instance.headsToBlast.Add(leftHeadCopy);
                }
                else
                {
                    GameObject particle = Instantiate(copyHeadParticles, this.transform.position, Quaternion.identity);

                    leftHeadCopyMovementScript.CallStopMovingAfterTime();

                    leftHeadCopy.name = "Head-" + myIndex;
                }


                Destroy(this.gameObject);
                return;
            }
        }

        if (HeadPositionsArray.Instance.rearrangingHeads == false)
        {
            HeadPositionsArray.Instance.CheckForPairs();
        }
        this.GetComponentInChildren<Rotate>().ChangeRotationSpeed(0);

        if (trailParticles)
        {
            if (trailParticles.activeInHierarchy)
            {
                trailParticles.SetActive(false);
            }
        }

        _canMove = false;
    }
    public void CallRoutineBlastAfterTime()
    {
        StartCoroutine(BlastHeadAfterSomeTime());
    }
    IEnumerator BlastHeadAfterSomeTime()
    {
        yield return new WaitForSeconds(1f);
        HeadPositionsArray.Instance.headsList.Remove(this.gameObject);
        GetComponent<HeadScript>().Blast();
    }

    public void CallStopMovingAfterTime()
    {
        StartCoroutine(CallAfterFrames());
    }

    IEnumerator CallAfterFrames()
    {
        yield return new WaitForSeconds(1.5f);
        StopMoving();
    }

    public void MakeMeDevilHead()
    {
        if (isBossHead)
        {
            GetComponent<HeadScript>().DecreaseHealth();
            return;
        }

        HeadPositionsArray.Instance.headsToBlast.Add(this.gameObject);

        int id = 100;
        foreach (var item in HeadPositionsArray.Instance.headsToBlast)
        {
            if (item.GetComponent<MoveTowards>().isDevilHead)
            {
                id = item.GetComponent<HeadScript>().Id;
            }
        }

        GetComponent<HeadScript>().MakeDevilHeads(id);
        if (isDevilHead)
        {
            GetComponent<HeadScript>().PlayDevilEffects();
            if (((index + 1) >= 0 && (index + 1) < HeadPositionsArray.Instance.headPositions.Length))
            {
                if (HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().atDestination)
                {
                    if (HeadPositionsArray.Instance.headsList[index + 1].GetComponent<MoveTowards>().isBossHead == false)
                    {
                        HeadPositionsArray.Instance.headsList[index + 1].GetComponent<HeadScript>().PlayDevilEffects();
                    }
                }
            }
            if (((index - 1) >= 0 && (index - 1) < HeadPositionsArray.Instance.headPositions.Length))
            {
                if (HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().atDestination)
                {
                    if (HeadPositionsArray.Instance.headsList[index - 1].GetComponent<MoveTowards>().isBossHead == false)
                    {
                        HeadPositionsArray.Instance.headsList[index - 1].GetComponent<HeadScript>().PlayDevilEffects();
                    }
                }
            }
            HeadPositionsArray.Instance.rearrangingHeads = true;
            HeadPositionsArray.Instance.canCheckForPairs = false;
        }
    }
}