using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Chests.Items;
using Game.UI;
using Narrative;
using Narrative.Data;
using SendAppMetrica;
using Skins;
using UnityEngine;

namespace Game.LevelControll
{
    public abstract class BaseLevel : MonoBehaviour
    {
        public event System.Action<bool> StageComplete;

        public event System.Action<bool> LevelComplete;

        public event System.Action Load;

        public event System.Action Unload;
        public float BossAttackSpeed;
        public const int StageCountOnLevel = 8;
        public LevelDiffcultry Diffcultry { get { return levelDiffcultry; } }

        [SerializeField] protected LevelDiffcultry levelDiffcultry = new LevelDiffcultry();

        [SerializeField] protected Player player;

        public Enemy.Enemy bossPrefab;

        [SerializeField] protected Stage[] stages;

        public int CurrStageNumber { get; set; }

        public StageColorScheme StageColorScheme;
        public Sprite LevelMenuBackground;
        public Color LevelMenuColor;
        private static readonly int Fade = Shader.PropertyToID("_Fade");
        private const string PrevLevelBossesKey = "PrevLvlBosses";

        public Stage CurrentStage { get; set; }

        [Header("Narrative")]
        [SerializeField] 
        private float bossFadeTime;
        public Dialogue CurrentDialogue { get; private set; }
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private ReplicaUI playerReplicaUiPrefab;
        [SerializeField]
        private ReplicaUI bossReplicaUiPrefab;
        public const string PrevStageLevelIdxKey = "PrevStageLevelIdx";

        public Enemy.Enemy Boss { get; set; }

        private Camera mainCamera;
        
        public int PrevStageLevelIdx
        {
            get => PlayerPrefs.GetInt(PrevStageLevelIdxKey, 0);
            set
            {
                PlayerPrefs.SetInt(PrevStageLevelIdxKey, value);
                PlayerPrefs.Save();
            }
        }


        public virtual void Awake()
        {
            if (GameHandler.Singleton.Player.Health == 0)
            {
                CurrStageNumber = 0;
            
                StaticGameObserver.SaveProgress(GetNumLevel(), 0);
            }
            
            StaticGameObserver.LoadStartProgress(out _, out var stage);
        
            if (stage == 0)
            {
                GameHandler.Singleton.Player.Health = GameHandler.Singleton.Player.MaxHealth;
            }
            
            this.mainCamera = Camera.main;
        }

        public virtual Stage[] GetStages()
        {
            return this.stages;
        }

        public virtual void RestartStage()
        {
            UnloadStage(CurrentStage);
            
            this.player.Respawn();
            
            LoadStage(CurrStageNumber);
        }

        public void RestartLevel()
        {
            UnloadStage(CurrentStage);
        
            LoadStage(0);
        }

        public virtual void LoadNextStage(float delay)
        {
            StartCoroutine(LoadNextStageWithDelay(delay));
        }
        
        private IEnumerator LoadNextStageWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
        
            while (Pause.IsPause)
            {
                yield return null;
            }
        
            if (CurrentStage)
            {
                UnloadStage(CurrentStage);
            }
        
            LoadStage(CurrStageNumber);
        }

        public virtual void StartGame()
        {
            
        }

