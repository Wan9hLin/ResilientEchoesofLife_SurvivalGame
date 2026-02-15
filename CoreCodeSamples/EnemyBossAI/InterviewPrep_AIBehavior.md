# 面试速记：状态驱动的生物 AI 行为系统

> 覆盖 4 种行为模式：**被动（Passive）**、**警惕/受惊（Alert/Flee）**、**主动攻击（Provoked Attack）**、**掠食者追猎（Predator Hunt）**。

## 1) 系统共性（可以先总述）

- 三类动物脚本都基于 `NavMeshAgent + Animator + Coroutine`：
  - `NavMeshAgent` 负责寻路、追逐、停靠距离。
  - `Animator` 通过 `isWalk / isMove / Attack / Hit / Dead` 等参数驱动动画状态。
  - `Coroutine` 用于将“巡逻、追逐、攻击、休息”拆成可中断流程。
- 战斗命中通过 `OnTriggerEnter` 与武器碰撞实现；死亡后统一掉落资源（肉）并回收对象。
- 出生点/游荡点由 `WanderPointsManager` 或 `WanderPointManager_4` 单例提供，生成数量由 `AnimalSpawnerScript` 维护上限。

---

## 2) 被动 + 警惕：`GenerAnimalController_2`

### 行为逻辑
- `Start()` 时进入 `Wander()` 协程：随机取游荡点，低速行走。
- `Update()` 检测玩家距离：
  - 距离小于 `alertDistance`：进入 `RunAndRest()`（受惊逃跑）。
  - 距离大于 `safeDistance`：回到游荡。
- `RunAndRest()` 分三段：
  1. 沿远离玩家方向跑动 `runDuration`。
  2. 用 `Mathf.Lerp` 在 `slowdownDuration` 内减速。
  3. 进入 `restDuration` 休息，再恢复巡游。

### 面试可说的设计点
- 这是“有限状态机思想 + 协程实现”：巡游/逃跑/休息互斥，状态切换由**距离阈值**触发。
- `alertDistance` 和 `safeDistance` 分离，形成“滞回区间”，可减少状态抖动。

---

## 3) 主动攻击：`AttackAnimalController`

### 行为逻辑
- 默认是游荡，不主动攻击。
- 被玩家命中后 `TakeDamage()` 将 `isProvoked = true`，进入“追击/攻击”分支。
- `Update()` 内根据距离和挑衅状态切换：
  - `isProvoked && distance <= attackDistance`：攻击。
  - `isProvoked && attackDistance < distance < safeDistance`：追击。
  - `distance >= safeDistance`：脱战，回到游荡。
- 攻击命中窗口由 `AttackColliderScript` 控制（通常由动画事件开关碰撞体）。

### 面试可说的设计点
- 这类敌人是“事件驱动仇恨”：**受击才进入战斗状态**。
- 通过 `attackStopDistance` 停止位移，避免贴脸抖动，动作表现更稳定。
- 订阅 `PlayerStatsManager.OnHealthChanged`：玩家死亡时停止追杀，避免无意义 AI 开销。

---

## 4) 掠食者追猎：`FindPlayerAnimalController`

### 行为逻辑
- 出生后立刻 `StartCoroutine(ChasePlayer())`，属于天然敌对。
- `Update()` 内根据距离切换追击/攻击：
  - `distance <= attackDistance` 攻击。
  - `distance > attackDistance` 追击。
- 额外有“室内/室外”联动：
  - 监听 `PlayerStatsManager.Instance.OnPlayerOutdoorStatusChanged`。
  - 若玩家进入室内，掠食者触发 `TeleportToSpawnPoint()` 并延迟销毁，避免卡在不连通导航区。

### 面试可说的设计点
- 这是“全局猎杀型 AI”：默认高压追踪玩家，提升生存紧张感。
- 使用玩家环境状态做 AI 降级处理，是实用的“可达性补丁”（比盲目寻路更稳定）。

---

## 5) 支撑脚本（面试中可快速补充）

- `AttackColliderScript`：攻击碰撞体默认关闭，仅在攻击关键帧开启，命中玩家后调用 `TakeDamage(5)`。
- `AnimalSpawnerScript`：定时在随机点生成动物，并记录 `currentAnimals`；动物死亡后回调 `OnAnimalDied()` 归还计数。
- `WanderPointsManager` / `WanderPointManager_4`：以单例提供场景中的离散游荡点，便于策划布点。

---

## 6) 一段 30 秒面试回答模板

“这个项目里的生物 AI 用的是**状态驱动**设计。被动动物是巡游-受惊逃跑-减速-休息的闭环；受击型敌人平时巡游，只有被打后才建立仇恨并在追击/攻击/脱战间切换；掠食者则是出生即主动追猎，并结合玩家室内外状态做寻路兜底。技术上统一依赖 `NavMeshAgent` 做移动，`Animator` 做表现，`Coroutine` 管控状态流程，攻击判定则通过动画事件开关攻击碰撞体。”
