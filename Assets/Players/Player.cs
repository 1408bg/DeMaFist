using System.Collections;
using UnityEngine;

namespace Players
{
    public enum PlayerTeam
    {
        TeamA,
        TeamB
    }

    public enum PlayerStatus
    {
        Normal,
        Stun,
        AirBone,
        Dead
    }
    public class Player : MonoBehaviour
    {
        [Header("플레이어 팀")] public PlayerTeam team;
        [Header("플레이어 모션")] public Animator ani;
        [Header("이속")]
        public float moveSpeed;
        [Header("체력")]
        public float maxHp;
        [Header("궁극기 게이지")]
        public float ultimateGauge;
        [Header("스턴시간")]
        public float stunTime;
        [Header("플레이어 리지드 바디")]
        public Rigidbody2D rb;
        [Header("플레이어 히트박스")]
        public Collider2D hitBox;
        [Header("플레이어 데미지 박스 순서데로 q, e, r")] public Collider2D[] damageBox;
        [Header("플레이어 블록/패링 박스 순서데로 블록 ,패링")] public Collider2D[] blockBox = new Collider2D[2];
        [Header("점프하는 힘")] public float jumpForce;
        [Header("플레이어 상태")] public PlayerStatus playerStatus;
        [Header("플레이어 발")] public GameObject foot;
        [Header("해당 플레이어 최대 점프 횟수")] public int maxJump;
        [Header("땅 설정")] public LayerMask ground;
        private float _currentUltimateGauge;
        private float _currentHp;
        private float _currentStunTime;
        private int _isFacingRight; 
        private int _currentJump;
        protected void SetUpPlayer()
        {
            _currentHp = 0;
            _currentHp = maxHp;
            _currentStunTime = stunTime;
            _currentUltimateGauge = 0;
            _isFacingRight = team == PlayerTeam.TeamA ? 1 : -1;
            playerStatus = PlayerStatus.Normal;
        }
        protected void CheckStatus()
        {
            
        }

        public void Stun(float force, float time,int forceMode)
        {
            if (playerStatus == PlayerStatus.Stun)
            {
                StopCoroutine(StunFlow(force, time ,forceMode));
            }
            StartCoroutine(StunFlow(force, time,forceMode));
        }

        public void AirBone(float force, float time ,int forceMode)
        {
            if (playerStatus != PlayerStatus.AirBone)
            {
                StartCoroutine(AirBoneFlow(force, time, forceMode));
            }
        }
        protected IEnumerator StunFlow(float force, float time,int forceMode)
        {
            var dir = 0;
            switch (forceMode)
            {
                case 0:
                    dir = 0;
                    break;
                case 1:
                    dir = 1;
                    break;
                case -1:
                    dir = -1;
                    break;
            }
            rb.velocity = new Vector2(0, 0);
            playerStatus = PlayerStatus.Stun;
            rb.AddForce(new Vector2(dir,0)* (force*_isFacingRight),ForceMode2D.Impulse);
            yield return new WaitForSeconds(time);
            playerStatus = PlayerStatus.Normal;
        }
        
        protected IEnumerator AirBoneFlow(float force, float time,int forceMode)
        {
            var dir = 0;
            switch (forceMode)
            {
                case 0:
                    dir = 0;
                    break;
                case 1:
                    dir = 1;
                    break;
                case -1:
                    dir = -1;
                    break;
            }
            rb.velocity = new Vector2(0, 0);
            playerStatus = PlayerStatus.AirBone;
            rb.AddForce(new Vector2(dir,0) * (force*_isFacingRight),ForceMode2D.Impulse);
            rb.AddForce(Vector2.up*(force*2),ForceMode2D.Impulse);
            yield return new WaitForSeconds(time);
            playerStatus = PlayerStatus.Normal;
        }
        protected void CheckSkill()
        {
            if (team == PlayerTeam.TeamA)
            {
                ASkillSet();
            }
            else
            {
                BSkillSet();
            }
        }

        protected void CheckFloor()
        {
            if (Physics2D.OverlapCircle(foot.transform.position, 0.1f, ground))
            {
                _currentJump = 0;
            }
            else if (_currentJump == 0 && Physics2D.OverlapCircle(foot.transform.position, 0.1f, ground) == false)
            {
                _currentJump += 1;
            }
        }

        protected void CheckMovement()
        {
            if (team == PlayerTeam.TeamA)
            {
                AMove();
            }
            else
            {
                BMove();
            }
        }

        protected void AMove()
        {
            if (playerStatus != PlayerStatus.Normal) return;
            var horizontal = 0;
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1;
                _isFacingRight = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1;
                _isFacingRight = 1;
            }
            transform.localScale = new Vector3(_isFacingRight, transform.localScale.y);
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }

        protected void ASkillSet()
        {
            if (playerStatus == PlayerStatus.Normal)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    DefaultSkill();
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    AbilitySkill();
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    UltimateSkill();
                }
                else if(Input.GetKeyDown(KeyCode.F))
                {
                    BlockSkill();
                }
                if (Input.GetKeyDown(KeyCode.W)&&_currentJump<maxJump)
                {
                    rb.AddForce(Vector2.up*jumpForce,ForceMode2D.Impulse);
                    _currentJump++;
                }
            }

        }

        protected void BSkillSet()
        {
            if (playerStatus == PlayerStatus.Normal)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    DefaultSkill();
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    AbilitySkill();
                }
                else if (Input.GetKeyDown(KeyCode.O))
                {
                    UltimateSkill();
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    BlockSkill();
                }
                if (Input.GetKeyDown(KeyCode.U)&&_currentJump<maxJump)
                {
                    rb.AddForce(Vector2.up*jumpForce,ForceMode2D.Impulse);
                    _currentJump++;
                }
            }
        }

        public virtual void DefaultSkill()
        {
            Debug.Log(team+"used defaultskill");
        }

        public virtual void AbilitySkill()
        {
            Debug.Log(team+"used abilityskill");
        }

        public virtual void BlockSkill()
        {
            Debug.Log(team+"used blockskill");
        }

        public virtual void UltimateSkill()
        {
            Debug.Log(team+"used ultimateskill");
        }
        protected void BMove()
        {
            if (playerStatus != PlayerStatus.Normal) return;
            var horizontal = 0;
            if (Input.GetKey(KeyCode.H))
            {
                horizontal = -1;
                _isFacingRight *= -1;
            }
            else if (Input.GetKey(KeyCode.K))
            {
                horizontal = 1;
                _isFacingRight *= -1;
            }

            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }
}