        public virtual int GetNumLevel()
        {
            return 0;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                CurrStageNumber = 8;
            
                StageFinish(true);
            }
        }

        public virtual void StageFinish(bool skip)
        {
            if (CurrStageNumber < GetStages().Length - 1)
            {
                CurrStageNumber++;
                StaticGameObserver.SaveProgress(GetNumLevel(), CurrStageNumber);
                StageCompleteEvent(skip);
            }
            else
            {
                CurrStageNumber++;

                UpdatePrevLevelBosses();
                
                PrevStageLevelIdx = Mathf.Clamp(GetNumLevel() - 1, 0, GetStages().Length - 1);
                
                StaticGameObserver.SaveProgress(GetNumLevel() + 1, 0);

                LevelCompleteEvent(skip);
            }        
        }

        private void InitDialogue(Dialogue dialogue, Transform bossTr)
        {
            dialogue.ResetDialogueState();
            dialogue.Canvas = this.canvas;
            dialogue.ReplicasContainer = DialogueUI.Singleton.ReplicasContainer;

            dialogue.AuthorsReplicasUI = new List<AuthorReplicaUI>
            {
                new AuthorReplicaUI
                {
                    Author = "Player",
                    ReplicaUi = this.playerReplicaUiPrefab,
                    AuthorTransform = this.player.transform,
                    Offset = new Vector2(-167f + 28f + 15f, 110 - 7)
                },
                new AuthorReplicaUI
                {
                    Author = "Boss",
                    ReplicaUi = this.bossReplicaUiPrefab,
                    AuthorTransform = bossTr.transform,
                    Offset = new Vector2(152.84f, 320-84.65f + 47f)
                }
            };
        }

        public void LoadStage(int num)
        {
            this.player.PickHP = 0;
            this.player.LoseCountHP = 0;
        
            var chestItems = FindObjectsOfType<ChestItem>();

            foreach (var chestItem in chestItems)
            {
                Destroy(chestItem.gameObject);
            }
        
            var flipCameraComp = this.mainCamera.GetComponent<MirrorFlipCamera>();
        
            if (CurrStageNumber == 0)
            {
                flipCameraComp.flipHorizontal = GetNumLevel() % 2 == 0;
            }
            else
            {
                flipCameraComp.flipHorizontal = false;
            }
        
            Load?.Invoke();

            CurrentStage = Instantiate(GetStages()[num], Vector3.zero, Quaternion.identity);

            if (!StageColorScheme.name.Equals("Japan3ColorScheme"))
            {
                var starBlink = CurrentStage.transform.Find("StarBlink");

                if (starBlink != null)
                {
                    starBlink.gameObject.SetActive(false);
                }
            }
            
            TimeControll.Singleton.UnpauseTimeControl();
            Time.timeScale = 1f;
            LevelInfoUI.LevelInfo.SetCurrLvlAndStageText(GetNumLevel(), num+1);
            LevelInfoUI.LevelInfo.SetCurrConfigText(DifficultyRepository.Instance.CurrentDifficultyConfig+1);
            
            StageColorScheme.Apply(CurrentStage);

            var dialogueData = DialogueDataController.GetInstance().DialogueData;

            var dialoguesCount = (dialogueData.StagesDialogueData.Count / 2);

            var lvlMod = GetNumLevel() % dialoguesCount;

            if (lvlMod == 0)
            {
                lvlMod = dialoguesCount;
            }
            
            var currStageDialogueData =
                dialogueData.StagesDialogueData.FirstOrDefault(d =>
                    d.Level == lvlMod && d.Stage == CurrStageNumber + 1);
                    
            if (currStageDialogueData != null)
            {
                var currStageDialogue = this.gameObject.AddComponent<Dialogue>();
                
                currStageDialogue.Replicas = new List<Replica>();

                foreach (var replica in currStageDialogueData.Replicas)
                {
                    currStageDialogue.Replicas.Add(replica);
                }

                currStageDialogue.DelayBetweenReplicas = 0.5f;
                
                var bossNarrative = SpawnBoss(bossPrefab, CurrentStage.NarrativeBossPlace.position, Quaternion.Euler(0, -90, 0));
                
                if (CurrStageNumber == 0)
                {
                    bossNarrative.NeedUpdateTarget = false;
                
                    if (!bossNarrative.name.Equals("BossBazooka(Clone)"))
                    {
                        bossNarrative.SetActiveIKFabrics(false);
                    
                        var weapon = bossNarrative.GetWeapon();
                        weapon.transform.SetParent(bossNarrative.RightFinger);
                        weapon.transform.localPosition = new Vector3(0.00029f, -0.00103f, 0f);
                        weapon.transform.localRotation = Quaternion.Euler(-85.619f, -118.397f, 28.02f);
                    }

                    CameraControll.Singleton.SetPosition(bossNarrative.transform.position + Vector3.up + Vector3.right * 0.8f);
                }

                StartCoroutine(ShowDialogueProcess(1.5f, currStageDialogue, bossNarrative));
            }
            else
            {
                StartCoroutine(EnemiesStartAttack(0.5f));
            }
            
            if (num == GetStages().Length - 1)
            {
                Boss = SpawnBoss(bossPrefab, CurrentStage.BossPlace.position, bossPrefab.transform.rotation);;
                
                Boss.transform.SetParent(CurrentStage.transform);
            }
            
            CurrentStage.InitStage(this.player);

            CallLoadStage();
            
            TimerStage.StartRecord();
        }

        private IEnumerator BossElevator()
        {
            var endElevator = CurrentStage.GetComponent<EndElevator>();
            
            var bossElevator = SpawnBoss(bossPrefab, endElevator.elevator.position, bossPrefab.transform.rotation);
            
            bossElevator.transform.SetParent(CurrentStage.transform);
            
            yield return null;
            
            bossElevator.attackTarget.StopAttack();

            EnemyControll.Singleton.RemoveEnemy(bossElevator);

            endElevator.SetBoss(bossElevator);
        }

        private IEnumerator EnemiesStartAttack(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var enemies = EnemyControll.Singleton.GetArray();
            
            foreach (var enemy in enemies)
            {
                enemy.attackTarget.StartAttack();
                enemy.SetActiveIKFabrics(true);
            }
        }

        private IEnumerator ShowDialogueProcess(float delay, Dialogue dialogue, Enemy.Enemy boss)
        {
            yield return new WaitForSeconds(delay);

            if (dialogue == null)
            {
                yield break;
            }

            if (CurrStageNumber == 0)
            {
                this.player.StateController.BeginControll();
            }

            CurrentDialogue = dialogue;
            
            InitDialogue(dialogue, boss.transform);
            dialogue.ShowDialog();
            
            DialogueUI.Singleton.SetActive(true);

            while (!dialogue.IsShowed)
            {
                yield return null;
            }
            
            DialogueUI.Singleton.SetActive(false);

            if (CurrStageNumber != 0)
            {
                yield return StartCoroutine(BossFade(this.bossFadeTime, boss));

                Destroy(boss.gameObject);
            }

            yield return new WaitForSeconds(0.1f);
            
            if (CurrStageNumber != 0)
            {
                this.player.StateController.EndControll();

                var enemies = EnemyControll.Singleton.GetArray();
                
                foreach (var enemy in enemies)
                {
                    enemy.attackTarget.StartAttack();
                    enemy.SetActiveIKFabrics(true);
                }

                DragToAim.Singleton.Show();
                DragToAim.Singleton.Hide(3f);
            }

            CurrentDialogue = null;
            
            if (CurrStageNumber == 0)
            {
                var endElevator = CurrentStage.GetComponent<EndElevator>();

                var elevatorPos = endElevator.elevator.transform.position;
            
                var bossStartRot = boss.transform.eulerAngles;
                var bossTargetRot = new Vector3(boss.transform.eulerAngles.x, boss.transform.eulerAngles.y + 180f, boss.transform.eulerAngles.z);

                var rotTime = 0.2f;
                var currRotTime = rotTime;
                
                // поворачиваем босса лицом к лифту
                while (currRotTime > 0f)
                {
                    var lerp = 1 - currRotTime / rotTime;

                    boss.transform.eulerAngles = Vector3.Slerp(bossStartRot, bossTargetRot, lerp);
                
                    currRotTime -= Time.deltaTime;

                    yield return null;
                }
                
                boss.RunAnim();
                
                // босс бежит в лифт
                while (boss.transform.position.x < elevatorPos.x)
                {
                    boss.transform.position += boss.transform.forward * 8f * Time.deltaTime;

                    yield return null;
                }
                
                boss.IdleAnim();
                
                bossStartRot = boss.transform.eulerAngles;
                bossTargetRot = new Vector3(boss.transform.eulerAngles.x, boss.transform.eulerAngles.y + 180f, boss.transform.eulerAngles.z);
                
                currRotTime = rotTime;
                
                // босс уже в лифте. поворачиваем босса лицом к выходу
                while (currRotTime > 0f)
                {
                    var lerp = 1 - currRotTime / rotTime;

                    boss.transform.eulerAngles = Vector3.Slerp(bossStartRot, bossTargetRot, lerp);
                
                    currRotTime -= Time.deltaTime;

                    yield return null;
                }

                var liftHeight = 7.5f;

                elevatorPos = endElevator.elevator.transform.position;
                
                var elevStartPos = elevatorPos;
                var elevTargetPos = elevStartPos + Vector3.up * liftHeight;

                var liftTime = 1.0f;
                var currLiftTime = liftTime;

                var bossStartPos = boss.transform.position;
                var bossEndPos = bossStartPos + Vector3.up * liftHeight;

                // босс уезжает на лифте
                while (currLiftTime > 0)
                {
                    var lerp = 1 - currLiftTime / liftTime;
                    
                    endElevator.elevator.transform.position = Vector3.Lerp(elevStartPos, elevTargetPos, lerp);
                    boss.transform.position = Vector3.Lerp(bossStartPos, bossEndPos, lerp);
                
                    currLiftTime -= Time.deltaTime;

                    yield return null;
                }
                
                Destroy(boss.gameObject);
                
                elevatorPos = endElevator.elevator.transform.position;
                
                elevStartPos = elevatorPos;
                elevTargetPos = elevStartPos - Vector3.up * liftHeight;

                currLiftTime = liftTime;
                
                // босс уехал на лифте. опускаем лифт
                while (currLiftTime > 0)
                {
                    var lerp = 1 - currLiftTime / liftTime;
                    
                    endElevator.elevator.transform.position = Vector3.Lerp(elevStartPos, elevTargetPos, lerp);
                
                    currLiftTime -= Time.deltaTime;

                    yield return null;
                }
            
                this.player.StateController.PlayerRun();
            
                // игрок бежит к лифту
                while (this.player.transform.position.x < elevatorPos.x)
                {
                    this.player.transform.position += this.player.transform.right * 8f * Time.deltaTime;

                    yield return null;
                }
                
                this.player.transform.position = new Vector3(elevatorPos.x, this.player.transform.position.y, this.player.transform.position.z);
                
                this.player.StateController.PlayerStay();
                
                elevatorPos = endElevator.elevator.transform.position;
                
                elevStartPos = elevatorPos;
                elevTargetPos = elevStartPos + Vector3.up * liftHeight;

                var playerStartPos = this.player.transform.position;
                var playerEndPos = playerStartPos + Vector3.up * liftHeight;

                currLiftTime = liftTime;
                
                // игрок уезжает на лифте
                while (currLiftTime > 0)
                {
                    var lerp = 1 - currLiftTime / liftTime;
                    
                    endElevator.elevator.transform.position = Vector3.Lerp(elevStartPos, elevTargetPos, lerp);
                    this.player.transform.position = Vector3.Lerp(playerStartPos, playerEndPos, lerp);
                
                    currLiftTime -= Time.deltaTime;

                    yield return null;
                }
                
                DialogueUI.Singleton.SetActive(false);
                
                TimeControll.Singleton.UnpauseTimeControl();
                Time.timeScale = 1f;
            }
        }

        private IEnumerator BossFade(float time, Enemy.Enemy boss)
        {
            var materials = boss.GetMaterials();

            var fadeMaterials = materials.Where(m => m.HasProperty("_Fade")).ToArray();

            var t = time;
            
            while (t > 0f)
            {
                var lerp = t / time;

                foreach (var fadeMaterial in fadeMaterials)
                {
                    fadeMaterial.SetFloat(Fade, lerp);
                }

                t -= Time.deltaTime;

                yield return null;
            }

            var fadeEffect = boss.transform.Find("FadeEffect");
            
            fadeEffect.SetParent(CurrentStage.transform);
            
            fadeEffect.GetComponent<ParticleSystem>().Play();
        }

        public StageIconParams[] GetStageIconParams()
        {
            var stageIconParams = new StageIconParams[GetStages().Length - 1];
        
            for (var i = 0; i < stageIconParams.Length; i++)
            {
                var stg = i + 1;
            
                stageIconParams[i] = new StageIconParams
                {
                    Bonus = false,
                    Boss = stg == GetStages().Length - 1,
                    Completed = stg < CurrStageNumber,
                    Current = stg == CurrStageNumber,
                    Skin = SkinRepository.Instance.GetSkinForOpen(GameHandler.Singleton.Level.GetNumLevel(), stg + 1) != null
                };
            }

            return stageIconParams;
        }
        
        protected void LoadStage(Stage stage, int stageNum, StageColorScheme colorScheme, Enemy.Enemy bossPrefab)
        {
            Load?.Invoke();

            CurrentStage = Instantiate(stage, Vector3.zero, Quaternion.identity);
            
            if (!StageColorScheme.name.Equals("Japan3ColorScheme"))
            {
                var starBlink = CurrentStage.transform.Find("StarBlink");

                if (starBlink != null)
                {
                    starBlink.gameObject.SetActive(false);
                }
            }
            
            colorScheme.Apply(CurrentStage);
            
            LevelInfoUI.LevelInfo.SetCurrLvlAndStageText(GetNumLevel(), stageNum+1);
            LevelInfoUI.LevelInfo.SetCurrConfigText(DifficultyRepository.Instance.CurrentDifficultyConfig+1);

            if (stageNum == 0)
            {
                var boss = SpawnBoss(bossPrefab, CurrentStage.NarrativeBossPlace.position, bossPrefab.transform.rotation);
            }

            if (stageNum == GetStages().Length - 1)
            {
                var boss = SpawnBoss(bossPrefab, CurrentStage.BossPlace.position, bossPrefab.transform.rotation);

                if (bossPrefab.name.Equals("BossGangster"))
                {
                    boss.attackTarget.speedAttack = 0.5f;
                }

                if (bossPrefab.name.Equals("BossRobot"))
                {
                    boss.attackTarget.speedAttack = 0.65f;
                }
            }
            
            CurrentStage.InitStage(this.player);
            
            CallLoadStage();
            
            TimerStage.StartRecord();
        }
        

        public Enemy.Enemy SpawnBoss(Enemy.Enemy bossPrefab, Vector3 pos, Quaternion rot)
        {
            var boss = Instantiate(bossPrefab, pos, rot);

            boss.attackTarget.StartAttackDelay = 0f;

            if (BossAttackSpeed > 0f)
            {
                boss.attackTarget.speedAttack = BossAttackSpeed;
            }         
            
            BossUI.Instance.SetActive(false);
            
            BossUI.Instance.Boss = boss;

            return boss;
        }
        

        protected virtual void CallLoadStage()
        {   
        }

        public void UnloadStage(Stage stage)
        {
            stage.DisableStage();
            Destroy(stage.gameObject);
            CallUnloadStage();
        }

        protected virtual void CallUnloadStage()
        {

        }

        protected void LevelCompleteEvent(bool skip)
        {
            LevelComplete?.Invoke(skip);
        }

        protected void StageCompleteEvent(bool skip)
        {
            StageComplete?.Invoke(skip);
        }
        
        public void UpdatePrevLevelBosses()
        {
            var prevLvlBossesStr = PlayerPrefs.GetString(PrevLevelBossesKey, string.Empty);

            var prevLvlBosses = new List<string>();

            if (!string.IsNullOrEmpty(prevLvlBossesStr))
            {
                prevLvlBosses = prevLvlBossesStr.Split(',').ToList();
            }

            if (prevLvlBosses.Count >= 3)
            {
                prevLvlBosses[0] = prevLvlBosses[1];
                prevLvlBosses[1] = prevLvlBosses[2];
                prevLvlBosses[2] = this.bossPrefab.name;
            }
            else
            {
                prevLvlBosses.Add(this.bossPrefab.name);
            }

            prevLvlBossesStr = prevLvlBosses.Aggregate(string.Empty, (current, prevLvlBoss) => current + (prevLvlBoss + ","));

            prevLvlBossesStr = prevLvlBossesStr.Remove(prevLvlBossesStr.Length - 1);

            PlayerPrefs.SetString(PrevLevelBossesKey, prevLvlBossesStr);
        }

        public List<string> GetPrevLevelBosses()
        {
            var prevLevelBossesStr = PlayerPrefs.GetString(PrevLevelBossesKey, string.Empty);

            var prevLvlBosses = new List<string>();
            
            if (!string.IsNullOrEmpty(prevLevelBossesStr))
            {
                prevLvlBosses = prevLevelBossesStr.Split(',').ToList();
            }

            return prevLvlBosses;
        }
    }
}
