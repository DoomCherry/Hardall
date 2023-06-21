using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class EnemyBrain : MonoBehaviour
{
    [Required]
    [SerializeField] private EnemySpaceOrientator _enemyOrientator;

    #region Messages
    [ShowIf("ComponentContainError"), ReadOnly]
    [InfoBox(nameof(StepsMarkHandler) + " is not contain in scene or not initialized!", EInfoBoxType.Error)]
    public string _componentStatus = "This component need to " + 
                                       nameof(StepsMarkHandler) + 
                                       ". Please add empty Game object in scene and add" + 
                                       nameof(StepsMarkHandler) + 
                                       " component them.";

    public bool ComponentContainError => Application.isPlaying && StepsMarkHandler.Value == null;
    
    #endregion




    private void OnEnable()
    {
        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.AddSystem<EnemyBrain>(MakeDecision);
        });
    }

    public void MakeDecision()
    {
        if (_enemyOrientator == null)
            return;

        if (StepsMarkHandler.Value == null)
            return;
    }

    private void OnDisable()
    {
        PlayerLoopExtentions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
        {
            system.RemoveSystem<EnemyBrain>(false);
        });
    }
}
