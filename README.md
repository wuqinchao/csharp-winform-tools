# csharp-winform-tools

自用csharp小工具集合

## Reversion 
项目版本号自动更新工具, 根据日期及时间自动修改版本号中的Build与Revision, 可在项目PreBuildEvent事件中执行
### 命令行参数
	-f <file> [-s <date>] [-v <version>]
	-f <file> 指定需要修改的包含版本号的.cs文件
	-s <date> BUILD号起始日期, 如2015-01-01, 不指定则默认为2015-01-01,如已指定-v, 则此参数无效
	-v <version> 指定目标版本号, 如不指定则自动生成
### 返回值 0 成功, 1 参数错误, 2 文件不存在
