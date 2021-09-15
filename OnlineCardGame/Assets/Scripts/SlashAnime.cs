using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashAnime : MonoBehaviour
{
    public Animator anime;
    public static SlashAnime Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("WTF IS HAPPENING");
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            var tempRotation = transform.rotation;
            tempRotation.eulerAngles = new Vector3(0, 0, 180);
            transform.rotation = tempRotation;
        }
    }

    public void startAnime(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        anime.Play("Slash");
    }
}
