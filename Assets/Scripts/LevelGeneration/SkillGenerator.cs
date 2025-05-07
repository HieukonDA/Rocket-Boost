using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillGenerator 
{
    private readonly GameObject skillCirclePrefab; // Prefab của SkillCircle
    private readonly ICoinManager coinManager; // Để kiểm tra và deactive coin
    private readonly int maxSkillsPerLevel; // Tối đa skill mỗi level
    private int spawnedSkillCount; // Đếm số skill đã sinh ra 

    public SkillGenerator(GameObject skillCirclePrefab, ICoinManager coinManager, int maxSkillsPerLevel = 2)
    {
        this.skillCirclePrefab = skillCirclePrefab;
        this.coinManager = coinManager;
        this.maxSkillsPerLevel = maxSkillsPerLevel;
        spawnedSkillCount = 0; // Khởi tạo số lượng skill đã sinh ra
    }

    public void ResetSkillCount()
    {
        spawnedSkillCount = 0; // Reset khi sinh level mới
    }

    public void TrySpawnSkill(GameObject segment, int segmentIndex)
    {
        Debug.LogError($"SkillSpawner: tên  {segment.name} được  {segmentIndex} đoạn.");
        // Kiểm tra nếu đoạn này chia hết cho 5 và chưa vượt quá giới hạn skill
        if (spawnedSkillCount >= maxSkillsPerLevel || segmentIndex % 2 != 0)
        {
            Debug.Log($"SkillSpawner: {segment.name} không spawn.");
            return;
        }

        // Tìm CoinSpawnPoints trong segment
        Transform coinSpawnPoints = segment.transform.Find("CoinSpawnPoints");
        if (coinSpawnPoints == null)
        {
            Debug.LogError($"Không tìm thấy CoinSpawnPoints trong {segment.name}.");
            return;
        }

        // Lấy tất cả CoinPoint trong CoinSpawnPoints
        List<Transform> coinPoints = new List<Transform>();
        foreach (Transform child in coinSpawnPoints)
        {
            if (child.CompareTag("CoinPoint"))
            {
                coinPoints.Add(child);
            }
        }

        if (coinPoints.Count == 0)
        {
            Debug.LogError($"Không tìm thấy CoinPoints trong {segment.name}.");
            return;
        }

        Debug.LogError($"SkillSpawner: Có {coinPoints.Count} CoinPoints trong {segment.name}.");

        // Chọn ngẫu nhiên CoinPoint
        Transform selectedCoinPoint = coinPoints[Random.Range(0, coinPoints.Count)];

        // Kiểm tra & deactivate coin nếu có
        GameObject existingCoin = coinManager.FindCoinAtPosition(selectedCoinPoint.position);
        if (existingCoin != null)
        {
            existingCoin.SetActive(false);
        }

        // Spawn SkillCircle
        GameObject skillCircle = Object.Instantiate(skillCirclePrefab, selectedCoinPoint.position, Quaternion.identity, SkillManager.Instance.transform);
        skillCircle.tag = "Shield";
        Debug.LogError($"SkillSpawner: Spawned skill circle tại {selectedCoinPoint.position} trong {segment.name} trong cái .");

        SkillData skillData = SkillManager.Instance.GetSkillDataByTag("Shield");
        if (skillData != null && skillData.SkillModel != null)
        {
            GameObject model = Object.Instantiate(skillData.SkillModel, skillCircle.transform);
            model.transform.localPosition = Vector3.zero;
        }
        else if (skillData == null)
        {
            Debug.LogError($"SkillSpawner: Không tìm thấy SkillData cho tag 'Shield'.");
        }
        else if (skillData.SkillModel == null)
        {
            Debug.LogError($"SkillSpawner: Không tìm thấy SkillModel cho tag 'Shield'.");
        }

        spawnedSkillCount++;
        Debug.LogError($"SkillSpawner: Spawned skill tại CoinPoint trong {segment.name}.");
    }
}