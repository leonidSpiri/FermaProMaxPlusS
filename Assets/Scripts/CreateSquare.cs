using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateSquare : MonoBehaviour
{
    public GameObject wheat;
    public GameObject chicken;
    public GameObject cow;
    public ChooseWhatToPlant choose;
    public Text txtWheat, txtChicken, txtCow, txtMoney;
    public AudioSource audioChicken, audioCow;
    public int ActiveSquare;
    private int whatIsActive = 3, chickenEgg = 0, cowMilk = 0;
    private bool isReady = false;
    private Save save = new Save();
    private string path;

    private float lastClick = 0;
    private float waitTime = 1.0f;
    private float downTime;
    private bool isHandled = false;


    private void Start()
    {
        UnityThread.initUnityThread();
        String buffPath = "Save_" + transform.name + ".json";
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, buffPath);
#else
        path = Path.Combine(Application.dataPath, buffPath);
#endif
        if (File.Exists(path))
        {
            save = JsonUtility.FromJson<Save>(File.ReadAllText(path));
            whatIsActive = save.whatIsActive;
            chickenEgg = save.chickenEgg;
            cowMilk = save.cowMilk;
            isReady = save.isReady;

            if (transform.childCount == 0)
            {
                var position = transform.position;
                Vector3 vector3 = new Vector3(position.x, position.y + 0.5f, position.z);
                switch (whatIsActive)
                {
                    case 0:
                        Instantiate(wheat, vector3, Quaternion.identity, transform);
                        PlantWheat();
                        break;
                    case 1:
                        Instantiate(chicken, vector3, Quaternion.identity, transform);
                        PlantChicken();
                        break;
                    case 2:
                        Instantiate(cow, vector3, Quaternion.identity, transform);
                        PlantCow();
                        break;
                }
            }
        }
    }


    void OnMouseDown()
    {
        downTime = Time.time;
        isHandled = false;

        if (Time.time - lastClick < 0.3)
        {
            //double clicked the target
        }

        lastClick = Time.time;
        if (isReady) Get();
        else
        {
            switch (ActiveSquare)
            {
                case 0:
                    Add(wheat);
                    break;
                case 1:
                    Add(chicken);
                    break;
                case 2:
                    Add(cow);
                    break;
                default:
                    Add(wheat);
                    break;
            }
        }
    }

    void OnMouseDrag()
    {
        if ((Time.time > downTime + waitTime) && !isHandled)
        {
            isHandled = true;
            Delete();
        }
    }


    private void Get()
    {
        if (isReady)
        {
            GetComponent<Renderer>().material.color = new Color32(84, 53, 13, 255);

            if (ActiveSquare == 0)
            {
                var counter = int.Parse(txtWheat.text);
                counter++;
                txtWheat.text = counter.ToString();
                PlantWheat();
            }

            if (ActiveSquare == 1)
            {
                var counter = int.Parse(txtChicken.text);
                counter += chickenEgg;
                chickenEgg = 0;
                txtChicken.text = counter.ToString();
                PlantChicken();
            }

            if (ActiveSquare == 2)
            {
                var counter = int.Parse(txtCow.text);
                counter += cowMilk;
                cowMilk = 0;
                txtCow.text = counter.ToString();
                PlantCow();
            }

            isReady = false;
        }
    }

    private void Delete()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GetComponent<Renderer>().material.color = new Color32(84, 53, 13, 255);
        isReady = false;
        whatIsActive = 3;
    }

    private void Add(GameObject obj)
    {
        int money = int.Parse(txtMoney.text);
        int wheatCount = int.Parse(txtWheat.text);
        if (transform.childCount == 0)
        {
            var position = transform.position;
            Vector3 vector3 = new Vector3(position.x, position.y + 0.5f, position.z);
            if (ActiveSquare == 0 && money > 0)
            {
                money--;
                Instantiate(obj, vector3, Quaternion.identity, transform);
                PlantWheat();
            }

            if (ActiveSquare == 1 && money > 1 && wheatCount > 2)
            {
                money -= 2;
                Instantiate(obj, vector3, Quaternion.identity, transform);
                audioChicken.Play();
                PlantChicken();
            }

            if (ActiveSquare == 2 && money > 2 && wheatCount > 3)
            {
                money -= 3;
                Instantiate(obj, vector3, Quaternion.identity, transform);
                audioCow.Play();
                PlantCow();
            }

            whatIsActive = ActiveSquare;
            txtMoney.text = money.ToString();
        }
    }

    private void PlantCow()
    {
        var counter = int.Parse(txtWheat.text);
        Thread t = new Thread(() =>
        {
            while (ActiveSquare != 2 && counter < 3)
            {
                Thread.Sleep(1500);
                UnityThread.executeInUpdate(() => { counter = int.Parse(txtWheat.text); });
            }

            UnityThread.executeInUpdate(() =>
            {
                counter -= 3;
                txtWheat.text = counter.ToString();
            });

            Thread.Sleep(40000);


            if (ActiveSquare == 2)
            {
                isReady = true;
                cowMilk++;
                UnityThread.executeInUpdate(() =>
                {
                    GetComponent<Renderer>().material.color = new Color32(194, 124, 0, 255);
                });
            }


            UnityThread.executeInUpdate(() => { counter = int.Parse(txtWheat.text); });
        });
        t.Start();
    }

    private void PlantChicken()
    {
        var counter = int.Parse(txtWheat.text);
        Thread t = new Thread(() =>
        {
            while (ActiveSquare != 1 && counter < 2)
            {
                Thread.Sleep(1500);
                UnityThread.executeInUpdate(() => { counter = int.Parse(txtWheat.text); });
            }

            UnityThread.executeInUpdate(() =>
            {
                if (chickenEgg % 3 == 0)
                {
                    counter--;
                    txtWheat.text = counter.ToString();
                }
            });

            Thread.Sleep(30000);


            if (ActiveSquare == 1 && counter > 1)
            {
                isReady = true;
                chickenEgg++;
                UnityThread.executeInUpdate(() =>
                {
                    GetComponent<Renderer>().material.color = new Color32(194, 124, 0, 255);
                });
            }


            UnityThread.executeInUpdate(() => { counter = int.Parse(txtWheat.text); });
        });
        t.Start();
    }

    private void PlantWheat()
    {
        Thread t = new Thread(() =>
        {
            Thread.Sleep(20000);

            if (ActiveSquare == 0)
            {
                isReady = true;
                UnityThread.executeInUpdate(() =>
                {
                    GetComponent<Renderer>().material.color = new Color32(194, 124, 0, 255);
                });
            }
        });
        t.Start();
    }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        save.whatIsActive = whatIsActive;
        save.chickenEgg = chickenEgg;
        save.cowMilk = cowMilk;
        save.isReady = isReady;
         File.WriteAllText(path, JsonUtility.ToJson(save));
    }
#endif
    public void OnApplicationQuit()
    {
        save.whatIsActive = whatIsActive;
        save.chickenEgg = chickenEgg;
        save.cowMilk = cowMilk;
        save.isReady = isReady;
        File.WriteAllText(path, JsonUtility.ToJson(save));
    }
}

[Serializable]
public class Save
{
    public int whatIsActive = 0, chickenEgg, cowMilk;
    public bool isReady;
}