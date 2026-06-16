using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int spriteID;
    private int id;
    private bool isFlipped;
    private bool turning;
    [SerializeField]
    private Image img;
    public bool IsBlankCard => spriteID == -2;
    public bool IsFlipped => isFlipped;

    private bool matched;

    public bool IsMatched => matched;

    public void SetMatched()
    {
        matched = true;
    }
   
    private IEnumerator CardFlip(Transform thisTransform, float time, bool changeSprite)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }
       
        if (changeSprite)
        {
            isFlipped = !isFlipped;

          
            if (isFlipped)
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            ChangeSprite();
            StartCoroutine(CardFlip(transform, time, false));
        }
        else
            turning = false;
    }
    
    public void Flip()
    {
        AudioManager.Instance.PlaySound(0);
        turning = true;
        StartCoroutine(CardFlip(transform, 0.25f, true));
        Debug.Log("flip");
    }

    public void CardBtnEvt()
    {
        Debug.Log($"CLICK -> isFlipped:{isFlipped} turning:{turning} canClick:{CardManager.Instance.canClick()}");

        if (isFlipped)
        {
            Debug.Log("BLOCKED: isFlipped");
            return;
        }

        if (turning)
        {
            Debug.Log("BLOCKED: turning");
            return;
        }

        if (!CardManager.Instance.canClick())
        {
            Debug.Log("BLOCKED: canClick");
            return;
        }

        Debug.Log("FLIPPING");

        Flip();
        StartCoroutine(SelectionEvent());
    }
    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);

        if (spriteID == -2)
        {
            CardManager.Instance.BlankCardClicked(id);
            yield break;
        }

        CardManager.Instance.cardClicked(spriteID, id);
    }

   
    public int SpriteID
    {
        set
        {
            spriteID = value;
            isFlipped = true;
            ChangeSprite();
        }
        get { return spriteID; }
    }
  
    public int ID
    {
        set { id = value; }
        get { return id; }
    }

   
    private void ChangeSprite()
    {
        if (img == null) return;

        if (isFlipped)
        {
            if (spriteID == -2)
                img.sprite = CardManager.Instance.BlankCard();
            else if (spriteID >= 0)
                img.sprite = CardManager.Instance.GetSprite(spriteID);
        }
        else
        {
            img.sprite = CardManager.Instance.CardBack();
        }
    }
   
   
   
   
    public void Inactive()
    {
        StartCoroutine(Fade());
    }
   
    private IEnumerator Fade()
    {
        float rate = 1.0f / 2.5f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            img.color = Color.Lerp(img.color, Color.clear, t);

            yield return null;
        }
    }
  
    public void Active()
    {
        matched = false;

        gameObject.SetActive(true);

        if (img)
            img.color = Color.white;
    }
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);

        isFlipped = false;
        turning = false;

        img.sprite = CardManager.Instance.CardBack();
    }
    public void HideMatched()
    {
        matched = true;

        if (img)
            img.color = Color.clear;
    }
    public void ForceInvisible()
    {
        matched = true;

        if (img)
            img.color = Color.clear;
    }
    public static int CardClick { get;  set; }
    public static int CardMatch { get;  set; }

}
