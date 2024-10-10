# GameJamTurnBase

Unity版本:2022.3.17f1

## UI
1. ~~根据输入大小，生成格子~~
2. 人物血条
   ![image](https://github.com/user-attachments/assets/fe14eb1b-ffd0-4737-98b1-cafcfc01ad78)
4. 人物输入按钮（共八个）+键盘输入
   ![image](https://github.com/user-attachments/assets/c29442fb-690d-4d0e-9391-8377af8374f0)
5. 关卡倒计时
  ![image](https://github.com/user-attachments/assets/218f5895-3aa2-42df-b7a4-a0e3bc08dc15)
6. boss生命条
  ![image](https://github.com/user-attachments/assets/42441cf5-b905-4180-8210-f6a389f7dfce)
7. 人物选择后将指令放入格子+简单动画
   ![image](https://github.com/user-attachments/assets/298c814d-4ed4-4a5f-8fe2-9286f1c982d4)
8. 确定按钮
   ![image](https://github.com/user-attachments/assets/79e005e2-725e-42cd-a850-1c6bcac2772d)
   a. 记录所有指令并调用
   b. 当玩家按下键盘的时候就把这个指令加到list里面，确定之后就只需要遍历list来调用就可以了

## 人物动作
1. 移动（单次为1格）
2. 攻击-长剑（x-2到x+2，五个位置，造成2点伤害）
3. 攻击-锤（x-1,x+2,y+1，四个位置，造成4点伤害）
4. 防护罩（本回合剩余时间，所受伤害减少半颗)
   a. 可以在收到伤害时先检查有没有防护罩，有的话就-0.5的伤害
   b. 每一轮应该把防护罩这个bool设为false
5. 伤药葫芦（恢复两颗星，一局战斗可以用五次）
   a. 用一个int来记录伤药葫芦用了几次，每次玩家想用的时候检查这个数值是不是大于0
   b. 考虑用回复会把星星加到over limit的情况
6. 跳跃（跳起一格，下一个指令才落回来。如果跳跃是本回合最后一个指令，在回合结束时落地）
   a. 跳跃可以看成是一种特殊的移动
   b. 因为这里涉及到不能连续跳两次，所以我们需要一个东西来存上一个指令是什么
   c. 并且在做下一个指令的时候，也需要判断上一个指令是否是跳跃
7. 跳跃攻击（组合技，先到y+1位置，然后攻击x-2到x+2五个位置，？攻击力没写）@策划
   a. 理论上来说是不需要单独写函数的，只需要先跳跃然后长剑攻击然后在跳跃回到位置
   b. 考虑到我们动画在跳跃来回可能不一样。跳跃的函数里面可以加一个bool isUp，以此来调用不同动画函数
8. 法阵 @豆豆


## boss
1. boss数据布置（一个头两个手，共三个位置/格子）
   a. boss位置应该是可以简单更换的，例如用一个input来接收他目前的三个位置
2. 生命值（总共40，手部受到伤害1倍，头部收到伤害2倍）
   a. boss的代码中我们应该提供一个UnderAttack函数，接受一个int attackValue。
   b. 在玩家代码中我们只管把几点攻击力传给boss，由boss自己判定被打到了哪里并且减血
3. ---封印（boss失去所有生命值进入封印，限制时间为3分钟，封印失败会恢复10点）
   暂无封印流程，优先级低 @策划
4. 攻击-连续下劈（永远是左边的手攻击，因为玩家在左边）
   前摇
    ![image](https://github.com/user-attachments/assets/302751a0-d9a0-40a5-848b-451e97938413)
   a. 攻击范围为y,y-1,y-2三个位置
   b. 所有位置信息都要判定是否出界。我们可以复用一个判定函数来完成所有判定
   c. 这部分实际上有两部分。一个前摇，前摇通过移动boss手掌来完成。另一个是攻击，就像我们讨论的伪代码。
6. 左右双掌（@策划 需要解释）
   前摇
   ![image](https://github.com/user-attachments/assets/28822d08-21d2-45e9-9d25-0831b491d82a)
   这么看的话好像是图有点问题。能完全避开攻击的是图中中间位置，绿色格子是能通过操作避开攻击。
   a. 攻击范围为y-1,y-2,(x-1,y-2),(x+1,y-2)
7. 全屏激光
   a. 首先扫视屏幕所有位置除了头部正下方(顺时针)
   b.（逆时针）。顺逆时针对于代码来说没有影响，需要一个比较好看的动画 @美术
   ![image](https://github.com/user-attachments/assets/f009877f-590f-49a4-b2dd-71e4371c34ee)
   c. 双拳收拢变掌（动画）
   d. 双拳都攻击x-1,x+1,y+1,y+2.这里不用特意考虑中间是双倍伤害，因为两边都会攻击同一个格子（单个伤害为半颗）
   e. 重复d
8. 地面清扫
   ![image](https://github.com/user-attachments/assets/4a20b989-9f50-479e-9721-cb8532db98d8)
   a. ![image](https://github.com/user-attachments/assets/2cfa107c-e23e-432b-915e-aeead981af88)
   b. ![image](https://github.com/user-attachments/assets/d56bfb4b-1d7f-458e-b9ce-96ebf142c524)
   c. ![image](https://github.com/user-attachments/assets/9484d533-f889-4dad-8e31-eaa21b1c2546)
   d. ![image](https://github.com/user-attachments/assets/ecc2c7fe-b870-4c2f-9033-309600d04880)
   e. 收招
9. 范围拍击
    ![image](https://github.com/user-attachments/assets/68026aff-7233-4ef2-af97-9e066b7eb9d1)
   a. ![image](https://github.com/user-attachments/assets/bfe6921b-cc80-4b42-8eec-ec0687d061ee)
   b. ![image](https://github.com/user-attachments/assets/185cd0ca-93c9-498a-bc62-38359d3ce876)
   c. ![image](https://github.com/user-attachments/assets/99f374d1-a68e-41da-b629-d8556ec7d8e3)
   d. ![image](https://github.com/user-attachments/assets/9db758d9-9a44-461b-8969-a641a7c9470e)
   e. ![image](https://github.com/user-attachments/assets/ba775035-35e4-4554-b8e4-8448ee32df14)
   @ 策划，这里是造成两次伤害吗 还是只有撞击的一下造成伤害


## 其他
1. boss指令
   a. 目前先写死
   b. 我们可以有一个list来装所有指令，在scriptableobject中开一块放指令list。例如指令list[0]=连续下劈，list[1]=地面清扫。那数据list[0]=1也就是boss第一个指令是地面清扫
2. 玩家死亡处理：死亡的一方玩家，保存当前物品状态，满血回到boss战斗的第一回合
   a. 需要一个东西来存玩家有什么物品
   b. 一个数值来存玩家在哪个回合？方便测试.正常来说我们可以通过改变这个数值直接到某个回合


## 规则
1. 代码里所有公共变量用大写开头例如NodeManager，私有变量用小写开头例如startTime
2. 复用的所有gamobject放入prefab.改动的时候尽量改prefab减少merge conflict
3. 所有任务都开新的分支（分支名字为做的任务名），做好之后先和main merge，然后建立PR
4. 检查别人的PR时，一定要切到那个分支去测试
5. 尽量避免复制粘贴相同代码，复用性的代码都写成函数








   







