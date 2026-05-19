using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

namespace ChronicSurvival.Units
{
    public class Enemy : Unit
    {
        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private DiseaseType diseaseType;
        [SerializeField] private BlobPreset visualPreset;

        public EnemyType EnemyType => enemyType;
        public DiseaseType DiseaseType => diseaseType;

        protected override void Awake()
        {
            base.Awake();
            team = UnitTeam.Disease;
        }

        protected override void Start()
        {
            base.Start();
            
            if (blobVisual != null)
            {
                blobVisual.ApplyPreset(visualPreset);
            }
        }

        public void SetEnemyType(EnemyType type, DiseaseType disease)
        {
            enemyType = type;
            diseaseType = disease;
            ApplyTypeStats(type, disease);
        }

        private void ApplyTypeStats(EnemyType type, DiseaseType disease)
        {
            switch (disease)
            {
                case DiseaseType.Diabetes:
                    ApplyDiabetesStats(type);
                    break;
                case DiseaseType.Hypertension:
                    ApplyHypertensionStats(type);
                    break;
                case DiseaseType.Cancer:
                    ApplyCancerStats(type);
                    break;
            }

            currentHealth = maxHealth;
            
            if (blobVisual != null)
            {
                blobVisual.ApplyPreset(visualPreset);
            }
        }

        private void ApplyDiabetesStats(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Basic:
                    maxHealth = 50f;
                    damage = 5f;
                    attackSpeed = 1f;
                    moveSpeed = 1f;
                    visualPreset = BlobPreset.SugarBlob;
                    break;

                case EnemyType.Medium:
                    maxHealth = 150f;
                    damage = 15f;
                    attackSpeed = 1f;
                    moveSpeed = 1.5f;
                    visualPreset = BlobPreset.SugarBlob;
                    break;

                case EnemyType.Elite:
                    maxHealth = 200f;
                    damage = 30f;
                    attackSpeed = 1.2f;
                    moveSpeed = 3f;
                    visualPreset = BlobPreset.SugarBlob;
                    break;
            }
        }

        private void ApplyHypertensionStats(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Basic:
                    maxHealth = 60f;
                    damage = 12f;
                    attackSpeed = 1.5f;
                    moveSpeed = 2.5f;
                    visualPreset = BlobPreset.PressurePulse;
                    break;

                case EnemyType.Medium:
                    maxHealth = 100f;
                    damage = 20f;
                    attackSpeed = 1f;
                    moveSpeed = 2f;
                    visualPreset = BlobPreset.PressurePulse;
                    break;

                case EnemyType.Elite:
                    maxHealth = 150f;
                    damage = 40f;
                    attackSpeed = 2f;
                    moveSpeed = 3.5f;
                    visualPreset = BlobPreset.PressurePulse;
                    break;
            }
        }

        private void ApplyCancerStats(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Basic:
                    maxHealth = 70f;
                    damage = 8f;
                    attackSpeed = 1f;
                    moveSpeed = 1.5f;
                    visualPreset = BlobPreset.DamagedCell;
                    break;

                case EnemyType.Medium:
                    maxHealth = 120f;
                    damage = 15f;
                    attackSpeed = 1f;
                    moveSpeed = 2f;
                    visualPreset = BlobPreset.DamagedCell;
                    break;

                case EnemyType.Elite:
                    maxHealth = 200f;
                    damage = 25f;
                    attackSpeed = 1f;
                    moveSpeed = 2.5f;
                    visualPreset = BlobPreset.DamagedCell;
                    break;
            }
        }
    }

    public enum EnemyType
    {
        Basic,
        Medium,
        Elite,
        Boss
    }

    public enum DiseaseType
    {
        Diabetes,
        Hypertension,
        Cancer
    }
}
