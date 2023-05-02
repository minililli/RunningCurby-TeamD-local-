# RunningCurby-TeamD-local-
Localwork(for backup)

## 팀프로젝트 개요(소개)
- 진행방식 : 팀프로젝트(4인)
- 진행기간 : 2023.03.06~ 2023.5.4(약 2개월)
- 맡은임무 : RunningStage, 비동기 LoadingScene, 게임오브젝트 생성/소환 관련 Pool, Factory 등 내부적처리 등

## 게임 소개
2D 러닝 액션 플랫폼게임입니다.
MainScene -> RunningStage -> BattleStage -> BossStage -> endingCredit 순으로 진행됩니다.
진행하다 죽으면 해당 씬 재시작, 또는 게임 종료를 할 수 있습니다.

## 어려웠던 점과 극복했던 방법

1. 보이지 않는 부분의 스크립트 처리

게임에서 자주 생성되는 오브젝트들을 Pool로 관리하려고 Factory, ObjectPool, PoolObject를 활용했는데, 게임오브젝트로서 보이는 부분이 아니어서 작업할 때 왜 오류가 뜨는지 안보이는 부분이라 더 알기 어려워 힘들었습니다.
오류가 뜨는 부분에 대해 왜 그럴까 회의해봐도 팀원들 역시 모르겠다는 대답만 돌아와 2주간 정체됐었습니다. 고민하고 고쳐보고 여기저기 물어보고 한 끝에 오류들을 다 수정할 수 있었습니다. 
게임오브젝트가 보이는 부분들에 대한 스크립트 처리였으면 더 즐거웠을 것 같았고, 개인프로젝트에서 충족해보려고 합니다.


2. 깃허브 Merge시 일부 예전 스크립트로 변경되는 점

팀원들끼리 병합했을 때 예상치 못하게 이전 상태로 돌아가는 문제를 많이 겪어, 
아래 두가지 방법을 적용해 잘 마무리 할 수 있었습니다.

첫째, 각 개인 폴더에 작업하여 넣어두고 다같이 있을때 파일위치설정
Merge 후 파일 확일할 때 오류 수정등을 거친 후 해당하는 폴더로 위치 이동/변경하는 방법

둘째, 작업한 파일만 선택해 Push 하기
백업할 로컬폴더와 개인Repository를 준비해두고, 작업한 파일만 선택해 push했지만 실수로 다른 파일까지 덮어썼다면 그 부분만 다시 push 하는 방식으로 오류에 대비하였습니다. 

3. 해보지 않은 스크립트에 대한 아쉬움

팀작업이라 정한 범위 외의 범위를 하면 혼돈이 생겨서 정한 부분작업 위주로만 작업을 했습니다.
연습 및 다양한 스크립트 작성을 해보고자 서로 맡기 애매한 스크립트에 대해서는 자발적으로 맡아서 했습니다.
해보지 못해 아쉬운 부분에 대해서는 개인 프로젝트에서 충족해보려고 합니다.