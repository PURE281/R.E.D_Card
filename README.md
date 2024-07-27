# 项目介绍
## 项目初心
被R.E.D的妹妹们所感染，希望能不断学习，提高自己的能力
项目灵感根据百分百出品第十四期中关于妹妹们对自己的超能力的设定及导演组的设定，加上自己的一些想法
项目主要流程以抽卡，养卡，战斗，剧情的大概流程进行展开

## 项目所用到工具
- csv toolkit
- dotween
- 

## 项目功能
### 抽卡系统
消耗指定物品进行抽卡，在背包系统中可查看，并进行升星、装饰等功能
#### UI布局
如标题
#### 特效
卡牌升级、升星的简单特效
#### 点击卡牌查看详细信息


### 养成系统
#### 查看当前卡牌信息
#### 通过战斗胜利/剧情走向获得卡牌提升
#### 不同阶段的立绘变化

### 战斗系统
#### 回合制的卡牌系统
##### 每张卡对应不同技能
##### 支持联结
##### 支持彩蛋
##### 能力有限初期以视频为主进行技能演示
#### 机制
可在战斗中进行相同卡牌的升星，不同类型卡牌的融合以及不同类型卡牌的连携
并且支持当场上存在某些特定卡牌则直接获取胜利的机制

### 联网功能
#### 注册用户
#### 记录用户卡牌信息
#### 排行功能

todolist
- [X] 十连抽
- [X] 基本的一些功能,踩些关于AB加载,项目部署至linux的坑
- [ ] 查看已有卡组
- [ ] 根据每次战斗胜利后进行卡牌相应的升级和养成
- [ ] 剧情系统
- [ ] 战斗系统
- [x] ui布局
- [ ] 联网功能


## 更新日志
- 放弃网页版部署计划
- 修改了战斗界面中的初始化卡牌逻辑，在初始化卡牌图片时，一并初始化卡牌的技能和属性及介绍等
- 添加了初始化卡牌后的继续添加卡牌功能

### 20240711
- 给卡牌添加了相应的音频，在点击的时候会进行播放
- 修改了ui布局，添加了主角和boss的图片
-

### 20240715 Am
- 将原本通过ab包进行加载的方式改成了csv获取资源信息，然后直接resources.load的方式生成
- 抽卡场景的逻辑删除，后面用战斗系统的逻辑
- 修改ui布局
- 给卡类的carditem添加了拖动和打出的代码逻辑

#### 未做部分
为提高可玩性，添加升星和连携的功能
相同的卡可进行升星
不相同的卡但支持连携可合成双人卡

### 20240715 Pm
- 取消拖拽，改为鼠标点击

### 20240717
- 为满足升星，连携，融合等功能，给carddata中的卡牌信息添加了新的三个字段，并且为给后面的卡牌熟练度进行扩展，添加了熟练度和等级的划分，这个目前还不着急
- 添加了委托的管理类
- 战斗系统中添加了实时刷新当前选中的卡牌中是否有可以进行升星， 连携和融合的逻辑判断
- 修改了部分ui布局


### 20240719 pm
- 丰富了升星的功能逻辑，基本已满足升星功能，需要优化特效和音效即可
- 再次优化了升星功能逻辑，将原本复杂的流程和影响体验的流程优化掉了，现在可以实现按照点击的顺序进行升星的提示，并且在升星之后的卡片刷新也完成

### 20240722 pm
- 添加了一些关于每次打出卡牌时的战斗状态的判定
- 修改了UI布局
- 解决了打包到安卓后出现的csv读取路径异常的问题

### 20240723 pm
- 基本完成了对战逻辑
- 将敌我双方的ui修改为动态加载
- 添加胜利和失败的简单结算画面
- 添加了简单敌人的AI
- 添加了上述功能中对应的一些图片资源

### 20240724 am
- 完成了抽卡系统稀有度的设定（r,sr,ssr,ur）
- 修改了抽卡场景中的card预制体
- 由于功能大相径庭，因此将战斗场景和抽卡场景的carditem脚本进行了区分
- 在抽卡场景中为不同的稀有度用dotween做了简单的效果区分
- 恢复抽卡场景中的点击功能，点击可播放对应卡牌的音频，还需要优化出现的内容

### 20240725 pm
- 完善对战系统逻辑，添加角色攻击功能，添加对攻击和护甲的逻辑处理
- 添加了两边攻击力和护甲值的简易UI显示和简单动效


### 20270726 pm
- 添加查看卡组场景，添加部分代码，分页查询所有卡牌功能

### 20240727 pm
- 修改了读写csv文件的插件
- 添加卡牌查询的场景