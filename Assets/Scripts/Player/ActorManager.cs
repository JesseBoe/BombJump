using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    public GameObject playerCamera;

    public GameObject playerGo;

    public static ActorManager instance = null;
    public CheckPoint checkPoint = new CheckPoint();
    public List<SuperActor> Actors = new List<SuperActor>();
    private AudioSource source;
    public AudioSource sfx;

    public int scale = 1;
    public bool resettingScene = false;
    public bool hasStar = false;
    public bool hasBomb = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        source = GetComponent<AudioSource>();
        PlayLoopingSound("LevelMusic");
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        
	}

    public void PlaySound ( string name, float vol)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
        sfx.PlayOneShot(clip, vol);
        //source.Play();
    }
    public void PlaySoundDeley(string name, float vol, float time)
    {
        StartCoroutine(playSoundAfterTime(name, vol, time));
    }
    public void PlayLoopingSound (string name)
    {
        source.clip = Resources.Load<AudioClip>("Sounds/" + name);
        source.loop = true;
        source.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("LeftBumper"))
        {
            resettingScene = true;
            Actors.Clear();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SceneManager.LoadScene("Anenegry");
        }
	}

    private void FixedUpdate()
    {
        IEnumerable<SuperActor> query = Actors.OrderBy(h => h.GetColliderHeightCord());
        foreach (var item in query)
        {
            item.Act();
        }
    }

    IEnumerator playSoundAfterTime(string name, float vol, float delay)
    {
        yield return new WaitForSeconds(delay);

        ActorManager.instance.PlaySound(name, vol);
    }
}
