using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerRandomColor : MonoBehaviour
{
    [SerializeField] private List<Sprite> listSprite;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerImage;
    private static readonly int Player = Animator.StringToHash("Player");

    private void Awake()
    {
        RamdomColor();
    }

    void RamdomColor()
    {
        int id = Random.Range(0, listSprite.Count);
        playerImage.sprite = listSprite[id];
        animator.SetInteger(Player,id);
    }
}
