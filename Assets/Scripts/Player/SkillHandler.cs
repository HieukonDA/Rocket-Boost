using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    private bool hasShield = false;
    private bool hasPierce = false;
    private IAudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
    }

    public void ApplySkill(GameObject skillCircle)
    {
        int skillType = Random.Range(0, 2); // 0: Shield, 1: Pierce
        if (skillType == 0) hasShield = true;
        else hasPierce = true;
        Destroy(skillCircle);
        audioManager?.PlaySound("skillpickup");
    }

    public bool HasShield() => hasShield;
    public bool HasPierce() => hasPierce;

    public void ResetSkills()
    {
        hasShield = false;
        hasPierce = false;
    }
}