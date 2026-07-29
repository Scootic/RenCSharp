using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RenCSharp.Combat.Interfaces;
using RenCSharp.EXPERIMENTAL;
namespace RenCSharp.Combat.Enemies
{
    [RequireComponent(typeof(Collider))]
    public class Base_Projectile : MonoBehaviour, IRemovableObject
    {
        [Header("Base Projectile")]
        [Header("Movement")]
        [SerializeReference] protected Projectile_MovementType movementType = new StraightLine_Projectile_MovementType();
        [Header("On Hit")]
        [SerializeReference] protected List<Projectile_OnHitEffect> onHitEffects = new();
        [Header("On Despawn")]
        [SerializeReference] protected Projectile_DespawnType despawnType = new Empty_Projectile_DespawnType();
        [Header("On Update")]
        [SerializeReference] protected List<Projectile_CustomUpdate> updateTypes = new();
        [Header("Update Behavior")]
        [SerializeField, Tooltip("Do custom updates while the projectile's collider is inactive.")] protected bool customUpdateWhileInactive = true;
        [SerializeField, Tooltip("Do custom updates while the projectile's collider is active.")] protected bool customUpdateWhileActive = true;
        [SerializeField, Tooltip("Does the movement behavior while the projectile's collider is inactive.")] protected bool movementWhileInactive = true;
        [SerializeField, Tooltip("Does the movement behavior while the projectile's collider is active.")] protected bool movementWhileActive = true;
        [Header("Stats")]
        [SerializeField] protected bool damageOverTime = false;
        [SerializeField] protected bool destroyOnHit = true;
        [SerializeField, Min(0.1f)] protected float baseDamage = 1;
        [SerializeField, Min(0.15f)] protected float lifetime = 10;
        [SerializeField, Range(0, 1)] protected float spawnSoundVol = 1;
        [SerializeField, Min(0.1f)] protected float colliderEnableTime = 0.1f;
        [SerializeField] protected AudioClip spawnSound;
        [SerializeField] protected Image sprite;
        [SerializeField] protected ProjectileHitType hitType = ProjectileHitType.Normal;

        protected IDamage receiver;
        protected Vector3 moveDir, endScale;
        protected Color endC;
        protected Collider myCol;
        protected Coroutine spawnInRoutine;
        protected Rigidbody receiverRB, myRB;
        public float Lifetime => lifetime;
        public float ColliderEnableTime => colliderEnableTime;
        public float SpawnSoundVol => spawnSoundVol;
        public AudioClip SpawnSound => spawnSound;
        public Vector3 GetMoveDir => moveDir;
        public Projectile_MovementType GetMovementType => movementType;
        public Vector2 SizeDelta => gameObject.GetComponent<RectTransform>().sizeDelta;
        public Texture DisplayTexture {
            get
            {
                if (sprite.sprite != null) return sprite.sprite.texture;
                else return null;
            }
        }
        public Rect DisplayTextureRect
        {
            get
            {
                if (sprite.sprite == null) return new Rect();
                Vector4 spriteBorder = sprite.sprite.border;
                Rect unscaledRect = sprite.sprite.textureRect;
                //try to get the unscaled rect of the borders, since that's how projectiles are rendered.
                unscaledRect = new Rect(unscaledRect.x + spriteBorder.x, unscaledRect.y + spriteBorder.w,
                    unscaledRect.width - spriteBorder.x - spriteBorder.z, unscaledRect.height - spriteBorder.y - spriteBorder.w);
                Vector2 textureSize = new Vector2(DisplayTexture.width, DisplayTexture.height);
                //normalize the unscaled rect by the size of the texture.
                Rect scaledRect = new Rect(unscaledRect.x / textureSize.x, unscaledRect.y / textureSize.y,
                    unscaledRect.width / textureSize.x, unscaledRect.height / textureSize.y);
                return scaledRect;
            }
        }

        public ProjectileHitType GetHitType => hitType;

        /// <summary>
        /// Sets the move direction that's used in update to change the projectile's position.
        /// </summary>
        /// <param name="v3">Something, probably player position</param>
        public virtual void UpdateMoveDir(Vector3 v3, bool first = false)
        {
            movementType.UpdateMoveDir(v3, first);
        }

