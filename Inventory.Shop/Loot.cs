using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer sr;
    public Animator anim;
    public int quantity;
    public bool canBePickedUp = true;

    public static event Action<ItemSO, int> OnItemLooted;

    public void OnValidate()
    {
        if (itemSO == null)
            return;

        UpdateAppearance();
    }

    public void Initialize(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;

      
        canBePickedUp = true;

        UpdateAppearance();

        if (sr != null)
        {
            sr.sortingLayerName = "UI"; 
            sr.sortingOrder = 100; 
        }
    }

    private void UpdateAppearance()
    {
        if (sr != null)
            sr.sprite = itemSO.icon;
        this.name = itemSO.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canBePickedUp == true)
        {
        
            if (anim != null)
            {
                anim.Play("New Animation");
            }

            
            OnItemLooted?.Invoke(itemSO, quantity);

            
            Destroy(gameObject, anim != null ? 0.5f : 0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
      
        if (collision.CompareTag("Player") && !wasDroppedByEnemy)
        {
            canBePickedUp = true;
        }
    }

  
    private bool wasDroppedByEnemy = false;


    public void SetAsEnemyDrop()
    {
        wasDroppedByEnemy = true;
        canBePickedUp = true;
    }
}
