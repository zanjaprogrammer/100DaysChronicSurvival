using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

namespace ChronicSurvival.Units
{
    public class ImmuneCell : Unit
    {
        [Header("Immune Cell Type")]
        [SerializeField] private ImmuneCellType cellType;
        [SerializeField] private BlobPreset visualPreset;

        [Header("Special Abilities")]
        [SerializeField] private bool hasSpecialAbility = false;
        [SerializeField] private float abilityCooldown = 5f;

        private float abilityTimer;

        public ImmuneCellType CellType => cellType;

        protected override void Awake()
        {
            base.Awake();
            team = UnitTeam.Immune;
        }

        protected override void Start()
        {
            base.Start();
            
            if (blobVisual != null)
            {
                blobVisual.ApplyPreset(visualPreset);
            }

            abilityTimer = abilityCooldown;
        }

        protected override void Update()
        {
            base.Update();

            if (hasSpecialAbility)
            {
                abilityTimer -= Time.deltaTime;
                if (abilityTimer <= 0)
                {
                    UseSpecialAbility();
                    abilityTimer = abilityCooldown;
                }
            }
        }

        protected virtual void UseSpecialAbility()
        {
            // Override in specific cell types
        }

        public void SetCellType(ImmuneCellType type)
        {
            cellType = type;
            ApplyTypeStats(type);
        }

        private void ApplyTypeStats(ImmuneCellType type)
        {
            switch (type)
            {
                case ImmuneCellType.Macrophage:
                    maxHealth = 200f;
                    damage = 15f;
                    attackSpeed = 1f;
                    moveSpeed = 1.5f;
                    visualPreset = BlobPreset.Macrophage;
                    break;

                case ImmuneCellType.TCell:
                    maxHealth = 100f;
                    damage = 40f;
                    attackSpeed = 1.25f;
                    moveSpeed = 3f;
                    visualPreset = BlobPreset.TCell;
                    break;

                case ImmuneCellType.BCell:
                    maxHealth = 80f;
                    damage = 25f;
                    attackSpeed = 0.8f;
                    moveSpeed = 2f;
                    attackRange = 3f;
                    visualPreset = BlobPreset.BCell;
                    break;

                case ImmuneCellType.NKCell:
                    maxHealth = 120f;
                    damage = 50f;
                    attackSpeed = 0.67f;
                    moveSpeed = 3.5f;
                    visualPreset = BlobPreset.NKCell;
                    break;

                case ImmuneCellType.Neutrophil:
                    maxHealth = 90f;
                    damage = 20f;
                    attackSpeed = 1.43f;
                    moveSpeed = 4f;
                    visualPreset = BlobPreset.Neutrophil;
                    break;
            }

            currentHealth = maxHealth;
            
            if (blobVisual != null)
            {
                blobVisual.ApplyPreset(visualPreset);
            }
        }
    }

    public enum ImmuneCellType
    {
        Macrophage,
        TCell,
        BCell,
        NKCell,
        Neutrophil
    }
}
