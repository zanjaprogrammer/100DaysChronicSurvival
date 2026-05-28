using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

namespace ChronicSurvival.Units
{
    public class ImmuneCell : Unit
    {
        [Header("Squad Role")]
        [SerializeField] private ImmuneSquadRole squadRole = ImmuneSquadRole.Officer;

        [Header("Legacy Type")]
        [SerializeField] private ImmuneCellType cellType;
        [SerializeField] private BlobPreset visualPreset;

        [Header("Special Abilities")]
        [SerializeField] private bool hasSpecialAbility = false;
        [SerializeField] private float abilityCooldown = 5f;

        private float abilityTimer;

        public ImmuneCellType CellType => cellType;
        public ImmuneSquadRole SquadRole => squadRole;
        public bool IsLeader => squadRole == ImmuneSquadRole.Leader;

        protected override void Awake()
        {
            base.Awake();
            team = UnitTeam.Immune;
        }

        protected override void Start()
        {
            base.Start();
            ApplyCurrentVisual();
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
        }

        public void ConfigureSquadRole(ImmuneSquadRole role)
        {
            squadRole = role;
            if (role == ImmuneSquadRole.Leader)
            {
                maxHealth = 180f;
                damage = 22f;
                attackSpeed = 1.15f;
                moveSpeed = 2.6f;
                detectionRange = 6.5f;
                attackRange = 1.2f;
                if (currentHealth > 0f)
                {
                    currentHealth = Mathf.Min(currentHealth, maxHealth);
                }
            }
            else
            {
                maxHealth = 120f;
                damage = 15f;
                attackSpeed = 1.0f;
                moveSpeed = 2.35f;
                detectionRange = 5.5f;
                attackRange = 1.05f;
                if (currentHealth > 0f)
                {
                    currentHealth = Mathf.Min(currentHealth, maxHealth);
                }
            }

            ApplyCurrentVisual();
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
            ApplyCurrentVisual();
        }

        private void ApplyCurrentVisual()
        {
            if (blobVisual == null)
            {
                return;
            }

            if (squadRole == ImmuneSquadRole.Leader)
            {
                blobVisual.SetColors(
                    new Color(0.08f, 0.21f, 0.56f, 1f),
                    new Color(0.20f, 0.42f, 0.85f, 0.48f));
                blobVisual.SetSize(0.7f);
                blobVisual.SetAnimationSpeed(0.85f);
                return;
            }

            if (squadRole == ImmuneSquadRole.Officer)
            {
                blobVisual.SetColors(
                    new Color(0.23f, 0.63f, 0.95f, 1f),
                    new Color(0.48f, 0.80f, 1f, 0.45f));
                blobVisual.SetSize(0.52f);
                blobVisual.SetAnimationSpeed(1.05f);
                return;
            }

            blobVisual.ApplyPreset(visualPreset);
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

    public enum ImmuneSquadRole
    {
        Leader,
        Officer,
        Legacy
    }
}
