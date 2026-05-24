# Plan: Issue #23 — BossModeManager & 보스 준비 화면 UI

## Context
로비에서 "보스 모드" 버튼을 클릭하면 현재 `Debug.Log`만 출력된다.
보스 모드 준비 화면(세이브 슬롯 선택 + 보스 선택 + 패턴 미리보기)을 구현하고,
선택 완료 후 보스전으로 정상 진입시키는 것이 목표다.

## 기존 패턴 참고
- `AdventurePartyContext` — 씬 간 컨텍스트 전달 정적 클래스
- `AdventureReadyUI` — 슬롯 동적 생성 + 선택/해제 콜백 패턴
- `GameStateMachine.TransitionTo(GameState)` — 씬 전환 (`(int)state` = 빌드 인덱스)
- `SaveManager.TryLoad(int slot, out SaveSlotData)` — 슬롯 데이터 로드
- `EnemyTable.All` + `bossPattern != null` 필터 → 보스 목록

---

## 구현 계획

### Step 1: GameState에 BossReady 추가
**파일**: `Assets/Scripts/Core/GameState.cs`
- `BossReady` 값을 enum 마지막에 추가
- **verify**: Build Settings에서 BossReady.unity 씬을 동일 인덱스에 등록

### Step 2: BossPartyContext 생성
**파일**: `Assets/Scripts/Core/BossPartyContext.cs` (신규)
```csharp
public static class BossPartyContext
{
    public static int SaveSlotIndex;   // 선택한 세이브 슬롯 (0~2)
    public static string BossId;       // 선택한 보스 EnemyData.id
}
```
`AdventurePartyContext`와 동일한 정적 컨텍스트 패턴.

### Step 3: UI 컴포넌트 4종 (신규 스크립트)
폴더: `Assets/Scripts/UI/BossReady/`

#### SaveSlotUI.cs
- `Setup(int slotIndex, SaveSlotData data)` / `Setup(int slotIndex)` (빈 슬롯)
- 선택 시 `OnSelected(SaveSlotUI)` 콜백 호출
- 슬롯 번호, 캐릭터 이름 3개, 저장 날짜 표시

#### BossSlotUI.cs
- `Setup(EnemyData boss)` — 아이콘, 이름, 레어도 표시
- 선택 시 `OnSelected(BossSlotUI)` 콜백 호출

#### PatternCardUI.cs
- `Setup(Modifier mod, bool isPassive)` — 카드 한 장 (모디파이어 이름 + 설명)

#### PatternPreviewUI.cs
- `Show(BossPattern pattern)` — 패시브 카드 + 턴 카드 5개를 가로 배치
- `Hide()`

### Step 4: BossReadyUI.cs (씬 루트 컨트롤러)
**파일**: `Assets/Scripts/UI/BossReady/BossReadyUI.cs`

```
[Header] 세이브 슬롯 3개 (Transform saveSlotParent)
[Header] 보스 슬롯 (Transform bossSlotParent) — EnemyTable에서 bossPattern != null 필터
[Header] 패턴 미리보기 패널 (PatternPreviewUI)
[Header] 덱 요약 패널 (캐릭터/조커 이름 표시)
[Header] 풀스크린 미리보기 패널 (CanvasGroup)
[Header] 시작 버튼, 뒤로 버튼

private int selectedSlot = -1;
private EnemyData selectedBoss = null;
```

흐름:
1. `Start()` → SaveSlotUI 3개 + BossSlotUI N개 동적 생성
2. 슬롯 선택 → 덱 요약 패널 업데이트 (캐릭터 이름, 조커 이름)
3. 보스 선택 → PatternPreviewUI.Show(boss.bossPattern)
4. 둘 다 선택 시 시작 버튼 활성화
5. 시작 클릭 → 풀스크린 미리보기 Coroutine → BossPartyContext 설정 → `GameState.Adventure` 전환

### Step 5: LobbyUI 연결
**파일**: `Assets/Scripts/UI/LobbyUI.cs`
```csharp
public void OnBossModeClicked() =>
    GameStateMachine.Instance.TransitionTo(GameState.BossReady);
```

### Step 6: 씬 생성 (Unity 에디터 작업)
- `Assets/Scenes/BossReady.unity` 생성
- Build Settings에 추가 (GameState.BossReady의 int 값에 맞는 인덱스)
- BossReadyUI 프리팹 배치 및 인스펙터 연결

---

## 브랜치
`feature/part23-boss-mode-manager` (main에서 분기)

## Verification
1. 로비 → 보스 모드 버튼 클릭 → BossReady 씬 로드 확인
2. 세이브 슬롯 클릭 → 덱 요약(캐릭터/조커 이름) 표시 확인
3. 보스 선택 → 하단 패턴 미리보기 5턴 카드 표시 확인
4. 시작 버튼 클릭 → 풀스크린 미리보기 표시 후 Adventure 씬 전환 확인
5. Adventure 씬에서 `BossPartyContext.BossId`로 올바른 보스 로드 확인

## 미결 사항
- **GameState enum 현재 인덱스 확인 필요**: Lobby가 enum에 포함됐는지 확인 후 BossReady 삽입 위치 결정
- **Adventure 씬 연동**: StageManager가 BossPartyContext를 읽어 단일 보스 전투를 설정하는 로직은 별도 이슈/작업 범위로 분리 가능. 일단 씬 전환까지만 구현하고 StageManager 연동은 TODO 처리.
