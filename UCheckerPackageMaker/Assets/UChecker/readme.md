## 框架命名规则

### 检查扩充
规则扩展参考：

BasicAssetTreeView

RuleCommonTextureFormatCheck

修复规则扩展：

FixerRuleCommonTextureFormat

规则扩展class命名  Rule + 名字 + Check
修复挥着class命名  Fixer + 名字 

驼峰大小写

### 模板检查规则
资源导入类等通过模板导入,但仍会有未设置的资源上传或者需要写大量的代码这里增加模板资源
1. 图片
2. Model
3. Animation
4. Audio
5. Video 

### 获取自定义参数
在检查类中参数获取字段小写+下划线不区分大小写;最好都采用小写
eg: shader_name

