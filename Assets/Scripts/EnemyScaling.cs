using UnityEngine;

public class EnemyScaling : MonoBehaviour
{
    [SerializeField, Min(1f)] private float difficultyInterval = 60f;
    [SerializeField, Min(0)] private int healthIncreasePerStep = 25;
    [SerializeField, Min(0)] private int damageIncreasePerStep = 1;

    public int GetDifficultyStep(float elapsedTime)
    {
        float interval = Mathf.Max(1f, difficultyInterval);
        float runTime = Mathf.Max(0f, elapsedTime);
        return Mathf.FloorToInt(runTime / interval);
    }

    public int CalculateHealth(int baseHealth, float elapsedTime)
    {
        int difficultyStep = GetDifficultyStep(elapsedTime);
        int healthIncrease = Mathf.Max(0, healthIncreasePerStep) * difficultyStep;
        return Mathf.Max(1, baseHealth) + healthIncrease;
    }

    public int CalculateDamage(int baseDamage, float elapsedTime)
    {
        int difficultyStep = GetDifficultyStep(elapsedTime);
        int damageIncrease = Mathf.Max(0, damageIncreasePerStep) * difficultyStep;
        return Mathf.Max(0, baseDamage) + damageIncrease;
    }
}