        protected virtual void OnEnable()
        {
            movementType.SetProjectileTransform = transform;
            myCol = GetComponent<Collider>();
            myRB = GetComponent<Rigidbody>();
            movementType.SetProjectileRigidbody = myRB;
            if (sprite == null) sprite = GetComponentInChildren<Image>();
            if (despawnType == null) despawnType = new Empty_Projectile_DespawnType();
            spawnInRoutine = StartCoroutine(EnableTriggerOverTime());
            if (updateTypes.Count <= 0) return;
            foreach(Projectile_CustomUpdate pcu in updateTypes)
            {
                pcu.OnEnable();
            }
        }

        protected virtual IEnumerator EnableTriggerOverTime()
        {
            myCol.enabled = false;
            endScale = transform.localScale;
            endC = sprite.color;
            float t = 0;
            float eval;
            while(t < colliderEnableTime)
            {
                t += Time.deltaTime;
                eval = (float)t / (float)colliderEnableTime;
                transform.localScale = Vector3.Lerp(Vector3.zero, endScale, eval);
                sprite.color = Color.Lerp(CoolColors.transparent, endC, eval);
                yield return null;
            }
            transform.localScale = endScale;
            myCol.enabled = true;
        }

        //Handle movements and custom update behaviors
        protected virtual void Update()
        {
            if((movementWhileActive && myCol.enabled) || (movementWhileInactive && !myCol.enabled)) movementType.MovementBehavior();
            if (updateTypes.Count <= 0) return;
            if ((customUpdateWhileActive && myCol.enabled) || (customUpdateWhileInactive && !myCol.enabled))
            {
                foreach (Projectile_CustomUpdate pcu in updateTypes)
                {
                    pcu.UpdateBehavior();
                }
            }
        }
        //Set up the procedures to take damage
        protected virtual void OnTriggerEnter(Collider other)
        {
            receiver = other.GetComponent<IDamage>();
            receiverRB = other.GetComponent<Rigidbody>();
            bool actuallyTakeDamage = true;

            if (onHitEffects.Count > 0) //on hit regardless of whether or not we hit a damageable object. maybe? physics matrix should prevent bs interactions
            {
                foreach (Projectile_OnHitEffect hitE in onHitEffects)
                {
                    hitE.OnHit(other);
                }
            }

            if (!damageOverTime && receiver != null)
            {
                switch (hitType)
                {
                    case ProjectileHitType.StayStill:
                        actuallyTakeDamage = !Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                        break;
                    case ProjectileHitType.StayMoving:
                        actuallyTakeDamage = Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                        break;
                }

                if (actuallyTakeDamage) receiver.TakeDamage(baseDamage, false);
            }
            if (destroyOnHit && !BehindWall(other) && actuallyTakeDamage)
            {
                if (updateTypes.Count > 0)
                {
                    foreach (Projectile_CustomUpdate pcu in updateTypes)
                    {
                        pcu.OnRemove(false);
                    }
                }
                Object_Pooling.Despawn(gameObject, false);
            }
        }
        //scale base damage down based on time.deltaTime since DoT is a per frame kind of thing.
        //basically turns baseDamage into baseDPS
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!damageOverTime) return;
            if (receiver != null && receiverRB != null) 
            {
                bool actuallyTakeDamage = true;
                switch (hitType)
                {
                    case ProjectileHitType.StayStill:
                        actuallyTakeDamage = !Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                        break;
                    case ProjectileHitType.StayMoving:
                        actuallyTakeDamage = Mathf.Approximately(receiverRB.linearVelocity.magnitude, 0f);
                        break;
                }
                
                if(actuallyTakeDamage) receiver.TakeDamage(baseDamage * Time.deltaTime, true); 
            }
        }

        //hopefully no overlapping other triggers nonsense?
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (receiver != null) receiver = null;
        }

        protected bool BehindWall(Collider hitcol)
        {
            if (hitcol.CompareTag("Player")) return false; //if we hit a player, absolutely refuse to give AF
            Vector3 dirToHitObj = hitcol.transform.position - transform.position;
            dirToHitObj.Normalize();
            return Vector3.Dot(hitcol.transform.up, dirToHitObj) > 0; //should only return true if locally above, which is to say behind the wall
        }

        public virtual void OnRemove(bool playerTurn)
        {
            if(updateTypes.Count > 0)
            {
                foreach(Projectile_CustomUpdate pcu in updateTypes)
                {
                    pcu.OnRemove(playerTurn);
                }
            }
            despawnType.OnDespawn(playerTurn, transform);
            StopCoroutine(spawnInRoutine);
            transform.localScale = endScale;
            sprite.color = endC;
        }

        public void OnValidate()
        {
            
        }
    }
}
