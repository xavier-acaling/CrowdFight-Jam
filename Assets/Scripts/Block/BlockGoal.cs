using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockGoal : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject BlockVFX;
    public float Multiplier;
    public List<Transform> IntroPath = new List<Transform>();
    public float IntoPathSeconds;
    public Transform SpawnPosition;
    public Coroutine CBlockVFXEnable;
    public BlockDragHandler LastBlockDragHandler;
    public List<BlockDragHandler> ListAllBlocks = new List<BlockDragHandler>();
    public void RemoveBlock(BlockDragHandler blockDragHandler)
    {
        ListAllBlocks.Remove(blockDragHandler);

        blockDragHandler.transform.DOKill();

        Sequence seq = DOTween.Sequence();
        Destroy(blockDragHandler.gameObject);
        
        if (ListAllBlocks.Count == 0)
        {
             Vector3 baseScale = transform.localScale;

            seq.Append(transform.DOScale(new Vector3(
            baseScale.x * 1.2f, 
            baseScale.y * 0.8f, 
                        baseScale.z), 0.1f))  // squeeze
            .Append(transform.DOScale(new Vector3(
                        baseScale.x * 0.8f, 
                        baseScale.y * 1.2f, 
                        baseScale.z), 0.1f))  // stretch
            .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)) // shrink
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
     
    }
}
