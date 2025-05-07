using UnityEngine;
using System.Collections;
using System;

public class SkillManager : MonoBehaviour, ISkillManager
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private SkillData[] skillDatas;
    [SerializeField] private SkillEffectsConfig skillEffectsConfig; // Prefab của hiệu ứng skill
    private SkillData activeSkill;
    private GameObject activeSkillModel;
    private float skillTimer = 0f;
    private float totalTime = 0f; // Tổng thời gian còn lại
    private bool isSkillActive = false;
    private bool isShieldActive = false;

    private void Awake()
    {
    if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() 
    {
        if (isSkillActive)
        {
            skillTimer -= Time.deltaTime;
            HUDManager.Instance.UpdateTimer(skillTimer, totalTime);
            if (skillTimer <= 0f)
            {
                DeactivateSkill();
            }
        }
    }

    private void DeactivateSkill()
    {
        if (activeSkillModel != null)
        {
            Destroy(activeSkillModel);
            activeSkillModel = null;
        }
        activeSkill = null;
        isSkillActive = false;
        isShieldActive = false;
        skillTimer = 0f;
        Debug.Log("SkillManager: Skill deactivated");
    }
    
    public SkillData GetSkillDataByTag(string skillTag)
    {
        foreach (SkillData skill in skillDatas)
        {
            if (skill.SkillName.Equals(skillTag, System.StringComparison.OrdinalIgnoreCase))
            {
                return skill;
            }
        }
        Debug.LogWarning($"SkillManager: Skill with tag {skillTag} not found!");
        return null;
    }
    private void ApllySkillParticle(SkillData skill, Vector3 position)
    {
        if (skill.EffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(skill.EffectPrefab, position, Quaternion.identity);
            Destroy(effect.gameObject, skill.EffectPrefab.main.duration);
        }
    }

    private void ApplySkillSound(SkillData skill)
    {
        // Shield không cần hiệu ứng trực tiếp, chỉ cần HasShield() trả true
        if (skill.PickupSound != null)
        {
            AudioSource.PlayClipAtPoint(skill.PickupSound, transform.position);
        }
        else
        {
            AudioManager.Instance?.PlaySound("skillPickup");
        }
    }

    private void SpawnShieldModel(string skillTag, Transform playerTransform)
    {
        if (skillEffectsConfig != null)
        {
            GameObject prefab = skillEffectsConfig.GetPrefabByTag(skillTag);
            if (prefab != null)
            {
                activeSkillModel = Instantiate(prefab, playerTransform.position, Quaternion.identity, playerTransform);
                Debug.Log($"SkillManager: Spawned shield model for {skillTag} at {playerTransform.position}");
            }
            else
            {
                Debug.LogWarning($"SkillManager: No prefab found for tag {skillTag} in SkillEffectsConfig!");
            }
        }
    }


    public void ActivateSkill(string skillTag, Vector3 position, Transform playerTransform = null)
    {
        SkillData skill = GetSkillDataByTag(skillTag);
        if (skill == null) return;
        //tat skill trước đó
        if (isSkillActive)
        {
            DeactivateSkill();
        }
        
        activeSkill = skill;
        skillTimer = skill.Duration;
        totalTime = skill.Duration; // Lưu tổng thời gian còn lại
        isSkillActive = true;
        isShieldActive = skillTag.Equals("Shield", StringComparison.OrdinalIgnoreCase);

        ApplySkillSound(skill);
        ApllySkillParticle(skill, position);
        if (playerTransform != null)
        {
            SpawnShieldModel(skillTag, playerTransform);
            StartCoroutine(HandleSkillDuration(skill.Duration));
        }
        Debug.Log($"SkillManager: Activated {skill.SkillName} at {position}");
    }

    private IEnumerator HandleSkillDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        DeactivateSkill();
    }

    public bool IsShieldActive() // Phương thức mới để kiểm tra flag
    {
        return isShieldActive;
    }

    
    public float GetRemainingSkillTime()
    {
        throw new System.NotImplementedException();
    }

    public bool HasCoinMagnet()
    {
        throw new System.NotImplementedException();
    }

    public bool HasPierce()
    {
        throw new System.NotImplementedException();
    }

    public bool HasShield()
    {
        return isShieldActive;
    }

    public void ResetSkills()
    {
        if (isSkillActive) DeactivateSkill();
    }
}