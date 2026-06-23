// 전투 씬(GamePlay/BossPlay) 진행 상태 — GameManager.Phase로 노출되고
// 전이 시 OnBattlePhaseChanged 이벤트 발행. 새 진행 단계 추가 = enum 값 + 전이 지점 추가.
public enum BattlePhase
{
    None,
    StageIntro, // 스테이지/턴 인트로 연출
    JokerReward, // 조커 카드 보상 선택
    DrawPhase, // 블록 드로우 (플레이어 입력)
    Resolving, // 체인 정산·공격 연출
    PhaseTransition, // 보스 HP 구간 돌파 연출 (보스 플레이)
    StageClear, // 스테이지 클리어 연출
    GameOver,
    ModeClear, // 모드(어드벤처/보스) 최종 클리어
}
