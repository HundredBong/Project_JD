public enum LanguageType { KR, EN }

//카드 타입
public enum CardType
{
    Neutral,    //중립
    Warrior,    //전사
    Mage        //법사
}

//직업 타입
public enum JobType
{
    None,       //무직 (초보자)
    Warrior,    //전사
    Mage        //법사
}

//버프 타입
public enum BuffType
{
    AttackCombo,    //공격 콤보
    DefenseCombo,   //방어 콤보
    Spell,          //주문
    Defense         //방어
}

//카드 효과 타입
public enum CardEffectType
{                                     
    Damage,                           //데미지
    GainDefense,                      //방어 획득
    Draw,                             //카드 드로우
    GainCost,                         //코스트 획득
    GainAttackCombo,                  //공격 콤보 획득
    GainDefenseCombo,                 //방어 콤보 획득
    GainSpell,                        //주문 획득
    ConvertCombo,                     //콤보 전환 (공격 <-> 방어)
    DamageByAttackCombo,              //공격 콤보 기반 데미지
    DefenseByDefenseCombo,            //방어 콤보 기반 방어
    DamageByDamageTaken,              //받은 피해 기반 데미지
    DefenseByDamageTaken,             //받은 피해 기반 방어
    GainAttackComboByDamageTaken,     //받은 피해 기반 공격 콤보
    ConsumeAllAttackCombo,            //모든 공격 콤보 소모
    ConsumeAllDefenseCombo,           //모든 방어 콤보 소모
    ConsumeAllSpell,                  //모든 주문 소모
    DamageBySpell,                    //주문 기반 데미지
    DefenseBySpell,                   //주문 기반 방어
    MultiplySpell,                    //주문 배율 변경 (기도 카드용)
}

//게임 상태
public enum GameState
{
    Idle,                     //대기 
    Battle,                   //전투
    Reward,                   //보상 선택
    CardSelect,               //카드 선택 화면
    CardDelete,               //카드 삭제 화면
    GameOver                  //게임 오버
}

//전투 페이즈
public enum BattlePhase
{
    TurnStart,
    PlayerTurn,
    TurnEnd,
    EnemyTurn,
    BattleEnd
}
