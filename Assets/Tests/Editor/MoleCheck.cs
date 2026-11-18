using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityGameFramework.Runtime;

[InitializeOnLoad]
public static class MoleCheck
{
    private const string PrefabPath = "Assets/Prefabs/Characters/Mole.prefab";
    private const string RunningKey = "MoleCheck.Running";
    private static double startedAt = EditorApplication.timeSinceStartup;
    private static Vector3 initialPosition;
    private static float initialDistance;
    private static float checkAfter;
    private static int phase;
    private static int hpBeforeAttack;
    private static int expectedDamage;
    private static float phaseStartedAt;
    private static EnemyLogic dyingEnemy;
    private static string dyingFsmName;

    [MenuItem("Tools/Tests/Mole Prefab And Animations")]
    public static void CheckPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Assert.IsNotNull(prefab);
        var previewScene = EditorSceneManager.NewPreviewScene();
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, previewScene);
        try
        {
            var renderer = instance.transform.Find("Visual").GetComponent<SpriteRenderer>();
            Assert.IsNull(instance.GetComponent<SpriteRenderer>());
            Assert.IsNotNull(renderer);
            var stats = new SerializedObject(instance.GetComponent<StatComponent>());
            Assert.AreEqual(renderer, stats.FindProperty("damageSprite").objectReferenceValue);
            var animator = instance.GetComponent<Animator>();
            Assert.IsNotNull(animator);
            Assert.IsFalse(animator.applyRootMotion);
            var controller = animator.runtimeAnimatorController as AnimatorController;
            Assert.IsNotNull(controller);
            Assert.AreEqual("Idle", controller.layers[0].stateMachine.defaultState.name);
            Assert.AreEqual(4, controller.layers[0].stateMachine.states.Length);
            Assert.AreEqual("isMove", controller.parameters[0].name);

            foreach (var child in controller.layers[0].stateMachine.states)
            {
                var clip = child.state.motion as AnimationClip;
                Assert.IsNotNull(clip);
                bool isAttack = child.state.name == "Attack";
                bool isDown = child.state.name == "Down";
                Assert.AreEqual(!isAttack && !isDown, AnimationUtility.GetAnimationClipSettings(clip).loopTime);
                var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                Assert.AreEqual(1, bindings.Length);
                Assert.AreEqual("Visual", bindings[0].path);
                Assert.AreEqual("m_Sprite", bindings[0].propertyName);
                Assert.IsTrue(AnimationUtility.GetObjectReferenceCurve(clip, bindings[0]).Length > 1);
                Vector3 rootPosition = instance.transform.position;
                clip.SampleAnimation(instance, 0f);
                Sprite firstFrame = renderer.sprite;
                clip.SampleAnimation(instance, 1f / clip.frameRate + 0.001f);
                Assert.IsNotNull(renderer.sprite);
                if (isDown)
                    Assert.AreEqual(firstFrame, renderer.sprite);
                else
                    Assert.AreNotEqual(firstFrame, renderer.sprite);
                Assert.AreEqual(rootPosition, instance.transform.position);
                Assert.AreEqual(isAttack || isDown ? 0 : 1, child.state.transitions.Length);
                if (isAttack)
                {
                    var events = AnimationUtility.GetAnimationEvents(clip);
                    Assert.AreEqual(2, events.Length);
                    Assert.AreEqual("OnAttackHit", events[0].functionName);
                    Assert.AreEqual("OnAttackEnd", events[1].functionName);
                    Assert.IsTrue(events[0].time > 0f && events[0].time < events[1].time);
                    Assert.IsTrue(Mathf.Abs(events[1].time - clip.length) < 0.001f);
                }
                else if (isDown)
                {
                    var events = AnimationUtility.GetAnimationEvents(clip);
                    Assert.AreEqual(1, events.Length);
                    Assert.AreEqual("OnDeathEnd", events[0].functionName);
                    Assert.IsTrue(Mathf.Abs(events[0].time - clip.length) < 0.001f);
                }
                else
                {
                    Assert.IsFalse(child.state.transitions[0].hasExitTime);
                    Assert.AreEqual(0f, child.state.transitions[0].duration);
                }
            }
            Assert.IsTrue(instance.transform.Find("HealthBar").localScale.x > 0f);
            Debug.Log("MOLE_PREFAB_CHECK_PASSED: Visual, locomotion, attack events, Down pose and death event.");
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(previewScene);
        }
    }

    [MenuItem("Tools/Tests/Enemy HP Bar")]
    public static void CheckHealthBar()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        Assert.IsNotNull(prefab);
        var previewScene = EditorSceneManager.NewPreviewScene();
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, previewScene);
        try
        {
            var stats = instance.GetComponent<StatComponent>();
            Assert.IsNotNull(stats);
            stats.enabled = false;
            var bar = instance.GetComponentInChildren<HealthBarComponent>();
            Assert.IsNotNull(bar);
            var fill = instance.transform.Find("HealthBar/Fill/Visual");
            Assert.IsTrue(instance.transform.Find("HealthBar").localPosition.y < -0.4f);
            stats.ResetHealth();
            bar.SendMessage("LateUpdate");
            float fullWidth = fill.GetComponent<SpriteRenderer>().bounds.size.x;
            float leftEdge = fill.GetComponent<SpriteRenderer>().bounds.min.x;

            stats.TakeDamage(stats.MaxHp / 2);
            bar.SendMessage("LateUpdate");
            Assert.IsTrue(Mathf.Abs(fill.GetComponent<SpriteRenderer>().bounds.size.x
                - fullWidth * 0.5f) < 0.001f);
            Assert.IsTrue(Mathf.Abs(fill.GetComponent<SpriteRenderer>().bounds.min.x
                - leftEdge) < 0.001f, "HP bar must shrink from the right edge.");

            stats.Heal(stats.MaxHp);
            bar.SendMessage("LateUpdate");
            Assert.IsTrue(Mathf.Abs(fill.GetComponent<SpriteRenderer>().bounds.size.x
                - fullWidth) < 0.001f);
            stats.TakeDamage(int.MaxValue);
            bar.SendMessage("LateUpdate");
            Assert.AreEqual(0f, fill.parent.localScale.x);
            stats.ResetHealth();
            bar.SendMessage("LateUpdate");
            Assert.AreEqual(1f, fill.parent.localScale.x);
            Debug.Log("ENEMY_HP_BAR_CHECK_PASSED: damage, left anchor, heal, zero HP, reset.");
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(previewScene);
        }
    }

    static MoleCheck()
    {
        EditorApplication.update += Check;
    }

    [MenuItem("Tools/Tests/Mole Combat And Death")]
    public static void Run()
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
            throw new InvalidOperationException("Save the current scene before running the enemy check.");

        startedAt = EditorApplication.timeSinceStartup;
        phase = 0;
        CheckPrefab();
        EditorSceneManager.OpenScene("Assets/Scenes/TestScene.unity");
        SessionState.SetBool(RunningKey, true);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PrepareInput()
    {
        if (!SessionState.GetBool(RunningKey, false))
            return;
        if (Mouse.current == null) InputSystem.AddDevice<Mouse>();
        if (Keyboard.current == null) InputSystem.AddDevice<Keyboard>();
    }

    private static void Check()
    {
        if (!SessionState.GetBool(RunningKey, false))
            return;

        try
        {
            Assert.IsTrue(EditorApplication.timeSinceStartup - startedAt < 60d,
                "Timed out waiting for the test scene and enemy.");
            if (!EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;

            var player = UnityEngine.Object.FindFirstObjectByType<PlayerLogic>();
            if (player == null)
                return;

            if (phase == 10)
            {
                var entityComponent = GameEntry.GetComponent<EntityComponent>();
                if (entityComponent.HasEntity(2))
                {
                    Assert.IsTrue(Time.time < checkAfter, "Down ended without recycling the enemy.");
                    return;
                }

                Assert.IsFalse(GameEntry.GetComponent<FsmComponent>().HasFsm<EnemyLogic>(dyingFsmName));
                dyingEnemy.OnAttackHit();
                dyingEnemy.OnAttackEnd();
                dyingEnemy.OnDeathEnd();
                Assert.AreEqual(hpBeforeAttack - expectedDamage, player.GetComponent<StatComponent>().CurrentHp);
                entityComponent.ShowEntity<EnemyLogic>(2, PrefabPath, "Enemy", player.transform);
                phase = 11;
                return;
            }

            var enemy = UnityEngine.Object.FindFirstObjectByType<EnemyLogic>();
            if (enemy == null)
                return;

            var agent = enemy.GetComponent<NavMeshAgent>();
            var animator = enemy.GetComponent<Animator>();
            var fsms = GameEntry.GetComponent<FsmComponent>();
            var fsm = fsms.GetFsm<EnemyLogic>(enemy.Entity.Id.ToString());
            var playerStats = player.GetComponent<StatComponent>();
            var enemyStats = enemy.GetComponent<StatComponent>();
            Assert.IsNotNull(fsm);
            if (phase == 0)
            {
                Assert.IsTrue(agent.isOnNavMesh, "Enemy did not spawn on the baked NavMesh.");
                Assert.IsFalse(agent.updateRotation);
                Assert.IsFalse(agent.updateUpAxis);
                var renderer = enemy.transform.Find("Visual").GetComponent<SpriteRenderer>();
                Assert.IsNotNull(renderer.sprite);
                Assert.AreEqual(Color.white, renderer.color);
                Assert.IsNotNull(animator.runtimeAnimatorController);
                initialPosition = enemy.transform.position;
                initialDistance = Vector3.Distance(initialPosition, player.transform.position);
                checkAfter = Time.time + 0.4f;
                phase = 1;
            }
            else if (Time.time >= checkAfter && phase == 1)
            {
                Assert.IsTrue(Vector3.Distance(enemy.transform.position, initialPosition) > 0.2f,
                    "Enemy did not move.");
                Assert.IsTrue(Vector3.Distance(enemy.transform.position, player.transform.position)
                    < initialDistance - 0.2f, "Enemy did not approach the player.");
                Assert.IsTrue(fsm.CurrentState is EnemyStateChase);
                Assert.IsTrue(animator.GetBool("isMove"));
                Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("Move"));
                phase = 2;
            }
            else if (phase == 2 && fsm.CurrentState is EnemyStateAttack)
            {
                Assert.IsTrue(agent.isStopped);
                Assert.IsFalse(animator.GetBool("isMove"));
                Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
                hpBeforeAttack = playerStats.CurrentHp;
                Assert.AreEqual(playerStats.MaxHp, hpBeforeAttack, "Damage occurred before the hit frame.");
                expectedDamage = Mathf.Max(0, enemyStats.Attack - playerStats.Defense);
                Assert.IsTrue(expectedDamage > 0);
                phase = 3;
            }
            else if (phase == 3 && playerStats.CurrentHp < hpBeforeAttack)
            {
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp);
                enemy.OnAttackHit();
                enemy.OnAttackHit();
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp,
                    "Duplicate hit events applied damage more than once.");
                phase = 4;
            }
            else if (phase == 4 && !(fsm.CurrentState is EnemyStateAttack))
            {
                Assert.IsFalse(enemy.CanAttack);
                phaseStartedAt = Time.time;
                phase = 5;
            }
            else if (phase == 5)
            {
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp);
                if (Time.time - phaseStartedAt < 0.8f)
                    Assert.IsFalse(fsm.CurrentState is EnemyStateAttack, "Attack cooldown was skipped.");

                if (!(fsm.CurrentState is EnemyStateAttack))
                    return;

                Assert.IsTrue(NavMesh.SamplePosition(player.transform.position + Vector3.down * 3f,
                    out NavMeshHit hit, 0.5f, agent.areaMask));
                Assert.IsTrue(Vector3.Distance(hit.position, enemy.transform.position) > 2f);
                player.transform.position = hit.position;
                checkAfter = Time.time + 0.8f;
                phase = 6;
            }
            else if (phase == 6 && Time.time >= checkAfter)
            {
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp,
                    "An attack hit after the player left its range.");
                Assert.IsTrue(Vector3.Distance(agent.destination, player.transform.position) < 0.1f,
                    "Enemy did not update its destination after the player moved.");
                Assert.IsTrue(fsm.CurrentState is EnemyStateChase);
                string fsmName = enemy.Entity.Id.ToString();
                enemyStats.TakeDamage(10);
                var entities = GameEntry.GetComponent<EntityComponent>();
                entities.HideEntity(2);
                Assert.IsFalse(agent.enabled, "Pooled enemy left its NavMeshAgent enabled.");
                Assert.IsFalse(fsms.HasFsm<EnemyLogic>(fsmName));
                enemy.OnAttackHit();
                enemy.OnAttackEnd();
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp);
                entities.ShowEntity<EnemyLogic>(2, PrefabPath, "Enemy", player.transform);
                phase = 7;
            }
            else if (phase == 7)
            {
                Assert.AreEqual(enemyStats.MaxHp, enemyStats.CurrentHp);
                Assert.IsTrue(NavMesh.SamplePosition(enemy.transform.position + Vector3.left * 0.9f,
                    out NavMeshHit hit, 0.25f, agent.areaMask));
                player.transform.position = hit.position;
                checkAfter = Time.time + 0.15f;
                phase = 8;
            }
            else if (phase == 8 && Time.time >= checkAfter)
            {
                Assert.IsTrue(fsm.CurrentState is EnemyStateAttack, "Attack did not reset on pooled reuse.");
                dyingEnemy = enemy;
                dyingFsmName = enemy.Entity.Id.ToString();
                enemyStats.TakeDamage(int.MaxValue);
                enemy.OnAttackHit();
                enemy.OnAttackEnd();
                Assert.AreEqual(hpBeforeAttack - expectedDamage, playerStats.CurrentHp,
                    "A dead enemy applied an attack event.");
                checkAfter = Time.time + 0.1f;
                phase = 9;
            }
            else if (phase == 9 && Time.time >= checkAfter)
            {
                Assert.IsTrue(fsm.CurrentState is EnemyStateDead, "Death did not interrupt Attack.");
                Assert.IsTrue(animator.GetCurrentAnimatorStateInfo(0).IsName("Down"));
                Assert.IsFalse(agent.enabled);
                Assert.IsFalse(enemy.GetComponent<Collider2D>().enabled);
                Assert.IsFalse(enemy.CanAttack);
                checkAfter = Time.time + 1.5f;
                phase = 10;
            }
            else if (phase == 11)
            {
                Assert.AreEqual(enemyStats.MaxHp, enemyStats.CurrentHp);
                Assert.IsTrue(enemy.GetComponent<Collider2D>().enabled);
                Assert.IsTrue(agent.enabled);
                Assert.IsFalse(fsm.CurrentState is EnemyStateDead);
                enemy.OnDeathEnd();
                Assert.IsTrue(GameEntry.GetComponent<EntityComponent>().HasEntity(2),
                    "A reused enemy accepted an old death event.");
                GameEntry.GetComponent<EntityComponent>().HideEntity(2);
                Debug.Log("MOLE_COMBAT_CHECK_PASSED: chase, attack, dodge, death interrupt, Down, recycle and reuse.");
                Finish(0);
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            Finish(1);
        }
    }

    private static void Finish(int exitCode)
    {
        SessionState.SetBool(RunningKey, false);
        if (Application.isBatchMode)
            EditorApplication.Exit(exitCode);
        else
            EditorApplication.isPlaying = false;
    }
}
