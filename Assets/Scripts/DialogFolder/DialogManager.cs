using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public Text dialogText;
    public Image TextPointer;
    public bool InDialog;
    public Animator animator;
    public float wait;

    private float speedMultiplier;
    private bool doneTypeing;
    private bool firstBox;
    private Queue<string> sentences;
    private float delayMultiplier;

	// Use this for initialization
	void Start ()
    {
        sentences = new Queue<string>();
        clearStates();
        speedMultiplier = 1;
        delayMultiplier = 1;
        wait = 0;
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            boop();
        }
        if (!doneTypeing)
        {
            TextPointer.color = Color.clear;
        }
        else
        {
            TextPointer.color = Color.white;
        }
    }

    public void StartDialog(Dialog dialog)
    {
        InDialog = true;

        foreach (string sentence in dialog.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        Debug.Log("yo");
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        string sentence = sentences.Dequeue();
        string num = sentence.Remove(0, sentence.IndexOf("/") + 1);
        sentence = sentence.Remove(sentence.IndexOf("/"));
        delayMultiplier = 1;

        if (firstBox)
        {
            firstBox = false;
            delayMultiplier = 3.5f;
        }

        else
        {
            delayMultiplier = 0;
        }

        if (float.Parse(num) == 0)
        {
            speedMultiplier = 1;
        }
        else
        {
            speedMultiplier = float.Parse(num);
        }
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        float delay = 0;
        while (delay < .32f * delayMultiplier)
        {
            delay += Time.deltaTime;
            yield return null;
        }
        doneTypeing = false;
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            ActorManager.instance.PlaySound("TextSpit", .6f);
            yield return new WaitForSeconds(.04f * speedMultiplier);
        }
        doneTypeing = true;
        yield return null;
    }

    private void EndDialog()
    {
        animator.Play("DialogBoxClose");
        clearStates();
    }

    private void clearStates()
    {
        delayMultiplier = 1;
        InDialog = false;
        wait = 0f;
        doneTypeing = false;
        firstBox = true;
        sentences.Clear();

        Invoke("clearText", .4f);
    }

    private void clearText()
    {
        dialogText.text = "";
    }

    public void boop()
    {
        if (InDialog)
        {
            if (doneTypeing)
            {
                doneTypeing = false;
                DisplayNextSentence();
            }
        }
    }
}
