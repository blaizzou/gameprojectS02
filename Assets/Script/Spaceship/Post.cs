﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Post : MonoBehaviour {
    public AudioClip exit;
    public AudioClip enter;
    private  AudioSource audioData;
    public GameObject post;
    private GameObject postPlayer;
    private GameObject playerCam;
    private PhotonView PV;
    public bool occupied = false;
    [SerializeField]
    private ParticleSystem effectUsing;
    [SerializeField]
    private ParticleSystem effectFree;

    public enum postType {
        Canon,
        Turret,
        Thruster,
        Shield,
        Rotate,
        MiniShip
    }

    public Post.postType type;

    void Awake () {
        PV = GetComponent<PhotonView> ();
        audioData = GetComponent<AudioSource>();
    }

    public void GetOutPost () {
        if (postPlayer) {
            switch (type) {
                case postType.Canon:
                    Canon tmpCanon = post.GetComponent<Canon> ();
                    tmpCanon.enabled = false;
                    break;
                case postType.Thruster:
                    ThrusterControls tmp = post.GetComponent<ThrusterControls> ();
                    tmp.enabled = false;
                    break;
                case postType.Turret:
                    Turret turret = post.GetComponent<Turret> ();
                    turret.enabled = false;
                    break;
                case postType.Rotate:
                    RotateShip rotate = post.GetComponent<RotateShip> ();
                    rotate.enabled = false;
                    break;
                case postType.Shield:
                    Shield shield = post.GetComponent<Shield> ();
                    shield.enabled = false;
                    break;
                case postType.MiniShip:
                    postPlayer.transform.GetChild (1).gameObject.SetActive (true);
                    MiniShip ship = post.GetComponent<MiniShip> ();
                    ship.ReturnShip ();
                    ship.enabled = false;
                    break;
                default:
                    break;
            }
            audioData.PlayOneShot (exit, 1);
            postPlayer = null;
            PV.RPC ("FreePost", RpcTarget.Others);
        }
    }

    public void UsePost (GameObject player) {
        if (!postPlayer) {
            postPlayer = player;
            audioData.PlayOneShot (enter, 1);
            PV.RPC ("OccupyPost", RpcTarget.Others);
            PhotonView playerPV = postPlayer.GetComponentInChildren<PhotonView> ();
            PhotonView postPV = post.GetComponent<PhotonView> ();
            // PhotonView[] photonViews = post.GetComponents<PhotonView>();
            // foreach (PhotonView view in photonViews)
            // {
            //     print(view.ViewID);
            // }
            if (postPV)
                postPV.RequestOwnership ();
            switch (type) {
                case postType.Canon:
                    Canon tmpCanon = post.GetComponent<Canon> ();
                    tmpCanon.enabled = true;
                    break;
                case postType.Thruster:
                    ThrusterControls tmp = post.GetComponent<ThrusterControls> ();
                    tmp.enabled = true;
                    break;
                case postType.Turret:
                    Turret turret = post.GetComponent<Turret> ();
                    turret.enabled = true;
                    break;
                case postType.Rotate:
                    RotateShip rotate = post.GetComponent<RotateShip> ();
                    rotate.enabled = true;
                    break;
                case postType.Shield:
                    Shield shield = post.GetComponent<Shield> ();
                    shield.enabled = true;
                    break;
                case postType.MiniShip:
                    postPlayer.transform.GetChild (1).gameObject.SetActive (false);
                    MiniShip ship = post.GetComponent<MiniShip> ();
                    ship.enabled = true;
                    ship.ActivateShip ();
                    break;
                default:
                    break;
            }
        }
    }

    public void CanUse () {
        effectFree.Play ();
    }

    public void CantUse () {
        effectFree.Stop ();
    }

    [PunRPC]
    void FreePost () {
        occupied = false;
        effectUsing.Stop ();
    }

    [PunRPC]
    void OccupyPost () {
        occupied = true;
        effectUsing.Play ();
    }
}